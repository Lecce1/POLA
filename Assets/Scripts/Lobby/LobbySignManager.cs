using UnityEngine;

public class LobbySignManager : MonoBehaviour
{
    public void Close()
    {
        if (LobbyManager.instance.sign.activeSelf)
        {
            LobbyManager.instance.sign.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            if (!LobbyManager.instance.sign.activeSelf)
            {
                LobbyManager.instance.sign.SetActive(true);
            }
        }
    }
}
