using UnityEngine;

public class ECSMenuScript : MonoBehaviour
{
    public bool OpenMenu;
    public bool OpenOtherMenu;
    public GameObject UIEcs;
    public CameraMove CM;

    public void Update()
    {
        if (Input.GetKeyUp("escape"))
        {
            EcsBut();
        }
    }
    public void EcsBut()
    {
        if(OpenMenu == true)
        {
            if(OpenOtherMenu == false)
            {
                CM.canMove = true;
            }
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Debug.Log("Close");
            UIEcs.SetActive(false);
            OpenMenu = false;
        }
        else if(OpenMenu == false)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            CM.canMove = false;
            Debug.Log("Open");
            UIEcs.SetActive(true);
            OpenMenu = true;
        }
    }
    public void SmenaLaught()
    {
        Debug.Log("Smena");
    }
    public void ExitGame()
    {
        Debug.Log("Exit");
        Application.Quit();
    }
}
