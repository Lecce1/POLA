using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements.Experimental;

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
    
    // Start is called before the first frame update
    void Start()
    {
        if (wayPoints.Count > 0)
        {
            StartCoroutine(nameof(Move));
        }

        if (rotatePoints.Count > 0)
        {
            StartCoroutine(nameof(Rotate));
        }
    }
    
    IEnumerator Move()
    {
        int curIndex = 0;
        int nextIndex = 1 % wayPoints.Count;
        
        while (true)
        {
            float start = Time.time;
            var data = wayPoints[curIndex];
            float invDuration = 1 / data.duration;
            Vector3 curPos = transform.position;
            Vector3 nextPos = data.isRelativePos ? curPos + wayPoints[curIndex].wayPoint : wayPoints[curIndex].wayPoint;
            Vector3 delta = PlayerController.instance.transform.position - transform.position;
            
            if (data.duration == 0)
            {
                transform.position = nextPos;
            }
            
            while (data.duration + start > Time.time)
            {
                var function = EasingFunction.GetEasingFunction(data.ease);
                float process = function(0, 1, (Time.time - start) * invDuration);
                transform.position = Vector3.Lerp(curPos, nextPos, process);

                var pos = transform.position;
                
                if (data.followX)
                {
                    pos.x = PlayerController.instance.transform.position.x - delta.x;
                }

                if (data.followY)
                {
                    pos.y = PlayerController.instance.transform.position.y - delta.y;
                }
                
                transform.position = pos;
                yield return null;
            }

            curIndex = nextIndex;
            nextIndex = (nextIndex + 1) % wayPoints.Count;
            
            if (!isLoopMove && nextIndex == 0)
            {
                yield break;
            }
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
                transform.eulerAngles = Vector3.Lerp(curAngle, nextAngle, process);

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

