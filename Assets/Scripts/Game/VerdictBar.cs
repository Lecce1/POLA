using System;
using UnityEngine;

public class VerdictBar : MonoBehaviour
{
    public delegate void OnTriggerExitEvent(Collider other);
    public event OnTriggerExitEvent onTriggerExitEvent;

    public Array[] collider = new Array[2];
    [Serializable]
    public class Array
    {
        public Collider[] contact = new Collider[2];
    }

    void OnTriggerEnter(Collider other)
    {
        Obstacle obstacle = PlayerController.GetObstacle(other.gameObject);
        
        if (obstacle != null && !obstacle.wasInteracted)
        {
            Debug.Log("?");
            int i = obstacle.isUp ? 1 : 0;

            if (collider[i].contact[0] == null)
            {
                collider[i].contact[0] = other;
            }
            else
            {
                collider[i].contact[1] = other;
            }
        }
    }

    // void OnTriggerStay(Collider other)
    // {
    //     Obstacle obstacle = PlayerController.GetObstacle(other.gameObject);
    //     
    //     if (obstacle != null && !obstacle.wasInteracted)
    //     {
    //         int i = obstacle.isUp ? 1 : 0;
    //         
    //         if (collider[i].contact[0] == null )
    //         {
    //             collider[i].contact[0] = other;
    //         }
    //         else if(collider[i].contact[0] != other)
    //         {
    //             collider[i].contact[1] = other;
    //         }
    //     }
    // }

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

            if (collider[i].contact[1] != null)
            {
                collider[i].contact[0] = collider[i].contact[1];
                collider[i].contact[1] = null;
            }
            else
            {
                collider[i].contact[0] = null;
            }
        }
    }
}