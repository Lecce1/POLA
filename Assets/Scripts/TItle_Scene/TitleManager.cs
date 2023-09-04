using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using GoogleMobileAds.Api;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [FoldoutGroup("타이틀")]
    [Title("패널")]
    public GameObject title;
    [FoldoutGroup("타이틀")]
    [Title("시작 버튼")]
    public GameObject start_Btn;
    [FoldoutGroup("타이틀")]
    [Title("버전")]
    [SerializeField]
    private Text versionText;
    
    [FoldoutGroup("로그인")]
    [Title("패널")]
    public GameObject login;
    [FoldoutGroup("로그인")]
    [Title("아이디")]
    public InputField loginID;
    [FoldoutGroup("로그인")]
    [Title("비밀번호")]
    public InputField loginPW;
    [FoldoutGroup("로그인")]
    [Title("비밀번호 저장")]
    public Toggle loginRememberMe;
    [FoldoutGroup("로그인")]
    [Title("로그인 버튼")]
    public Button loginBtn;

    [FoldoutGroup("회원가입")]
    [Title("패널")]
    public GameObject signUp;
    [FoldoutGroup("회원가입")]
    [Title("아이디")]
    public InputField signUpID;
    [FoldoutGroup("회원가입")]
    [Title("비밀번호")]
    public InputField signUpPW;
    [FoldoutGroup("회원가입")]
    [Title("닉네임")]
    public InputField signUpNick;
    
    [FoldoutGroup("에러")]
    [Title("패널")]
    public GameObject error;
    [FoldoutGroup("에러")]
    [Title("내용")]
    public Text errorContent;
    [FoldoutGroup("에러")]
    [Title("네트워크 패널")]
    public GameObject errorNetwork;
    
    [FoldoutGroup("정보")]
    [Title("오디오 믹서")]
    public AudioMixer audioMixer;
    [FoldoutGroup("정보")]
    [Title("알림")]
    [SerializeField]
    private int isPush = 0;
    [FoldoutGroup("정보")]
    [Title("진동")]
    [SerializeField]
    private int isVibration = 0;
    [FoldoutGroup("정보")]
    [Title("음악")]
    [SerializeField]
    private float musicValue = 1;
    [FoldoutGroup("정보")]
    [Title("효과음")]
    [SerializeField]
    private float fxValue = 1;
    [FoldoutGroup("정보")]
    [Title("닉네임")]
    [SerializeField]
    private string nickName;
    [FoldoutGroup("정보")]
    [Title("서버 접속 여부")]
    [SerializeField]
    private bool isServer = false;
    [FoldoutGroup("정보")]
    [Title("에러 종류")]
    [SerializeField]
    private string errorType;

    [FoldoutGroup("서버")]
    [Title("소켓")]
    private TcpClient socket;
    [FoldoutGroup("서버")]
    [Title("스트림")]
    public NetworkStream stream;
    [FoldoutGroup("서버")]
    [Title("읽기")]
    public StreamWriter writer;
    [FoldoutGroup("서버")]
    [Title("쓰기")]
    public StreamReader reader;
    
    [FoldoutGroup("광고")]
    [Title("ID")]
    [SerializeField]
    private string adID;
    [FoldoutGroup("광고")]
    [Title("광고")]
    private RewardedAd rewardedAd;
    
    private Stack<GameObject> backStack;

    void Start()
    {
        Application.targetFrameRate = 144;
        versionText.text = "버전 : " + Application.version;
        backStack = new Stack<GameObject>();
        Login_Load(loginRememberMe);
    }

    public void Start_Btn()
    {
        title.SetActive(false);
        start_Btn.SetActive(false);
        ServerConnect();
    }
    
    void ServerConnect()
    {
        try
        {
            /*socket = new TcpClient("192.168.200.167", 12345);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
            Debug.Log("서버 접속 완료");*/
            isServer = true;

            if (loginID.text == "" && loginPW.text == "")
            {
                login.SetActive(true);
            }
            else if (loginID.text != "" && loginPW.text != "")
            {
                Login_Save(loginRememberMe);
                loginBtn.interactable = false;
                StartCoroutine(HttpServerManager.instance.Login(loginID.text, loginPW.text));
                loginBtn.interactable = true;
            }

            loginBtn.interactable = true;
            errorNetwork.SetActive(false);
        }
        catch (Exception e)
        {
            isServer = false;
            errorNetwork.SetActive(true);

            Debug.Log("서버 접속 실패" + e);
            Invoke("ServerConnect", 1.0f);
        }
    }

    public void LoginSuccess()
    {
        Debug.Log("로그인 완료");
        SceneManager.LoadScene("Loading");
    }

    public void LoginFail()
    {
        Debug.Log("로그인 실패");
        login.SetActive(false);
        error.SetActive(true);
        errorContent.text = "Incorrect username or password";
        errorType = "login";
        loginID.text = null;
        loginPW.text = null;
        backStack.Push(error);
    }

    public void LoginBtn()
    {
        loginBtn.interactable = false;
        StartCoroutine(HttpServerManager.instance.Login(loginID.text, loginPW.text));
        loginBtn.interactable = true;
    }

    public void LoginSignUp()
    {
        signUp.SetActive(true);
        login.SetActive(false);
        backStack.Push(signUp);
    }
    
    public void SignUpBtn()
    {
        StartCoroutine(HttpServerManager.instance.SignUp(signUpID.text, signUpPW.text, signUpNick.text));
    }

    public void RegisterSuccess()
    {
        signUp.SetActive(false);
        login.SetActive(true);
        Debug.Log("회원가입 완료");
    }

    public void RegisterFail(string type)
    {
        Debug.Log($"회원가입 실패 : {type}" );
        signUp.SetActive(false);
        error.SetActive(true);

        if (type == "아이디 중복")
        {
            errorContent.text = "이미 존재하는 아이디 입니다.";
        }
        else if (type == "닉네임 중복")
        {
            errorContent.text = "이미 존재하는 닉네임 입니다.";
        }
        else if (type == "아이디 조건")
        {
            errorContent.text = "비밀번호를 최소 8자 이상 입력해주세요.";
        }
        else if (type == "비밀번호 조건")
        {
            errorContent.text = "비밀번호를 최소 9자 이상 입력해주세요.";
        }

        errorType = "signUp";
        backStack.Push(error);
    }

    void Login_Save(Toggle toggle)
    {
        if (toggle.isOn)
        {
            PlayerPrefs.SetString("ID", loginID.text);
            PlayerPrefs.SetString("PW", loginPW.text);
            PlayerPrefs.SetInt("IsOn", 1);
        }
        else
        {
            PlayerPrefs.DeleteAll();
        }
    }

    void Login_Load(Toggle toggle)
    {
        loginID.text = PlayerPrefs.GetString("ID");
        loginPW.text = PlayerPrefs.GetString("PW");
        
        if (PlayerPrefs.GetInt("IsOn") == 1)
        {
            toggle.isOn = true;
        }
        else
        {
            toggle.isOn = false;
        }

        isPush = PlayerPrefs.GetInt("isPush");

        if (PlayerPrefs.GetFloat("fxValue") != 0)
        {
            fxValue = PlayerPrefs.GetFloat("fxValue");
        }

        if (PlayerPrefs.GetFloat("musicValue") != 0)
        {
            musicValue = PlayerPrefs.GetFloat("musicValue");
        }

        isVibration = PlayerPrefs.GetInt("isVibration");
        audioMixer.SetFloat("FX", Mathf.Log10(fxValue) * 20);
        audioMixer.SetFloat("Music", Mathf.Log10(musicValue) * 20);
    }

    public void Back()
    {
        if (backStack.Count <= 0)
        {
            Debug.LogError("뒤로가기 패널이 비었습니다.");
            return;
        }
        
        switch (backStack.Pop().name)
        {
            case "SignUp":
                signUp.SetActive(false);
                login.SetActive(true);
                break;
            
            case "Error":
                error.SetActive(false);

                if (errorType == "login")
                {
                    login.SetActive(true);
                }
                else if (errorType == "signUp")
                {
                    signUp.SetActive(true);
                }

                break;
        }
    }
}