using UnityEngine;

public class VerdictBar : MonoBehaviour
{
    public Collider[] contact;
    public delegate void OnTriggerExitEvent(Collider other);
    public event OnTriggerExitEvent onTriggerExitEvent;

    private void Start()
    {
        contact = new Collider[2];
    }

    private void OnTriggerEnter(Collider other)
    {
        Obstacle obstacle = PlayerController.GetObstacle(other.gameObject);
        
        if (obstacle != null)
        {
            int i = obstacle.isUp ? 1 : 0;
            contact[i] = other;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Obstacle obstacle = PlayerController.GetObstacle(other.gameObject);
        
        if (obstacle != null)
        {
            int i = obstacle.isUp ? 1 : 0;
            contact[i] = other;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (onTriggerExitEvent != null)
        {
            onTriggerExitEvent(other);
        }
        
        Obstacle obstacle = PlayerController.GetObstacle(other.gameObject);
        
        if (obstacle != null)
        {
            int i = obstacle.isUp ? 1 : 0;
            contact[i] = null;
        }
    }
}