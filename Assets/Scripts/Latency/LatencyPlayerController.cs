using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LatencyPlayerController : MonoBehaviour
{
    [FoldoutGroup("음악")]
    [Title("BPM")]
    [SerializeField]
    private float bpm;
    
    [FoldoutGroup("애니메이터")]
    public Animator animator;
    
    [FoldoutGroup("레이턴시")]
    [Title("현재 레이턴시")]
    [SerializeField]
    private int latency;
    [FoldoutGroup("레이턴시")]
    [Title("레이턴시 총합")]
    [SerializeField]
    private int latencySum;
    [FoldoutGroup("레이턴시")]
    [Title("레이턴시 평균")]
    [SerializeField]
    private int latencyAvg;
    
    [FoldoutGroup("변수")] 
    [Title("누른 횟수")] 
    [SerializeField]
    private int count;
    [FoldoutGroup("변수")] 
    [SerializeField]
    private int passedBeat = 0;
    [FoldoutGroup("변수")] 
    [SerializeField]
    private bool wasClickedThisTime = false;


    void Start()
    {
        Init();
    }

    void FixedUpdate()
    {
        if (!LatencyManager.instance.isFinish && LatencyManager.instance.isStart)
        {
            transform.position += transform.forward * (bpm / 7.5f * Time.fixedDeltaTime);
        }
    }
    
    void Init()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("isReady", true);
        Physics.gravity = new Vector3(0, -9.81f, 0);
        bpm = LatencyAudioManager.instance.bpm;
        latency = 0;
    }

    public void OnDown()
    {
        if (LatencyManager.instance.startPanel.activeSelf || LatencyManager.instance.isFinish || wasClickedThisTime)
        {
            return;
        }
        
        int latencyDelta = latency;
        int sampledTime = LatencyAudioManager.instance.GetSampledTime();
        latency = sampledTime % (int)(60000f / bpm);
        latencyDelta -= latency;
        latencySum += latency;
        transform.position += transform.forward * (latencyDelta * bpm / 7500f);
        count++;
        wasClickedThisTime = true;
        Debug.Log(latency);
    }

    public void BeatUpdate()
    {
        if (LatencyManager.instance.isFinish)
        {
            return;
        }

        passedBeat++;
        wasClickedThisTime = false;
        
        if (passedBeat > 20)
        {
            animator.SetBool("isReady", true);
            latencyAvg = latencySum / count;
            LatencyAudioManager.instance.audioBGM.Pause();
            LatencyManager.instance.Finish(count, latencyAvg);
        }
    }
    
    public void OnClick()
    {
        if (EventSystem.current.currentSelectedGameObject != null && (LatencyManager.instance.esc.activeSelf || LatencyManager.instance.set.activeSelf) && GetComponent<PlayerInput>().currentControlScheme != "MOBILE")
        {
            if (LatencyManager.instance.set.activeSelf)
            {
                switch (EventSystem.current.currentSelectedGameObject.name)
                {
                    case "Toggle":
                        if (EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>().isOn)
                        {
                            EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>().isOn = false;
                            DBManager.instance.isVibration = false;
                        }
                        else if (!EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>().isOn)
                        {
                            EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>().isOn = true;
                            DBManager.instance.isVibration = true;
                        }
                        break;
                }
            }
            else
            {
                EventSystem.current.currentSelectedGameObject.GetComponent<Button>().onClick.Invoke();
            }
        }
    }
    
    public void OnCancel()
    {
        if (!LatencyManager.instance.isResultPanelOpen)
        {
            if (!LatencyManager.instance.isPanelOpen)
            {
                LatencyManager.instance.Button("Esc");
            }
            else
            {
                LatencyManager.instance.Back();
            }
        }
    }
}
