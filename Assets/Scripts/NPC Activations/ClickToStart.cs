using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickToStart : MonoBehaviour
{
    private NPCDialogue npcDialogue {get { return GetComponent<NPCDialogue>(); } }
    private void OnMouseDown()
    {
        //Debug.Log(npcDialogue.playingDialogue + " " + npcDialogue.playingResponse);
        if (!npcDialogue.playingDialogue && !npcDialogue.playingResponse)
        {
            npcDialogue.PlayDialogue();
        }
    }
}
