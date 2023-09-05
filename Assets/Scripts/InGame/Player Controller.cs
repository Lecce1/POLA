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
    }
    
    void FixedUpdate()
    {
        Moving();
    }

    private void Moving()
    {
        rigid.AddForce(new Vector3(stats.speedAccel, 0, 0) * Time.deltaTime, ForceMode.Force);
        
        if (rigid.velocity.x >= stats.maxSpeed)
        {
            var velocity = rigid.velocity;
            velocity.x = stats.maxSpeed;
            rigid.velocity = velocity;
        }
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

    public void OnClick()
    {
        var idx = stats.colorIndex;
        meshRenderer.material.color = playerColors[idx];
        stats.colorIndex = ++idx < playerColors.Length ? idx : 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            jumpCount = 0;
        }
    }

    private void OnCollisionStay(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.CompareTag("Obstacle") &&
            collisionInfo.gameObject.GetComponent<MeshRenderer>().material.color != meshRenderer.material.color)
        {
            Debug.Log("Die");
            stats.maxSpeed = 0;
            Destroy(gameObject, 3);
        }
    }
}
