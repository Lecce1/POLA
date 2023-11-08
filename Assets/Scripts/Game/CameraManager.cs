using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public PlayerController player;
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
        if (GameManager.instance.isCountDown)
        {
            return;
        }
        
        transposer.m_FollowOffset = Vector3.Lerp(transposer.m_FollowOffset, offsetNormal * (player.groundGap * 1.5f), 0.03125f);
    }
}
