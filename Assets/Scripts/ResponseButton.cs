using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponseButton : MonoBehaviour
{
    public int responseNumber;

    public string playerFlag;

    private PlayerManager playerManager { get { return FindObjectOfType<PlayerManager>(); } }

    public NPCDialogue npcDialogue;

    public void SelectResponse()
    {
        playerManager.playerFlags[playerFlag] = true;
        npcDialogue.responseNumber = responseNumber;
        npcDialogue.PlayDialogue();
    }
}
