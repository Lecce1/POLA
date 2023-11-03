using UnityEngine;

public class VerdictBar : MonoBehaviour
{
    [SerializeField]
    private NewPlayerController player;

    void Start()
    {
        player = transform.parent.parent.GetComponent<NewPlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        player.curEvaluation++;
        player.target = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        player.curEvaluation--;

        if (player.curEvaluation <= 0)
        {
            if (!player.wasTouched && !player.isInvincibility)
            {
                player.Hurt();
                player.target = null;
            }
            
            player.wasTouched = false;
        }
    }
}
