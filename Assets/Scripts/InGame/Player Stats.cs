using UnityEngine;

namespace InGame
{
    [CreateAssetMenu(fileName = "NewPlayerStats", menuName = "My Player/New Player Stats")]
    public class PlayerStats : ScriptableObject
    {
        [Header("General Stats")] 
        public float jumpForce = 10f;
        public float maxSpeed = 15f;
        public int maxJump = 2;
        public float speedAccel = 100f;

        [Header("Color Stats")] 
        public int colorIndex = 0;
    }
}