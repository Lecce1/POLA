using UnityEngine;
using UnityEngine.UI;

public class VerdictPrefab : MonoBehaviour
{
    private float delay = 0;
    private float alpha = 1f;
    public bool isUp;
    
    void Start()
    {
        Invoke("Destroy", 0.5f);
        GetComponent<Animation>().Play();
    }
    
    void Update()
    {
        delay += Time.deltaTime * 50f;
        alpha -= Time.deltaTime * 2;
        
        if (isUp)
        {
            transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -250 + delay);

        }
        else
        {
            transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -250 - delay);
        }
        
        GetComponent<Image>().color = new Color(1, 1, 1, alpha);
    }

    void Destroy()
    {
        Destroy(gameObject);
    }
}
