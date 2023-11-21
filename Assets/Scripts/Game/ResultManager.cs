using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    public List<Image> starImage;
    public Sprite starOn;
    public Sprite starOff;
    
    public void Check1()
    {
        if (DBManager.instance.stageArray[DBManager.instance.currentChapter].stage[DBManager.instance.currentStage - 1]
                .starCount >= 1)
        {
            starImage[0].sprite = starOn;
        }
        else
        {
            starImage[0].sprite = starOff;
        }
    }
    
    public void Check2()
    {
        if (DBManager.instance.stageArray[DBManager.instance.currentChapter].stage[DBManager.instance.currentStage - 1]
                .starCount >= 2)
        {
            starImage[1].sprite = starOn;
        }
        else
        {
            starImage[1].sprite = starOff;
        }
    }
    
    public void Check3()
    {
        if (DBManager.instance.stageArray[DBManager.instance.currentChapter].stage[DBManager.instance.currentStage - 1]
                .starCount >= 3)
        {
            starImage[2].sprite = starOn;
        }
        else
        {
            starImage[2].sprite = starOff;
        }
    }
    
    public void Check4()
    {
        if (DBManager.instance.stageArray[DBManager.instance.currentChapter].stage[DBManager.instance.currentStage - 1]
                .starCount >= 4)
        {
            starImage[3].sprite = starOn;
        }
        else
        {
            starImage[3].sprite = starOff;
        }
    }
}
