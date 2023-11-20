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

    private WaitForSeconds second = new (1f);

    void Start()
    {
        Physics.gravity = new Vector3(0, -9.81f, 0);
        bpm = LatencyAudioManager.instance.bpm;
    }

    void FixedUpdate()
    {
        transform.position += transform.forward * (bpm / 7.5f * Time.fixedDeltaTime);
    }

    void OnDown()
    {
        LatencyManager.instance.latencyNoteList[count % 10].transform.position += Vector3.forward * 80;
        latency = 500 * ++count - (int)(Time.time * 1000);
        latencySum += latency;
        latencyAvg = latencySum / count;
        DBManager.instance.latency = latencyAvg;
        //transform.position += transform.forward * 7500 / (latency * bpm);
    }
}
