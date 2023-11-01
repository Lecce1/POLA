using System;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;


[RequireComponent(typeof(NoteMake))]
public class MapCreator : MonoBehaviour
{
    [Serializable]
    private class Note
    {
        public enum noteType
        {
            Hole,
            Breakable,
            Slope,
            Slider,
            Grapple,
            Rotate
        }

        public float noteTime;
        public noteType type;
        public string attribute;
    }
    
    private string url = "Assets/Scripts/InGame2/Beats/object.csv";
    
    [FoldoutGroup("오브젝트")] 
    [SerializeField]
    private GameObject breakable;
    
    [FoldoutGroup("오브젝트")] 
    [SerializeField]
    private GameObject slider;

    [FoldoutGroup("오브젝트")] 
    [SerializeField]
    private GameObject slope;
    
    [FoldoutGroup("오브젝트")] 
    [SerializeField]
    private GameObject grapple;

    [FoldoutGroup("오브젝트")] 
    [Title("설치될 장애물을 모아둔 리스트")]
    [InfoBox("Attribute에는 float, Vector3, quaternion만 쓸 수 있음.\n" +
             "사용 양식\n" +
             "Hole: float(길이)\t\tBreakable: 없음\n" +
             "Slope: Vector3(도착점)\tSlide: float(길이) \n" +
             "Rotate: Quaternion(회전)\tgrapple: Vector3(도착점)\n\n" +
             "string형을 각 형식에 파싱가능하도록 작성")]
    [SerializeField]
    [TableList(ShowPaging = true)]
    private List<Note> notes = new ();

    [FoldoutGroup("기타")] 
    private NoteMake noteMaker;

    [FoldoutGroup("기타")] 
    private GameObject createdNoteObject;
    
    [Button("초기화", ButtonSizes.Large)]
    public void Reset()
    {
        noteMaker = GetComponent<NoteMake>();
        if (noteMaker.noteTime.Count == 0)
        {
            noteMaker.Load();
        }
        
        notes.Clear();
        
        for (int i = 0; i < noteMaker.noteTime.Count; i++)
        {
            Note n = new()
            {
                noteTime = noteMaker.noteTime[i],
                type = Note.noteType.Breakable,
                attribute = ""
            };
            notes.Add(n);
        }
    }
    
    [Button("생성", ButtonSizes.Large)]
    public void Create()
    {
        Vector3 lastPos = Vector3.zero;

        if (createdNoteObject != null)
        {
            DestroyImmediate(createdNoteObject);
        }
        createdNoteObject = new GameObject();
        createdNoteObject.name = "Created Object";
        for (int i = 0; i < notes.Count; i++)
        {
            Note n = notes[i];
            Vector3 pos = new Vector3(n.noteTime * 5f + 11.5f, 0, 0);
            GameObject obj = null;
            switch (n.type)
            {
                case Note.noteType.Breakable:
                    obj = Instantiate(breakable, pos, Quaternion.identity);
                    break;
                case Note.noteType.Slider:
                    obj = Instantiate(slider, pos, Quaternion.identity);
                    break;
                case Note.noteType.Slope:
                    obj = Instantiate(slope, pos, Quaternion.identity);
                    break;
                case Note.noteType.Grapple:
                    obj = Instantiate(grapple, pos, Quaternion.identity);
                    break;
            }

            if (obj != null)
            {
                obj.transform.parent = createdNoteObject.transform;
            }
        }
    }
    
    [Button("세이브")]
    [HorizontalGroup("Split", 0.497f)]
    public void Save()
    {
        noteMaker = GetComponent<NoteMake>();
        if (noteMaker.noteTime.Count == 0)
        {
            noteMaker.Load();
        }

        using StreamWriter sw = new StreamWriter(url);
        string str = "";
        for (int i = 0; i < noteMaker.noteTime.Count; i++)
        {
            if (i < notes.Count)
            {
                Note n = notes[i];
                str = n.noteTime + "," + n.type + ",";
                if (n.attribute.Trim() == "")
                {
                    switch (n.type)
                    {
                        case Note.noteType.Slope:
                            str += "\"(0f, 0f, 5f)\"";
                            break;
                        case Note.noteType.Rotate:
                            str += "\"(0f, 1f, 0f, 90f)\"";
                            break;
                        case Note.noteType.Hole:
                            str += "4f";
                            break;
                        case Note.noteType.Slider:
                            str += "4f";
                            break;
                    }
                }
                else
                {
                    str += "\"" + n.attribute + "\"";
                }
            }
            else
            {
                str = noteMaker.noteTime[i] + "," + Note.noteType.Breakable + "," + 4f;
            }
            sw.WriteLine(str);
        }
    }
    
    [Button("로드")]
    [HorizontalGroup("Split", 0.498f)]
    public void Load()
    {
        if (File.Exists(url))
        {
            notes.Clear();
            string[] lines = File.ReadAllLines(url);
            for (int i = 0; i < lines.Length; i++)
            {
                string[] splited = lines[i].Split(',');
                
                Note n = new Note();
                n.noteTime = float.Parse(splited[0]);
                n.type = Enum.Parse<Note.noteType>(splited[1]);
                n.attribute = "";
                for (int j = 2; j < splited.Length; j++)
                {
                    n.attribute += splited[j] + ",";
                }
                n.attribute = n.attribute.TrimEnd(',').Trim('"');
                notes.Add(n);
            }
        }
        else
        {
            Debug.LogError("불러올 파일이 존재하지 않음.");
        }
    }
}