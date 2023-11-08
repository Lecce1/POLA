using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public NoteType type = NoteType.NormalNote;
    public float beatLength = 1;
    public bool isUp = false;
    public bool canDestroy = true;
    public int perfectScore = 100;
    public int greatScore = 50;
    public int goodScore = 25;
    public int[] scoreList = new int[3];
    public int comboCount = 1;
    public int damage = 1;
    public bool wasInteracted = false;

    private void Start()
    {
        scoreList[0] = perfectScore;
        scoreList[1] = greatScore;
        scoreList[2] = goodScore;
    }
}

public enum NoteType
{
    NormalNote,
    MoveNote,
    Wall,
    Heart
}