using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LatencyManager : MonoBehaviour
{
    [FoldoutGroup("패널")] 
    [SerializeField]
    private GameObject countDownPanel;
    
    [FoldoutGroup("오브젝트")] 
    public GameObject[] latencyNoteList = new GameObject[10];
    [FoldoutGroup("오브젝트")] 
    [SerializeField]
    private GameObject latencyNote;
    [FoldoutGroup("오브젝트")] 
    [SerializeField]
    private GameObject latencyFolder;

    [FoldoutGroup("플레이어")] 
    [SerializeField]
    private LobbyPlayerController playerController;
    [FoldoutGroup("변수")]
    [SerializeField]
    private bool isCountDown;
    [FoldoutGroup("변수")] 
    [Title("종료 여부")] 
    public bool isDone = false;
    
    public static LatencyManager instance;
    private WaitForSeconds waitForSeconds = new WaitForSeconds(1f);
    
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

    IEnumerator CountDown()
    {
        if (LatencyAudioManager.instance.audioSource.clip.loadState == AudioDataLoadState.Unloaded)
        {
            LatencyAudioManager.instance.audioSource.clip.LoadAudioData();
        }

        isCountDown = true;
        playerController.GetComponent<Animator>().SetBool("isCountDown", isCountDown);
        countDownPanel.gameObject.SetActive(true);
        LatencyAudioManager.instance.audioSource.Pause();
        playerController.GetComponent<PlayerInput>().enabled = false;

        int i = 4;
        
        while (i > 0)
        {
            countDownPanel.transform.GetChild(0).GetComponent<Text>().text = i == 4 ? "Ready" : i.ToString();

            i--;
            yield return waitForSeconds;
        }

        countDownPanel.transform.GetChild(0).GetComponent<Text>().text = "GO!";
        Invoke(nameof(CountDownDisable), 0.5f);
    }

    void CountDownDisable()
    {
        if (playerController.enabled)
        {
            countDownPanel.gameObject.SetActive(false);
            isCountDown = false;
            playerController.GetComponent<Animator>().SetBool("isCountDown", isCountDown);
            playerController.GetComponent<PlayerInput>().enabled = true;
        }
    }

    public void ShowEndWindow()
    {
        isDone = true;
        Debug.Log(DBManager.instance.latency);
    }
}