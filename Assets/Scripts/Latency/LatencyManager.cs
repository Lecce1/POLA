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
    private int currentIndex;
    [FoldoutGroup("변수")]
    [SerializeField]
    private bool isCountDown;
    
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
        currentIndex = 0;
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
            if (i == 4)
            {
                countDownPanel.transform.GetChild(0).GetComponent<Text>().text = "Ready";
            }
            else
            {
                countDownPanel.transform.GetChild(0).GetComponent<Text>().text = i.ToString();
            }

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
}