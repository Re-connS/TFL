using UnityEngine;
using UnityEngine.UI;

public class MapItemUp : MonoBehaviour
{
    public GameObject Lastic;
    public GameObject Karand;
    public Transform Ruka;
    public GameObject MapUI;
    public AudioSource AS;
    public AudioClip Clip1;
    public AudioClip Clip2;
    private int currentPictIndex = 0; 
    public Reicast Reic;
    public GameObject[] Pict;
    public GameObject[] Slots;
    public Sprite OnSlot;
    public Sprite OffSlot;
    public Transform LasticPos;
    public Transform KarandPos;
    public bool InPlayer;
    private GameObject currentItem;
    
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InPlayer = true;
            Debug.Log("Игрок рядом с предметами");
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InPlayer = false;
            DropCurrentItem();
        }
    }

    public void Update()
    {
        if (!InPlayer) return;
        
        // Сначала проверяем метки (можно ставить и без предмета в руке)
        Metka();
        
        // Потом проверяем взятие предметов
        HandleItemPickup();
    }
    
    private void HandleItemPickup()
    {
        bool isLasticActive = Lastic != null && Lastic.GetComponent<ActivCode>().Activ;
        bool isKarandActive = Karand != null && Karand.GetComponent<ActivCode>().Activ;

        if (!isLasticActive && !isKarandActive) return;

        GameObject itemToTake = null;
        
        if (isLasticActive && currentItem != Lastic)
        {
            itemToTake = Lastic;
            if (MapUI != null) MapUI.SetActive(false);
            if (Karand != null) Karand.GetComponent<ActivCode>().Activ = false;
            Debug.Log("Пытаемся взять ластик");
        }
        else if (isKarandActive && currentItem != Karand)
        {
            itemToTake = Karand;
            if (MapUI != null) MapUI.SetActive(true);
            if (Lastic != null) Lastic.GetComponent<ActivCode>().Activ = false;
            Debug.Log("Пытаемся взять карандаш");
            UpdateSlotsUI();
        }
        else
        {
            return;
        }

        if (HasItemInHand() && GetItemInHand() != itemToTake)
        {
            DropItem(GetItemInHand());
        }

        TakeItem(itemToTake);
    }
    
    private void UpdateSlotsUI()
    {
        if (Slots == null || Slots.Length == 0 || Pict == null || Pict.Length == 0) return;
        
        for (int i = 0; i < Slots.Length - 1 && i < Pict.Length; i++)
        {
            if (Slots[i] != null && Slots[i].transform.childCount > 0)
            {
                Image slotImage = Slots[i].transform.GetChild(0).GetComponent<Image>();
                Image pictImage = Pict[i] != null ? Pict[i].GetComponent<Image>() : null;
                
                if (slotImage != null && pictImage != null)
                {
                    slotImage.sprite = pictImage.sprite;
                }
            }
        }
    }
    
    public void Metka()
    {
        // Прокрутка колесиком мыши
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            ChangeMarkIndex(scroll);
        }
        
        // Клик мышью
        if (Input.GetMouseButtonDown(0))
        {
            // Проверяем, что Reic и hit существуют
            if (Reic == null)
            {
                Debug.LogWarning("Reicast не назначен!");
                return;
            }
            
            if (Reic.hit.collider == null)
            {
                Debug.Log("Луч не попал в объект");
                return;
            }
            
            // Установка метки (карандаш)
            if (currentItem == Karand && Reic.hit.collider.gameObject.CompareTag("Map"))
            {
                if (Pict == null || Pict.Length == 0 || currentPictIndex >= Pict.Length || Pict[currentPictIndex] == null)
                {
                    Debug.LogWarning("Префаб метки не назначен!");
                    return;
                }
                AS.clip = Clip1;
                AS.Play();
                Debug.Log($"{currentItem.name}_{Reic.hit.collider.gameObject.tag}");
                GameObject mark = Instantiate(Pict[currentPictIndex], Reic.hit.point, Pict[currentPictIndex].transform.rotation);
                mark.tag = "Mark"; // Устанавливаем тег для созданной метки
            }
            
            // Удаление метки (ластик)
            if (currentItem == Lastic && Reic.hit.collider.gameObject.CompareTag("Mark"))
            {
                AS.clip = Clip2;
                AS.Play();
                Debug.Log($"Удаляю метку: {Reic.hit.collider.gameObject.name}");
                Destroy(Reic.hit.collider.gameObject);
            }
        }
    }
    
    private void ChangeMarkIndex(float scrollDirection)
    {
        if (Pict == null || Pict.Length == 0) return;
        
        currentPictIndex += scrollDirection > 0 ? 1 : -1;
        
        // Зацикливание
        if (currentPictIndex >= Pict.Length) currentPictIndex = 0;
        if (currentPictIndex < 0) currentPictIndex = Pict.Length - 1;
        
        // Обновление UI слотов
        UpdateSlotHighlight();
        
        Debug.Log($"Выбрана метка {currentPictIndex}: {(Pict[currentPictIndex] != null ? Pict[currentPictIndex].name : "пусто")}");
    }
    
    private void UpdateSlotHighlight()
    {
        if (Slots == null || Slots.Length == 0) return;
        
        // Сбрасываем все слоты
        for (int i = 0; i < Slots.Length; i++)
        {
            if (Slots[i] != null)
            {
                Image slotImage = Slots[i].GetComponent<Image>();
                if (slotImage != null && OffSlot != null)
                {
                    slotImage.sprite = OffSlot;
                }
            }
        }
        
        // Подсвечиваем текущий слот
        if (currentPictIndex < Slots.Length && Slots[currentPictIndex] != null)
        {
            Image currentSlotImage = Slots[currentPictIndex].GetComponent<Image>();
            if (currentSlotImage != null && OnSlot != null)
            {
                currentSlotImage.sprite = OnSlot;
            }
        }
    }
    
    private bool HasItemInHand()
    {
        return Ruka != null && Ruka.childCount > 0;
    }

    private GameObject GetItemInHand()
    {
        return HasItemInHand() ? Ruka.GetChild(0).gameObject : null;
    }

    private void TakeItem(GameObject item)
    {
        if (item == null) return;

        Vector3 neg = item.transform.localScale;
        item.transform.SetParent(Ruka);
        item.transform.position = Ruka.position;
        item.transform.localRotation = Quaternion.identity;
        item.transform.localScale = neg;
        
        item.GetComponent<ActivCode>().Activ = true;
        currentItem = item;
        
        Debug.Log($"Взял предмет: {item.name}");
    }

    private void DropItem(GameObject item)
    {
        if (item == null) return;

        Transform targetPos = GetTargetPosition(item);
        
        if (targetPos != null)
        {
            Vector3 neg2 = item.transform.localScale;
            item.transform.SetParent(targetPos);
            item.transform.localScale = neg2;
            item.transform.position = targetPos.position;
            item.transform.localRotation = Quaternion.identity;
        }

        if (item.GetComponent<ActivCode>() != null)
        {
            item.GetComponent<ActivCode>().Activ = false;
        }
        
        currentItem = null;
        Debug.Log($"Положил предмет: {item.name}");
    }

    private void DropCurrentItem()
    {
        if (MapUI != null) MapUI.SetActive(false);
        
        if (HasItemInHand())
        {
            DropItem(GetItemInHand());
        }
    }

    private Transform GetTargetPosition(GameObject item)
    {
        if (item == Lastic) return LasticPos;
        if (item == Karand) return KarandPos;
        return null;
    }
}