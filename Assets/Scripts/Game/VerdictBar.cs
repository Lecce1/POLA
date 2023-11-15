using System.Collections.Generic;
using UnityEngine;

public class VerdictBar : MonoBehaviour
{
    public delegate void OnTriggerExitEvent(Collider other);
    public event OnTriggerExitEvent onTriggerExitEvent;

    public Queue<Collider>[] contacts = new Queue<Collider>[2];
    public List<Collider> tmp1 = new List<Collider> ();
    
    public List<Collider> tmp2 = new List<Collider> ();

    private void Start()
    {
        contacts[0] = new Queue<Collider>();
        contacts[1] = new Queue<Collider>();

        tmp1 = new List<Collider>();
        tmp2 = new List<Collider>();
    }

    private void Update()
    {
        tmp1.Clear();
        tmp2.Clear();
        
        foreach (var item in contacts[0])
        {
            tmp1.Add(item);
        }
        foreach (var item in contacts[1])
        {
            tmp2.Add(item);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Obstacle obstacle = PlayerController.GetObstacle(other.gameObject);
        
        if (obstacle != null && !obstacle.wasInteracted)
        {
            int i = obstacle.isUp ? 1 : 0;
            contacts[i].Enqueue(other);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (onTriggerExitEvent != null)
        {
            onTriggerExitEvent(other);
        }

        Obstacle obstacle = PlayerController.GetObstacle(other.gameObject);
        
        if (obstacle != null)
        {
            int i = obstacle.isUp ? 1 : 0;
            if (contacts[i].Count != 0 && contacts[i].Peek().gameObject != null && obstacle == PlayerController.GetObstacle(contacts[i].Peek().gameObject))
            {
                contacts[i].Dequeue();
            }
        }
    }
}