using UnityEngine;

public class VerdictBar : MonoBehaviour
{
    public Collider contact;
    
    private void OnTriggerEnter(Collider other)
    {
        if (contact != null)
        {
            contact = other;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        contact = null;
    }
}
