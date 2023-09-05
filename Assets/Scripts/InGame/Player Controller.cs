using Cinemachine;
using InGame;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rigid { get; protected set; }
    
    public int jumpCount { get; protected set; }

    public Color[] playerColors;

    public PlayerStats stats;
    
    private MeshRenderer _meshRenderer;

    void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody>();
        _meshRenderer = gameObject.GetComponent<MeshRenderer>();
        jumpCount = 0;
        stats.maxSpeed = 15f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Moving();
    }

    private void Moving()
    {
        rigid.AddForce(new Vector3(stats.speedAccel, 0, 0) * Time.deltaTime, ForceMode.Force);
        if (rigid.velocity.x >= stats.maxSpeed)
        {
            var v = rigid.velocity;
            v.x = stats.maxSpeed;
            rigid.velocity = v;
        }
        
    }

    public void OnJump()
    {
        if (jumpCount < stats.maxJump)
        {
            var v = rigid.velocity;
            v.y = 0;
            rigid.velocity = v;
            rigid.AddForce(Vector3.up * stats.jumpForce, ForceMode.Impulse);
            jumpCount++;
        }
    }

    public void OnClick()
    {
        var idx = stats.colorIndex;
        _meshRenderer.material.color = playerColors[idx];
        stats.colorIndex = (++idx < playerColors.Length) ? idx : 0;
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
        if (collisionInfo.gameObject.tag == "Obstacle" &&
            collisionInfo.gameObject.GetComponent<MeshRenderer>().material.color != _meshRenderer.material.color)
        {
            Debug.Log("Die");
            stats.maxSpeed = 0;
            Destroy(gameObject, 3);
        }
    }
}
