using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float currentSpeed { get; private set; }
    private int jumpCount;
    private bool isJumping;
    public Color[] playerColors;
    public PlayerStatsManager stats;
    public float maxSlopeAngle = 50.0f;
    public RaycastHit slopeHit;
    MeshRenderer meshRenderer;
    Rigidbody rigid;
    Transform transform;

    public static PlayerController instance;

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
    
    void Update()
    {
        Moving();
        OnAttackSlow();
        OnTimeReturn();
    }

    void Moving()
    {
        SlopeProcess();
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
        
        currentSpeed = rigid.velocity.x;
    }
    
    void Die()
    {
        stats.current.isDead = true;
        rigid.useGravity = false;
        GameManager.instance.Reset();
        Destroy(gameObject, 3);
    }

    void SlopeProcess()
    {
        if (IsOnSlope())
        {
            Vector3 velocity = AdjustDirectionToSlope(Vector3.right);
            Debug.Log(velocity);
            var slopeVelocity = velocity.y * Time.deltaTime * currentSpeed;
            var vel = rigid.velocity;
            vel.y = slopeVelocity;
            rigid.velocity = vel;
        }
    }
    
    IEnumerator Jump()
    {
        float pressedJumpStartTime = Time.time;
        float inverseJumpLength = 1 / stats.current.jumpLength;
        
        while (Time.time - pressedJumpStartTime < stats.current.jumpLength && isJumping)
        {
            var velocity = rigid.velocity;
            velocity.y = stats.current.jumpForce;
            rigid.velocity = velocity;
            stats.current.jumpForce -= Time.deltaTime * stats.current.jumpForce * inverseJumpLength;
            SlopeProcess();
            Debug.Log(rigid.velocity.y);
            yield return null;
        }
    }

    public void OnJumpButtonDown()
    {
        if (jumpCount >= stats.current.maxJump)
            return;
        
        stats.current.jumpForce = stats.origin.jumpForce;
        isJumping = true;
        StartCoroutine(Jump());
        jumpCount++;
    }

    public void OnJumpButtonUp()
    {
        isJumping = false;
    }

    public void OnColorChanged()
    {
        var idx = stats.current.colorIndex;
        meshRenderer.material.color = playerColors[idx];
        stats.current.colorIndex = ++idx < playerColors.Length ? idx : 0;
    }
    
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

    void OnTimeReturn()
    {
        float slowLength = 2f;
        Time.timeScale += (1f / slowLength) * Time.unscaledDeltaTime;
        Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }
    
    bool IsOnSlope()
    {
        float maxDistance = 0.5f;
        Ray ray = new Ray(transform.position + Vector3.up, transform.up);

        if (Physics.Raycast(ray, out slopeHit, maxDistance))
        {
            var angle = Vector3.Angle(Vector3.down, slopeHit.normal);
            return angle != 0f && angle < maxSlopeAngle;
        }

        return false;
    }
    
    Vector3 AdjustDirectionToSlope(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
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
    }

    void OnCollisionStay(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.CompareTag("Obstacle") && !stats.current.isInvincibility &&
            collisionInfo.gameObject.GetComponent<MeshRenderer>().material.color != meshRenderer.material.color)
        {
            Die();
        }
    }
}