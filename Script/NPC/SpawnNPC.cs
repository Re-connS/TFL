using UnityEngine;
using System.Linq;
using System.Collections.Generic;
public class SpawnNPC : MonoBehaviour
{
    public TimeRaund TR;
    public Vector3 Point1;
    public Vector3 Point2;
    private float _lastHour;
    private float _totalSpawnWeight; //Значение для рандома
    [System.Serializable]
    public struct Spawnable
    {
        public GameObject gameObject;
        public float weight;
    }
    public List<Spawnable> spawnList;
    void Start()
    {
        _totalSpawnWeight = spawnList.Sum(spawnable => spawnable.weight);
    }
    public void FixedUpdate()
    {
        if((int)TR.Minut % 30 == 0 && (int)TR.Minut != _lastHour)
        {
            _lastHour = (int)TR.Minut;
            float pick = Random.value * _totalSpawnWeight;
            float cumulativeWeight = 0;
            foreach (var spawnable in spawnList)
            {
                cumulativeWeight += spawnable.weight;
                Vector3 tes = new Vector3(Random.Range(Point1.x,Point2.x),Random.Range(Point1.y,Point2.y),Random.Range(Point1.z,Point2.z));
                if (pick <= cumulativeWeight)
                {
                    Instantiate(spawnable.gameObject, tes, Quaternion.identity);
                }
                return;
            }   
        }
    }
}