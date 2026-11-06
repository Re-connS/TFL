using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class RadioSistem : MonoBehaviour
{
    [Header("Настройки радио")]
    [SerializeField] private float realKanal;
    public float maxKanal = 100f;
    public float minKanal = 0f;
    [SerializeField] private float step = 0.1f;
    [SerializeField] private float sectorTolerance = 0.50f;

    [Header("Компоненты")]
    [SerializeField] private ActivCode activCode;
    [SerializeField] private CameraMove cameraMove;
    [SerializeField] private EnergiSistem energySystem;
    [Header("Звуки")]
    public AudioSource AS;
    public AudioSource ASVois;
    public AudioClip[] AC;
    public float MaxDistanKanal;
    
    [Header("UI")]
    [SerializeField] private TMP_Text frequencyText;
    [SerializeField] private LocalizedText LT;
    [SerializeField] private LocalizedText LT2;
    private string lastMainKey;
    private string lastSuffixKey;
    private Kanals lastChannelGO;
    private bool lastIsInLight;
    
    [Header("Камера")]
    [SerializeField] private Transform posCameraRadio;
    [SerializeField] private Transform posCameraPlayer;
    [SerializeField] private GameObject cameraPlayer;
    
    [Header("Анимации")]
    [SerializeField] private Animator doorAnimator; // Комментарий: это НЕ дверь, а анимация радио
    [SerializeField] private Animator handAnimator;
    [SerializeField] private Animator lampAnimator;
    
    [Header("Указатель")]
    [SerializeField] private Transform pointer;
    [SerializeField] private Vector3 pointerMinPosition;
    [SerializeField] private Vector3 pointerMaxPosition;
    
    [Header("Каналы")]
    public List<Kanals> channels = new List<Kanals>();

    [System.Serializable]
    public class Kanals
    {
        public GameObject gameObject;
        public float weight;
        public string SectorName;
    }

    // Состояния
    private bool previousActivState;
    
    // Кэшированные значения
    private float normalizedValue;
    private string currentSectorName = "ShSHhsHS"; 
    
    // Константы
    private const int LAMP_STATE_OFF = 0;
    private const int LAMP_STATE_ON = 1;
    private const int LAMP_STATE_ERROR = 2;
    
    private const int HAND_STATE_IDLE = 0;
    private const int HAND_STATE_RIGHT = 1;
    private const int HAND_STATE_LEFT = 2;
    
    // Кэш для производительности
    private WaitForEndOfFrame waitForEndOfFrame;
    private Kanals currentChannel;
    private bool isInitialized = false;
    public ECSMenuScript EMS;

    private void Start()
    {
        InitializeComponents();
        UpdateCameraState();
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized) return;
        
        // Обновляем указатель (оптимизировано)
        UpdatePointer();
        
        // Обработка ввода
        HandleInput();
        
        // Проверка энергии
        if (!IsEnergyAvailable())
        {
            AS.Stop();
            ASVois.Stop();
            DeactivateRadio();
            return;
        }
        else
        {
            if(AS.isPlaying == false)
            {
                AS.Play();
                ASVois.Play();
            }
        }
        
        // Обновление состояния радио
        UpdateRadioState();
        
        // Обновление UI и анимаций
        UpdateUIAndAnimations();
        
        // Основная логика настройки частоты
        if (activCode != null && activCode.Activ)
        {
            HandleFrequencyControl();
        }
        RadioNoize();
    }

    // ========== ИНИЦИАЛИЗАЦИЯ ==========

    private void InitializeComponents()
    {
        if (activCode != null)
        {
            previousActivState = activCode.Activ;
            if (cameraMove != null) cameraMove.canMove = !activCode.Activ;
        }
        
        // Кэшируем компоненты для производительности
        if (frequencyText == null)
            frequencyText = GetComponentInChildren<TMP_Text>();
        
        // Начальное состояние указателя
        UpdatePointer();
    }

    // ========== ОБНОВЛЕНИЕ УКАЗАТЕЛЯ ==========

    private void UpdatePointer()
    {
        if (pointer == null) return;
        
        // Защита от деления на ноль
        float range = maxKanal - minKanal;
        normalizedValue = Mathf.Approximately(range, 0f) 
            ? 0f 
            : Mathf.Clamp01((realKanal - minKanal) / range);
        
        // Используем локальные переменные для читаемости
        Vector3 newPosition = Vector3.Lerp(pointerMinPosition, pointerMaxPosition, normalizedValue);
        
        // Присваиваем только если изменилось (оптимизация)
        if (pointer.localPosition != newPosition)
        {
            pointer.localPosition = newPosition;
        }
    }

    // ========== ОБРАБОТКА ВВОДА ==========

    private void HandleInput()
    {
        // Выключение радио по E (только если активно)
        if (activCode != null && activCode.Activ && Input.GetKeyUp(KeyCode.E) && EMS.OpenMenu != true)
        {
            DeactivateRadio();
        }
    }

    // ========== УПРАВЛЕНИЕ ЭНЕРГИЕЙ ==========

    private bool IsEnergyAvailable()
    {
        return energySystem != null && energySystem.RealEnergi > 0 && energySystem.BoxEhergi[2].Activ == true && energySystem.AC.Activ == true;
    }

    private void DeactivateRadio()
    {
        if (activCode == null) return;
        
        if (activCode.Activ)
        {
            activCode.Put();
        }
        
        if (cameraMove != null) 
        {
            cameraMove.canMove = true;
        }
        
        previousActivState = false;
        UpdateCameraState();
        UpdateRadioAnimation(false);
    }

    // ========== УПРАВЛЕНИЕ СОСТОЯНИЕМ РАДИО ==========

    private void UpdateRadioState()
    {
        if (activCode == null || activCode.Activ == previousActivState) return;
        
        // Обновление движения камеры
        if (cameraMove != null) 
        {
            cameraMove.canMove = !activCode.Activ;
        }
        
        // Обновление камеры
        UpdateCameraState();
        
        // Обновление анимации радио
        UpdateRadioAnimation(activCode.Activ);
        
        previousActivState = activCode.Activ;
    }

    private void UpdateRadioAnimation(bool isActive)
    {
        if (doorAnimator != null)
        {
            doorAnimator.SetBool("Open", isActive);
        }
    }

    private void UpdateCameraState()
    {
        if (cameraPlayer == null || posCameraPlayer == null || posCameraRadio == null) return;
        
        bool shouldAttachToRadio = activCode != null && activCode.Activ;
        Transform targetParent = shouldAttachToRadio ? posCameraRadio : posCameraPlayer;
        
        // Проверяем, нужно ли менять родителя
        if (cameraPlayer.transform.parent != targetParent)
        {
            cameraPlayer.transform.SetParent(targetParent);
            cameraPlayer.transform.localPosition = Vector3.zero;
            cameraPlayer.transform.localRotation = Quaternion.identity;
        }
    }

    // ========== ОБНОВЛЕНИЕ UI И АНИМАЦИЙ ==========

    private void UpdateUIAndAnimations()
    {
        // Обновление сектора
        UpdateSectorInfo();
        
        // Обновление лампы
        UpdateLampState();
    }
    private void UpdateSectorInfo()
    {
        if (LT == null || channels == null) return;

        // Нет каналов вообще — всегда шум
        if (channels.Count == 0)
        {
            SetSectorKeys("Shum", "");
            currentSectorName = string.Empty;
            return;
        }

        // Ищем ближайший канал в пределах допуска
        currentChannel = null;
        float closestDistance = sectorTolerance;

        foreach (var channel in channels)
        {
            if (channel == null) continue;
            float distance = Mathf.Abs(channel.weight - realKanal);
            if (distance <= closestDistance)
            {
                closestDistance = distance;
                currentChannel = channel;
            }
        }

        // Канал не найден — шум
        if (currentChannel == null)
        {
            SetSectorKeys("Shum", "");
            currentSectorName = string.Empty;
            return;
        }

        // Канал найден — формируем ключи
        currentSectorName = currentChannel.SectorName;

        string mainKey;
        string suffixKey = "";

        var npc = currentChannel.gameObject != null
            ? currentChannel.gameObject.GetComponent<NPCWalk>()
            : null;

        if (npc == null)
        {
            // NPC нет — просто имя сектора
            mainKey = currentChannel.SectorName;
        }
        else
        {
            // isLowHP: NPC ещё "жив" достаточно долго
            bool isLowHP = npc.realLiveTimeNPC > npc.maxLiveTimeNPC / 4f;

            mainKey = npc.isInLight ? "LightM" : currentChannel.SectorName;

            if (isLowHP && npc.DeadNps == false)
            {
                suffixKey = "LowHPNPS";
            }
            else
            {
                suffixKey = "";
            }   
        }

        SetSectorKeys(mainKey, suffixKey);
    }

    // Единая точка обновления ключей — исключает дублирование и лишние вызовы
    private void SetSectorKeys(string mainKey, string suffixKey)
    {
        // Меняем только при реальном изменении (учитывая смену канала)
        if (mainKey == lastMainKey
            && suffixKey == lastSuffixKey
            && currentChannel == lastChannelGO)
            return;

        lastMainKey = mainKey;
        lastSuffixKey = suffixKey;
        lastChannelGO = currentChannel;

        if (LT != null)  LT.SetKey(mainKey, suffixKey);
        if (LT2 != null) LT2.SetKey(mainKey, suffixKey);
    }
    private void UpdateLampState()
    {
        if (lampAnimator == null) return;
        
        int lampState = LAMP_STATE_OFF;
        
        if (channels != null && channels.Count > 0)
        {
            bool hasMatch = false;
            foreach (var channel in channels)
            {
                if (Mathf.Abs(channel.weight - realKanal) <= sectorTolerance)
                {
                    hasMatch = true;
                    break;
                }
            }
            
            lampState = hasMatch ? LAMP_STATE_ON : LAMP_STATE_ERROR;
        }
        
        if (lampAnimator.GetInteger("La") != lampState)
        {
            lampAnimator.SetInteger("La", lampState);
        }
    }

    // ========== УПРАВЛЕНИЕ ЧАСТОТОЙ ==========

    private void HandleFrequencyControl()
    {
        // Обновление текста частоты
        if (frequencyText != null)
        {
            string newFrequencyText = realKanal.ToString("F2");
            if (frequencyText.text != newFrequencyText)
            {
                frequencyText.text = newFrequencyText;
            }
        }
        
        // Обработка ввода
        bool isMovingRight = Input.GetKey(KeyCode.D);
        bool isMovingLeft = Input.GetKey(KeyCode.A);
        
        if (isMovingRight || isMovingLeft)
        {
            float direction = isMovingRight ? 1f : -1f;
            float newFrequency = realKanal + direction * step * Time.deltaTime * 10f;
            
            // Округляем до 2 знаков
            newFrequency = Mathf.Round(newFrequency * 100f) / 100f;
            
            // Проверяем границы
            if (newFrequency >= minKanal && newFrequency <= maxKanal)
            {
                realKanal = newFrequency;
            }
            else
            {
                realKanal = Mathf.Clamp(newFrequency, minKanal, maxKanal);
            }
            
            SetHandAnimation(isMovingRight ? HAND_STATE_RIGHT : HAND_STATE_LEFT);
        }
        else
        {
            SetHandAnimation(HAND_STATE_IDLE);
        }
    }

    private void SetHandAnimation(int state)
    {
        if (handAnimator != null && handAnimator.GetInteger("Act") != state)
        {
            handAnimator.SetInteger("Act", state);
        }
    }

    // ========== ПУБЛИЧНЫЕ МЕТОДЫ ==========

    public void SetFrequency(float newFrequency)
    {
        realKanal = Mathf.Clamp(newFrequency, minKanal, maxKanal);
        realKanal = Mathf.Round(realKanal * 100f) / 100f;
    }

    public float GetFrequency()
    {
        return realKanal;
    }

    public float GetNormalizedFrequency()
    {
        float range = maxKanal - minKanal;
        return Mathf.Approximately(range, 0f) ? 0f : Mathf.Clamp01((realKanal - minKanal) / range);
    }

    public string GetCurrentSector()
    {
        return currentSectorName;
    }
    
    // ========== ДОПОЛНИТЕЛЬНЫЕ МЕТОДЫ ==========
    
    // Для отладки
    private void OnDrawGizmosSelected()
    {
        if (pointer != null && Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(pointer.position, 0.1f);
        }
    }
    // ========== Изменение звука от канала ==========
    private void RadioNoize()
    {
        Kanals nearest = null;
        float nearestDistance = float.MaxValue;

        foreach (var channel in channels)
        {
            float distance = Mathf.Abs(channel.weight - realKanal);
            if (distance <= MaxDistanKanal && distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = channel;
            }
        }
        float t = Mathf.Clamp01(nearestDistance / MaxDistanKanal);

        AS.volume = t;
        ASVois.volume = 1f - t;
    }
}