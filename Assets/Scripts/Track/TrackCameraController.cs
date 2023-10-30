using UnityEngine;
using Sirenix.OdinInspector;

public class TrackCameraController : MonoBehaviour 
{
    [Title("플레이어")] 
    public GameObject player;
    [Title("Offset")] 
    [SerializeField]
    private Vector3 offset;

    void LateUpdate ()
    {
        transform.position = player.transform.position + offset;  
    }
}
