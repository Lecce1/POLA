using Sirenix.OdinInspector;
using UnityEngine;

public class Polaroid : MonoBehaviour
{
    [Title("폴라로이트 카드")]
    public GameObject polaroid_Card;
    
    public void Check()
    {
        if (TitleManager.instance.isPress)
        {
            GetComponent<Animation>().Stop();
            GetComponent<AudioSource>().Play();
            Invoke("Polaroid_Card", 3.0f);
        }
    }

    void Polaroid_Card()
    {
        polaroid_Card.GetComponent<Animation>().Play();
        polaroid_Card.GetComponent<AudioSource>().Play();
        StartCoroutine(TitleManager.instance.Cinemachine());
    }
}
