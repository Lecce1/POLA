using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;

public class TitleManager : MonoBehaviour
{
    [Title("Press Key 텍스트")]
    public GameObject title_Press_Text;
    [Title("Virtual Camera")]
    public CinemachineVirtualCamera virtualCamera;
    [Title("Dolly Cart")]
    public CinemachineDollyCart dollyCart;
    [Title("SmoothPath")]
    public CinemachineSmoothPath smoothPath;
    [Title("PlayerController")]
    public LobbyPlayerController playerController;
    [Title("PlayerInput")]
    public PlayerInput playerInput;
    [Title("타이틀 오브젝트")]
    public List<GameObject> titleObject;
    [Title("로비 오브젝트")]
    public List<GameObject> lobbyObject;
    [Title("버튼 클릭 여부")]
    public bool isPress;

    [FoldoutGroup("폴라로이드")] 
    [Title("폴라로이드")]
    public GameObject polaroid;
    [FoldoutGroup("폴라로이드")] 
    [Title("폴라로이드 카드")]
    public GameObject polaroid_Card;
    
    public static TitleManager instance;
    public FadeManager fadeManager;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
        Application.targetFrameRate = 144;
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
        if (Input.anyKey)
        {
            title_Press_Text.SetActive(false);
            isPress = true;
        }
    }

    void Init()
    {
        if (!DBManager.instance.isCinemachine)
        {
            for (int i = 0; i < titleObject.Count; i++)
            {
                titleObject[i].SetActive(true);
            }
        
            for (int i = 0; i < lobbyObject.Count; i++)
            {
                lobbyObject[i].SetActive(false);
            }

            playerController.enabled = false;
            playerInput.enabled = false;
        }
        else
        {
            for (int i = 0; i < titleObject.Count; i++)
            {
                titleObject[i].SetActive(false);
            }
        
            for (int i = 0; i < lobbyObject.Count; i++)
            {
                lobbyObject[i].SetActive(true);
            }
            
            title_Press_Text.SetActive(false);
            playerController.enabled = true;
            playerInput.enabled = true;
            StartCoroutine(fadeManager.FadeOut());
            Invoke("Destroy", 3.0f);
        }
    }

    void Destroy()
    {
        Destroy(gameObject);
    }

    public IEnumerator Cinemachine()
    {
        virtualCamera.LookAt = polaroid_Card.transform;
        dollyCart.enabled = true;
        bool isFade = false;
        
        while (dollyCart.m_Position != smoothPath.PathLength)
        {
            if ((smoothPath.PathLength - dollyCart.m_Position < 50) && isFade == false)
            {
                isFade = true;
                StartCoroutine(FadeManager.instance.FadeIn());
            }
            yield return null;
        }

        DBManager.instance.isCinemachine = true;
        Invoke("Init", 2.0f);
    }
}