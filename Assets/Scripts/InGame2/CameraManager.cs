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
        transposer.m_FollowOffset = Vector3.Lerp(transposer.m_FollowOffset, offsetNormal * (player.groundGap * 1.5f), 0.03125f);
    }
}
