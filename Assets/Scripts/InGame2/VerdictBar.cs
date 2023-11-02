using UnityEngine;

public class VerdictBar : MonoBehaviour
{
    private NewPlayerController player;

    private void Start()
    {
        player = transform.parent.parent.GetComponent<NewPlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        player.curEvaluation++;
        if (other.gameObject.CompareTag("Breakable"))
        {
            player.target = other.gameObject;
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        player.curEvaluation--;

        if (player.curEvaluation == 0 && !player.wasTouched && !player.isInvincibility)
        {
            player.Hurt();
            player.target = null;
        }
    }
}
