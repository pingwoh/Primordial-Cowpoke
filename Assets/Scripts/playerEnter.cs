using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerEnter : MonoBehaviour
{
    private CapsuleCollider2D playerCollider { get{ return FindObjectOfType<CowpokeController>().GetComponent<CapsuleCollider2D>(); } }
    private NPCDialogue npcDialogue { get{ return GetComponent<NPCDialogue>(); } }
    private bool fresh = true;

    void Start()
    {
        
    }

    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (fresh)
        {
            if(collision == playerCollider)
            {
                npcDialogue.PlayDialogue();
                fresh = false;
            }
        }
    }
}
