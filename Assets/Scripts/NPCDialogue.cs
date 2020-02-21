using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCDialogue : MonoBehaviour
{
    //have text portion of dialogue script finished.
    //need to finish audio and animation portions.
    public Dialogue dialogue;

    //should probably move dialogueManager interaction to another script attached to npc.
    private DialogueManager dialogueManager { get { return FindObjectOfType<DialogueManager>(); } }
    private PlayerManager playerManager { get { return FindObjectOfType<PlayerManager>(); } }
    private Dictionary<string, bool> playerTriggers { get{ return playerManager.playerTriggers; } }

    private bool playingDialogue = false;
    private bool playingResponse = false;
    private int conversationNumber = 0;


    private BoxCollider2D collider {get { return GetComponent<BoxCollider2D>(); } }
    void Start()
    {
        dialogue.character = gameObject;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            PlayDialogue();
        }
    }

    private void OnMouseDown()
    {
        PlayDialogue();
    }

    public void PlayDialogue()
    {
        if (playingResponse)
        {
            playingResponse = dialogueManager.EndResponse();

            if (playingResponse == false)
            {
                if (dialogue.triggerNextConversation[conversationNumber])
                {
                    conversationNumber++;
                    playingResponse = false;
                    //PlayDialogue();
                }
                else
                {
                    dialogueManager.EndDialogue();
                    playingResponse = false;
                }
            }
        }

        if (playingDialogue)
        {
            playingDialogue = dialogueManager.ContinueDialogue();
            if(playingDialogue == false)
            {
                playerManager.playerTriggers[dialogue.triggerActivations[conversationNumber]] = true;

                if (dialogue.responseToggles[conversationNumber])
                {
                    string[] responses = dialogue.responseList[conversationNumber].responses.ToArray();
                    string[] triggers = dialogue.responseActivationsList[conversationNumber].activations.ToArray();
                    dialogueManager.StartResponse(responses, triggers, this);
                    playingResponse = true;
                }
                else
                {
                    dialogueManager.EndDialogue();
                }

                //conversationNumber++;
            }
        }
        else if(playingResponse == false)
        {
            bool hasNextConverstaion = true;
            int startingConversationNumber = conversationNumber;
            while (playerTriggers[dialogue.triggerSelections[conversationNumber]] == false)
            {
                conversationNumber++;
                if(conversationNumber > dialogue.triggerSelections.Count - 1)
                {
                    hasNextConverstaion = false;
                    break;  
                }
            }
            if (hasNextConverstaion)
            {
                playingDialogue = true;
                string[] newSentences = dialogue.conversationList[conversationNumber].sentences.ToArray();
                dialogueManager.StartDialogue(dialogue.characterName, newSentences);
            }
            else
            {
                dialogueManager.EndDialogue();
                conversationNumber = startingConversationNumber;
            }


        }

    }

}
