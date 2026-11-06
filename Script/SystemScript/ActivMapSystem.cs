using UnityEngine;

public class ActivMapSystem : MonoBehaviour
{
    public ActivCode AC1;
    public ActivCode AC2;
    public GameObject Map1;
    public GameObject Map2;

    public void Update()
    {
        if(AC1.Activ == true)
        {
            Map1.SetActive(true);
        }
        else
        {
            Map1.SetActive(false);
        }
        if(AC2.Activ == true)
        {
            Map2.SetActive(true);
        }
        else
        {
            Map2.SetActive(false);
        }
    }
}
