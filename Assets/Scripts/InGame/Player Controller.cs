using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float currentSpeed { get; private set; }
    private int jumpCount;
    private bool isJumping;
    public Color[] playerColors;
    public PlayerStatsManager stats;
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
        
        jumpCount = 0;
        currentSpeed = rigid.velocity.x;
    }
    
    void Update()
    {
        Moving();
        OnAttackSlow();
    }

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
        
        currentSpeed = rigid.velocity.x;
    }
    
    void Die()
    {
        stats.current.isDead = true;
        rigid.useGravity = false;
        GameManager.instance.Reset();
        Destroy(gameObject, 3);
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
        float maxDistance = 30f;
        Ray ray = new Ray(transform.position, transform.right);
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
        float slowFactor = 0.43f;
        Ray ray = new Ray(transform.position +  new Vector3(1, 0, 0), transform.right);
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
            else
            {
                Time.timeScale = 1f;
            } 
        }
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
