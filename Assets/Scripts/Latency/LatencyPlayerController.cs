using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class LatencyPlayerController : MonoBehaviour
{
    [FoldoutGroup("음악")]
    [Title("BPM")]
    [SerializeField]
    private float bpm;
    [FoldoutGroup("변수")]
    [Title("레이턴시")]
    [SerializeField]
    private int latency = 0;
    [FoldoutGroup("변수")]
    [SerializeField]
    private int latencySum = 0;
    [FoldoutGroup("변수")]
    [SerializeField]
    private int latencyAvg = 0;
    [FoldoutGroup("변수")]
    [Title("누른 횟수")]
    [SerializeField]
    private int count = 0;
    [FoldoutGroup("변수")]
    [Title("시작 시점")]
    [SerializeField]
    private float startTime = 0;

    void Start()
    {
        Physics.gravity = new Vector3(0, -9.81f, 0);
        bpm = LatencyAudioManager.instance.bpm;
        latency = 0;
    }
    
    void FixedUpdate()
    {
        if (!LatencyManager.instance.isDone && LatencyManager.instance.isStart)
        {
            transform.position += transform.forward * (bpm / 7.5f * Time.fixedDeltaTime);
        }
    }

    void OnDown()
    {
        if (LatencyManager.instance.isDone)
        {
            return;
        }

        int latencyDelta = latency;
        
        if (!LatencyManager.instance.isStart && count == 0)
        {
            LatencyManager.instance.LatencyStart();
            startTime = Time.time;
            latencyDelta = (int)-startTime * 1000;
        }
        
        
        LatencyManager.instance.latencyNoteList[count % 10].transform.position += Vector3.forward * 80;
        latency = (int)startTime + (int)(60000f / bpm) * ++count - (int)(Time.time * 1000);
        latencyDelta -= latency;
        Debug.Log(latencyDelta);
        latencySum += latency;
        latencyAvg = latencySum / count;
        transform.position -= transform.forward * (latencyDelta * bpm / 7500f);

        if (count > 20)
        {
            LatencyManager.instance.ShowEndWindow();
            LatencyAudioManager.instance.StopAudio();
        }
    }
}
