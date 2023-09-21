using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private int jumpCount;
    
    [SerializeField]
    private bool isJumping;
    
    [SerializeField]
    private float maxSlopeAngle = 50.0f;
    
    private Collision collisionInfo;
    
    [FoldoutGroup("스텟")]
    public float currentSpeed { get; private set; }
    
    [FoldoutGroup("스텟")]
    public PlayerStatsManager stats;
    
    [FoldoutGroup("일반")]
    public Color[] playerColors;
    
    [SerializeField]
    MeshRenderer meshRenderer;
    
    [SerializeField]
    Rigidbody rigid;
    
    [SerializeField]
    Transform transform;

    public static PlayerController instance;

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
        meshRenderer = GetComponent<MeshRenderer>();
        transform = GetComponent<Transform>();

        Time.timeScale = 1f;
        jumpCount = 0;
        currentSpeed = rigid.velocity.x;
    }

    private void FixedUpdate()
    {
        Moving();
        OnAttackSlow();
        OnTimeReturn();
        currentSpeed = rigid.velocity.x;
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
        stats.current.isDead = true;
        rigid.useGravity = false;
        GameManager.instance.Reset();
        Destroy(gameObject, 3);
    }
    
    /// <summary>
    /// 점프 기능 구현
    /// </summary>
    IEnumerator Jump()
    {
        float pressedJumpStartTime = Time.time;
        float inverseJumpLength = 1 / stats.current.jumpLength;
        
        while (Time.time - pressedJumpStartTime < stats.current.jumpLength && isJumping)
        {
            if (!SlopeProcess(collisionInfo))
            {
                var velocity = rigid.velocity;
                velocity.y = stats.current.jumpForce;
                rigid.velocity = velocity;
            }
            stats.current.jumpForce -= Time.deltaTime * stats.current.jumpForce * inverseJumpLength;
            yield return null;
        }
    }

    /// <summary>
    /// 경사면 충돌에 관한 처리 및 계산
    /// </summary>
    /// <param name="collisionInfo">플레이어 기준 오브젝트에 머리를 부딪혔을 때</param>
    bool SlopeProcess(Collision collisionInfo)
    {
        if (collisionInfo == null)
        {
            return false;
        }

        Vector3 tmp;
        float angle = -1;
        
        foreach (var item in collisionInfo.contacts)
        {
            if (Vector3.Angle(Vector3.up, item.normal) >= 90)
            {
                continue;
            }
            tmp = Vector3.ProjectOnPlane(Vector3.right, item.normal).normalized;
            angle = 90 - Vector3.Angle(Vector3.down, tmp);
        }
        
        if (angle <= maxSlopeAngle && angle > 0)
        {
            var velocity = rigid.velocity;
            velocity.x = currentSpeed;
            velocity.y = Mathf.Tan(angle * Mathf.Deg2Rad) * velocity.x;
            rigid.velocity = velocity;
            Moving();
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// 점프 버튼을 눌렀을 때
    /// </summary>
    public void OnJumpButtonDown()
    {
        if (jumpCount >= stats.current.maxJump)
        {
            return;
        }
        
        stats.current.jumpForce = stats.origin.jumpForce;
        isJumping = true;
        StartCoroutine(Jump());
        jumpCount++;
    }

    /// <summary>
    /// 점프 버튼을 뗐을 때
    /// </summary>
    public void OnJumpButtonUp()
    {
        isJumping = false;
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
        float maxDistance = 5f;
        Ray ray = new Ray(transform.position + Vector3.right, transform.right);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            if (hit.transform.CompareTag("Breakable"))
            {
                Destroy(hit.transform.gameObject);
            }
        }
    }
    
    /// <summary>
    /// 장애물에 가까이 다가갔을 때 슬로우 효과
    /// </summary>
    void OnAttackSlow()
    {
        float Distance = 5f;
        float slowFactor = 0.5f;
        Ray ray = new Ray(transform.position +  Vector3.right, transform.right);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, Distance))
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
        Time.timeScale += (1f / slowLength) * Time.unscaledDeltaTime;
        Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (Physics.Raycast(transform.position, Vector3.down + Vector3.right * 0.6f, 1f) 
            && collision.gameObject.layer == LayerMask.NameToLayer("Ground") && !stats.current.isDead)
        {
            jumpCount = 0;   
        }

        if (collision.gameObject.CompareTag("Breakable") && !stats.current.isInvincibility)
        {
            Die();
        }

        collisionInfo = collision;
    }

    void OnCollisionStay(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.CompareTag("Obstacle") && !stats.current.isInvincibility &&
            collisionInfo.gameObject.GetComponent<MeshRenderer>().material.color != meshRenderer.material.color)
        {
            Die();
        }

        this.collisionInfo = collisionInfo;
    }

    private void OnCollisionExit(Collision other)
    {
        collisionInfo = null;
    }
}