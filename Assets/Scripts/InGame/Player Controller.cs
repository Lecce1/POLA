using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Cinemachine;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(PlayerStats))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float xValue = 1.0f;
    [SerializeField] private float jumpPower = 5.0f;

    public PlayerStats stats { get; protected set; }
    private int jumpConunt = 0;
    private int clickNum = 0;
    private Rigidbody rigid;
    private CinemachineVirtualCamera cam;
    private MeshRenderer _meshRenderer;
    void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody>();
        _meshRenderer = gameObject.GetComponent<MeshRenderer>(); 
        jumpConunt = 2;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Moving();
    }

    private void Moving()
    {
        rigid.AddForce(new Vector3(xValue, 0, 0) * Time.deltaTime);
        if (rigid.velocity.x >= 15)
        {
            var v = rigid.velocity;
            v.x = 15;
            rigid.velocity = v;
        }
        Debug.Log(rigid.velocity.x);
        
    }

    public void OnJump()
    {
        if (jumpConunt != 0)
        {
            var v = rigid.velocity;
            v.y = 0;
            rigid.velocity = v;
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            jumpConunt -= 1;
        }
    }

    public void OnClick()
    {
        if (clickNum % 3 == 0)
        {
            _meshRenderer.material.color = Color.red;
        }
        else if (clickNum % 3 == 1)
        {
            _meshRenderer.material.color = Color.blue;
        }
        else if (clickNum % 3 == 2)
        {
            _meshRenderer.material.color = Color.black;
        }

        clickNum += 1;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            jumpConunt = 2;
        }

        if (collision.gameObject.tag == "Obstacle" &&
            collision.gameObject.GetComponent<MeshRenderer>().material.color != _meshRenderer.material.color)
        {
            Debug.Log("Die");
            xValue = 0;
            Destroy(gameObject, 3);
        }
    }

    private void OnCollisionStay(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.tag == "Obstacle" &&
            collisionInfo.gameObject.GetComponent<MeshRenderer>().material.color != _meshRenderer.material.color)
        {
            Debug.Log("Die");
            xValue = 0;
            Destroy(gameObject, 3);
        }
    }
}
