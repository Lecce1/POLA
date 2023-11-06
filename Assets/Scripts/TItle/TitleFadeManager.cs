using UnityEngine;

public class TitleFadeManager : MonoBehaviour
{
    public void Finish()
    {
        TitleManager.instance.LoginSuccess();
    }
}
