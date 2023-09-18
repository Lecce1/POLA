using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float currentSpeed { get; private set; }
    private int jumpCount;
    private bool isJumping;
    public Color[] playerColors;
    PlayerStatsManager stats;
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
    
    void FixedUpdate()
    {
        Moving();
        Boost();
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
        Destroy(gameObject, 3);
    }

     void Boost()
    {
        if (stats.current.boostGauge > 0 && Input.GetKey(KeyCode.Space))
        {
            stats.current.maxSpeed += 10f;
            stats.current.acceleration += 500f;
            stats.current.boostGauge -= Time.deltaTime;
        }
        else
        {
            stats.current.boostGauge += Time.deltaTime;
            stats.current.maxSpeed = stats.origin.maxSpeed;
            stats.current.acceleration = stats.origin.acceleration;
        }

        stats.current.boostGauge = Mathf.Clamp(stats.current.boostGauge, 0, 2);
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

    void OnCollisionEnter(Collision collision)
    {
        if (Physics.Raycast(transform.position, Vector3.down + Vector3.right * 0.6f, 1f) 
            && collision.gameObject.layer == LayerMask.NameToLayer("Ground") && !stats.current.isDead)
        {
            jumpCount = 0;   
        }
    }

    void OnCollisionStay(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.CompareTag("Obstacle") &&
            collisionInfo.gameObject.GetComponent<MeshRenderer>().material.color != meshRenderer.material.color)
        {
            Die();
        }
    }
}
