using System;
using System.Diagnostics;
using UnityEngine;
using Sirenix.OdinInspector;
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

    [Title("충돌 콜라이더 이름")] 
    [SerializeField] 
    private string colliderName = String.Empty;
    
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
            if (temp.name == "Door")
            {
                isDoor = true;
                
                switch (temp.transform.GetComponent<LobbyDoorManager>().name)
                {
                    case "Stage1":
                        if (!temp.transform.GetComponent<LobbyDoorManager>().isLock)
                        {
                            LobbyManager.instance.Join_Btn(true);
                            colliderName = temp.transform.GetComponent<LobbyDoorManager>().name;
                        }
                    
                        break;
                
                    case "Stage2":
                        if (!temp.transform.GetComponent<LobbyDoorManager>().isLock)
                        {
                            LobbyManager.instance.Join_Btn(true);
                            colliderName = temp.transform.GetComponent<LobbyDoorManager>().name;
                        }
                    
                        break;
                }
                

            }
        }

        if (!isDoor)
        {
            if (colliderName != String.Empty && !collider[0].transform.GetComponent<LobbyDoorManager>() && LobbyManager.instance.isJoinOn == true)
            {
                LobbyManager.instance.Join_Btn(false);
            }
        }
    }
}