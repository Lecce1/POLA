using Sirenix.OdinInspector;
using UnityEngine;

public class LatencyManager : MonoBehaviour
{
    [FoldoutGroup("패널")] 
    [SerializeField]
    private GameObject startPanel;
    
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
    }

    public void ShowEndWindow()
    {
        isDone = true;
        Debug.Log(DBManager.instance.latency);
    }
}