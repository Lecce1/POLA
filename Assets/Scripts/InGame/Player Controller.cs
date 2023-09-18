using System.Collections;
using TreeEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rigid;
    public int jumpCount { get; protected set; }
    public float jumpForce { get; protected set; }
    public Color[] playerColors;
    public PlayerStats stats;
    private MeshRenderer meshRenderer;
    private bool isJumping;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        jumpCount = 0;
        stats.maxSpeed = 20.0f;
        stats.isDead = false;
        jumpForce = stats.jumpForce;
    }
    
    void FixedUpdate()
    {
        Moving();
        OnAttackSlow();
    }

    private void Moving()
    {
        if (stats.isDead)
        {
            rigid.velocity = Vector3.zero;
            return;
        }
        
        if (rigid.velocity.x < stats.maxSpeed)
        {  
            rigid.AddForce(Vector3.right * stats.speedAccel * Time.deltaTime, ForceMode.Force);
        }

        if (transform.position.y < -5)
        {
            Die();
        }
    }

    private void Die()
    {
        stats.isDead = true;
        rigid.useGravity = false;
        GameManager.instance.Reset();
        Destroy(gameObject, 3);
    }
    

    IEnumerator Jump()
    {
        float pressedJumpStartTime = Time.time;
        float inverseJumpLength = 1 / stats.jumpLength;
        
        while (Time.time - pressedJumpStartTime < stats.jumpLength && isJumping)
        {  
            var velocity = rigid.velocity;
            velocity.y = jumpForce;
            rigid.velocity = velocity;
            jumpForce -= Time.deltaTime * jumpForce * inverseJumpLength;
            yield return null;
        }
    }

    public void OnJumpButtonDown()
    {
        if (jumpCount >= stats.maxJump)
        {
            return;
        }

        jumpForce = stats.jumpForce;
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
        var idx = stats.colorIndex;
        meshRenderer.material.color = playerColors[idx];
        stats.colorIndex = ++idx < playerColors.Length ? idx : 0;
    }
    
    public void OnAttackButtonClicked()
    {
        float maxDistance = 5f;
        Ray ray = new Ray(transform.position, transform.right);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            if (hit.transform.CompareTag("Breakable"))
            {
                Destroy(hit.transform.gameObject);
            }
        }

        Time.timeScale = 1f;
    }

    private void OnAttackSlow()
    {
        float Distance = 5f;
        float slowFactor = 0.43f;
        Ray ray = new Ray(transform.position, transform.right);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, Distance))
        {
            if (hit.collider.CompareTag("Breakable"))
            {
                if (hit.transform.position.x - transform.position.x < Distance)
                {
                    Time.timeScale = slowFactor;
                    Time.fixedDeltaTime = Time.timeScale * 0.02f;
                }
            }
            else
            {
                Time.timeScale = 1f;
                Time.fixedDeltaTime = 1f;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Physics.Raycast(transform.position, Vector3.down + Vector3.right * 0.6f, 1f) 
            && collision.gameObject.layer == LayerMask.NameToLayer("Ground") && !stats.isDead)
        {
            jumpCount = 0;   
        }
    }

    private void OnCollisionStay(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.CompareTag("Obstacle") &&
            collisionInfo.gameObject.GetComponent<MeshRenderer>().material.color != meshRenderer.material.color)
        {
            Die();
        }
    }
}
