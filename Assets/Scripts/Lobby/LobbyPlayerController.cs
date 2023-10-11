using UnityEngine;
using Sirenix.OdinInspector;

public class LobbyPlayerController : MonoBehaviour
{
    [Title("몸통")] 
    public GameObject body;
    
    [Title("방향")] 
    [SerializeField] 
    private int direction;
    
    [Title("속도")] 
    [SerializeField] 
    private float speed = 5.0f;
    
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
        
        switch (collider[0].name)
        {
            case "Stage1":
                LobbyManager.instance.Join_Btn(true);
                break;
                
            case "Stage2":
                LobbyManager.instance.Join_Btn(true);
                break;
            
            default:
                LobbyManager.instance.Join_Btn(false);
                break;
        }
    }
}