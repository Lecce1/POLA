using UnityEngine;
using UnityEngine.Serialization;

public class Obstacle : MonoBehaviour
{
    public NoteType type = NoteType.NormalNote;
    public float beatLength = 1;
    public bool isUp = false;
    public bool canDestroy = true;
    public int perfectScore = 100;
    public int greatScore = 50;
    public int comboCount = 1;
    public int damage = 1;
    public bool wasInteracted = false;
}

public enum NoteType
{
    NormalNote,
    MoveNote,
    Wall,
    Heart
}