using System.Collections.Generic;
using System.Text;
using Sirenix.OdinInspector;
using UnityEngine;

public class NoteMake : MonoBehaviour
{
    [FoldoutGroup("모드")]
    public bool noteWriteMod = false;
    
    [FoldoutGroup("모드")]
    public bool noteEditMod = false;
    
    [FoldoutGroup("모드")]
    public bool insertMod = false;

    [FoldoutGroup("모드")] 
    public bool noSave = false;

    [FoldoutGroup("모드")] 
    private Stack<Task> taskStack = new Stack<Task>();
    
    [FoldoutGroup("노트")]
    [Title("오브젝트")]
    public GameObject noteBar;
    
    [FoldoutGroup("노트")]
    [SerializeField]
    private List<GameObject> bars = new();
    
    [FoldoutGroup("노트")]
    [Title("위치")]
    public int nearestNote;
    
    [FoldoutGroup("노트")]
    [Title("위치")]
    public int curNote;
    
    [FoldoutGroup("노트")]
    [Title("기록")]
    [SerializeField]
    private string noteRecord;
    
    [FoldoutGroup("노트")]
    public List<float> noteTime = new();
    [FoldoutGroup("음악")]
    [Title("경로")]
    [SerializeField]
    private string url = "Assets/Scripts/InGame2/Beats/tmp.txt";
    
    [FoldoutGroup("음악")]
    [Title("BPM")]
    [SerializeField]
    private float bpm;
    
    [FoldoutGroup("음악")]
    [Title("오디오")]
    public AudioManager am;
    
    [FoldoutGroup("음악")]
    [Title("카운트")]
    [SerializeField]
    public float beatCount = 0;
    
    [FoldoutGroup("일반")]
    [Title("플레이어")]
    public NewPlayerController player;
    
    [FoldoutGroup("앰플")]
    [Title("오브젝트")]
    public GameObject ampStick;
    
    [FoldoutGroup("앰플")]
    public GameObject ampParent;
    
    [FoldoutGroup("앰플")]
    public GameObject ampBar;
    
    [FoldoutGroup("앰플")]
    [Title("위치")]
    [SerializeField]
    private RectTransform ampTransform;
    
    /// <summary>
    /// 노트 작업 시 작업 기록을 담기 위한 클래스
    /// </summary>
    public class Task
    {
        public enum task
        {
            Deleted = 0,
            Inserted,
            Moved,
        }

        public Task(int idx, float prev, task t)
        {
            index = idx;
            preValue = prev;
            taskType = t;
        }

        public float preValue;
        
        public int index;

        public task taskType;
    }

    public static NoteMake instance;
    
    /// <summary>
    /// 초깃값 지정
    /// 저장된 노트가 있으면 불러옴
    /// </summary>
    void Start()
    {
        bpm = am.bpm;
        player = GameObject.Find("Player").GetComponent<NewPlayerController>();
        ampTransform = ampParent.GetComponent<RectTransform>();
        noteTime.Clear();
        
        if (noteWriteMod || noteEditMod)
        {
            MakeAmplitude();
            player.GetComponent<BoxCollider>().isTrigger = true;
            player.GetComponent<Rigidbody>().useGravity = false;
            ampBar.SetActive(true);
        }

        if (System.IO.File.Exists(url))
        {
            string text = System.IO.File.ReadAllText(url);
            string[] lines = text.Split('\n');
            
            for (int i = 0; i < lines.Length; i++)
            {
                noteTime.Add(float.Parse(lines[i]));
                var note = Instantiate(noteBar, new Vector3(noteTime[i] * 5f + 11.5f, 0, 0), Quaternion.identity);
                bars.Add(note);
                note.transform.GetChild(0).GetComponent<TextMesh>().text = i.ToString();
            }
        }
        
        if (instance == null)
        {
            instance = this;
        }
    }

    /// <summary>
    /// UI로 보여지는 음파의 이동
    /// </summary>
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

    [FoldoutGroup("노트")]
    [Button("불러오기", ButtonSizes.Large)]
    public void Load()
    {
        if (System.IO.File.Exists(url))
        {
            noteTime.Clear();
            string text = System.IO.File.ReadAllText(url);
            string[] lines = text.Split('\n');
            
            for (int i = 0; i < lines.Length; i++)
            {
                noteTime.Add(float.Parse(lines[i]));
            }
        }
    }
    
    /// <summary>
    /// A, D키로 이동 할 때 생기는 오차를 제거하기 위한 메서드
    /// </summary>
    /// <param name="d">오차값</param>
    void CalculateError(float d)
    {
        var pos = player.transform.position;
        pos.x += d;
        player.transform.position = pos;
    }
    
    /// <summary>
    /// NoteEditMod에서 사용하는 메서드
    /// 각종 기능들(A, D, X, 스페이스바, TAB, ESC, Shift + Z키)을 구현해놓음.
    /// 각각 되감기, 빨리감기, 지우기, 삽입/수정, 모드 변경, 저장토글, 작업 취소
    /// </summary>
    void EditNote()
    {
        if (Input.GetKeyDown(KeyCode.A) && beatCount > 2f)
        {
            CalculateError(0.5f);
            player.transform.position -= player.transform.forward * 10f;
            am.audio.time -= 120 / bpm;
            ampTransform.anchoredPosition += Vector2.right * 100f;
            beatCount -= 2;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            CalculateError(0.5f);
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
                taskStack.Push(new Task(nearestNote, noteTime[nearestNote], Task.task.Moved));
                bars[nearestNote].transform.position = new Vector3(noteTime[nearestNote] * 5f + 12F, 0, 0);
                noteTime[nearestNote] = beatCount;
            }
            else
            {
                taskStack.Push(new Task(curNote, 0, Task.task.Inserted));
                bars.Insert(curNote, Instantiate(noteBar, new Vector3(beatCount * 5f + 12f, 0, 0), Quaternion.identity));
                noteTime.Insert(curNote, beatCount);
            }
        }
        
        if (Input.GetKeyDown(KeyCode.X) && curNote != -1)
        {
            taskStack.Push(new Task(curNote, noteTime[nearestNote], Task.task.Deleted));
            var remove = bars[curNote];
            bars.RemoveAt(curNote);
            Destroy(remove);
            noteTime.RemoveAt(curNote);
        }
        
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            insertMod = !insertMod;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            noSave = !noSave;
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Z))
        {
            UndoTask();
        }
    }

    /// <summary>
    /// Shift+Z키를 눌렀을 때 실행취소를 함.
    /// Task 클래스와 스택을 기반으로 가장 최근에 작업한 내용을 실행취소.
    /// </summary>
    void UndoTask()
    {
        if (taskStack.Count <= 0)
        {
            return;
        }

        Task t = taskStack.Pop();

        switch (t.taskType)
        {
            case Task.task.Moved :
                bars[t.index].transform.position = new Vector3(t.preValue * 5f + 12f, 0, 0);
                noteTime[t.index] = t.preValue;
                break;
            
            case Task.task.Deleted :
                bars.Insert(t.index, Instantiate(noteBar, new Vector3(t.preValue * 5f + 12f, 0, 0), Quaternion.identity));
                noteTime.Insert(t.index, t.preValue);
                break;
            
            case Task.task.Inserted :
                var remove = bars[t.index];
                bars.RemoveAt(t.index);
                Destroy(remove);
                noteTime.RemoveAt(t.index);
                break;
        }
    }

    /// <summary>
    /// 내 주변에 가장 가까운 노트의 인덱스가 무엇인지,
    /// 가장 최근에 지나친 노트가 무엇인지 찾음.
    /// </summary>
    /// <param name="findKey">찾고 싶은 값</param>
    /// <returns>findKey와 가장 가까운 값</returns>
    int FindNearestIndex(float findKey)
    {
        int low = 0;
        int high = noteTime.Count;
        
        while (low <= high && low < noteTime.Count)
        {
            int mid = (low + high) / 2;
            
            if (noteTime[mid] > findKey)
            {
                high = mid - 1;
            }
            else if (noteTime[mid] == findKey)
            {
                low = mid;
                break;
            }
            else
            {
                low = mid + 1;
            }
        }

        curNote = low - 1;
        
        if (low >= noteTime.Count)
        {
            low = noteTime.Count - 1;
        }
        
        if (low != 0 && noteTime[low] - findKey > findKey - noteTime[low - 1])
        {
            low--;
        }
        return low;
    }
    
    /// <summary>
    /// 0.1박자마다 한번 박자를 셈.
    /// </summary>
    public void BeatCounter()
    {
        beatCount *= 10;
        beatCount++;
        beatCount = Mathf.Round(beatCount);
        beatCount *= 0.1f;
    }
    
    /// <summary>
    /// UI로 노래의 파형이 어떻게 생겼는지 보여줌.
    /// </summary>
    void MakeAmplitude()
    {
        AudioClip clip = am.audio.clip;
        int numSamples = clip.samples;
        int BeatPerSamples = (int)(clip.frequency * (12f / bpm));
        float[] samples = new float[BeatPerSamples * clip.channels];
        int cnt = 0;

        for (int i = 0; i < numSamples; i += BeatPerSamples)
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

    /// <summary>
    /// 게임이 종료될 때 현재까지 기록해둔 박자를 저장하는 기능
    /// </summary>
    void OnApplicationQuit()
    {
        if (noSave)
        {
            return;
        }
        
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