using UnityEngine;
using UnityEngine.UI;

public class VerdictPrefab : MonoBehaviour
{
    private float alpha = 1f;
    public bool isUp;
    
    void Start()
    {
        Invoke("Destroy", 0.5f);
    }
    
    void Update()
    {
        alpha -= Time.deltaTime * 2;
        
        if (isUp)
        {
            transform.position += Vector3.up * (Time.deltaTime * 2.5f);

        }
        else
        {
            transform.position += Vector3.up * (Time.deltaTime * -2.5f);
        }
        
        GetComponent<Image>().color = new Color(1, 1, 1, alpha);
    }

    void Destroy()
    {
        Destroy(gameObject);
    }
}
