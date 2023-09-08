using System;
using System.Collections;
using System.Diagnostics;
using InGame;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rigid { get; protected set; }
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
        Boost();
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
        Debug.Log(stats.boostGauge);
        if (Input.GetKey(KeyCode.Space))
        {
            pressedBoostImage.fillAmount += Time.deltaTime;
            if (stats.boostGauge > 0 && pressedBoostImage.fillAmount == 1)
            {
                stats.maxSpeed = 30f;
                stats.speedAccel = 1500f;
                stats.boostGauge -= Time.deltaTime;
                boostGaugeImage.fillAmount -= Time.deltaTime;
            }
            if (stats.boostGauge == 0)
            {
                rigid.velocity = Vector3.zero;
            }
        }
        else
        {
            stats.boostGauge += Time.deltaTime;
            stats.maxSpeed = 20f;
            stats.speedAccel = 500f;
            boostGaugeImage.fillAmount += Time.deltaTime;
            pressedBoostImage.fillAmount = 0;
        }

        stats.boostGauge = Mathf.Clamp(stats.boostGauge, 0, 2);
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
            Debug.Log(jumpForce < 0);
            yield return null;
        }
    }

    public void OnJumpButtonDown()
    {
        if (jumpCount >= stats.maxJump) return;

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
        float maxDistance = 30f;
        Ray ray = new Ray(transform.position, transform.right);
        RaycastHit hit;
        
        Gizmos.color = Color.red;
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            if (hit.transform.CompareTag("Breakable"))
            {
                Destroy(hit.transform.gameObject);
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
