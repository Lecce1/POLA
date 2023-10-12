using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class LobbyPortalManager : MonoBehaviour
{
    [FoldoutGroup("정보")] 
    [Title("스테이지 위치")] 
    [SerializeField]
    private int stageNum;

    [FoldoutGroup("정보")]
    [Title("좌 우")]
    [SerializeField]
    private bool isLeft;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            if (isLeft)
            {
                LobbyPlayerController.instance.player.transform.position = DBManager.instance.lobbyRightPortal[stageNum - 1];
            }
            else
            {
                LobbyPlayerController.instance.player.transform.position = DBManager.instance.lobbyLeftPortal[stageNum];
            }
        }
    }
}
