using UnityEngine;

public class Magnetic : Effect
{
    private Collider[] overlaps = new Collider[15];

    /// <summary>
    /// 자석 효과
    /// </summary>
    public override void OnStepEffect(PlayerController player)
    {
        var overlap = Physics.OverlapSphereNonAlloc(player.transform.position, 20f, overlaps);

        for (int i = 0; i < overlap; i++)
        {
            if (overlaps[i].CompareTag("Collectable"))
            {
                overlaps[i].transform.position =
                    Vector3.MoveTowards(overlaps[i].transform.position, PlayerController.instance.transform.position, 30f * Time.deltaTime);
            }
        }
    }

    public override void OnExitEffect(PlayerController player)
    {
        
    }
}