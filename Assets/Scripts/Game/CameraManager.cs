using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject cameraInfo;
    public Vector3 offset;

    void FixedUpdate()
    {
        if (GameManager.instance.isCountDown)
        {
            return;
        }

        transform.position = cameraInfo.transform.position + offset;
    }
}
