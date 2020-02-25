using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTriggerExample : MonoBehaviour
{
    private SpriteRenderer spriteRenderer {get { return GetComponent<SpriteRenderer>(); } }
    public NPCDialogue npcdialogue;

    private Dictionary<string, bool> exampleSetDict {get { return FindObjectOfType<PlayerManager>().playerFlags; } }

    public void MakeSpriteRed()
    {
        spriteRenderer.color = Color.red;
        npcdialogue.PlayDialogue();

    }

    public void MakeSpriteGreen()
    {
        spriteRenderer.color = Color.green;
    }

    private void OnMouseDown()
    {
        exampleSetDict["ExampleFlag"] = true;
    }
}
