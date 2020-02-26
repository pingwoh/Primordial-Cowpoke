using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarricadeTrigger : MonoBehaviour
{
    private CapsuleCollider2D playerCollider { get { return FindObjectOfType<CowpokeController>().GetComponent<CapsuleCollider2D>(); } }
    private NPCDialogue npcDialogue { get { return GetComponent<NPCDialogue>(); } }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (collision == playerCollider)
            {
                npcDialogue.PlayDialogue();
            }
        }
    }
}
