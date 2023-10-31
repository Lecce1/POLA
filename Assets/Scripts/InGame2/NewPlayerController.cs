using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewPlayerController : MonoBehaviour
{
    [FoldoutGroup("일반")]
    [Title("리지드바디")]
    [SerializeField]
    private Rigidbody rigid;
    
    [FoldoutGroup("일반")]
    [Title("콜라이더")]
    [SerializeField]
    private BoxCollider originCollider;
    
    [FoldoutGroup("음악")]
    [Title("BPM")]
    [SerializeField]
    private float bpm;
    
    [FoldoutGroup("음악")]
    [Title("오디오")]
    [SerializeField]
    public AudioManager audioManager;
    
    [FoldoutGroup("변수")]
    [SerializeField]
    private Vector3 velocityToSet;
    
    [FoldoutGroup("변수")]
    [SerializeField]
    private bool isGravityReversed;

    [FoldoutGroup("변수")] 
    public bool isGrounded;
    
    [FoldoutGroup("변수")] 
    public bool isDead;
    
    [FoldoutGroup("변수")]
    [Title("공격")]
    public bool isAttacking;
    
    [FoldoutGroup("변수")]
    [SerializeField]
    private int attackCounter;
    
    [FoldoutGroup("변수")]
    [Title("점프")]
    public bool isJump;
    
    [FoldoutGroup("변수")]
    [SerializeField]
    private float jumpForce = 500f;
    
    [FoldoutGroup("변수")]
    [Title("슬라이드")]
    [SerializeField]
    private bool isSlide;

    [FoldoutGroup("일반")] 
    public Animator anim;
    
    [FoldoutGroup("일반")] 
    public PlayerParticle particle;
    
    void Start()
    {
        bpm = audioManager.bpm;
        rigid = GetComponent<Rigidbody>();
        originCollider = GetComponent<BoxCollider>();
        particle = GetComponent<PlayerParticle>();
        anim = GetComponent<Animator>(); ;
    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            Move();
        }
    }

    void Update()
    {
        KeyMapping();
    }

    /// <summary>
    /// 키보드 입력
    /// </summary>
    void KeyMapping()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (!isJump)
            {
                OnJump();
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (!isSlide)
            {
                Slide();
            }
        }
        
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            if (isSlide)
            {
                SlideOut();
            }
        }
    }

    /// <summary>
    /// BPM에 따른 움직임
    /// </summary>
    void Move()
    {
        if (rigid.velocity.x > bpm / 12f - bpm * Time.fixedDeltaTime)
        {
            rigid.velocity = new Vector3(bpm / 12f, rigid.velocity.y, 0);
        }
        else
        {
            rigid.AddForce(Vector3.right * bpm);
        }

        if (transform.position.y < -5)
        {
            Die();
        }
    }
    
    /// <summary>
    /// 점프 버튼을 눌렀을때
    /// </summary>
    public void OnJump()
    {
        if (isDead && isJump)
        {
            return;
        }
        
        isJump = true;
        anim.SetTrigger("Jump");
        anim.SetBool("IsJumping", isJump);
        rigid.AddForce(Vector3.up * jumpForce);
    }

    /// <summary>
    /// 점프 버튼을 꾹 눌렀을때
    /// </summary>
    public void Rope()
    {
        float Distance = 4f;
        float sphereScale = 15f;
        RaycastHit hit;
        
        if (Physics.SphereCast(transform.position, sphereScale / 2.0f,transform.forward, out hit, Distance))
        {
            if (hit.transform.CompareTag("Rope"))
            {
                
            }
        }
    }

    /// <summary>
    /// 슬라이드 버튼을 눌렀을때
    /// </summary>
    public void Slide()
    {
        if (isDead)
        {
            return;
        }
        
        isSlide = true;
        anim.SetTrigger("Slide");
        originCollider.center = new Vector3(0, 0.25f, 0);
        originCollider.size = new Vector3(1, 0.5f, 1);
    }

    /// <summary>
    /// 슬라이드 버튼을 뗐을때
    /// </summary>
    public void SlideOut()
    {
        isSlide = false;
        originCollider.center = new Vector3(0, 0.5f, 0);
        originCollider.size = new Vector3(1, 1, 1);
    }

    public void OnAttack()
    {
        // if (isDead || GetComponent<PlayerGrappling>().grapplePoint == null || GetComponent<PlayerSwing>().swingPoint == null)
        // {
        //     return;
        // }

        anim.SetTrigger("Attack");
        anim.SetInteger("AttackCounter", attackCounter % 2);
        isAttacking = true;
        anim.SetBool("IsAttacking", isAttacking);
        attackCounter++;
        
        float Distance = 5f;
        RaycastHit rayHit;
        char evaluation = 'F';
        
        if (Physics.Raycast(transform.position, transform.forward, out rayHit, Distance))
        {
            if (rayHit.transform.CompareTag("Breakable"))
            {
                Destroy(rayHit.transform.gameObject);

                float d = rayHit.transform.position.x - transform.position.x;

                if (d < 2f)
                {
                    evaluation = 'A';
                }
                else if (d < 2.5f)
                {
                    evaluation = 'B';
                }
                else if (d < 3.0f)
                {
                    evaluation = 'C';
                }
                else if (d <= 3.5f)
                {
                    evaluation = 'D';
                }
            }
            Debug.Log(evaluation);
        }
    }
    
    /// <summary>
    /// 어택 종료 애니메이션
    /// </summary>
    public void OnAttackAnimationEnd()
    {
        isAttacking = false;
        anim.SetBool("IsAttacking", isAttacking);
    }

    public void OnClick()
    {
        if (EventSystem.current.currentSelectedGameObject != null && GameManager.instance.set.activeSelf)
        {
            EventSystem.current.currentSelectedGameObject.GetComponent<Button>().onClick.Invoke();
        }
    }

    public void OnCancel()
    {
        if (!GameManager.instance.isPanelOpen)
        {
            GameManager.instance.Button("Set");
        }
        else
        {
            GameManager.instance.Back();
        }
    }

    /// <summary>
    /// 죽었을때
    /// </summary>
    void Die()
    {
        if (isDead)
        {
            return;
        }
        
        anim.SetTrigger("Die");
        isDead = true;
        rigid.useGravity = false;
        Invoke(nameof(Reset), 2f);
        Destroy(gameObject.GetComponent<Rigidbody>());
        StopAllCoroutines();
    }

    private void Reset()
    {
        SceneManager.LoadScene("Game 1");
    }

    /// <summary>
    /// 중력 반전
    /// </summary>
    public void OnReverseGravity()
    {
        var velocity = rigid.velocity;
        velocity.y = 0;
        rigid.velocity = velocity;
        if (!isGravityReversed)
        {
            isGravityReversed = true;
        }
        else
        {
            isGravityReversed = false;
        }

        Physics.gravity *= -1;
    }
    
    /// <summary>
    /// 타겟 위치로 점프하듯이 날아감
    /// </summary>
    /// <param name="targetPosition">타겟위치</param>
    /// <param name="trajectoryHeight">궤도높이</param>
    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);
    }
    
    /// <summary>
    /// 날아가는 속도
    /// </summary>
    private void SetVelocity()
    {
        rigid.velocity = velocityToSet;
    }
    
    /// <summary>
    /// 점프 속도 계산
    /// </summary>
    /// <param name="startPoint">시작지점</param>
    /// <param name="endPoint">도착지점</param>
    /// <param name="trajectoryHeight">궤도높이</param>
    /// <returns></returns>
    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity) + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        RaycastHit rayHit;
        
        if (Physics.Raycast(transform.position, transform.forward, out rayHit, 4f))
        {
            Gizmos.DrawWireSphere(rayHit.point, 0.5f);
        }

        Gizmos.DrawRay(transform.position, transform.forward * 4f);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(NoteMake.instance.beatCount);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isJump = false;
            isGrounded = true;
            anim.SetBool("IsJumping", isJump);
            anim.SetBool("IsGrounded", isGrounded);
            particle.LandParticle();
        }

        if (collision.transform.CompareTag("Breakable"))
        {
            Die();
        }
    }
    
    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
        anim.SetBool("IsGrounded", isGrounded);
    }
}