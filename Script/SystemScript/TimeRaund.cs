using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeRaund : MonoBehaviour
{
    public  Animator anim;
    public float MaxHours;
    public float Minut;
    public float Hour;
    public int Day;
    public TMP_Text TimeText;

    public void FixedUpdate()
    {
        if(Hour < MaxHours && Minut < 60)
        {
            Minut += Time.deltaTime;
            TimeText.text = "Day" + Day + "/" + Hour + ":" + (int)Minut;
        }
        if(Minut >= 60)
        {
            Hour += 1;
            Minut = 0;
            TimeText.text = "Day" + Day + "/" + Hour + ":" + (int)Minut;
        }
        if(Hour >= MaxHours)
        {
            anim.SetBool("EndDay",true);
        }
    }
    public void NextDay()
    {
        Hour = 6;
        Day += 1;
        TimeText.text = "Day" + Day + "/" + Hour + ":" + (int)Minut;
        anim.SetBool("EndDay",false);
    }
}