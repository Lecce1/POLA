using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Item : MonoBehaviour
{
    protected PlayerController player;
    private Rigidbody rigid;

    [FoldoutGroup("일반")] 
    public float rotateSpeed;
    
    [FoldoutGroup("일반")] 
    public float duration;
    
    [FoldoutGroup("일반")] 
    public bool isLingering;
    
    [FoldoutGroup("물리")] 
    public Vector3 initialVelocity = new Vector3(0, 12, 0);
    
    [FoldoutGroup("물리")] 
    public bool usePhysics;

    [FoldoutGroup("물리")] 
    public Vector3 dropDirection;
    
    [FoldoutGroup("물리")] 
    public bool randomizeDropDirection;
    
    
    

    void InitializeVelocity()
    {
        var direction = initialVelocity.normalized;

        if (randomizeDropDirection)
        {
            var randomX = Random.Range(3, 6);
            var randomY = Random.Range(3, 6);
            direction = Quaternion.Euler(randomX, 0, 0) * direction;
            direction = Quaternion.Euler(0, randomY, 0) * direction;
        }
        else
        {
            direction = dropDirection;
        }

        rigid.velocity = direction;
    }
    
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        
        InitializeVelocity();
        
        if (!usePhysics)
        {
            Destroy(rigid);
        }
        else
        {
            InitializeVelocity();
        }
    }
    
    public virtual void Update()
    {
        RotateItem();
    }

    void RotateItem()
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
    }

    private IEnumerator DestoryGameObject()
    {
        yield return null;
        Destroy(gameObject);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(DestoryGameObject());
            player = other.GetComponent<PlayerController>();
        }
    }
}