using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class HttpServerManager : MonoBehaviour
{
    [FoldoutGroup("정보")] 
    [Title("주소")] 
    private string url = "http://15.164.215.122/Capstone/";
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

        using (UnityWebRequest request = UnityWebRequest.Post(url + "Login.php", form))
        {
            yield return request.SendWebRequest();
            
            if (request.downloadHandler.text.Split('|')[0] == "로그인 완료")
            {
                titleManager.LoginSuccess();
                DBManager.instance.nickName = request.downloadHandler.text.Split('|')[1];
            }
            else
            {
                titleManager.LoginFail();
            }
            
            request.Dispose();
        }
    }
    
    public IEnumerator SignUp(string id, string pw, string nick)
    {
        WWWForm form = new WWWForm();
        form.AddField("ID", id);
        form.AddField("PW", pw);
        form.AddField("NICK", nick);
        
        using (UnityWebRequest request = UnityWebRequest.Post(url + "SignUp.php", form))
        {
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
    
    public IEnumerator DeleteAccount()
    {
        WWWForm form = new WWWForm();
        form.AddField("NICK", DBManager.instance.nickName);
        
        using (UnityWebRequest request = UnityWebRequest.Post(url + "Delete_Account.php", form))
        {
            yield return request.SendWebRequest();
            
            Debug.Log(request.downloadHandler.text);
            PlayerPrefs.DeleteAll();
            SceneManager.LoadScene("Title");
            
            request.Dispose();
        }
    }
}
