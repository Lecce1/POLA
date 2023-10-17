using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class NoteMake : MonoBehaviour
{
    private string noteRecord;
    public GameObject noteBar;
    private List<GameObject> bars = new();
    public List<float> noteTime = new();
    private string url = "Assets/Scripts/InGame2/Beats/tmp.txt";
    public bool noteWriteMod;
    public bool noteEditMod;
    public bool insertMod;
    public int nearestNote;
    public int curNote;
    private float beatCount = 0;
    private float bpm;
    public static NoteMake instance;

    public NewPlayerController player;
    public GameObject ampStick;
    public GameObject ampParent;
    public GameObject ampBar;
    private RectTransform ampTransform;
    public AudioManager am;

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<NewPlayerController>();
        ampTransform = ampParent.GetComponent<RectTransform>();
        if (instance == null)
        {
            instance = this;
        }
        
        noteTime.Clear();
        bpm = am.bpm;
        MakeAmplitude();

        if (noteWriteMod || noteEditMod)
        {
            Destroy(player.GetComponent<Collider>());
            ampBar.SetActive(true);
        }

        if (System.IO.File.Exists(url))
        {
            string text = System.IO.File.ReadAllText(url);
            
            string[] lines = text.Split('\n');
            
            for (int i = 0; i < lines.Length; i++)
            {
                noteTime.Add(float.Parse(lines[i]));
                bars.Add(Instantiate(noteBar, new Vector3(noteTime[i] * 5, 0, 0), Quaternion.identity));
            }
        }
    }

    void FixedUpdate()
    {
        ampTransform.anchoredPosition += Vector2.left * (bpm / 1.2f * Time.fixedDeltaTime);
    }

    void Update()
    {
        if (noteWriteMod)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                noteRecord += beatCount + "\n";
            }
        }

        if (noteEditMod)
        {
            EditNote();
        }
    }

    void EditNote()
    {
        if (Input.GetKeyDown(KeyCode.A) && player.transform.position.x > 1f)
        {   
            player.transform.position -= player.transform.forward * 5f;
            am.audio.time -= 60 / bpm;
            ampTransform.anchoredPosition += Vector2.right * 50f;
            beatCount--;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            player.transform.position += player.transform.forward * 5f;
            am.audio.time += 60 / bpm;
            ampTransform.anchoredPosition += Vector2.left * 50f;
            beatCount++;
        }
            
        nearestNote = FindNearestIndex(beatCount);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!insertMod)
            {
                bars[nearestNote].transform.position = new Vector3(noteTime[nearestNote] * 5, 0, 0);
                noteTime[nearestNote] = beatCount;
            }
            else
            {
                bars.Insert(curNote, Instantiate(noteBar, new Vector3(beatCount * 5, 0, 0), Quaternion.identity));
                noteTime.Insert(curNote, beatCount);
            }
        }
        
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (curNote != -1)
            {
                var remove = bars[curNote];
                bars.RemoveAt(curNote);
                Destroy(remove);
                noteTime.RemoveAt(curNote);
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            insertMod = !insertMod;
        }
    }

    int FindNearestIndex(float findKey)
    {
        int low = 0;
        int high = noteTime.Count;

        int cnt = 0;
        
        while (low <= high && low < noteTime.Count)
        {
            int mid = (low + high) / 2;
            
            if (cnt > 100)
            {
                Debug.Log("inf loop");
                break;
            }
            
            if (noteTime[mid] > findKey)
            {
                high = mid - 1;
            }
            
            else if (noteTime[mid] == findKey)
            {
                break;
            }
            
            else
            {
                low = mid + 1;
            }

            cnt++;
        }

        if (low >= noteTime.Count)
        {
            low = noteTime.Count - 1;
        }

        curNote = low - 1;
        if (low != 0 && noteTime[low] - findKey > findKey - noteTime[low - 1])
        {
            low--;
        }
        return low;
    }
    
    public void BeatCounter()
    {
        beatCount += 0.1f;
    }
    
    void MakeAmplitude()
    {
        AudioClip clip = am.audio.clip;
        int numSamples = clip.samples;
        int quarterBeatPerSamples = (int)(clip.frequency * (12f / bpm));
        
        float[] samples = new float[quarterBeatPerSamples * clip.channels];
        
        int cnt = 0;

        for (int i = 0; i < numSamples; i += quarterBeatPerSamples)
        {
            float sum = 0;
            clip.GetData(samples, i);
            
            foreach (var t in samples)
            {
                sum += Mathf.Abs(t);
            }
            
            sum /= samples.Length;
            float y = sum * 200f;
            GameObject stick = Instantiate(ampStick, ampParent.transform.position, Quaternion.identity, ampParent.transform);
            
            var rt = stick.GetComponent<RectTransform>();
            var pos = rt.anchoredPosition;
            pos.x = cnt++ * 10;
            
            var size = rt.sizeDelta;
            size.y = y;
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;
        }
    }

    void OnApplicationQuit()
    {
        if (noteWriteMod && noteRecord != null)
        {
            noteRecord = noteRecord.TrimEnd('\n');
            System.IO.File.WriteAllText(url, noteRecord, Encoding.Default);
        }

        if (noteEditMod)
        {
            string str = "";
            foreach (var item in noteTime)
            {
                str += item + "\n";
            }
            
            str = str.TrimEnd('\n');
            System.IO.File.WriteAllText(url, str, Encoding.Default);
        }
    }
}