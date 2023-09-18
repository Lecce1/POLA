using UnityEngine;

public class Magnetic : Effect
{
    private Collider[] overlaps = new Collider[15];
    public static Magnetic instance;

    void Awake() 
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public override void RunEffect(PlayerController player)
    {
        var overlap = Physics.OverlapSphereNonAlloc(player.transform.position, 20f, overlaps);

        for (int i = 0; i < overlap; i++)
        {
            if (overlaps[i].CompareTag("Collectable"))
            {
                overlaps[i].transform.position =
                    Vector3.MoveTowards(overlaps[i].transform.position, PlayerController.instance.transform.position, 20f * Time.deltaTime);
            }
        }
    }
}