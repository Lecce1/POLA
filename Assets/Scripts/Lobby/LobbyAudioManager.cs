using UnityEngine;
using UnityEngine.Audio;

public class LobbyAudioManager : MonoBehaviour
{
    public AudioMixer audioMixer;
    public static LobbyAudioManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
}
