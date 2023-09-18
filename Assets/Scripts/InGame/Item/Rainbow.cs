using System.Collections;
using UnityEngine;

public class Rainbow : Item, ITransparency
{
    public override IEnumerator RunItem()
    {
        float start = Time.time;
        
        while (start + duration > Time.time)
        {
            player.gameObject.layer = 7;
            yield return null;
            player.gameObject.layer = 0;
            
        }
        
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GetOpaque();
            StartCoroutine(RunItem());
        }
    }

    public void GetOpaque()
    {
        gameObject.GetComponent<MeshRenderer>().material = invisible;
    }
}
