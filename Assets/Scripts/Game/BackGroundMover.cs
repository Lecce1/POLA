using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class BackGroundMover : MonoBehaviour
{
    [FoldoutGroup("배경 Rect")]
    [SerializeField]
    RectTransform[] back = new RectTransform[2];
    [FoldoutGroup("배경 Rect")]
    [SerializeField]
    RectTransform canvasRect;

    [FoldoutGroup("변수")] 
    [SerializeField] 
    float moveSpeed;
    [FoldoutGroup("변수")] 
    [Title("현재 인덱스")]
    [SerializeField] 
    private int currentIndex = 0;
    
    public void Init(Sprite background)
    {
        gameObject.SetActive(true);
        canvasRect = GetComponent<RectTransform>();
        back[0].GetComponent<Image>().sprite = background;
        back[1].GetComponent<Image>().sprite = background;
        back[1].offsetMin = new Vector2(canvasRect.rect.width - 10, 0);
        back[1].offsetMax = new Vector2(canvasRect.rect.width, 0);
        currentIndex = 0;
    }

    void Update()
    {
        if (GameManager.instance.isCountDown && !GameManager.instance.isStart)
        {
            return;
        }
        
        for (int i = 0; i < 2; i++)
        {
            var item = back[i].anchoredPosition;
            item.x -= Time.deltaTime * moveSpeed;
            back[i].anchoredPosition = item;
        }

        var currentPos = back[currentIndex].anchoredPosition;
        if (currentPos.x <= -canvasRect.rect.width)
        {
            currentPos.x = canvasRect.rect.width - 10;
            back[currentIndex].anchoredPosition = currentPos;
            currentIndex++;
            currentIndex %= 2;
        }
    }
}
