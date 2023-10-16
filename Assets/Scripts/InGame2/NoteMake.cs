using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class NoteMake : MonoBehaviour
{
    private string notes;
    public GameObject note;
    public List<float> noteTime = new();
    private string url = @"C:\Users\kerte\OneDrive\Desktop\tmp.txt";
    public bool noteWriteMod;
    public GameObject amplitudeStick;
    public GameObject ampleParent;
    private float count = 0;
    private float bpm;
    public AudioManager am;
    
    // Start is called before the first frame update
    void Start()
    {
        noteTime.Clear();
        bpm = am.bpm;
        
        MakeAmplitude();

        if (System.IO.File.Exists(url))
        {
            string text = System.IO.File.ReadAllText(url);
            string[] lines = text.Split('\n');
            
            for (int i = 0; i < lines.Length; i++)
            {
                noteTime.Add( float.Parse(lines[i]));
            }

            for (int i = 0; i < noteTime.Count; i++)
            {
                Instantiate(note, new Vector3(noteTime[i] * 4 + 1f, 0, 0), Quaternion.identity);
            }
        }
    }

    private void FixedUpdate()
    {
        var a = ampleParent.GetComponent<RectTransform>();
        a.anchoredPosition += Vector2.left * (bpm / 1.5f * Time.fixedDeltaTime);
    }

    void Update()
    {
        if (noteWriteMod)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                notes += count + "\n";
            }
        
            if (Input.GetKeyDown(KeyCode.E))
            {
                notes = notes.TrimEnd('\n');
                System.IO.File.WriteAllText(url, notes, Encoding.Default);
            }
        }
    }
    
    public void BeatCounter()
    {
        count += 0.25f;
    }
    
    void MakeAmplitude()
    {
        AudioClip clip = am.audio.clip;
        int numSamples = clip.samples;
        int quarterBeatPerSamples = (int)(clip.frequency * (15f / bpm));
        
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
            GameObject stick = Instantiate(amplitudeStick, ampleParent.transform.position, Quaternion.identity, ampleParent.transform);
            
            var rt = stick.GetComponent<RectTransform>();
            var pos = rt.anchoredPosition;
            pos.x = cnt++ * 10;
            
            var size = rt.sizeDelta;
            size.y = y;
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;
        }
    }
}