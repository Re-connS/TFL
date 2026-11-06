using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;

public class TorgNPC : MonoBehaviour
{
    public DoorAndNPC DAN;
    public GameObject DialogUI;
    public EnergiSistem ES;
    public Temperatura Temper;
    public int NumPredlog;
    [System.Serializable]
    public struct Tovar
    {
        public string nameTovar;
        public float KolTovar;
        public string nameCena;
        public float KolCena;
    }
    public List<Tovar> TovarList = new List<Tovar>();
    public void Start()
    {
        DialogUI = GameObject.Find("CanvasMain").transform.GetChild(3).gameObject;;
        DAN = GameObject.Find("Door").GetComponent<DoorAndNPC>();
        ES = GameObject.Find("Generator").GetComponent<EnergiSistem>();
        Temper = GameObject.Find("Perhi").GetComponent<Temperatura>();
        NumPredlog = Random.Range(0, TovarList.Count-1);
        if(TovarList[NumPredlog].nameCena == "Energi")
        {
            DialogUI.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<TMP_Text>().text =  $"{(int)TovarList[NumPredlog].KolCena}";
            DialogUI.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<TMP_Text>().text =  $"{(int)TovarList[NumPredlog].KolTovar}";
        }
        if(TovarList[NumPredlog].nameCena == "Drofa")
        {
            DialogUI.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<TMP_Text>().text =  $"{(int)TovarList[NumPredlog].KolCena}";
            DialogUI.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<TMP_Text>().text =  $"{(int)TovarList[NumPredlog].KolTovar}";
        }
    }
    public void Update()
    {
        if(DAN.previousActivState == true && DialogUI.activeInHierarchy == false)
        {
            DialogUI.SetActive(true);
        }
        if(DialogUI.activeInHierarchy == true)
        {
            if(Input.GetKeyDown(KeyCode.D))
            {
                Pokupka(NumPredlog);
                DialogUI.SetActive(false);
                DAN.DestroyNPC();
            }
            else if(Input.GetKeyDown(KeyCode.A))
            {
                DialogUI.SetActive(false);
                DAN.DestroyNPC();
            }
        }
    }
    public void Pokupka(int num)
    {
        if(TovarList[num].nameCena == "Energi")
        {
            if(ES.RealEnergi - TovarList[num].KolCena >= 0)
            {
                Temper.Drofa += (int)TovarList[num].KolTovar;
                return;
            }
        }
        if(TovarList[num].nameCena == "Drofa")
        {
            if(Temper.Drofa - TovarList[num].KolCena >= 0)
            {
                ES.RealEnergi += (int)TovarList[num].KolTovar;
                return;
            }
        }
    }
}
