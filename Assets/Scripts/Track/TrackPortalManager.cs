using UnityEngine;

public class TrackPortalManager : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            TrackManager.instance.Button("Back");
        }
    }
}