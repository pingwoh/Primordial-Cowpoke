using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleDialogueTrigger : MonoBehaviour
{
    private NPCDialogue npcdialogue {get { return GetComponent<NPCDialogue>(); } }


    void Start()
    {
    }

    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        npcdialogue.PlayDialogue();
    }
}
