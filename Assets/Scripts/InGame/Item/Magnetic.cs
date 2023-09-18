using System.Collections;
using UnityEngine;

public class Magnetic : Item, ITransparency
{
    private Collider[] overlaps = new Collider[15];

    public override IEnumerator RunItem()
    {
        float start = Time.time;
        
        while (start + duration > Time.time)
        {
            var overlap = Physics.OverlapSphereNonAlloc(transform.position, 20f, overlaps);
            Debug.Log(overlap);

            for (int i = 0; i < overlap; i++)
            {
                if (overlaps[i].CompareTag("Collectable"))
                {
                    overlaps[i].transform.position =
                        Vector3.MoveTowards(overlaps[i].transform.position, player.transform.position, 20f * Time.deltaTime);
                }
            }
            yield return null;
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
