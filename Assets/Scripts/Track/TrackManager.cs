using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TrackManager : MonoBehaviour
{
    [FoldoutGroup("패널")] 
    [Title("Info")] 
    public GameObject info;
    [FoldoutGroup("패널")] 
    [Title("설정")] 
    public GameObject set;
    
    [FoldoutGroup("기타")] 
    [Title("Info 패널 온오프 여부")] 
    [SerializeField]
    public bool isInfoPanelOn = false;
    
    [FoldoutGroup("기타")] 
    [Title("버튼 타입")] 
    [SerializeField]
    public string btn_Type;
    [FoldoutGroup("기타")] 
    [Title("패널 열림 여부")]
    public bool isPanelOpen = false;
    
    [FoldoutGroup("설정 패널")] 
    [Title("언어 Dropdown")]
    public Dropdown set_Language_Dropdown;
    [FoldoutGroup("설정 패널")] 
    [Title("언어 왼쪽 화살표")]
    public GameObject set_Language_LeftArrow;
    [FoldoutGroup("설정 패널")] 
    [Title("언어 오른쪽 화살표")]
    public GameObject set_Language_RightArrow;
    
    [FoldoutGroup("스카이박스")]
    public List<Material> stage_Skybox;
    
    // 뒤로가기 스택
    private Stack<GameObject> backStack;
    public static TrackManager instance;
    
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

    void Init()
    {
        //ChangeSkybox();
    }
    
    public void ChangeSkybox()
    {
        RenderSettings.skybox = stage_Skybox[0];
    }
    
    public void Info_OnOff(bool isOn)
    {
        if (isOn == true)
        {
            info.GetComponent<Animator>().Play("InfoOn");
            isInfoPanelOn = true;
        }
        else if (isOn == false)
        {
            info.GetComponent<Animator>().Play("InfoOff");
            isInfoPanelOn = false;
        }
    }
    
    public void Set_Language()
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[set_Language_Dropdown.value];
        DBManager.instance.language = set_Language_Dropdown.value;

        if (set_Language_Dropdown.value == 0)
        {
            set_Language_LeftArrow.SetActive(false);
            set_Language_RightArrow.SetActive(true);
        }
        else
        {
            set_Language_LeftArrow.SetActive(true);
            set_Language_RightArrow.SetActive(false);
        }
    }
    
    public void Button(string type)
    {
        switch (type)
        {
            case "Start":
                if (info.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f)
                {
                    if (DBManager.instance != null)
                    {
                        DBManager.instance.nextScene = DBManager.instance.gameSceneName;
                    }

                    SceneManager.LoadScene("Loading");
                }
                break;
            
            case "Set":
                if (!set.activeSelf)
                {
                    set.SetActive(true);
                    backStack.Push(set);
                    isPanelOpen = true;
                    set_Language_Dropdown.value = DBManager.instance.language;
                }
                break;
            
            case "Back":
                if (DBManager.instance != null)
                {
                    DBManager.instance.nextScene = "Lobby";
                }

                SceneManager.LoadScene("Loading");
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
        }

        if (isCheck == true)
        {
            isPanelOpen = false;
        }
    }
}
