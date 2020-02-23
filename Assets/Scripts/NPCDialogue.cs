using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCDialogue : MonoBehaviour
{
    public Dialogue dialogue;
    
    //animator attached to the NPC
    private Animator NPCAnimator { get { return GetComponent<Animator>(); } }
    //list of animations to play during dialogue
    private Queue<string> currentAnimations = new Queue<string>();

    //dialogueManager in the scene
    private DialogueManager dialogueManager { get { return FindObjectOfType<DialogueManager>(); } }
    //playerManager in the scene
    private PlayerManager playerManager { get { return FindObjectOfType<PlayerManager>(); } }
    //audioManager in the scene
    private AudioManager audioManager { get { return FindObjectOfType<AudioManager>(); } }
    //player's dictionary for their triggers
    private Dictionary<string, bool> playerTriggers { get{ return playerManager.playerTriggers; } }

    private bool playingDialogue = false;
    private bool playingResponse = false;
    private int conversationNumber = 0;

    void Start()
    {
    }

    void Update()
    {
        //plays dialogue when N key is pressed down
        //probably need to change so that the N key can only be pressed when dialogue as actually begun
        if (Input.GetKeyDown(KeyCode.N))
        {
            PlayDialogue();
        }
    }

    private void OnMouseDown()
    {
        //starts dialogue when the player clicks on the npc. Currently has no difference from pressing N but should in the future
        //want to set up so that you can only click on and NPC when not currently in dialogue.
        PlayDialogue();
    }

    public void PlayDialogue()
    {
        //might rewrite this PlayDialogue section. It all works its just a little confusing and probably hard to change in the future

        //starts by checking if a response is currently playing. If not, checks if dialogue is currently playing.
        //If neither is playing, 

        //only runs if the NPC is currently displaying the response screen of dialogue
        if (playingResponse)
        {
            //returns false once the dialogue manager runs through its EndResponse() method.
            playingResponse = dialogueManager.EndResponse();
            //should always return false but here just in case.
            if (playingResponse == false)
            {
                //runs if the dialogue script says the current conversation should trigger the next conversation
                if (dialogue.conversations[conversationNumber].triggerNextConversation)
                {
                    //moves to the next conversation
                    conversationNumber++;
                    //no longer displaying the response screen
                    playingResponse = false;
                }
                //runs if dialogue script says current conversation shouldn't trigger next conversation
                else
                {
                    //Ends the dialogue because response is over and no new dialogue is being triggered.
                    dialogueManager.EndDialogue();
                    //no longer displaying response screen
                    playingResponse = false;
                    //stops the loop
                    return;
                }
            }
        }
        //only runs if the NPC is currently displaying dialogue on screen.
        if (playingDialogue)
        {
            //plays the next line of dialogue.
            //returns true if there is another senetence to display, returns false if there are no more sentences to display
            playingDialogue = dialogueManager.ContinueDialogue();
            //if there aren't any more sentences to display
            if (playingDialogue == false)
            {
                //sets the trigger attached to the player to true if dialogue script says that trigger should be set once dialogue is finished
                playerTriggers[dialogue.conversations[conversationNumber].setTrigger] = true;
                //runs if the NPC is set to have a response to the sentences that were displayed
                if (dialogue.conversations[conversationNumber].hasResponse)
                {
                    //an array of the responses on the dialogue script
                    string[] responses = new string[dialogue.conversations[conversationNumber].responses.Count];
                    //an array of the triggers to set for each response that are attached to the dialogue script
                    string[] triggers = new string[dialogue.conversations[conversationNumber].responses.Count];
                    int counter = 0;
                    //gives those arrays their values.
                    foreach (Response response in dialogue.conversations[conversationNumber].responses)
                    {
                        responses[counter] = response.responseText;
                        triggers[counter] = response.setTrigger;
                        counter++;
                    }
                    //stops any dialogue audio playing on the audiomanager because dialogue has finished or been skipped.
                    audioManager.StopDialogueAudio();
                    //tells the dialogue manager to startd displaying the player's responses. Takes the responses, triggers, and this script
                    dialogueManager.StartResponse(responses, triggers, this);
                    //not playing dialogue anymore because responses are playing
                    playingDialogue = false;
                    playingResponse = true;
                }
                //runs if the NPC is not set to have any responses.
                else
                {
                    //stops audio because dialogue is over and tells dialogue manager to close dialogue window because there is nothing else to display
                    audioManager.StopDialogueAudio();
                    dialogueManager.EndDialogue();
                }
            }
            //runs if there are more sentences to display
            else
            {
                //stops the previous sound playing through the audio manager
                audioManager.StopDialogueAudio();
                //plays the current sound sentence's
                audioManager.PlayDialogueAudio();
                //plays the NPC's animation associated with the sentence being displayed.
                NPCAnimator.Play(currentAnimations.Dequeue());
            }
        }
        //only runs if the NPC doesn't have any dialogue or responses on the screen.
        else if(playingResponse == false)
        {
            //tracks if the NPC can play another conversation based on their current conversation number and the player's current triggers
            bool hasNextConverstaion = true;
            //gets current conversation number in case there aren't any conversations that work with current triggers.
            int startingConversationNumber = conversationNumber;
            //iterates through conversations attached to the dialogue script until one matches the triggers attached to the player
            while (playerTriggers[dialogue.conversations[conversationNumber].requiredTrigger] == false)
            {
                conversationNumber++;
                //detects if there aren't any more conversations and none of them have matched with the player's triggers
                if(conversationNumber > dialogue.conversations.Count - 1)
                {
                    //no next conversation because nothing matched
                    hasNextConverstaion = false;
                    break;  
                }
            }
            //runs if there was a match between conversation and trigger
            if (hasNextConverstaion)
            {
                //start of playing dialogue
                playingDialogue = true;
                //string array for all the sentences in the current conversation
                string[] newSentences = new string[dialogue.conversations[conversationNumber].statements.Count];
                int counter = 0;
                //gives newSentence array its values. Also sends the sound clips for each sentence to the audio manager and creates a list of the NPC's animations to play
                foreach(Statement statement in dialogue.conversations[conversationNumber].statements)
                {
                    //gets all animations
                    currentAnimations.Enqueue(NPCAnimator.runtimeAnimatorController.animationClips[statement.animationSelection].name);
                    //gets all sounds
                    audioManager.dialogueSounds.Enqueue(statement.audioProperties);
                    newSentences[counter] = statement.statementText;
                    counter++;
                }
                //starts displaying dialogue through the dialoguemanager. Takes the NPC's name and its current sentences
                dialogueManager.StartDialogue(dialogue.characterName, newSentences);
                //starts playing sound through the audio manager. Only plays one sound at a time.
                audioManager.PlayDialogueAudio();
                //plays the animation associated with the first sentence of dialogue.
                NPCAnimator.Play(currentAnimations.Dequeue());

            }
            //if there aren't any conversations that match with triggers
            else
            {
                //stops any dialogue audio that's playing. This is probably redundant but I have it here for safety.
                audioManager.StopDialogueAudio();
                //Ends dialogue because there weren't any matches for what to display next, so there isn't anything to dispaly
                dialogueManager.EndDialogue();
                //resets conversationNumber so that NPC will display the last conversation displayed when activated.
                conversationNumber = startingConversationNumber;
            }


        }

    }

}
