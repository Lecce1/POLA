using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class PlayerTrails : MonoBehaviour
{
    [FoldoutGroup("일반")] 
    public Transform hand;
    
    [FoldoutGroup("일반")] 
    [SerializeField]
    private NewPlayerController player;
    
    [FoldoutGroup("일반")] 
    [SerializeField]
    private TrailRenderer trail;
    
    void Start()
    {
        trail = GetComponent<TrailRenderer>();
        player = GetComponentInParent<NewPlayerController>();
        
        transform.parent = hand;
        transform.localPosition = Vector3.zero;
        trail.enabled = false;
    }

    void Update() => Trails();

    void Trails()
    {
        if (player.isJump || !player.isGrounded)
        {
            trail.enabled = true;
        }
        else
        {
            trail.enabled = false;
        }
    }
}