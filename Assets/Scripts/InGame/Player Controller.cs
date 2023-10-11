using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Animations;

public class PlayerController : MonoBehaviour
{
    [FoldoutGroup("변수")]
    [SerializeField]
    private int jumpCount;
    
    [FoldoutGroup("변수")]
    [SerializeField]
    private float maxSlopeAngle = 50.0f;
    
    [FoldoutGroup("변수")]
    [SerializeField]
    private int attackCounter = 0;

    [FoldoutGroup("변수")]
    [SerializeField]
    private float gravityScale = 9.81f;
    
    [FoldoutGroup("변수")]
    [SerializeField]
    private bool isAttacking = false;
    
    [FoldoutGroup("변수")]
    [SerializeField]
    private bool isGravityReversed = false;
    
    [FoldoutGroup("변수")]
    public bool isGrounded = false;
    
    [FoldoutGroup("변수")]
    public bool isJumping = false;
    
    [FoldoutGroup("변수")]
    public bool isFlip = false;
    
    [FoldoutGroup("스텟")]
    public float currentSpeed { get; private set; }
    
    [FoldoutGroup("스텟")]
    public PlayerStatsManager stats;

    [FoldoutGroup("색깔")] 
    public Color[] playerColors;
    
    [FoldoutGroup("일반")] 
    [SerializeField]
    MeshRenderer meshRenderer;

    [FoldoutGroup("일반")] 
    public Animator anim;
    
    [FoldoutGroup("일반")] 
    public Transform rayPosition;
    
    [FoldoutGroup("일반")] 
    public Rigidbody rigid;
    
    [FoldoutGroup("일반")] 
    public PlayerParticle particle;
    
    public static PlayerController instance;
    private Collision collisionInfo;
    private Coroutine jumpCoroutine;
    WaitForFixedUpdate fixedUpdate = new WaitForFixedUpdate();
    private Vector3 tmp;
    
    /// <summary>
    /// 인스턴스 생성
    /// </summary>
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    
    void Start()
    {
        stats = GetComponent<PlayerStatsManager>();
        rigid = GetComponent<Rigidbody>();
        meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        particle = GetComponent<PlayerParticle>();
        anim = GetComponent<Animator>();
        
        Time.timeScale = 1f;
        jumpCount = 0;
        currentSpeed = rigid.velocity.x;
        tmp = transform.position;
    }

    void Update()
    {
        KeyboardControl();
    }

    void FixedUpdate()
    {
        Moving();
        OnAttackSlow();
        OnTimeReturn();
        currentSpeed = rigid.velocity.x;
    }

    void KeyboardControl()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            OnJumpButtonDown();
        }
        
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            OnJumpButtonUp();
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            OnColorChanged();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            OnAttackButtonClicked();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            OnReverseGravity();
        }
    }
    
    /// <summary>
    /// 플레이어의 움직임 ( 전진 ) 및 사망
    /// </summary>
    void Moving()
    {
        if (stats.current.isDead)
        {
            rigid.velocity = Vector3.zero;
            return;
        }

        if (rigid.velocity.x < stats.current.maxSpeed)
        {
            rigid.AddForce(Vector3.right * stats.current.acceleration * Time.deltaTime, ForceMode.Force);
        }

        if (transform.position.y < -5)
        {
            Die();
        }
    }
    
    /// <summary>
    /// 플레이어 사망처리 
    /// </summary>
    void Die()
    {
        if (stats.current.isDead)
        {
            return;
        }
        
        anim.SetTrigger("Die");
        stats.current.isDead = true;
        rigid.useGravity = false;
        isGravityReversed = false;
        GameManager.instance.Reset();
        Destroy(gameObject.GetComponent<BoxCollider>());
        StopAllCoroutines();
    }

    /// <summary>
    /// 점프 버튼을 눌렀을 때
    /// </summary>
    public void OnJumpButtonDown()
    {
        if (jumpCount < stats.current.maxJump && !stats.current.isDead)
        {
            stats.current.jumpForce = stats.origin.jumpForce;
            isJumping = true;
            anim.SetTrigger("Jump");
            jumpCoroutine = StartCoroutine(Jump());
            jumpCount++;
        }
    }

    /// <summary>
    /// 점프 버튼을 뗐을 때
    /// </summary>
    public void OnJumpButtonUp()
    {
        if (isJumping)
        {
            isJumping = false;
            if (jumpCoroutine != null)
            {
                StopCoroutine(jumpCoroutine);
                jumpCoroutine = null;
            }
        }
    }
    
    /// <summary>
    /// 점프 기능 구현
    /// </summary>
    IEnumerator Jump()
    {
        float pressedJumpStartTime = Time.time;
        float inverseJumpLength = 1 / stats.current.jumpLength;
        int direction = isGravityReversed ? -1 : 1;

        while (Time.time - pressedJumpStartTime < stats.current.jumpLength && isJumping)
        {
            if (!SlopeProcess(collisionInfo))
            {
                var velocity = rigid.velocity;
                velocity.y = stats.current.jumpForce * direction;
                rigid.velocity = velocity;
            }
            Debug.Log("실제 움직인 값: " + (transform.position - tmp) + ", velocity: " + rigid.velocity * Time.fixedDeltaTime);
            tmp = transform.position;
            stats.current.jumpForce -= Time.fixedDeltaTime * stats.current.jumpForce * inverseJumpLength;
            yield return fixedUpdate;
        }
    }

    /// <summary>
    /// 플레이어 기준 머리에 경사면을 부딪혔을 때의 충돌에 관한 처리 및 계산
    /// </summary>
    /// <param name="collisionInfo">플레이어의 충돌정보</param>
    /// <returns>충돌 처리를 했다면 true, 처리를 하지 않았다면 false를 반환</returns>
    bool SlopeProcess(Collision collisionInfo)
    {
        if (collisionInfo == null)
        {
            return false;
        }

        float angle = -1;
        
        foreach (var item in collisionInfo.contacts)
        {
            var tmp = Vector3.ProjectOnPlane(Vector3.right, -item.normal).normalized;
            angle = 90 - Vector3.Angle(Vector3.down, tmp);
        }
        
        if (angle <= maxSlopeAngle && angle > 0)
        {
            var velocity = rigid.velocity;
            velocity.x = currentSpeed;
            velocity.y = -Mathf.Tan(angle * Mathf.Deg2Rad) * velocity.x;
            rigid.velocity = velocity;
            rigid.velocity -= Physics.gravity;
            Moving();
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// 색상 변경
    /// </summary>
    public void OnColorChanged()
    {
        var idx = stats.current.colorIndex;
        meshRenderer.material.color = playerColors[idx];
        stats.current.colorIndex = ++idx < playerColors.Length ? idx : 0;
    }
    
    /// <summary>
    /// 공격 구현
    /// </summary>
    public void OnAttackButtonClicked()
    {
        if (stats.current.isDead)
        {
            return;
        }
        
        anim.SetTrigger("Attack");
        anim.SetInteger("AttackCounter", attackCounter % 2);
        isAttacking = true;
        anim.SetBool("IsAttacking", isAttacking);
        attackCounter++;

        float Distance = 10f;
        float sphereScale = 15f;
        RaycastHit hit;
        
        if (Physics.SphereCast(transform.position, sphereScale / 2.0f, transform.forward, out hit, Distance))
        {
            if (hit.transform.CompareTag("Breakable"))
            {
                if (meshRenderer.material.color == hit.collider.GetComponent<MeshRenderer>().material.color)
                {
                    Destroy(hit.transform.gameObject);
                }
            }
        }
    }
    
    /// <summary>
    /// 장애물에 가까이 다가갔을 때 슬로우 효과
    /// </summary>
    void OnAttackSlow()
    {
        float Distance = 5f;
        float sphereScale = 15f;
        float slowFactor = 0.4f;
        RaycastHit hit;
        
        if (Physics.SphereCast(transform.position, sphereScale / 2.0f, transform.forward, out hit, Distance))
        {
            if (hit.collider.CompareTag("Breakable"))
            {
                if (stats.current.isInvincibility)
                {
                    Destroy(hit.transform.gameObject);
                    return;
                }
                
                if (hit.transform.position.x - transform.position.x < Distance)
                {
                    Time.timeScale = slowFactor;
                    Time.fixedDeltaTime = Time.timeScale * 0.02f;
                }
            }
        }
    }

    /// <summary>
    /// 슬로우 효과로 인하여 느려졌던 시간을 원래대로 복구
    /// </summary>
    void OnTimeReturn()
    {
        float slowLength = 2f;
        Time.timeScale += 1f / slowLength * Time.unscaledDeltaTime;
        Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }
    
    /// <summary>
    /// 어택 종료 애니메이션
    /// </summary>
    public void OnAttackAnimationEnd()
    {
        isAttacking = false;
        anim.SetBool("IsAttacking", isAttacking);
    }
    
    /// <summary>
    /// 중력 반전
    /// </summary>
    public void OnReverseGravity()
    {
        anim.SetTrigger("Flip");
        isFlip = true;
        anim.SetBool("IsFlip", isFlip);
        
        if (!isGravityReversed)
        {
            isGravityReversed = true;
            Physics.gravity = new Vector3(0, gravityScale * stats.origin.jumpForce / 3, 0);
        }
        else
        {
            isGravityReversed = false;
            Physics.gravity = new Vector3(0, -gravityScale * stats.origin.jumpForce / 3, 0);
        }
    }

    public void OnFlipAnimationEnd()
    {
        isFlip = false;
        anim.SetBool("IsFlip", isFlip);
    }

    void OnCollisionEnter(Collision collision)
    {
        collisionInfo = collision;
        
        if (collision.gameObject.CompareTag("Breakable") && !stats.current.isInvincibility)
        {
            Die();
        }

        if (collisionInfo.gameObject.layer == LayerMask.NameToLayer("Ground") && !stats.current.isDead)
        {
            isGrounded = true;
            anim.SetBool("IsGrounded", true);
            particle.LandParticle();
        }
    }

    void OnCollisionStay(Collision collisionInfo)
    {
        this.collisionInfo = collisionInfo;
        
        if (collisionInfo.gameObject.CompareTag("Obstacle") && !stats.current.isInvincibility && 
            collisionInfo.gameObject.GetComponent<MeshRenderer>().material.color != meshRenderer.material.color)
        {
            Die();
        }

        float currentSlopeAngle = 0f;
        foreach (var item in collisionInfo.contacts)
        {
            var max = Vector3.Angle(Vector3.up, item.normal);
            if (max > currentSlopeAngle)
            {
                currentSlopeAngle = max;
            }
        }
        
        if (currentSlopeAngle > maxSlopeAngle)
        {
            return;
        }
        
        if (collisionInfo.gameObject.layer == LayerMask.NameToLayer("Ground") && !stats.current.isDead)
        {
            jumpCount = 0;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        collisionInfo = null;
        isGrounded = false;
        anim.SetBool("IsGrounded", false);

        if (isJumping && jumpCount == 0)
        {
            jumpCount++;
        }
    }
}