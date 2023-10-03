using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerStatsManager : MonoBehaviour
{
    [FoldoutGroup("스텟")]
    public PlayerStats origin;
    
    [FoldoutGroup("스텟")]
    public PlayerStats current;

    private void Start()
    {
        current.jumpForce = origin.jumpForce;
        current.maxJump = origin.maxJump;
        current.jumpLength = origin.jumpLength;
        
        current.maxSpeed = origin.maxSpeed;
        current.acceleration = origin.acceleration;
        
        current.isDead = origin.isDead;
        current.isInvincibility = origin.isInvincibility;
        current.colorIndex = origin.colorIndex;
    }
}