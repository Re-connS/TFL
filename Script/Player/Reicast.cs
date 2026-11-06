using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
public class Reicast :  MonoBehaviour
{
    public Camera mainCamera;
    public TMP_Text Key;
    public RaycastHit hit;


    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.GetComponentInParent<UnityEngine.UI.Button>() != null)
            {
                UnityEngine.UI.Button button = hit.collider.GetComponentInParent<UnityEngine.UI.Button>();
                if (button != null)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (Physics.Raycast(ray, out hit))
                        {
                            button.onClick.Invoke();
                        }
                    }
                }
            }
            else if (hit.collider.gameObject.GetComponent<ActivCode>() != null || hit.collider.gameObject.tag == "Item")
            {
                Key.text = "F";
                if (Input.GetKeyDown(KeyCode.F) && hit.collider.gameObject.GetComponent<ActivCode>() != null)
                {
                    hit.collider.gameObject.GetComponent<ActivCode>().Put();
                }
            }
            else
            {
                Key.text = null;
            }
        }
    }    
}