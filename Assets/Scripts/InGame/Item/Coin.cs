using System.Collections;
using UnityEngine;

public class Coin : Item, ITransparency
{
    public override IEnumerator RunItem()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(RunItem());
        }
    }

    public void GetOpaque()
    {
        gameObject.GetComponent<MeshRenderer>().material = invisible;
    }
}
