using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;

public class DoorAndNPC : MonoBehaviour
{
    public ActivCode AC;
    public CameraMove CM;
    public bool previousActivState;
    public List<GameObject> NpcList = new List<GameObject>();
    public Transform DoorPoint;
    public Transform DoorCamera;
    public Transform PlayerCamera;
    public GameObject CameraPla;
    public GameObject UIDoor;
    public ECSMenuScript EMS;

    void Start()
    {
        // Инициализируем начальное состояние
        if (AC != null)
        {
            previousActivState = AC.Activ;
            if (CM != null) CM.canMove = !AC.Activ;
        }
    }
    public void AddNps(GameObject gmNpc)
    {
        NpcList.Add(gmNpc);
    }
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && AC.Activ == true && EMS.OpenMenu == false)
        {
            AC.Activ = false;
        }
        if (AC != null && AC.Activ != previousActivState)
        {
            
            // Состояние изменилось - меняем CM.canMove один раз
            if (CM != null) CM.canMove = !AC.Activ;
            if(AC.Activ == false)
            {
                CameraPla.transform.SetParent(PlayerCamera);
                CameraPla.transform.localPosition = new Vector3(0f,0.323f,0f);
                UIDoor.SetActive(false);
            }
            else
            {
                CameraPla.transform.SetParent(DoorCamera);
                CameraPla.transform.localPosition = Vector3.zero;
                CameraPla.transform.localRotation = Quaternion.identity;
                UIDoor.SetActive(true);
            }
            previousActivState = AC.Activ;
        }
        if(DoorPoint.childCount == 0 && NpcList.Count > 0 && AC.Activ == true)
        {
            Instantiate(NpcList[0],DoorPoint);
        }
        if(DoorPoint.childCount == 0 && NpcList.Count == 0)
        {
            AC.Activ = false;
        }
        
    }
    public void DestroyNPC()
    {
        if(DoorPoint.childCount > 0)
        {
            NpcList.RemoveAt(0);
            Destroy(DoorPoint.GetChild(0).gameObject);
        }
    }
}
