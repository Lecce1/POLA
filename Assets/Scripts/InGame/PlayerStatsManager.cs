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
        current.maxSpeed = origin.maxSpeed;
        current.isDead = false;
        current.isInvincibility = false;
    }
}