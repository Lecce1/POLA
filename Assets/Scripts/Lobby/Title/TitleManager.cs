using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    [Title("Fade")]
    public Image fade;
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
            LobbyManager.instance.currentGround = 0;
            StartCoroutine(FadeOut());
        }
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
                StartCoroutine("FadeIn");
            }
            yield return null;
        }

        DBManager.instance.isCinemachine = true;
        Invoke("Init", 2.0f);
    }

    IEnumerator FadeIn()
    {
        float count = 0;
        
        while (fade.color.a < 1)
        {
            count += 0.01f;
            fade.color = new Color(0, 0, 0, count);
            yield return null;
        }
    }
    
    IEnumerator FadeOut()
    {
        float count = 1;
        
        while (fade.color.a > 0)
        {
            count -= 0.01f;
            fade.color = new Color(0, 0, 0, count);
            yield return null;
        }
        
        Destroy(gameObject);
    }
}