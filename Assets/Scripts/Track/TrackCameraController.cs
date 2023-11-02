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

        Vector3 direction = (player.transform.position - transform.position).normalized;
        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, Mathf.Infinity,
            1 << LayerMask.NameToLayer("Environment"));
        
        for (int i = 0; i < hits.Length; i++)
        {
            TransparentObject obj = hits[i].transform.GetComponent<TransparentObject>();
            obj.Transparent();
        }
    }
}
