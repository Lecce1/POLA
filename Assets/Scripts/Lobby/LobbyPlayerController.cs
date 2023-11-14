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
    private Rigidbody rigidbody;
    
    [Title("충돌 콜라이더")] 
    [SerializeField] 
    private Collider[] collider;

    [Title("움직임 가능 여부")] 
    [SerializeField] 
    public bool isMoveAvailable = true;
    
    [Title("문 충돌 여부")] 
    public bool isDoor;
    
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
    }
    
    void Update()
    {
        Move();
        Collider();
    }

    void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position,
            LobbyManager.instance.moveRoute[DBManager.instance.currentGround].routeList[DBManager.instance.currentRouteIdx].transform.position + LobbyManager.instance.offset,
            Time.deltaTime * speed);
                
        if (transform.position.x == LobbyManager.instance.moveRoute[DBManager.instance.currentGround].routeList[DBManager.instance.currentRouteIdx]
                .transform.position.x)
        {
            if (DBManager.instance.currentRouteIdx == LobbyManager.instance.moveRoute[DBManager.instance.currentGround].defaultRouteIdx)
            {
                body.transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                body.transform.rotation = Quaternion.Euler(0, 0, 0);
            }

            isMove = false;
            anim.SetBool("isMove", isMove);
        }
    }

    public void OnMove(InputValue value)
    {
        if (isMoveAvailable && isMove == false)
        {
            Vector2 vector = value.Get<Vector2>();
        
            if (vector.x < 0)
            {
                body.transform.rotation = Quaternion.Euler(0, -90, 0);
                isMove = true;
                anim.SetBool("isMove", isMove);
                
                if (DBManager.instance.currentRouteIdx - 1 >= 0)
                {
                    DBManager.instance.currentRouteIdx--;
                }
                
                Move();
            }
            else if (vector.x > 0)
            {
                body.transform.rotation = Quaternion.Euler(0, 90, 0);
                isMove = true;
                anim.SetBool("isMove", isMove);

                if (DBManager.instance.currentRouteIdx + 1 < LobbyManager.instance.moveRoute[DBManager.instance.currentGround].routeList.Count)
                {
                    DBManager.instance.currentRouteIdx++;
                }

                Move();
            }
        }
    }
    
    public void OnMove(bool isLeft)
    {
        if (isMoveAvailable && isMove == false)
        {
            if (isLeft == true)
            {
                body.transform.rotation = Quaternion.Euler(0, -90, 0);
                isMove = true;
                anim.SetBool("isMove", isMove);
                
                if (DBManager.instance.currentRouteIdx - 1 >= 0)
                {
                    DBManager.instance.currentRouteIdx--;
                }
                
                Move();
            }
            else
            {
                body.transform.rotation = Quaternion.Euler(0, 90, 0);
                isMove = true;
                anim.SetBool("isMove", isMove);

                if (DBManager.instance.currentRouteIdx + 1 < LobbyManager.instance.moveRoute[DBManager.instance.currentGround].routeList.Count)
                {
                    DBManager.instance.currentRouteIdx++;
                }

                Move();
            }
        }
    }

    public void OnClick()
    {
        if (transform.GetComponent<PlayerInput>().currentControlScheme != "MOBILE")
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

                    if (LobbyManager.instance.join_Btn_Type != "Move" && LobbyManager.instance.join_Btn_Type != "Back")
                    {
                        isMoveAvailable = false;
                    }
                }
            }
            else if (LobbyManager.instance.set.activeSelf)
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
                isMoveAvailable = true;
            }
        }
        else if (!LobbyManager.instance.isPanelOpen)
        {
            if (LobbyManager.instance.info.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("InfoOn"))
            {
                if (LobbyManager.instance.info.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f)
                {
                    LobbyManager.instance.Info_OnOff(false);
                }
            }
            else if (!LobbyManager.instance.esc.activeSelf)
            {
                LobbyManager.instance.Button("Esc");
                isMoveAvailable = false;
            }
        }
    }

    public void OnTab()
    {
        if (LobbyManager.instance.shop.activeSelf)
        {
            LobbyShopManager.instance.Tab();
        }
    }

    public void OnLeftTab()
    {
        if (LobbyManager.instance.shop.activeSelf)
        {
            LobbyShopManager.instance.Prev();
        }
    }
    
    public void OnRightTab()
    {
        if (LobbyManager.instance.shop.activeSelf)
        {
            LobbyShopManager.instance.Next();
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

    void Collider()
    {
        collider = Physics.OverlapBox(transform.position, new Vector3(1, 3, 1), transform.rotation);
        bool isCheck = false;
        
        if (rigidbody.velocity.magnitude <= 10)
        {
            foreach (var temp in collider)
            {
                if (temp.CompareTag("Door"))
                {
                    isCheck = true;
                
                    switch (temp.transform.GetComponent<DoorManager>().name)
                    {
                        case "Set":
                            LobbyManager.instance.DoorInit("Set", "Set");
                            break;

                        case "Shop":
                            LobbyManager.instance.DoorInit("Shop", "Shop");
                            break;

                        case "Story":
                            LobbyManager.instance.DoorInit("Move", "Join");
                            break;
                        
                        case "Chapter":
                            if (temp.GetComponent<DoorManager>().isLock == false)
                            {
                                LobbyManager.instance.DoorInit("Move", "Join");

                            }
                            
                            DBManager.instance.currentChapter = temp.GetComponent<DoorManager>().chapterNum;
                            break;
                        
                        case "Stage":
                            if (LobbyManager.instance.isInfoPanelOn == false)
                            {
                                TrackInfo.instance.Init(DBManager.instance.currentChapter, temp.transform.GetComponent<DoorManager>().stageNum);
                                LobbyManager.instance.DoorInit("Stage", "Join");
                                DBManager.instance.currentStage = temp.GetComponent<DoorManager>().stageNum;
                            }
                            break;
                        
                        case "Back":
                            LobbyManager.instance.DoorInit("Back", "Join");
                            break;
                    }
                }
            }
        }
        
        if (isCheck)
        {
            isDoor = true;
        }
        else
        {
            isDoor = false;
        }
        
        if (LobbyManager.instance.isJoinBtnOn && !isDoor && !collider[0].transform.GetComponent<DoorManager>())
        {
            LobbyManager.instance.Join_Btn_OnOff(false);
        }
    }
}