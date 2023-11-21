using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class LatencyManager : MonoBehaviour
{
    [FoldoutGroup("패널")] 
    [SerializeField]
    private GameObject startPanel;
    [FoldoutGroup("패널")] 
    public GameObject endPanel;
    [FoldoutGroup("패널")] 
    [SerializeField]
    private GameObject keyPanel;
    [FoldoutGroup("패널")] 
    [SerializeField]
    private Text latencyText;
    
    [FoldoutGroup("오브젝트")] 
    public GameObject[] latencyNoteList = new GameObject[10];
    [FoldoutGroup("오브젝트")] 
    [SerializeField]
    private GameObject latencyNote;
    [FoldoutGroup("오브젝트")] 
    [SerializeField]
    private GameObject latencyFolder;

    [FoldoutGroup("변수")] 
    [Title("종료 여부")] 
    public bool isDone = false;
    [FoldoutGroup("변수")] 
    [Title("시작 여부")] 
    public bool isStart = false;
    
    public static LatencyManager instance;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        startPanel.SetActive(true);
        
        for (int i = 1; i <= 10; i++)
        {
            var obj = Instantiate(latencyNote, latencyFolder.transform, true);
            obj.transform.position = transform.forward * 8 * i + transform.forward;
            latencyNoteList[i - 1] = obj;
        }
    }

    public void LatencyStart()
    {
        isStart = true;
        LatencyAudioManager.instance.audioBGM.Play();
        startPanel.SetActive(false);
        keyPanel.SetActive(true);
    }

    public void ShowEndPanel(int latencyAvg)
    {
        isDone = true;
        latencyText.text = "기기 오프셋: " + latencyAvg + "ms";
        endPanel.SetActive(true);
        DBManager.instance.latency = latencyAvg;
        keyPanel.SetActive(false);
    }
}