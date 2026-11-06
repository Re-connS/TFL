using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class NPCWalk : MonoBehaviour
{
    [Header("Основные настройки")]
    public float maxLiveTimeNPC = 60f;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float killDistance = 3f;
    
    [Header("Компоненты")]
    [SerializeField] private RadioSistem radioSystem;
    [SerializeField] private DoorAndNPC doorAndNPC;
    [SerializeField] private Transform target;
    
    [Header("Сектора")]
    [SerializeField] private string[] sectorXNames;
    
    [Header("Визуал")]
    [SerializeField] private GameObject npcVisualPrefab;
    
    [Header("Состояние")]
    public float realLiveTimeNPC;
    private float radioChannel;
    public bool isInLight = false;
    private bool hasChannel = false;
    private Rigidbody rb;
    public bool DeadNps;
    
    // Константы
    private const float SECTOR_SIZE = 111f;
    private const int MAX_CHANNEL_ATTEMPTS = 100;
    private const float CHANNEL_TOLERANCE = 0.01f;
    public string sectorName;

    private void Start()
    {
        InitializeComponents();
        AssignRadioChannel();
    }

    private void FixedUpdate()
    {
        if (!hasChannel) return;
        
        if (isInLight)
        {
            MoveToTarget();
            ResetLifeTimer();
        }
        else
        {
            UpdateSectorDescription();
            UpdateLifeTimer();
        }
    }

    // ========== ИНИЦИАЛИЗАЦИЯ ==========

    private void InitializeComponents()
    {
        // Находим компоненты
        if (radioSystem == null)
            radioSystem = GameObject.Find("Radio")?.GetComponent<RadioSistem>();
            
        if (doorAndNPC == null)
            doorAndNPC = GameObject.Find("Door")?.GetComponent<DoorAndNPC>();
            
        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player")?.transform;
            
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.freezeRotation = true;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | 
                            RigidbodyConstraints.FreezeRotationZ;
        }
        
        // Проверка зависимостей
        if (radioSystem == null)
            Debug.LogError($"NPC {gameObject.name}: RadioSistem не найден!");
            
        if (target == null)
            Debug.LogError($"NPC {gameObject.name}: Target не найден!");
    }

    // ========== УПРАВЛЕНИЕ КАНАЛОМ ==========

    private void AssignRadioChannel()
    {
        if (radioSystem == null) return;
        
        int attempts = 0;
        while (attempts < MAX_CHANNEL_ATTEMPTS)
        {
            attempts++;
            
            // Генерируем случайный канал
            radioChannel = Mathf.Round(
                Random.Range(radioSystem.minKanal, radioSystem.maxKanal) * 100f
            ) / 100f;
            
            // Проверяем, занят ли канал
            bool isChannelTaken = false;
            foreach (var channel in radioSystem.channels)
            {
                if (Mathf.Approximately(channel.weight, radioChannel))
                {
                    isChannelTaken = true;
                    break;
                }
            }
            
            // Если канал свободен - занимаем
            if (!isChannelTaken)
            {
                CreateRadioChannel();
                hasChannel = true;
                Debug.Log($"NPC {gameObject.name} занял канал {radioChannel}");
                return;
            }
        }
        
        Debug.LogWarning($"NPC {gameObject.name}: Не удалось найти свободный канал после {MAX_CHANNEL_ATTEMPTS} попыток");
    }

    private void CreateRadioChannel()
    {
        if (radioSystem == null) return;
        
        var newChannel = new RadioSistem.Kanals
        {
            weight = radioChannel,
            gameObject = this.gameObject,
            SectorName = GetCurrentSectorKey()
        };
        
        radioSystem.channels.Add(newChannel);
    }
    // ========== ПОЛУЧЕНИЕ ОПИСАНИЙ СЕКТОРА ==========
    private string GetCurrentSectorKey()
    {
        int xSec = Mathf.FloorToInt(transform.position.x / SECTOR_SIZE);
        int zSec = Mathf.FloorToInt(transform.position.z / SECTOR_SIZE);
        
        // Защита от выхода за границы массива
        if (sectorXNames == null || sectorXNames.Length == 0)
            return $"{xSec}{zSec}";
            
        int clampedX = Mathf.Clamp(xSec, 0, sectorXNames.Length - 1);
        return $"{sectorXNames[clampedX]}{zSec}";
    }
    private void UpdateSectorDescription()
    {
        if (radioSystem == null || !hasChannel) return;
        
        string sectorKey = GetCurrentSectorKey();
        
        // Обновляем информацию в радио
        for (int i = radioSystem.channels.Count - 1; i >= 0; i--)
        {
            if (Mathf.Approximately(radioSystem.channels[i].weight, radioChannel))
            {
                radioSystem.channels[i].SectorName = sectorKey;
                break;
            }
        }
    }

    // ========== ДВИЖЕНИЕ ==========
    private void MoveToTarget()
    {
        if (target == null || rb == null) return;
        
        // Горизонтальное направление к цели
        Vector3 direction = target.position - transform.position;
        direction.y = 0;
        
        float distance = direction.magnitude;
        if (distance < 0.1f) return;
        
        direction.Normalize();
        
        // Устанавливаем скорость
        Vector3 newVelocity = direction * speed;
        newVelocity.y = rb.linearVelocity.y; // Сохраняем вертикальную скорость
        
        rb.linearVelocity = newVelocity;
        
        // Поворачиваем NPC в сторону движения
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 5f);
        }
        
        // Проверка расстояния до цели (убийство)
        if (distance < killDistance)
        {
            GiveLoot();
            DestroyNPC(false);
        }
    }

    // ========== ТАЙМЕР ЖИЗНИ ==========

    private void ResetLifeTimer()
    {
        realLiveTimeNPC = 0f;
    }

    private void UpdateLifeTimer()
    {
        realLiveTimeNPC += Time.deltaTime;
        
        if (realLiveTimeNPC >= maxLiveTimeNPC)
        {
            DestroyNPC(true);
        }
    }

    // ========== ТРИГГЕРЫ ==========

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LightTag") || other.gameObject.name == "LightM")
        {
            isInLight = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("LightTag") || other.name == "LightM")
        {
            isInLight = false;
        }
    }

    // ========== УНИЧТОЖЕНИЕ ==========

    private void DestroyNPC(bool Dead)
    {
        if (Dead == true)
        {
            DeadNps = true;
            
            // Помечаем канал как "DeadNps"
            if (radioSystem != null && hasChannel)
            {
                for (int i = radioSystem.channels.Count - 1; i >= 0; i--)
                {
                    if (Mathf.Approximately(radioSystem.channels[i].weight, radioChannel))
                    {
                        radioSystem.channels[i].SectorName = "DeadNps";
                        break;
                    }
                }
            }
            Invoke(nameof(RemoveChannelAndDestroy), 30f);
        }
        else
        {
            RemoveChannelAndDestroy();
        }
    }

    private void RemoveChannelAndDestroy()
    {
        // Удаляем канал из радио
        if (radioSystem != null && hasChannel)
        {
            for (int i = radioSystem.channels.Count - 1; i >= 0; i--)
            {
                if (Mathf.Approximately(radioSystem.channels[i].weight, radioChannel))
                {
                    radioSystem.channels.RemoveAt(i);
                    hasChannel = false;
                    break;
                }
            }
        }
        Destroy(gameObject);
    }

    // ========== ЛУТ ==========
    private void GiveLoot()
    {
        if (doorAndNPC != null && npcVisualPrefab != null)
        {
            doorAndNPC.AddNps(npcVisualPrefab);
            Debug.Log($"NPC {gameObject.name} дал лут: {npcVisualPrefab.name}");
        }
    }

    // ========== ПУБЛИЧНЫЕ МЕТОДЫ ==========

    public float GetChannel()
    {
        return radioChannel;
    }
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
    public bool IsInLight()
    {
        return isInLight;
    }
}