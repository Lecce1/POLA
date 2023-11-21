using UnityEngine;

public class SkyboxManager : MonoBehaviour
{
    public float speed;
    float degree;

    void Start ()
    {
        degree = 0;
    }
    
    void Update ()
    {
        if (DBManager.instance.currentGround == 2)
        {
            degree += Time.deltaTime * speed;
        
            if (degree >= 360)
            {
                degree = 0;
            }
        
            RenderSettings.skybox.SetFloat("_Rotation", degree);
        }
    }
}
