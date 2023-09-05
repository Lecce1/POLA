using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Rigidbody playerRigid;
    [SerializeField] private CinemachineVirtualCamera VCM;
    
    void Start()
    {
        playerRigid = GameObject.Find("Player").GetComponent<PlayerController>().rigid;
        VCM = gameObject.GetComponent<CinemachineVirtualCamera>();
        VCM.m_Lens.FieldOfView = 40f;
    }
    
    void Update()
    {
        PovValue();
    }

    public void PovValue()
    {
        if (playerRigid == null) return;
        
        var speed = playerRigid.velocity.x;

        VCM.m_Lens.FieldOfView = speed + 40;
    }
}
