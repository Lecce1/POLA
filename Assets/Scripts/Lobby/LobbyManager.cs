using System.Collections.Generic;
using System.Diagnostics;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class LobbyManager : MonoBehaviour
{
    [FoldoutGroup("패널")] 
    [Title("설정")] 
    public GameObject set;
    [FoldoutGroup("패널")] 
    [Title("상점")] 
    public GameObject shop;
    [FoldoutGroup("패널")] 
    [Title("표지판")] 
    public GameObject sign;
    [FoldoutGroup("패널")] 
    [Title("에러_네트워크")] 
    public GameObject error_Network;
    [FoldoutGroup("패널")] 
    [Title("입장 버튼")] 
    public GameObject join_Btn;

    [FoldoutGroup("텍스트")] 
    [Title("스테이지 이름 버튼")]
    public Text stage_Name_Text;
    [FoldoutGroup("텍스트")] 
    [Title("입장 버튼")] 
    public Text join_Btn_Text;

    [FoldoutGroup("기타")] 
    [Title("입장 버튼 타입")] 
    [SerializeField]
    public string join_Btn_Type;
    [FoldoutGroup("기타")] 
    [Title("입장 버튼 온오프 여부")] 
    [SerializeField]
    public bool isJoinBtnOn = false;
    [FoldoutGroup("기타")] 
    [Title("패널 열림 여부")] 
    [SerializeField]
    public bool isPanelOpen = false;
    [FoldoutGroup("기타")]
    [Title("경쟁모드 버튼 클릭 여부")] 
    [SerializeField]
    private bool isMatching = false;

    // 뒤로가기 스택
    private Stack<GameObject> backStack;
    public static LobbyManager instance;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        backStack = new Stack<GameObject>();
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
        Debug.Log(EventSystem.current.currentSelectedGameObject);
    }

    void Init()
    {
        switch (DBManager.instance.currentStageNum)
        {
            case 0:
                LobbyPlayerController.instance.player.transform.position = DBManager.instance.stage0_Pos;
                break;
            
            case 1:
                LobbyPlayerController.instance.player.transform.position = DBManager.instance.stage1_Pos;
                break;
            
            case 2:
                LobbyPlayerController.instance.player.transform.position = DBManager.instance.stage2_Pos;
                break;
        }
    }

    public void DoorInit(string name, string btnText, string nameText, bool isLock)
    {
        if (!isLock)
        {
            join_Btn_Type = name;
            join_Btn.GetComponent<Button>().onClick.RemoveAllListeners();
            join_Btn.GetComponent<Button>().onClick.AddListener(() => LobbyManager.instance.Button(name));
            join_Btn_Text.text = btnText;
            stage_Name_Text.text = nameText;
            Join_Btn_OnOff(true, false);
        }
        else
        {
            stage_Name_Text.text = nameText;
            Join_Btn_OnOff(true, true);
        }
    }

    public void Join_Btn_OnOff(bool isOn, bool onlyStage)
    {
        if (isOn == true)
        {
            if (!onlyStage && join_Btn.activeSelf)
            {
                join_Btn.GetComponent<Animator>().Play("JoinOn");
            }
            
            stage_Name_Text.GetComponent<Animator>().Play("StageNameOn");
            isJoinBtnOn = true;
        }
        else if (isOn == false)
        {
            if (!onlyStage && join_Btn.activeSelf)
            {
                join_Btn.GetComponent<Animator>().Play("JoinOff");
            }
            
            stage_Name_Text.GetComponent<Animator>().Play("StageNameOff");
            isJoinBtnOn = false;
        }
    }

    public void Button(string type)
    {
        switch (type)
        {
            case "Stage":
                if (join_Btn.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
                {
                    if (DBManager.instance != null)
                    {
                        DBManager.instance.nextScene = "Track";
                    }

                    SceneManager.LoadScene("Loading");
                }
                break;

            case "Set":
                if (!set.activeSelf)
                {
                    if (join_Btn.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
                    {
                        set.SetActive(true);
                        backStack.Push(set);
                        isPanelOpen = true;

                        if (join_Btn.activeSelf)
                        {
                            join_Btn.SetActive(false);
                        }
                    }
                }
                break;
            
            case "Shop":
                if (!shop.activeSelf)
                {
                    if (join_Btn.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
                    {
                        shop.SetActive(true);
                        backStack.Push(shop);
                        isPanelOpen = true;
                    
                        if (join_Btn.activeSelf)
                        {
                            join_Btn.SetActive(false);
                        }
                    }
                }
                break;
            
            case "Sign":
                if (!sign.activeSelf)
                {
                    if (join_Btn.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
                    {
                        sign.SetActive(true);
                        backStack.Push(sign);
                        isPanelOpen = true;
                    
                        if (join_Btn.activeSelf)
                        {
                            join_Btn.SetActive(false);
                        }
                    }
                }
                break;
        }
    }
    
    public void Back()
    {
        if (backStack.Count <= 0)
        {
            Debug.LogError("뒤로가기 패널이 비었습니다.");
            return;
        }

        bool isCheck = false;
        
        switch (backStack.Pop().name)
        {
            case "Set":
                if (set.activeSelf)
                {
                    set.SetActive(false);
                    isCheck = true;
                }
                break;
            
            case "Shop":
                if (shop.activeSelf)
                {
                    shop.SetActive(false);
                    isCheck = true;
                }
                break;
            
            case "Sign":
                if (sign.activeSelf)
                {
                    sign.SetActive(false);
                    isCheck = true;
                }
                break;
        }

        if (isCheck == true)
        {
            isPanelOpen = false;
                    
            if (join_Btn.activeSelf == false)
            {
                join_Btn.SetActive(true);
                
                if (!LobbyPlayerController.instance.isDoor)
                {
                    Join_Btn_OnOff(false, false);
                }
            }
        }
    }
}
