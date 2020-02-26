using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerEnter : MonoBehaviour
{
    private CapsuleCollider2D playerCollider { get { return FindObjectOfType<CowpokeController>().GetComponent<CapsuleCollider2D>(); } }
    private NPCDialogue npcDialogue { get { return GetComponent<NPCDialogue>(); } }
    private bool fresh = true;
    public GameObject Vampire;
    private bool deadcheck;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (fresh)
        {
            if (collision == playerCollider)
            {
                npcDialogue.PlayDialogue();
                fresh = false;
            }
        }
        deadcheck = Vampire.GetComponent<GetShot>().dead;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        deadcheck = Vampire.GetComponent<GetShot>().dead;
        if (!fresh && Input.GetKeyDown(KeyCode.E) && deadcheck == false)
        {
            npcDialogue.PlayDialogue();
        }
    }
}
