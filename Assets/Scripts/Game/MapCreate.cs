using System;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


[RequireComponent(typeof(NoteMake))]
public class MapCreator : MonoBehaviour
{
    [Serializable]
    private class Note
    {
        public NoteType type;
        public bool isUp;
        public float noteTime;
        public float length;
        public int objectType;
    }
    
    private string url = "Assets/Scripts/Game/Beats/object.csv";
    
    [FoldoutGroup("오브젝트")] 
    [SerializeField]
    private GameObject[] normalNotes;

    [FoldoutGroup("오브젝트")] 
    [Title("롱노트 오프셋")]
    [SerializeField]
    private Vector3[] longNoteOffset;
    [FoldoutGroup("오브젝트")] 
    [SerializeField]
    private GameObject[] moveNotes;
    [FoldoutGroup("오브젝트")] 
    [SerializeField]
    private GameObject[] Walls;
    [FoldoutGroup("오브젝트")] 
    [SerializeField]
    private GameObject heart;
    [FormerlySerializedAs("indexPlain")]
    [FoldoutGroup("오브젝트")] 
    [SerializeField]
    private GameObject indexObject;
    [FoldoutGroup("오브젝트")] 
    [Title("설치될 장애물을 모아둔 리스트")]
    [SerializeField]
    [TableList(ShowPaging = true, ShowIndexLabels = true)]
    private List<Note> notes = new ();

    [FoldoutGroup("기타")] 
    private NoteMake noteMaker;
    [FoldoutGroup("기타")] 
    [SerializeField]
    private LayerMask ground;
    [FormerlySerializedAs("offset")] 
    [FoldoutGroup("기타")] 
    public float defaultOffset;
    [FoldoutGroup("기타")] 
    [Title("토글 온오프")]
    private bool toggleIndex;
    
    
    [Button("생성", ButtonSizes.Large)]
    [HorizontalGroup("Split1", 0.895f)]
    public void Create()
    {
        GameObject createdNoteObject = GameObject.Find("Created Object");
        
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
            Vector3 pos;
            RaycastHit hit = new RaycastHit();
            
            if (!n.isUp)
            {
                pos = new Vector3(n.noteTime * 8f + defaultOffset, -5f, 0);
                Physics.Raycast(new Ray(pos, Vector3.up), out hit, 10, ground);
                pos = hit.point;
                pos.y -= 1;
                q = Quaternion.AngleAxis(180, progressDirection);
            }
            else
            {
                pos = new Vector3(n.noteTime * 8f + defaultOffset, 5f, 0);
                Physics.Raycast(new Ray(pos, Vector3.down), out hit, 10, ground);
                pos = hit.point;
                pos.y += 1;
            }

            GameObject obj = null;
            Obstacle obstacle = null;
            
            switch (n.type)
            {
                case NoteType.NormalNote:
                    int rand = Random.Range(0, normalNotes.Length);
                    obj = n.length == 0 ? Instantiate(normalNotes[rand]) : new GameObject();
                    obj.transform.position = pos;

                    if (!n.isUp)
                    {
                        obj.transform.rotation *= q;
                    }

                    obstacle = obj.AddComponent<Obstacle>();
                    obstacle.type = NoteType.NormalNote;
                    obstacle.canDestroy = true;
                    
                    if (n.length == 0)
                    {
                        if (n.isUp)
                        {
                            obj.transform.rotation *= Quaternion.Euler(0, 0, 45);
                        }
                        else
                        {
                            obj.transform.rotation *= Quaternion.Euler(0, 0, -45);
                        }
                        obstacle.perfectScore = 100;
                        obstacle.greatScore = 50;
                    }
                    else
                    {
                        obstacle.perfectScore = 200;
                        obstacle.greatScore = 100;

                        for (int j = 0; j < n.length * 8; j++)
                        {
                            Vector3 forwardVector = q * Vector3.right;
                            GameObject inObj;
                            if (!n.isUp)
                            {
                                var reversedOffset = longNoteOffset[j % normalNotes.Length];
                                reversedOffset.y *= -1;
                                inObj = Instantiate(normalNotes[j % normalNotes.Length], pos + forwardVector * j + reversedOffset, q);
                            }
                            else
                            {
                                inObj = Instantiate(normalNotes[j % normalNotes.Length], pos + forwardVector * j + longNoteOffset[j % normalNotes.Length], q);
                            }
                            inObj.transform.parent = obj.transform;
                            inObj.transform.rotation *= Quaternion.Euler(-180, -90, -90);
                        }
                    }
                    
                    break;
                
                case NoteType.MoveNote:
                    obj = Instantiate(moveNotes[n.objectType], pos, q);
                    obstacle = obj.AddComponent<Obstacle>();
                    obstacle.type = NoteType.MoveNote;
                    obstacle.canDestroy = true;
                    obstacle.perfectScore = 150;
                    obstacle.greatScore = 100;
                    break;
                
                case NoteType.Wall:
                    obj = n.length == 0 ? Instantiate(Walls[n.objectType], pos, q) : new GameObject();
                    obstacle = obj.AddComponent<Obstacle>();
                    obstacle.canDestroy = false;
                    obstacle.type = NoteType.Wall;
                    
                    if (n.length == 0)
                    {
                        obstacle.canDestroy = false;
                        obstacle.perfectScore = 100;
                    }
                    else
                    {
                        obj.name = "LongWall";
                        obstacle.perfectScore = 200;
                        
                        for (int j = 0; j < n.length * 8; j++)
                        {
                            Vector3 forwardVector = q * progressDirection;
                            GameObject inObj = Instantiate(Walls[n.objectType], pos + forwardVector * j, q);
                            inObj.transform.parent = obj.transform;
                        }
                    }
                    
                    break;
                
                case NoteType.Heart:
                    obj = Instantiate(heart, pos, q);
                    break;
            }

            if (obj != null && obstacle != null)
            {
                obj.name += " " + i;
                obj.transform.parent = createdNoteObject.transform;
                obstacle.beatLength = n.length;
                obstacle.isUp = n.isUp;
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
                type = NoteType.NormalNote,
                isUp = false,
                length = 0f,
                objectType = 0
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
                str = n.noteTime + "," + n.type + "," + n.isUp + "," + n.length + "," + n.objectType;
            }
            else
            {
                str = noteMaker.noteTime[i] + "," + NoteType.NormalNote + ",False," + 0 + "," + 0;
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
                    type = Enum.Parse<NoteType>(split[1]),
                    isUp = bool.Parse(split[2]),
                    length = float.Parse(split[3]),
                    objectType = int.Parse(split[4])
                };
                notes.Add(n);
            }
        }
        else
        {
            Debug.LogError("불러올 파일이 존재하지 않음.");
        }
    }
    
    [Button("인덱싱")]
    public void Indexing()
    {
        GameObject createdObject = GameObject.Find("Created Object");
        GameObject indexGroup = GameObject.Find("Index Group");
        
        toggleIndex = !toggleIndex;
        
        if (indexGroup != null)
        {
            DestroyImmediate(indexGroup);
            return;
        }

        indexGroup = new GameObject { name = "Index Group" };
        
        if (toggleIndex)
        {
            int i = 1;
            
            foreach (Transform item in createdObject.transform)
            {
                Vector3 pos = item.position;
                var index = Instantiate(indexObject, pos, Quaternion.identity);
                index.GetComponent<TextMesh>().text = i++.ToString();
                index.transform.parent = indexGroup.transform;
            }
        }
        
    }
}