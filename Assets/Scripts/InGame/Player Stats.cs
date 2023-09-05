using UnityEngine;
using Sirenix.OdinInspector;

namespace InGame
{
    [CreateAssetMenu(fileName = "PlayerStats", menuName = "Player/Player Stats")]
    public class PlayerStats : ScriptableObject
    {
        [FoldoutGroup("일반 스텟")]
        [Title("패널")]
        public float jumpForce = 10f;
        
        [FoldoutGroup("일반 스텟")]
        [Title("패널")]
        public float maxSpeed = 15f;
        
        [FoldoutGroup("일반 스텟")]
        [Title("패널")]
        public int maxJump = 2;
        
        [FoldoutGroup("일반 스텟")]
        [Title("패널")]
        public float speedAccel = 100f;
        
        [FoldoutGroup("일반 스텟")]
        [Title("패널")]
        public bool isDead = false;

        [FoldoutGroup("일반 스텟")] 
        [Title("패널")] 
        public float boostGauge = 2;

        [FoldoutGroup("색깔 스텟")]
        [Title("패널")]
        public int colorIndex = 0;
    }
}