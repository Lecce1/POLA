using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TrackPlayerController : MonoBehaviour
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
    private float speed = 20.0f;
    
    [Title("방향")]
    public Vector3 playerVector;
    
    [Title("움직임 여부")]
    public bool isMove = false;

    [Title("애니메이터")]
    public Animator anim;

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
    public bool isDoor = false;

    public static TrackPlayerController instance;

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
        if (isMove)
        {
            rigidbody.AddForce(playerVector * speed);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(playerVector), Time.deltaTime * 10);
        }
    }

    public void OnMove(InputValue value)
    {
        if (isMoveAvailable)
        {
            Vector2 temp = value.Get<Vector2>();
            playerVector = new Vector3(temp.x, 0, temp.y);

            if (playerVector.x == 0 && playerVector.z == 0)
            {
                isMove = false;
            }
            else
            {
                isMove = true;
            }
        }
        else
        {
            isMove = false;
        }
        
        anim.SetBool("isMove", isMove);
    }
    
    public void OnClick()
    {
        if (transform.GetComponent<PlayerInput>().currentControlScheme != "MOBILE")
        {
            if (TrackManager.instance.isInfoPanelOn)
            {
                TrackManager.instance.Button(TrackManager.instance.btn_Type);
                isMoveAvailable = false;
            }
            else if (TrackManager.instance.set.activeSelf)
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

                        TrackManager.instance.Set_Language();
                        break;
                }
            }
        }
    }
    
    public void OnLeftControl()
    {
        if (transform.GetComponent<PlayerInput>().currentControlScheme != "MOBILE")
        {
            if (TrackManager.instance.set.activeSelf)
            {
                switch (EventSystem.current.currentSelectedGameObject.name)
                {
                    case "Dropdown":
                        if (TrackManager.instance.set_Language_Dropdown.value > 0)
                        {
                            TrackManager.instance.set_Language_Dropdown.value--;
                        }

                        TrackManager.instance.Set_Language();
                        break;
                }
            }
        }
    }
    
    public void OnRightControl()
    {
        if (transform.GetComponent<PlayerInput>().currentControlScheme != "MOBILE")
        {
            if (TrackManager.instance.set.activeSelf)
            {
                switch (EventSystem.current.currentSelectedGameObject.name)
                {
                    case "Dropdown":
                        if (TrackManager.instance.set_Language_Dropdown.value < DBManager.instance.supportLanguageNum - 1)
                        {
                            TrackManager.instance.set_Language_Dropdown.value++;
                        }

                        TrackManager.instance.Set_Language();
                        break;
                }
            }
        }
    }
    
    public void OnCancel()
    {
        if (TrackManager.instance.isPanelOpen)
        {
            TrackManager.instance.Back();
            isMoveAvailable = true;
        } 
        else if (GetComponent<PlayerInput>().currentControlScheme == "PC" && !TrackManager.instance.set.activeSelf)
        {
            TrackManager.instance.Button("Set");
            isMoveAvailable = false;
        }
    }
    
    public void OnSet()
    {
        if (!TrackManager.instance.set.activeSelf)
        {
            isMoveAvailable = false;
            TrackManager.instance.Button("Set");
        }
    }

    void Collider()
    {
        collider = Physics.OverlapBox(transform.position, new Vector3(2, 2, 2), transform.rotation);
        bool isCheck = false;
        
        if (rigidbody.velocity.magnitude <= 5)
        {
            foreach (var temp in collider)
            {
                if (temp.CompareTag("Door"))
                {
                    isCheck = true;
                    TrackInfo.instance.Init(DBManager.instance.currentStageNum, temp.transform.GetComponent<DoorManager>().trackNum);
                    TrackManager.instance.Info_OnOff(true);
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
        
        if (TrackManager.instance.isInfoPanelOn && !isDoor && !collider[0].transform.GetComponent<DoorManager>())
        {
            TrackManager.instance.Info_OnOff(false);
        }
    }
}