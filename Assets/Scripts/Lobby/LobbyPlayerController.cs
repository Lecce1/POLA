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
    
    [Title("방향")] 
    [SerializeField] 
    private int direction;
    
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
        rigidbody.AddForce(direction * speed, 0, 0);
    }

    public void OnMove(InputValue value)
    {
        if (isMoveAvailable)
        {
            Vector2 vector = value.Get<Vector2>();
        
            if (vector.x < 0)
            {
                body.transform.rotation = Quaternion.Euler(0, -90, 0);
                direction = -1;
                isMove = true;
                anim.SetBool("isMove", isMove);
            }
            else if (vector.x == 0)
            {
                direction = 0;
                isMove = false;
                anim.SetBool("isMove", isMove);
            }
            else if (vector.x > 0)
            {
                body.transform.rotation = Quaternion.Euler(0, -270, 0);
                direction = 1;
                isMove = true;
                anim.SetBool("isMove", isMove);
            }
        }
        else
        {
            direction = 0;
            isMove = false;
            anim.SetBool("isMove", isMove);
        }
    }

    public void OnClick()
    {
        if (transform.GetComponent<PlayerInput>().currentControlScheme != "MOBILE")
        {
            if (!LobbyManager.instance.isPanelOpen && LobbyManager.instance.isJoinBtnOn)
            {
                LobbyManager.instance.Button(LobbyManager.instance.join_Btn_Type);
                isMoveAvailable = false;
            }
            else if (LobbyManager.instance.set.activeSelf)
            {
                switch (EventSystem.current.currentSelectedGameObject.name)
                {
                    case "Toggle":
                        if (EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>().isOn)
                        {
                            EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>().isOn = false;
                        }
                        else if (!EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>().isOn)
                        {
                            EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>().isOn = true;
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
                        
                        LobbyManager.instance.Set_Language();
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
        else if (GetComponent<PlayerInput>().currentControlScheme == "PC" && !LobbyManager.instance.set.activeSelf)
        {
            LobbyManager.instance.Button("Set");
            isMoveAvailable = false;
        }
    }

    public void OnClose()
    {
        if (!LobbyManager.instance.isPanelOpen)
        {
            if (!LobbyManager.instance.exit.activeSelf)
            {
                LobbyManager.instance.Button("Exit");
                isMoveAvailable = false;
            }
        }
    }

    /*public void OnScrollWheel(InputValue value)
    {
        scrollVec = value.Get<Vector2>();
        scrollRect.content.anchoredPosition = new Vector2(scrollRect.content.anchoredPosition.x - (scrollVec.x * 10), 0);
    }*/
    
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

                        LobbyManager.instance.Set_Language();
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

                        LobbyManager.instance.Set_Language();
                        break;
                }
            }
        }
    }

    public void OnMoveDown(bool isLeft)
    {
        if (isLeft == true)
        {
            body.transform.rotation = Quaternion.Euler(0, -90, 0);
            direction = -1;
            isMove = true;
            anim.SetBool("isMove", isMove);
        }
        else
        {
            body.transform.rotation = Quaternion.Euler(0, -270, 0);
            direction = 1;
            isMove = true;
            anim.SetBool("isMove", isMove);
        }
    }

    public void OnMoveUp()
    {
        direction = 0;
        isMove = false;
        anim.SetBool("isMove", isMove);
    }

    public void OnSet()
    {
        if (!LobbyManager.instance.set.activeSelf)
        {
            LobbyManager.instance.isSetBtn = true;
            LobbyManager.instance.Button("Set");
            isMoveAvailable = false;
        }
    }

    void Collider()
    {
        collider = Physics.OverlapBox(transform.position, new Vector3(1, 1, 1), transform.rotation);
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
                        case "Sign":
                            LobbyManager.instance.DoorInit("Sign", "OK");
                            break;
                    
                        case "Set":
                            LobbyManager.instance.DoorInit("Set", "Set");
                            break;

                        case "Shop":
                            LobbyManager.instance.DoorInit("Shop", "Shop");
                            break;

                        case "Stage":
                            LobbyManager.instance.DoorInit("Stage", "Join");
                            DBManager.instance.currentStageNum = 1;
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
            LobbyManager.instance.Join_Btn_OnOff(false, false);
        }
    }
}