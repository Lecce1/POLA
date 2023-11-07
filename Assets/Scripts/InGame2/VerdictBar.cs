using UnityEngine;

public class VerdictBar : MonoBehaviour
{
    public Collider contact;
    public delegate void OnTriggerExitEvent(Collider other);
    public event OnTriggerExitEvent onTriggerExitEvent;

    private void Start()
    {
        contact = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        contact = other;
    }

    private void OnTriggerStay(Collider other)
    {
        contact = other;
    }

    private void OnTriggerExit(Collider other)
    {
        if (onTriggerExitEvent != null)
        {
            onTriggerExitEvent(other);
        }
        contact = null;
    }
}