using System;
using System.Reflection;
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
    
    /// <summary>
    /// 만약 물리가 존재하면 물리 값을 초기화
    /// </summary>
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
    
    /// <summary>
    /// 아이템 회전 효과
    /// </summary>
    void RotateItem()
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 먹은 아이템이 무슨 아이템인지 파악 후 아이템 매니저로 넘겨주는 역할
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        Type targetType = Type.GetType(type);
        
        if (targetType != null)
        {
            MethodInfo method = typeof(ItemManager).GetMethod(nameof(ItemManager.RunEffect))!.MakeGenericMethod(targetType);
            method.Invoke(ItemManager.instance, new object[] { duration, isLingering });
        }
        Destroy(gameObject);
    }
}

/// <summary>
/// 아이템 양식을 담당하는 추상클래스
/// </summary>
public abstract class Effect
{
    public abstract void OnStepEffect(PlayerController player);
    public abstract void OnExitEffect(PlayerController player);
}