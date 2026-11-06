using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class MaikSistem : MonoBehaviour
{
    public float RealGradus;
    public float MaxGradus;
    public float MinGragus;
    public ActivCode AC;
    public ActivCode ACMaik;
    public GameObject boxCollider;
    public CameraMove CM;
    public GameObject Maik;
    public TMP_Text teXt;
    private bool previousActivState;
    public Animator anim;
    public AudioSource AS;

    void Start()
    {
        // Инициализируем начальное состояние
        previousActivState = AC.Activ;
    }
    public void FixedUpdate()
    {
        boxCollider.GetComponent<BoxCollider>().enabled = ACMaik.Activ;
        if (AC.Activ != previousActivState)
        {
            // Состояние изменилось - меняем CM.canMove один раз
            CM.canMove = !AC.Activ;
            previousActivState = AC.Activ; // Обновляем сохраненное состояние
        }
        if(AC.Activ == true)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                AC.Put();
            }
            teXt.text = RealGradus.ToString("F0");
            if(Input.GetKey(KeyCode.D) == true)
            {
                RealGradus += 0.05f;
                if(AS.isPlaying == false)
                {
                    AS.Play();
                }
                anim.SetBool("Right", true);
                anim.SetBool("Left", false);
                anim.speed = 1f;
            }
            else if(Input.GetKey(KeyCode.A) == true)
            {
                RealGradus -= 0.05f;
                if(AS.isPlaying == false)
                {
                    AS.Play();
                }
                anim.SetBool("Left", true);
                anim.SetBool("Right", false);
                anim.speed = 1f;
            }
            else
            {
                if(AS.isPlaying == true)
                {
                    AS.Stop();
                }
                anim.speed = 0f;
                anim.SetBool("Left", false);
                anim.SetBool("Right", false);
            }
        }
        if(AC.Activ == false)
        {
            if(AS.isPlaying == false)
            {
                AS.Stop();
            }
        }
        if(RealGradus > MaxGradus)
        {
            RealGradus = 0;
        }
        if(RealGradus < MinGragus)
        {
            RealGradus = 360;
        }
        Maik.transform.rotation = Quaternion.Euler(0f, RealGradus + 90f, 90f);
    }
}