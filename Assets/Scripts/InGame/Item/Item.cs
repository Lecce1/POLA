using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class Item : MonoBehaviour
{
    [SerializeField]
    private Rigidbody rigid;
    
    [FoldoutGroup("종류")] 
    public string type;

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
    
    void Update()
    {
        RotateItem();
    }
    
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
    
    void RotateItem()
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        switch (type)
        {
            case "Magnetic":
                ItemManager.instance.RunEffect<Magnetic>(duration, isLingering);
                break;
            case "Accelation":
                ItemManager.instance.RunEffect<Accelation>(duration, isLingering);
                break;
            case "Rainbow":
                ItemManager.instance.RunEffect<Rainbow>(duration, isLingering);
                break;
            case "Coin":
                ItemManager.instance.RunEffect<Coin>(duration, isLingering);
                break;
        }
        Destroy(gameObject);
    }
}

public abstract class Effect
{
    public abstract void RunEffect(PlayerController player);

    public abstract void RunExit(PlayerController player);
}