using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private CinemachineVirtualCamera VCM;
    
    void Start()
    {
        player = GameObject.Find("Player");
        VCM = gameObject.GetComponent<CinemachineVirtualCamera>();
        VCM.m_Lens.FieldOfView = 40f;
    }
    
    void Update()
    {
        PovValue();
    }

    public void PovValue()
    {
        var speed = player.gameObject.GetComponent<PlayerController>().rigid.velocity.x;
        var maxSpeed = player.gameObject.GetComponent<PlayerController>().stats.maxSpeed;

        VCM.m_Lens.FieldOfView = speed + 40;
    }
}
