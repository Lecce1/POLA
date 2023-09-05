using InGame;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rigid { get; protected set; }
    public int jumpCount { get; protected set; }
    public Color[] playerColors;
    public PlayerStats stats;
    private MeshRenderer meshRenderer;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        jumpCount = 0;
        stats.maxSpeed = 20.0f;
        stats.isDead = false;
    }
    
    void FixedUpdate()
    {
        Moving();
        Boost();
    }

    private void Moving()
    {
        Debug.Log(rigid.velocity.x);
        if (stats.isDead)
        {
            rigid.velocity = Vector3.zero;
            return;
        }
        
        if (rigid.velocity.x < stats.maxSpeed)
        {  
            rigid.AddForce(Vector3.right * stats.speedAccel * Time.deltaTime, ForceMode.Force);
        }

        if (transform.position.y < -5) Die();
    }

    private void Die()
    {
        stats.isDead = true;
        rigid.useGravity = false;
        Destroy(gameObject, 3);
    }

    private void Boost()
    {
        if (stats.boostGauge > 0 && Input.GetKey(KeyCode.Space))
        {
            stats.maxSpeed = 30f;
            stats.speedAccel = 1500f;
            stats.boostGauge -= Time.deltaTime;
        }
        else
        {
            stats.boostGauge += Time.deltaTime;
            stats.maxSpeed = 20f;
            stats.speedAccel = 500f;
        }

        stats.boostGauge = Mathf.Clamp(stats.boostGauge, 0, 2);
    }

    public void OnJump()
    {
        if (jumpCount < stats.maxJump)
        {
            var velocity = rigid.velocity;
            velocity.y = 0;
            rigid.velocity = velocity;
            rigid.AddForce(Vector3.up * stats.jumpForce, ForceMode.Impulse);
            jumpCount++;
        }
    }

    public void OnColorChanged()
    {
        var idx = stats.colorIndex;
        meshRenderer.material.color = playerColors[idx];
        stats.colorIndex = ++idx < playerColors.Length ? idx : 0;
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
