using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ItemGiftSkript : MonoBehaviour
{
    //ресурсы nps
    [System.Serializable]
    public struct ResursT
    {
        public int MinKol;
        public int MaxKol;
        public string Name;
        public Sprite IconResurs;
    }
    public List<ResursT> ListResult = new List<ResursT>();
    public GameObject slot1;
    public GameObject slot2;
    public ResursT res;
    public ResursT res2;
    public int Kolres1;
    public int Kolres2;
    public DoorAndNPC DAN;
    //ресуры игрока
    public EnergiSistem ES;
    public MoralSystem MS;
    public Temperatura Tem;
    public LocalizedText LT;
    public LocalizedText LT2;
    public ECSMenuScript EMS;
    public void Start()
    {
        slot1 = GameObject.Find("AVariti");
        slot2 = GameObject.Find("BVariti");
        ES = GameObject.Find("Generator").GetComponent<EnergiSistem>();
        Tem = GameObject.Find("PecMa").GetComponent<Temperatura>();
        MS = GameObject.Find("Player").GetComponent<MoralSystem>();
        DAN = GameObject.Find("Door").GetComponent<DoorAndNPC>();
        LT = slot1.transform.GetChild(1).GetComponent<LocalizedText>();
        LT2 = slot2.transform.GetChild(1).GetComponent<LocalizedText>();
        EMS = GameObject.Find("Player").GetComponent<ECSMenuScript>();
        ResPic();
    }
    public void ResPic()
    {
        //выбор типа ресурса на 2 слота
        res = ListResult[Random.Range(0, ListResult.Count)];
        res2 = ListResult[Random.Range(0, ListResult.Count)];
        while(res2.Name == res.Name)
        {
            res2 = ListResult[Random.Range(0, ListResult.Count)];
        }
        Kolres1 = Random.Range(res.MinKol,res.MaxKol+1);
        Kolres2 = Random.Range(res2.MinKol,res2.MaxKol+1);
        //смена текста в 1 слоте
        slot1.transform.GetChild(2).GetComponent<Image>().sprite = res.IconResurs;
        LT.key = res.Name;
        slot1.transform.GetChild(1).GetComponent<TMP_Text>().text = LocalizationManager.Instance.GetText(res.Name) + Kolres1;
        //смена текста в 2 слоте
        slot2.transform.GetChild(2).GetComponent<Image>().sprite = res2.IconResurs;
        LT2.key = res2.Name;
        slot2.transform.GetChild(1).GetComponent<TMP_Text>().text = LocalizationManager.Instance.GetText(res2.Name) + Kolres2;
    }
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.A) && EMS.OpenMenu != true && DAN.AC.Activ == true)
        {
            if(res.Name == "ES")
            {
                ES.RealEnergi += Kolres1;
            }
            if(res.Name == "Tem")
            {
                Tem.Drofa += Kolres1;
            }
            if(res.Name == "MS")
            {
                MS.RealMoral += Kolres1;
            }
            DAN.DestroyNPC();
        }
        else if(Input.GetKeyDown(KeyCode.D) && EMS.OpenMenu != true && DAN.AC.Activ == true)
        {
            if(res.Name == "ES")
            {
                ES.RealEnergi += Kolres2;
            }
            if(res.Name == "Tem")
            {
                Tem.Drofa += Kolres2;
            }
            if(res.Name == "MS")
            {
                MS.RealMoral += Kolres2;
            }
            DAN.DestroyNPC();
        }
    }
}