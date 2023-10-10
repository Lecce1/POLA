using UnityEngine;
using Sirenix.OdinInspector;

public class LobbyPlayerController : MonoBehaviour
{
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

    void Collider()
    {
        collider = Physics.OverlapBox(transform.position, new Vector3(2, 2, 2), transform.rotation);
        
        foreach(var temp in collider)
        {
            
        }
    }
    
    public void OnLeftDown()
    {
        direction = -1;
        isMove = true;
        anim.SetBool("isMove", isMove);
    }
    
    public void OnRightDown()
    {
        direction = 1;
        isMove = true;
        anim.SetBool("isMove", isMove);
    }

    public void OnBtnUp()
    {
        direction = 0;
        isMove = false;
        anim.SetBool("isMove", isMove);
    }
}