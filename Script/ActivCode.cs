using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ActivCode : MonoBehaviour
{
    public bool Activ;
    public LocalizedText LT;
    public string KeyLoc;
    public AudioSource AS;
    public Animator anim;
    public ECSMenuScript EMS;
    public string AnimBool;
    public CameraMove CM;
    public void Put()
    {
        if(Activ == false)
        {
            Activ = true;
            if(CM != null)
            {
                CM.canMove = false;
            }
            if(LT != null && KeyLoc != "")
            {
                LT.key = KeyLoc;
                LT.UpdateText();
            }
            if(AS != null)
            {
                AS.Play();
            }
            if(anim != null)
            {
                anim.SetBool(AnimBool,true);
            }
            if(EMS != null)
            {
                EMS.OpenOtherMenu = true;
            }
        }
        else
        {
            if(CM != null)
            {
                CM.canMove = true;
            }
            if(LT != null)
            {
                LT.key = "";
            }
            if(AS != null)
            {
                AS.Play();
            }
            if(anim != null)
            {
                anim.SetBool(AnimBool,false);
            }
            if(EMS != null)
            {
                EMS.OpenOtherMenu = false;
                if(EMS.OpenOtherMenu == false)
                {
                    Activ = false; 
                }
            }
            else
            {
                Activ = false;
            }
        }
    }
}
