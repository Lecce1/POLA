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
        public enum NoteType
        {
            Unbreakable,
            Breakable,
            Slope,
            Slider
        }

        public bool isUp;
        public float noteTime;
        public NoteType type;
        public float attribute;
    }
    
    private string url = "Assets/Scripts/InGame2/Beats/object.csv";
    
    [FoldoutGroup("오브젝트")] 
    [SerializeField]
    private GameObject unbreakable;
    
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
    [Title("설치될 장애물을 모아둔 리스트")]
    [SerializeField]
    [TableList(ShowPaging = true)]
    private List<Note> notes = new ();

    [FoldoutGroup("기타")] 
    private NoteMake noteMaker;

    [FoldoutGroup("기타")] 
    private GameObject createdNoteObject;
    
    [FoldoutGroup("기타")] 
    [SerializeField]
    private LayerMask ground;
    
    [Button("생성", ButtonSizes.Large)]
    [HorizontalGroup("Split1", 0.895f)]
    public void Create()
    {
        if (createdNoteObject != null)
        {
            DestroyImmediate(createdNoteObject);
        }
        createdNoteObject = new GameObject();
        createdNoteObject.name = "Created Object";
        Vector3 progressDirection = Vector3.right;
        for (int i = 0; i < notes.Count; i++)
        {
            Note n = notes[i];
            Quaternion q = Quaternion.AngleAxis(0, progressDirection);
            Vector3 pos = new Vector3(n.noteTime * 4f + 13.5f, 0.5f, 0);
            
            if (n.isUp)
            {
                Ray ray = new Ray(pos, Vector3.up);
                
                if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, ground))
                {
                    pos = hitInfo.point;
                    q = Quaternion.AngleAxis(180, progressDirection);
                }
            }
            
            GameObject obj = null;
            switch (n.type)
            {
                case Note.NoteType.Unbreakable:
                    obj = Instantiate(unbreakable, pos, q);
                    break;
                case Note.NoteType.Breakable:
                    obj = Instantiate(breakable, pos, q);
                    break;
                case Note.NoteType.Slider:
                    obj = Instantiate(slider, pos, q);
                    break;
                case Note.NoteType.Slope:
                    obj = Instantiate(slope, pos, q);
                    break;
            }

            if (obj != null)
            {
                obj.transform.parent = createdNoteObject.transform;
            }
        }
    }
    
    [Button("초기화", ButtonSizes.Large)]
    [HorizontalGroup("Split1", 0.1f)]
    public void Reset()
    {
        noteMaker = GetComponent<NoteMake>();
        if (noteMaker.noteTime.Count == 0)
        {
            noteMaker.NoteLoad();
        }
        
        notes.Clear();
        
        for (int i = 0; i < noteMaker.noteTime.Count; i++)
        {
            Note n = new()
            {
                noteTime = noteMaker.noteTime[i],
                type = Note.NoteType.Breakable,
                isUp = false,
                attribute = 0f
            };
            notes.Add(n);
        }
    }
    
    [Button("세이브")]
    [HorizontalGroup("Split2")]
    public void Save()
    {
        noteMaker = GetComponent<NoteMake>();
        if (noteMaker.noteTime.Count == 0)
        {
            noteMaker.NoteLoad();
        }

        using StreamWriter sw = new StreamWriter(url);
        string str;
        for (int i = 0; i < noteMaker.noteTime.Count; i++)
        {
            if (i < notes.Count)
            {
                Note n = notes[i];
                str = n.noteTime + "," + n.type + "," + n.isUp + "," + n.attribute;
            }
            else
            {
                str = noteMaker.noteTime[i] + "," + Note.NoteType.Breakable + ",False,";
            }
            sw.WriteLine(str);
        }
    }
    
    [Button("로드")]
    [HorizontalGroup("Split2")]
    public void Load()
    {
        if (File.Exists(url))
        {
            notes.Clear();
            string[] lines = File.ReadAllLines(url);
            foreach (var t in lines)
            {
                var split = t.Split(',');
                
                Note n = new Note
                {
                    noteTime = float.Parse(split[0]),
                    type = Enum.Parse<Note.NoteType>(split[1]),
                    isUp = bool.Parse(split[2]),
                    attribute = 0f
                };
                
                n.attribute = split[3] == "" ? 0f : float.Parse(split[3]);
                
                notes.Add(n);
            }
        }
        else
        {
            Debug.LogError("불러올 파일이 존재하지 않음.");
        }
    }
}