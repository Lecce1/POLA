using UnityEngine;
using UnityEngine.UI;

public class VerdictPrefab : MonoBehaviour
{
    private float alpha = 1f;
    void Start()
    {
        Invoke("Destroy", 0.5f);
    }
    void Update()
    {
        alpha -= Time.deltaTime * 2;
        transform.position += Vector3.up * Time.deltaTime * 2.5f;
        GetComponent<Image>().color = new Color(1, 1, 1, alpha);
    }

    void Destroy()
    {
        Destroy(gameObject);
    }
}
