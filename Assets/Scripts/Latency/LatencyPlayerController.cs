using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class LatencyPlayerController : MonoBehaviour
{
    [FoldoutGroup("음악")]
    [Title("BPM")]
    [SerializeField]
    private float bpm;
    
    [FoldoutGroup("애니메이터")]
    [SerializeField]
    private Animator playerAnimator;
    
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
    private int count = -1;
    [FoldoutGroup("변수")]
    [Title("시작 시점")]
    [SerializeField]
    private int startTime = 0;

    void Start()
    {
        playerAnimator.SetBool("isCountDown", true);
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
            if (count > 20)
            {
                LatencyManager.instance.ShowEndPanel(latencyAvg);
                Debug.Log("home");
            }
            return;
        }
        
        if (!LatencyManager.instance.isStart && count == -1)
        {
            playerAnimator.SetBool("isCountDown", false);
            LatencyManager.instance.LatencyStart();
            startTime = (int)(Time.time * 1000);
            count++;
            return;
        }
        
        int latencyDelta = latency;
        LatencyManager.instance.latencyNoteList[count % 10].transform.position += Vector3.forward * 80;
        count++;
        latency = (int)(Time.time * 1000) - startTime - (int)(60000f / bpm) * count;
        latencyDelta -= latency;
        latencySum += latency;
        transform.position += transform.forward * (latencyDelta * bpm / 7500f);
        
        if (count > 20)
        {
            playerAnimator.SetBool("isCountDown", true);
            latencyAvg = latencySum / count;
            LatencyAudioManager.instance.StopAudio();
        }
    }
}
