using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class VerdictBar : MonoBehaviour
{
    public delegate void OnTriggerExitEvent(Collider other);
    public event OnTriggerExitEvent onTriggerExitEvent;

    public readonly Queue<Collider>[] contacts = new Queue<Collider>[2];

    [FoldoutGroup("마지막 오브젝트")]
    public static GameObject lastObject;

    private void Start()
    {
        contacts[0] = new Queue<Collider>();
        contacts[1] = new Queue<Collider>();
    }

    void OnTriggerEnter(Collider other)
    {
        Obstacle obstacle = Verdict.GetObstacle(other.gameObject);
        
        if (obstacle != null && !obstacle.wasInteracted)
        {
            int i = obstacle.isUp ? 1 : 0;
            contacts[i].Enqueue(other);
        }
        
        if (other.gameObject == lastObject && !GameManager.instance.isResultPanel)
        {
            GameManager.instance.Invoke(nameof(GameManager.Finish), 2.0f);
            GameManager.instance.isResultPanel = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (onTriggerExitEvent != null)
        {
            onTriggerExitEvent(other);
        }

        Obstacle obstacle = Verdict.GetObstacle(other.gameObject);
        
        if (obstacle != null)
        {
            int i = obstacle.isUp ? 1 : 0;
            if (contacts[i].Count != 0 && contacts[i].Peek().gameObject != null && obstacle == Verdict.GetObstacle(contacts[i].Peek().gameObject))
            {
                contacts[i].Dequeue();
            }
        }
    }
}