using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float xValue = 1.0f;
    [SerializeField] private float jumpPower = 5.0f;
    private int jumpConunt = 0;
    private int clickNum = 0;
    private Rigidbody rigid;
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
        if (xValue >= 10.0f)
        {
            xValue = 10.0f;
            transform.Translate(xValue * Time.deltaTime, 0, 0, Space.World);
        }
        else
        {
            transform.Translate(xValue * Time.deltaTime, 0, 0, Space.World);
            xValue += 0.005f;
        }
    }

    public void OnJump()
    {
        if (jumpConunt != 0)
        {
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
