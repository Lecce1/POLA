using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject cameraInfo;
    public NewPlayerController player;
    private CinemachineTransposer transposer;
    public Vector3 offsetNormal;
    
    void Start()
    {
        transposer = transform.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>();
        offsetNormal = transposer.m_FollowOffset;
        offsetNormal = offsetNormal.normalized;
    }

    void FixedUpdate()
    {
        RaycastHit hitInfo1, hitInfo2;
        
        if (!Physics.Raycast(new Ray(player.transform.position + player.transform.up, player.transform.up), out hitInfo1, 10, player.ground))
        {
            hitInfo1.point = player.transform.position + player.transform.up * 10f;
        }
        
        if(!Physics.Raycast(new Ray(player.transform.position + player.transform.up, -player.transform.up), out hitInfo2, 10, player.ground))
        {
            hitInfo2.point = player.transform.position - player.transform.up * 10f;
        }
        
        cameraInfo.transform.position = (hitInfo1.point + hitInfo2.point) / 2;
        float distance = (hitInfo1.point - hitInfo2.point).magnitude;

        transposer.m_FollowOffset = Vector3.Lerp(transposer.m_FollowOffset, offsetNormal * (distance * 1.5f), 0.125f);
    }
}
