using UnityEngine;

public class PlayerStatsManager : MonoBehaviour
{
    public PlayerStats origin;
    public PlayerStats current;

    private void Start()
    {
        current.maxSpeed = origin.maxSpeed;
        current.isDead = false;
    }
}