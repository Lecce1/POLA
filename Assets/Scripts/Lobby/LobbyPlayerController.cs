using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LobbyPlayerController : MonoBehaviour
{
    [Title("플레이어")] 
    public GameObject player;
    
    [Title("몸통")] 
    public GameObject body;

    [Title("속도")] 
    [SerializeField] 
    private float speed = 10.0f;
    
    [Title("움직임 여부")] 
    [SerializeField] 
    private bool isMove;

    [Title("애니메이터")] 
    [SerializeField] 
    private Animator anim;

    [Title("RigidBody")] 
    [SerializeField] 
    private new Rigidbody rigidbody;
    
    [Title("충돌 콜라이더")] 
    [SerializeField] 
    private new Collider[] collider;

    [Title("움직임 가능 여부")] 
    [SerializeField] 
    public bool isMoveAvailable = true;

    public static LobbyPlayerController instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        StartCoroutine("Move");
    }

    IEnumerator Move()
    {
        while (transform.position.x != LobbyManager.instance.moveRoute[DBManager.instance.currentGround]
                   .routeList[DBManager.instance.currentRouteIdx]
                   .transform.position.x || transform.position.z != LobbyManager.instance.moveRoute[DBManager.instance.currentGround]
                   .routeList[DBManager.instance.currentRouteIdx]
                   .transform.position.z)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                LobbyManager.instance.moveRoute[DBManager.instance.currentGround].routeList[DBManager.instance.currentRouteIdx].transform.position + LobbyManager.instance.offset,
                Time.deltaTime * speed);
            yield return null;
        } 
        
        body.transform.rotation = Quaternion.Euler(-90, 180, 0);
        isMove = false;
        anim.SetBool("isMove", isMove);
        Collider();
    }

    public void OnMove(InputValue value)
    {
        if (isMoveAvailable && isMove == false)
        {
            Vector2 vector = value.Get<Vector2>();
            LobbyAudioManager.instance.PlayAudio("Player");

            if (vector.x < 0)
            {
                body.transform.rotation = Quaternion.Euler(-90, -90, 0);
                isMove = true;
                anim.SetBool("isMove", isMove);
                
                if (DBManager.instance.currentRouteIdx - 1 >= 0)
                {
                    DBManager.instance.currentRouteIdx--;
                }
                else
                {
                    DBManager.instance.currentRouteIdx = LobbyManager.instance.moveRoute[DBManager.instance.currentGround].routeList.Count - 1;
                    transform.position = LobbyManager.instance.moveRoute[DBManager.instance.currentGround].routeList[DBManager.instance.currentRouteIdx].transform.position + LobbyManager.instance.offset;
                }
                
                StartCoroutine("Move");
            }
            else if (vector.x > 0)
            {
                body.transform.rotation = Quaternion.Euler(-90, 90, 0);
                isMove = true;
                anim.SetBool("isMove", isMove);

                if (DBManager.instance.currentRouteIdx + 1 < LobbyManager.instance.moveRoute[DBManager.instance.currentGround].routeList.Count)
                {
                    DBManager.instance.currentRouteIdx++;
                }
                else
                {
                    DBManager.instance.currentRouteIdx = 0;
                    transform.position = LobbyManager.instance.moveRoute[DBManager.instance.currentGround].routeList[DBManager.instance.currentRouteIdx].transform.position + LobbyManager.instance.offset;
                }

                StartCoroutine("Move");
            }
        }
    }
    
    public void OnMove(bool isLeft)
    {
        if (isMoveAvailable && isMove == false)
        {
            if (isLeft)
            {
                body.transform.rotation = Quaternion.Euler(-90, -90, 0);
                isMove = true;
                anim.SetBool("isMove", isMove);
                
                if (DBManager.instance.currentRouteIdx - 1 >= 0)
                {
                    DBManager.instance.currentRouteIdx--;
                }
                else
                {
                    DBManager.instance.currentRouteIdx = LobbyManager.instance.moveRoute[DBManager.instance.currentGround].routeList.Count - 1;
                    transform.position = LobbyManager.instance.moveRoute[DBManager.instance.currentGround].routeList[DBManager.instance.currentRouteIdx].transform.position + LobbyManager.instance.offset;
                }
                
                StartCoroutine("Move");
            }
            else
            {
                body.transform.rotation = Quaternion.Euler(-90, 90, 0);
                isMove = true;
                anim.SetBool("isMove", isMove);

                if (DBManager.instance.currentRouteIdx + 1 < LobbyManager.instance.moveRoute[DBManager.instance.currentGround].routeList.Count)
                {
                    DBManager.instance.currentRouteIdx++;
                }
                else
                {
                    DBManager.instance.currentRouteIdx = 0;
                    transform.position = LobbyManager.instance.moveRoute[DBManager.instance.currentGround].routeList[DBManager.instance.currentRouteIdx].transform.position + LobbyManager.instance.offset;
                }

                StartCoroutine("Move");
            }
        }
    }

    public void OnBack()
    {
        if (DBManager.instance.currentGround != 0)
        {
            if (DBManager.instance.currentGround == 2 && !LobbyManager.instance.isPanelOpen && LobbyManager.instance.isInfoPanelOn)
            {
                if (LobbyManager.instance.info.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("InfoOn"))
                {
                    if (LobbyManager.instance.info.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                    {
                        LobbyManager.instance.Info_OnOff(false);
                        Collider();
                    }
                }
            }
            else if(!LobbyManager.instance.isPanelOpen && !LobbyManager.instance.isInfoPanelOn)
            {
                LobbyManager.instance.Button("Back");
            }
        }
    }

    public void OnDpadLeft()
    {
        OnMove(true);
    }
    
    public void OnDpadRight()
    {
        OnMove(false);
    }

    public void OnJoin()
    {
        if (!LobbyManager.instance.isPanelOpen && LobbyManager.instance.isJoinBtnOn)
        {
            if (LobbyManager.instance.info.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("InfoOn") && LobbyManager.instance.info.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
            {
                LobbyManager.instance.Button($"Info_Yes");
            }
            else
            {
                LobbyManager.instance.Button(LobbyManager.instance.join_Btn_Type);
            }
            
            if (LobbyManager.instance.join_Btn_Type != "Move" && LobbyManager.instance.join_Btn_Type != "Stage" && LobbyManager.instance.join_Btn_Type != "Shop")
            {
                isMoveAvailable = false;
            }
        }
    }

    public void OnClick()
    {
        if (transform.GetComponent<PlayerInput>().currentControlScheme != "MOBILE")
        {
            if (LobbyManager.instance.set.activeSelf)
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
                    
                    case "Dropdown":
                        if (LobbyManager.instance.set_Language_Dropdown.value == 0)
                        {
                            LobbyManager.instance.set_Language_Dropdown.value = 1;
                        }
                        else
                        {
                            LobbyManager.instance.set_Language_Dropdown.value = 0;
                        }
                        
                        LobbyManager.instance.Set_Change("Language");
                        break;
                }
            }
            else if (LobbyManager.instance.esc.activeSelf)
            {
                switch (EventSystem.current.currentSelectedGameObject.name)
                {
                    case "Set":
                        LobbyManager.instance.Button("Set");
                        break;
                    
                    case "Back":
                        LobbyManager.instance.Button("Exit");
                        break;
                }
            }
            else if (LobbyManager.instance.exit.activeSelf)
            {
                LobbyManager.instance.Button($"Exit_{EventSystem.current.currentSelectedGameObject.name}");
                isMoveAvailable = true;
            }
        }
    }

    public void OnCancel()
    {
        if (LobbyManager.instance.isPanelOpen)
        {
            if (!LobbyManager.instance.exit.activeSelf)
            {
                LobbyManager.instance.Back();
                
                if (!LobbyManager.instance.isInfoPanelOn)
                {
                    isMoveAvailable = true;
                }
            }
        }
        else if (!LobbyManager.instance.isPanelOpen)
        {
            if (!LobbyManager.instance.esc.activeSelf && DBManager.instance.currentPlatform == "PC")
            {
                LobbyManager.instance.Button("Esc");
                isMoveAvailable = false;
            }
        }
    }

    public void OnConsoleCancel()
    {
        if (!LobbyManager.instance.isPanelOpen)
        {
            if (!LobbyManager.instance.esc.activeSelf)
            {
                LobbyManager.instance.Button("Esc");
                isMoveAvailable = false;
            }
        }
    }

    public void OnLeftControl()
    {
        if (transform.GetComponent<PlayerInput>().currentControlScheme != "MOBILE")
        {
            if (LobbyManager.instance.set.activeSelf)
            {
                switch (EventSystem.current.currentSelectedGameObject.name)
                {
                    case "Dropdown":
                        if (LobbyManager.instance.set_Language_Dropdown.value > 0)
                        {
                            LobbyManager.instance.set_Language_Dropdown.value--;
                        }

                        LobbyManager.instance.Set_Change("Language");
                        break;
                }
            }
        }
    }
    
    public void OnRightControl()
    {
        if (transform.GetComponent<PlayerInput>().currentControlScheme != "MOBILE")
        {
            if (LobbyManager.instance.set.activeSelf)
            {
                switch (EventSystem.current.currentSelectedGameObject.name)
                {
                    case "Dropdown":
                        if (LobbyManager.instance.set_Language_Dropdown.value < DBManager.instance.supportLanguageNum - 1)
                        {
                            LobbyManager.instance.set_Language_Dropdown.value++;
                        }

                        LobbyManager.instance.Set_Change("Language");
                        break;
                }
            }
        }
    }

    public void Collider()
    {
        collider = Physics.OverlapBox(transform.position, new Vector3(1, 3, 1), transform.rotation);

        if (rigidbody.velocity.magnitude <= 10)
        {
            foreach (var temp in collider)
            {
                if (temp.CompareTag("Door"))
                {
                    switch (temp.transform.GetComponent<DoorManager>().name)
                    {
                        case "Shop":
                            LobbyManager.instance.DoorInit("Shop", "Shop");
                            break;
                        
                        case "Latency":
                            LobbyManager.instance.DoorInit("Latency", "Join");
                            break;

                        case "Play":
                            LobbyManager.instance.DoorInit("Move", "Join");
                            break;
                        
                        case "Contents":
                            if (LobbyManager.instance.isJoinBtnOn)
                            {
                                LobbyManager.instance.Join_Btn_OnOff(false, false);
                            }
                            break;
                        
                        case "Chapter":
                            if (temp.GetComponent<DoorManager>().isLock == false)
                            {
                                LobbyManager.instance.DoorInit("Move", "Join");
                            }
                            else
                            {
                                if (LobbyManager.instance.isJoinBtnOn)
                                {
                                    LobbyManager.instance.Join_Btn_OnOff(false, false);
                                }
                            }
                            
                            DBManager.instance.currentChapter = temp.GetComponent<DoorManager>().chapterNum;
                            break;
                        
                        case "Stage":
                            if (LobbyManager.instance.isInfoPanelOn == false)
                            {
                                DBManager.instance.currentStage = temp.GetComponent<DoorManager>().stageNum;
                                LobbyManager.instance.DoorInit("Stage", "Join");
                            }
                            break;
                        
                        case "Center":
                            if (LobbyManager.instance.join_Btn.activeSelf)
                            {
                                LobbyManager.instance.Join_Btn_OnOff(false, false);
                            }
                            break;
                    }
                }
            }
        }
    }
}