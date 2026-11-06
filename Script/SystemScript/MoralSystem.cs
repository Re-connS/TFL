using UnityEngine;

public class MoralSystem : MonoBehaviour
{
    public float RealMoral;
    public float MaxMoral;
    public float ProcentMoral;
    public float TimeEvent;
    private float _lastRealMoral;
    [System.Serializable]
    public class LighMorStat
    {
        public Light ligh;
        public float MaxIner;
        public float MaxOutput;
        public float MaxSilaSveta;
    }
    public LighMorStat[] LighMorStatsData;
    [System.Serializable]
    public class ACDate
    {
        public ActivCode AC;
        public float MaxEventTime;
        public float ChansProk;
        public float MinMoral;
    }
    public ACDate[] acDates;

    public void Start()
    {
        for(int i = 0; i < LighMorStatsData.Length; i++)
        {
            if(LighMorStatsData[i].ligh.type == LightType.Spot)
            {
                LighMorStatsData[i].MaxIner = LighMorStatsData[i].ligh.innerSpotAngle;
                LighMorStatsData[i].MaxOutput = LighMorStatsData[i].ligh.spotAngle;
            }
            else if(LighMorStatsData[i].ligh.type == LightType.Point)
            {
                LighMorStatsData[i].MaxSilaSveta = LighMorStatsData[i].ligh.range;
            }
        }
    }
    public void Update()
    {
        if(RealMoral > MaxMoral)
            RealMoral = MaxMoral;

        ProcentMoral = MaxMoral > 0 ? (RealMoral / MaxMoral) * 100f : 0f;

        if (!Mathf.Approximately(RealMoral, _lastRealMoral))
        {
            _lastRealMoral = RealMoral;

            for(int i = 0; i < LighMorStatsData.Length; i++)
            {
                if(LighMorStatsData[i].ligh.type == LightType.Spot)
                {
                    LighMorStatsData[i].ligh.innerSpotAngle = LighMorStatsData[i].MaxIner / 100 * ProcentMoral;
                    LighMorStatsData[i].ligh.spotAngle    = LighMorStatsData[i].MaxOutput / 100 * ProcentMoral;
                }
                else if(LighMorStatsData[i].ligh.type == LightType.Point)
                {
                    LighMorStatsData[i].ligh.range = LighMorStatsData[i].MaxSilaSveta / 100 * ProcentMoral;
                }
            }
        }

        TimeEvent += Time.deltaTime;
        for(int i = 0; i < acDates.Length; i++)
        {
            if(acDates[i].MaxEventTime < TimeEvent && Random.Range(0f, 100f) < acDates[i].ChansProk && acDates[i].MinMoral > RealMoral)
            {
                acDates[i].AC.Put();
                TimeEvent = 0;
                break;
            }
        }
    }
}
