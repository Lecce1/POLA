using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerStats", menuName = "My Player/New Player Stats")]
public class PlayerStats : MonoBehaviour
{
    public int jumpCount;
    public float playerSpeed;
    public float JumpForce;
    public Color[] playerColors;
}