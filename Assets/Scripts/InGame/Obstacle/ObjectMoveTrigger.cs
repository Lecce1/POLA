using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements.Experimental;

[RequireComponent(typeof(Rigidbody))]
public class ObjectMoveTrigger : MonoBehaviour
{
    [Serializable]
    public class MoveData
    {
        public Vector3 wayPoint;
        [VerticalGroup("Follow Player")]
        public bool followX, followY;
        public bool isRelativePos = true;
        public float duration = 0.5f;
        [EnumPaging]
        public EasingFunction.Ease ease;
    }
    
    [Serializable]
    public class RotateData
    {
        public Vector3 rotateAmount;
        public bool isRelativeAngle = true;
        public float duration = 0.5f;
        [EnumPaging]
        public EasingFunction.Ease ease;
    }
    [FoldoutGroup("Way Point")] 
    [Title("WayPointList")] 
    [TableList] 
    public List<MoveData> wayPoints = new ();

    [FoldoutGroup("Way Point")] 
    [Title("루프")] 
    public bool isLoopMove = true;
    
    [FoldoutGroup("Rotate Point")] 
    [Title("RotatePointList")] 
    [TableList] 
    public List<RotateData> rotatePoints = new ();

    [FoldoutGroup("Rotate Point")] 
    [Title("루프")] 
    public bool isLoopRotate = true;

    [SerializeField] 
    [Title("리지드바디")] 
    private Rigidbody rigid;

    [FoldoutGroup("무브")] 
    private int curIndex = -1;
    
    [FoldoutGroup("무브")] 
    private float invDuration = 0;

    [FoldoutGroup("무브")] 
    private Vector3 currentPos;
    
    [FoldoutGroup("무브")] 
    private Vector3 nextPos;
    
    [FoldoutGroup("무브")] 
    private MoveData data;

    [FoldoutGroup("무브")] 
    private float start;
    
    [FoldoutGroup("무브")] 
    private bool wasCirculate = false;

    private Vector3 tmp = Vector3.zero;
    
    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        rigid.isKinematic = false;
        rigid.useGravity = false;
        rigid.mass = Mathf.Infinity;
        rigid.angularDrag = 0;
        
        if (rotatePoints.Count > 0)
        {
            StartCoroutine(nameof(Rotate));
        }

        if (wayPoints.Count > 0)
        {
            SetNextMove();
            tmp = transform.position;
        }
    }

    private void FixedUpdate()
    {
        if (wayPoints.Count > 0)
        {
            Debug.Log(rigid.velocity);
            Move();
        }
    }
    
    void SetNextMove()
    {
        curIndex = (curIndex + 1) % wayPoints.Count;
        Debug.LogError(transform.position);
        
        data = wayPoints[curIndex];
        invDuration = 1 / data.duration;
        currentPos = transform.position;
        nextPos = data.isRelativePos ? currentPos + wayPoints[curIndex].wayPoint : wayPoints[curIndex].wayPoint;
        start = Time.time;

        if ((curIndex + 1) % wayPoints.Count == 0)
        {
            wasCirculate = true;
        }
    }
    
    void Move()
    {
        if (wasCirculate && !isLoopMove)
        {
            return;
        }
        
        if (data.duration + start > Time.time)
        {
            var function = EasingFunction.GetEasingFunction(data.ease);
            float process = function(0, 1, (Time.time - start) * invDuration);
            var nextFramePos = Vector3.Lerp(currentPos, nextPos, process);
            var delta = nextFramePos - transform.position;

            if (data.followX)
            {
                delta.x = PlayerController.instance.rigid.velocity.x;
            }

            if (data.followY)
            {
                delta.y = PlayerController.instance.rigid.velocity.y;
            }

            rigid.velocity = delta;
            Debug.Log("delta: " + delta  + ", " + (transform.position - tmp));
        }
        else
        {
            SetNextMove();
            tmp = transform.position;
        }
    }

    IEnumerator Rotate()
    {
        int curIndex = 0;
        int nextIndex = 1 % rotatePoints.Count;

        while (true)
        {
            float start = Time.time;
            var data = rotatePoints[curIndex];
            float inverseDuration = 1 / data.duration;
            Vector3 curAngle = transform.eulerAngles;
            Vector3 nextAngle = data.isRelativeAngle ? curAngle + data.rotateAmount : data.rotateAmount;

            if (data.duration == 0)
            {
                transform.position = nextAngle;
            }

            while (data.duration + start > Time.time)
            {
                var function = EasingFunction.GetEasingFunction(data.ease);
                float process = function(0, 1, (Time.time - start) * inverseDuration);
                var delta = Quaternion.Euler(Vector3.Lerp(curAngle, nextAngle, process) - rigid.rotation.eulerAngles);
                rigid.MoveRotation(rigid.rotation * delta);

                yield return null;
            }
            
            curIndex = nextIndex;
            nextIndex = (nextIndex + 1) % rotatePoints.Count;
            
            if (!isLoopRotate && nextIndex == 0)
            {
                yield break;
            }
        }
    }
}

