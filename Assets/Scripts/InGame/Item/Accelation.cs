using System.Collections;
using UnityEngine;

public class Accelation : Item, ITransparency
{
    public PlayerStats stats;

    public override IEnumerator RunItem()
    {
        stats.maxSpeed = 100f;
        stats.speedAccel = 5000f;
        yield return new WaitForSeconds(0.3f);
        stats.maxSpeed = 20f;
        stats.speedAccel = 500f;
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
