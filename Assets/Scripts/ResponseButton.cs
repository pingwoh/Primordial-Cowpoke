using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponseButton : MonoBehaviour
{
    public string playerTrigger;

    private PlayerManager playerManager { get { return FindObjectOfType<PlayerManager>(); } }

    public NPCDialogue npcDialogue;

    public void SelectResponse()
    {
        playerManager.playerTriggers[playerTrigger] = true;
        npcDialogue.PlayDialogue();
    }
}
