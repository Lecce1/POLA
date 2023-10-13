using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class NoteMake : MonoBehaviour
{
    private string notes;
    public GameObject note;
    public List<float> noteTime = new();
    private string url = @"C:\Users\LeeMinGyu\Desktop\tmp.txt";
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
                Instantiate(note, new Vector3(noteTime[i] * 4, 0, 0), Quaternion.identity);
            }
        }
    }

    private void FixedUpdate()
    {
        var a = ampleParent.GetComponent<RectTransform>();
        a.anchoredPosition += Vector2.left * (10 * Time.fixedDeltaTime);
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
        int sampleSize = 1024;

        float targetTime = 60 / bpm;
        int sampleRate = am.audio.clip.frequency;
        
        float[] samples = new float[sampleSize];

        int cnt = 0;
        for (int i = 0; i < am.audio.clip.length * sampleRate; i += (int)(sampleRate * targetTime))
        {
            am.audio.clip.GetData(samples, i);
            float avg = samples.Average();
            avg += 0.5f;
            Debug.Log(avg);
            GameObject stick = Instantiate(amplitudeStick, Vector3.zero, Quaternion.identity, ampleParent.transform);

            var rt = stick.GetComponent<RectTransform>();
            var pos = rt.anchoredPosition;
            pos.x = 10 * cnt++;
            var size = rt.sizeDelta;
            size.y *= avg;
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;
            
            stick.GetComponent<RectTransform>().rect.Set(cnt * 10, 0, 10, 300 * avg);
        }
    }
}