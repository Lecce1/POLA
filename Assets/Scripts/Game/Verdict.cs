using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Verdict : MonoBehaviour
{
    [FoldoutGroup("오브젝트")] 
    [SerializeField] 
    private GameObject verdictBar;
    [FoldoutGroup("오브젝트")] 
    [SerializeField]
    private GameObject lastPassedObject;
    
    [FoldoutGroup("변수")] 
    public bool isUp;
    [FoldoutGroup("변수")] 
    public bool isLongInteract;
    
    [FoldoutGroup("판정")] 
    [SerializeField] 
    private List<VerdictBar> verdictBarList;
    [FoldoutGroup("판정")] 
    public VerdictBar playerVerdict;
    
    void Start()
    {
        isUp = true;
        verdictBarList[2].onTriggerExitEvent += HandleGoodVerdictExit;
    }

    public static Obstacle GetObstacle(GameObject obj)
    {
        if (obj == null)
        {
            return null;
        }
        
        while (obj.transform != obj.transform.root)
        {
            Obstacle obstacle = obj.GetComponent<Obstacle>();
            
            if (obstacle != null)
            {
                return obstacle;
            }
            
            obj = obj.transform.parent.gameObject;
        }

        return null;
    }
    
    void HandleGoodVerdictExit(Collider other)
    {
        lastPassedObject = other.gameObject;
        Obstacle obstacleInfo = GetObstacle(lastPassedObject);
        
        if (obstacleInfo != null && !obstacleInfo.wasInteracted && obstacleInfo.type == NoteType.NormalNote && !isLongInteract)
        {
            if (obstacleInfo.isUp == isUp)
            {
                GetComponent<PlayerController>().Hurt(obstacleInfo, true);
            }
            else
            {
                ComboReset(obstacleInfo);
            }
        }
    }
    
    public void ComboReset(Obstacle obstacle)
    {
        obstacle.wasInteracted = true;
        DequeueUsedCollider(obstacle);
    }

    public void KeyUpOnLongInteract()
    {
        isLongInteract = false;
    }
    
    int GetVerdict(GameObject target, Obstacle targetInfo)
    {
        int idx = isUp ? 1 : 0;

        for (int i = 0; i < 4; i++)
        {
            foreach (var item in verdictBarList[i].contacts[idx])
            {
                if (targetInfo == GetObstacle(item.gameObject))
                {
                    if (verdictBarList[i].contacts[idx].Count != 0 && item.gameObject == target)
                    {
                        return i;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        return -1;
    }

    public int KeyDown(out Obstacle obstacle)
    {
        int i = isUp ? 1 : 0;
        obstacle = null;
        GameObject target;
        
        if (verdictBarList[3].contacts[i].Count != 0)
        {
            target = verdictBarList[3].contacts[i].Peek().gameObject;
        }
        else
        {
            return -1;
        }

        obstacle = GetObstacle(target);
        
        if (obstacle == null || obstacle.wasInteracted || obstacle.isUp != isUp)
        {
            return -1;
        }
        
        int evaluation = GetVerdict(target, obstacle);
        return evaluation;
    }

    public void DequeueUsedCollider(Obstacle obstacle)
    {
        int i = isUp ? 1 : 0;

        if (obstacle == null || !obstacle.wasInteracted)
        {
            return;
        }

        for (int idx = 0; idx < 4; idx++)
        {
            while (verdictBarList[idx].contacts[i].Count != 0 && obstacle == GetObstacle(verdictBarList[idx].contacts[i].Peek().gameObject))
            {
                verdictBarList[idx].contacts[i].Dequeue();
            }
        }
        
        while (playerVerdict.contacts[i].Count != 0 && obstacle == GetObstacle(playerVerdict.contacts[i].Peek().gameObject))
        {
            playerVerdict.contacts[i].Dequeue();
        }
    }
    
    public void DequeueUsedCollider(GameObject target) {
        int i = isUp ? 1 : 0;

        if (target == null)
        {
            return;
        }

        Obstacle obstacle = GetObstacle(target);
        
        if (obstacle == null)
        {
            return;
        } 
        
        for (int idx = 0; idx < 4; idx++)
        {
            if (verdictBarList[idx].contacts[i].Count != 0 && target == verdictBarList[idx].contacts[i].Peek().gameObject)
            {
                verdictBarList[idx].contacts[i].Dequeue();
            }
        }
        
        while (playerVerdict.contacts[i].Count != 0 && target == playerVerdict.contacts[i].Peek().gameObject)
        {
            playerVerdict.contacts[i].Dequeue();
        }
    }

    public bool CheckNextObject(GameObject target)
    {
        int i = isUp ? 1 : 0;

        if (verdictBarList[0].contacts[i].Count != 0 && target == verdictBarList[0].contacts[i].Peek().gameObject)
        {
            return true;
        }
        
        return false;
    }
}
