using JetBrains.Annotations;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Temperatura : MonoBehaviour
{
    public float MinTempera;
    public float MaxTempera;
    public float RealTempera;
    public ActivCode AC;
    public float TimeGoren;
    public int Drofa;
    public GameObject[] DrofaObject;
    public float SpeadFrost;
    public TMP_Text teXt;
    public float MaxTimeToDead;
    public float RealTimeToDead;
    public Animator anim;
    public Animator animPec;
    public AudioSource AS;

    public void FixedUpdate()
    {
        for(int i = 0; i < 10; i++)
        {
            if(i < Drofa)
            {
                DrofaObject[i].SetActive(true);
            }
            else
            {
                DrofaObject[i].SetActive(false);
            }
        }
        teXt.text = Drofa.ToString() + "/" + RealTempera.ToString("F2") +"C";
        if(AC.Activ == true && Drofa > 0)
        {
            TimeGoren += 60;
            RealTempera += 5;
            Drofa -= 1;
            AC.Activ = false;
        }
        if(TimeGoren > 0)
        {
            TimeGoren -= Time.deltaTime;
            animPec.SetBool("Gor", true);
            if(RealTempera < MaxTempera)
            {
                RealTempera += 0.1f;
            }
            if(AS.isPlaying == false)
            {
                AS.Play();
            }
        }
        else
        {
            animPec.SetBool("Gor", false);
            if(RealTempera > MinTempera)
            {
                RealTempera -= SpeadFrost;
            }
            if(AS.isPlaying == true)
            {
                AS.Stop();
            }
        }
        if(RealTempera <= MinTempera)
        {
            anim.SetBool("Mor",true);
            RealTimeToDead += Time.deltaTime;
        }
        else
        {
            anim.SetBool("Mor",false);
        }
        if(RealTimeToDead >= MaxTimeToDead)
        {
            //dead
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}