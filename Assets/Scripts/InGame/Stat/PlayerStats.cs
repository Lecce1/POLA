using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Player/Player Stats")]
public class PlayerStats : ScriptableObject
{
    [FoldoutGroup("일반")]
    [FoldoutGroup("일반/점프")]
    public float jumpForce = 10f;
    [FoldoutGroup("일반/점프")]
    public int maxJump = 2;
    [FoldoutGroup("일반/점프")]
    public float jumpLength = 1;
        
    [FoldoutGroup("일반/속도")]
    public float maxSpeed = 15f;
    [FoldoutGroup("일반/속도")]
    public float acceleration = 100f;
        
    [FoldoutGroup("일반/기타")]
    public bool isDead = false;
    [FoldoutGroup("일반/기타")]
    public bool isInvincibility = false;

    [FoldoutGroup("색상")]
    public int colorIndex = 0;
}