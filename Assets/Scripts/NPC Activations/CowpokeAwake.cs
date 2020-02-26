using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowpokeAwake : MonoBehaviour
{
    public NPCDialogue npcDialogue;
    private bool playOnce = true;

    void Update()
    {
        //add all the other buttons the player can press to get up here as ||.
        if (playOnce && Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            playOnce = false;
            StartCoroutine(WaitToDisplay());
        }
    }
    IEnumerator WaitToDisplay()
    {
        yield return new WaitForSeconds(1.5f);
        npcDialogue.PlayDialogue();
        Destroy(this);
    }
}
