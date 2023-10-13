using System;
using System.Diagnostics;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

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
    private bool isMove = false;

    [Title("애니메이터")] 
    [SerializeField] 
    private Animator anim;

    [Title("RigidBody")] 
    [SerializeField] 
    private Rigidbody rigidbody;
    
    [Title("충돌 콜라이더")] 
    [SerializeField] 
    private Collider[] collider;
    
    [Title("첫 충돌 여부")] 
    [SerializeField] 
    private bool isCollider;

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

    void Collider()
    {
        collider = Physics.OverlapBox(transform.position, new Vector3(1, 1, 1), transform.rotation);

        bool isDoor = false;
        
        foreach (var temp in collider)
        {
            if (temp.CompareTag("Door"))
            {
                isCollider = true;
                isDoor = true;
                
                switch (temp.transform.GetComponent<LobbyDoorManager>().name)
                {
                    case "Set":
                        DoorInit("Set", "설정", string.Empty, false);
                        break;

                    case "Shop":
                        DoorInit("Shop", "상점", string.Empty, false);
                        break;

                    case "Stage1":
                        DoorInit("Stage", "입장", "스테이지 1",temp.transform.GetComponent<LobbyDoorManager>().isLock);
                        break;

                    case "Stage2":
                        DoorInit("Stage", "입장", "스테이지 2", temp.transform.GetComponent<LobbyDoorManager>().isLock);
                        break;
                }
            }
        }

        if (isCollider && !isDoor && !collider[0].transform.GetComponent<LobbyDoorManager>())
        {
            LobbyManager.instance.Join_Btn_OnOff(false, false);
        }
    }
    
    void DoorInit(string name, string btnText, string nameText, bool isLock)
    {
        if (!isLock)
        {
            LobbyManager.instance.join_Btn.GetComponent<Button>().onClick.RemoveAllListeners();
            LobbyManager.instance.Join_Btn_OnOff(true, false);
            LobbyManager.instance.join_Btn.GetComponent<Button>().onClick.AddListener(() => LobbyManager.instance.Button(name));
            LobbyManager.instance.join_Btn_Text.text = btnText;
            LobbyManager.instance.stage_Name_Text.text = nameText;
            LobbyManager.instance.Join_Btn_OnOff(true, false);
        }
        else
        {
            LobbyManager.instance.stage_Name_Text.text = nameText;
            LobbyManager.instance.Join_Btn_OnOff(true, true);
        }
    }
}