using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Networking;

public class HttpServerManager : MonoBehaviour
{
    [FoldoutGroup("타이틀")] 
    [Title("시작 버튼")] 
    public TitleManager titleManager;
    
    public static HttpServerManager instance;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public IEnumerator Login(string id, string pw)
    {
        WWWForm form = new WWWForm();
        form.AddField("ID", id);
        form.AddField("PW", pw);
        
        UnityWebRequest request = UnityWebRequest.Post("http://enddl2560.dothome.co.kr/Capstone/Login.php", form);
        yield return request.SendWebRequest();

        if (request.downloadHandler.text.Split('|')[0] == "로그인 완료")
        {
            titleManager.LoginSuccess();
        }
        else
        {
            titleManager.LoginFail();
        }

        request.Dispose();
    }
    
    public IEnumerator SignUp(string id, string pw, string nick)
    {
        WWWForm form = new WWWForm();
        form.AddField("ID", id);
        form.AddField("PW", pw);
        form.AddField("NICK", nick);
        UnityWebRequest request = UnityWebRequest.Post("http://enddl2560.dothome.co.kr/Capstone/SignUp.php", form);
        yield return request.SendWebRequest();

        if (request.downloadHandler.text == "회원가입 완료")
        {
            titleManager.RegisterSuccess();
        }
        else
        {
            titleManager.RegisterFail(request.downloadHandler.text);
        }
        
        request.Dispose();
    }
}
