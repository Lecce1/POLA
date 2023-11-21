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
    
    [FoldoutGroup("변수")]
    [Title("레이턴시")]
    [SerializeField]
    private int latency;
    [FoldoutGroup("변수")]
    [SerializeField]
    private int latencySum;
    [FoldoutGroup("변수")]
    [SerializeField]
    private int latencyAvg;

    [FoldoutGroup("변수")] 
    [Title("누른 횟수")] 
    [SerializeField]
    private int count;
    [FoldoutGroup("변수")]
    [Title("시작 시점")]
    public int startTime;

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
        animator.SetBool("isCountDown", true);
        Physics.gravity = new Vector3(0, -9.81f, 0);
        bpm = LatencyAudioManager.instance.bpm;
        latency = 0;
    }

    public void OnDown()
    {
        if (!LatencyManager.instance.isFinish && count > 20)
        {
            LatencyManager.instance.Finish(latencyAvg);
            return;
        }

        if (count == -1)
        {
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
            animator.SetBool("isCountDown", true);
            latencyAvg = latencySum / count;
            LatencyAudioManager.instance.audioBGM.Pause();
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
        if (!LatencyManager.instance.isResultPanel)
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
