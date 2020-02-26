using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

[System.Serializable]
public class NPCDialogue : MonoBehaviour
{
    public Dialogue dialogue;
    public string NPCName { get { return gameObject.name; } }

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
    private Dictionary<string, bool> playerFlags { get { return playerManager.playerFlags; } }

    private CowpokeController playerController { get { return FindObjectOfType<CowpokeController>(); } }

    public bool playingDialogue = false;
    public bool playingResponse = false;
    private int conversationNumber = 0;
    public int responseNumber;
    public bool timerEnded = false;

    void Start()
    {
    }

    void Update()
    {
        //plays dialogue when N key is pressed down
        if (Input.GetKeyDown(KeyCode.Return) && playingDialogue)
        {
            PlayDialogue();
        }
    }

    public void PlayDialogue()
    {
        //might rewrite this PlayDialogue section. It all works its just a little confusing and probably hard to change in the future

        //starts by checking if a response is currently playing. If not, checks if dialogue is currently playing.
        //If neither is playing, 

        if (timerEnded)
        {
            timerEnded = dialogueManager.EndResponse();
            playerFlags[dialogue.conversations[conversationNumber].timerOutcomes.setFlag] = true;
            Outcomes timerOutcomes = dialogue.conversations[conversationNumber].timerOutcomes;

            //runs if the dialogue script says the current conversation should trigger the next conversation
            if (dialogue.conversations[conversationNumber].timerTriggerNextConversation && !timerOutcomes.triggerEvent.toggleBool)
            {
                //moves to the next conversation
                conversationNumber++;
                //no longer displaying the response screen
                playingResponse = false;
            }
            //runs if dialogue script says current conversation shouldn't trigger next conversation
            else if (!dialogue.conversations[conversationNumber].timerTriggerNextConversation && !timerOutcomes.triggerEvent.toggleBool)
            {
                //Ends the dialogue because response is over and no new dialogue is being triggered.
                dialogueManager.EndDialogue();
                //no longer displaying response screen
                playingResponse = false;
                if (timerOutcomes.queueNextConversation)
                {
                    conversationNumber++;
                    playerController.canMove = true;
                    return;
                }
                else
                {
                    //stops the loop
                    playerController.canMove = true;
                    return;
                }

            }
            else if (timerOutcomes.triggerEvent.toggleBool)
            {
                //Ends the dialogue because response is over and no new dialogue is being triggered.
                dialogueManager.EndDialogue();
                //no longer displaying response screen
                playingResponse = false;
                //stops the loop

                if (timerOutcomes.queueNextConversation)
                {
                    conversationNumber++;
                    playerController.canMove = true;
                }
                else
                {
                    playerController.canMove = true;
                }
                MonoBehaviour currentScript = timerOutcomes.eventScript;
                string methodString = timerOutcomes.eventMethod;
                MethodInfo currentMethod = currentScript.GetType().GetMethod(methodString);
                currentMethod.Invoke(currentScript, null);
                return;
            }
        }

        //only runs if the NPC is currently displaying the response screen of dialogue
        if (playingResponse)
        {
            //returns false once the dialogue manager runs through its EndResponse() method.
            playingResponse = dialogueManager.EndResponse();
            StopAllCoroutines();
            timerEnded = false;
            //should always return false but here just in case.
            if (playingResponse == false)
            {
                Response activatedResponse = dialogue.conversations[conversationNumber].responses[responseNumber];

                //runs if the dialogue script says the current conversation should trigger the next conversation
                if (activatedResponse.triggerNextConversation && !activatedResponse.outcomes.triggerEvent.toggleBool)
                {
                    //moves to the next conversation
                    conversationNumber++;
                    //no longer displaying the response screen
                    playingResponse = false;
                }
                //runs if dialogue script says current conversation shouldn't trigger next conversation
                else if (!activatedResponse.triggerNextConversation && !activatedResponse.outcomes.triggerEvent.toggleBool)
                {
                    //Ends the dialogue because response is over and no new dialogue is being triggered.
                    dialogueManager.EndDialogue();
                    //no longer displaying response screen
                    playingResponse = false;
                    if (activatedResponse.outcomes.queueNextConversation)
                    {
                        conversationNumber++;
                        playerController.canMove = true;
                        return;
                    }
                    else
                    {
                        //stops the loop
                        playerController.canMove = true;
                        return;
                    }

                }
                else if (activatedResponse.outcomes.triggerEvent.toggleBool)
                {
                    //Ends the dialogue because response is over and no new dialogue is being triggered.
                    dialogueManager.EndDialogue();
                    //no longer displaying response screen
                    playingResponse = false;
                    //stops the loop

                    if (activatedResponse.outcomes.queueNextConversation)
                    {
                        conversationNumber++;
                        playerController.canMove = true;
                    }
                    else
                    {
                        playerController.canMove = true;
                    }
                    MonoBehaviour currentScript = activatedResponse.outcomes.eventScript;
                    string methodString = activatedResponse.outcomes.eventMethod;
                    MethodInfo currentMethod = currentScript.GetType().GetMethod(methodString);
                    currentMethod.Invoke(currentScript, null);
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
                Conversation currentConversation = dialogue.conversations[conversationNumber];
                //sets the trigger attached to the player to true if dialogue script says that trigger should be set once dialogue is finished
                playerFlags[currentConversation.outcomes.setFlag] = true;
                //runs if the NPC is set to have a response to the sentences that were displayed
                if (currentConversation.hasResponse.toggleBool && !currentConversation.outcomes.triggerEvent.toggleBool)
                {
                    //an array of the responses on the dialogue script
                    string[] responses = new string[currentConversation.responses.Count];
                    //an array of the triggers to set for each response that are attached to the dialogue script
                    string[] triggers = new string[currentConversation.responses.Count];
                    int counter = 0;
                    //gives those arrays their values.
                    foreach (Response response in currentConversation.responses)
                    {
                        responses[counter] = response.responseText;
                        triggers[counter] = response.outcomes.setFlag;
                        counter++;
                    }
                    //stops any dialogue audio playing on the audiomanager because dialogue has finished or been skipped.
                    audioManager.StopDialogueAudio();
                    //tells the dialogue manager to startd displaying the player's responses. Takes the responses, triggers, and this script
                    dialogueManager.StartResponse(responses, triggers, this);
                    //not playing dialogue anymore because responses are playing
                    playingDialogue = false;
                    playingResponse = true;
                    timerEnded = false;
                    if (dialogue.conversations[conversationNumber].timerToggle.toggleBool)
                    {
                        StartCoroutine(ResponseTimer(currentConversation.timerValue));
                    }
                }
                //runs if the NPC is not set to have any responses.
                else if (!currentConversation.hasResponse.toggleBool && !currentConversation.outcomes.triggerEvent.toggleBool)
                {
                    if (currentConversation.outcomes.queueNextConversation)
                    {
                        conversationNumber++;
                    }
                    //stops audio because dialogue is over and tells dialogue manager to close dialogue window because there is nothing else to display
                    audioManager.StopDialogueAudio();
                    dialogueManager.EndDialogue();
                    playerController.canMove = true;
                }
                else if (currentConversation.outcomes.triggerEvent.toggleBool)
                {
                    if (currentConversation.outcomes.queueNextConversation)
                    {
                        conversationNumber++;
                    }
                    //stops audio because dialogue is over and tells dialogue manager to close dialogue window because there is nothing else to display
                    audioManager.StopDialogueAudio();
                    dialogueManager.EndDialogue();
                    playerController.canMove = true;
                }
            }
            //runs if there are more sentences to display
            else
            {
                //stops the previous sound playing through the audio manager
                audioManager.StopDialogueAudio();
                //plays the current sound sentence's
                if (dialogue.conversations[conversationNumber].hasAudio)
                {
                    audioManager.PlayDialogueAudio();
                }
                //plays the NPC's animation associated with the sentence being displayed.
                if (NPCAnimator != null)
                {
                    NPCAnimator.Play(currentAnimations.Dequeue());
                }
            }
        }
        //only runs if the NPC doesn't have any dialogue or responses on the screen.
        else if (playingResponse == false)
        {
            //tracks if the NPC can play another conversation based on their current conversation number and the player's current triggers
            bool hasNextConverstaion = true;
            //gets current conversation number in case there aren't any conversations that work with current triggers.
            int startingConversationNumber = conversationNumber;
            //iterates through conversations attached to the dialogue script until one matches the triggers attached to the player
            while (playerFlags[dialogue.conversations[conversationNumber].requiredFlag] == false)
            {
                conversationNumber++;
                //detects if there aren't any more conversations and none of them have matched with the player's triggers
                if (conversationNumber > dialogue.conversations.Count - 1)
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
                playerController.canMove = false;
                //string array for all the sentences in the current conversation
                string[] newSentences = new string[dialogue.conversations[conversationNumber].statements.Count];
                if (newSentences.Length > 0)
                {
                    int counter = 0;
                    //gives newSentence array its values. Also sends the sound clips for each sentence to the audio manager and creates a list of the NPC's animations to play
                    foreach (Statement statement in dialogue.conversations[conversationNumber].statements)
                    {
                        //gets all animations
                        if (NPCAnimator != null)
                        {
                            currentAnimations.Enqueue(NPCAnimator.runtimeAnimatorController.animationClips[statement.animationSelection].name);
                        }
                        //gets all sounds
                        if (dialogue.conversations[conversationNumber].hasAudio)
                        {
                            audioManager.dialogueSounds.Enqueue(statement.audioProperties);
                        }
                        newSentences[counter] = statement.statementText;
                        counter++;
                    }
                    //starts displaying dialogue through the dialoguemanager. Takes the NPC's name and its current sentences
                    dialogueManager.StartDialogue(dialogue.characterName, newSentences);
                    //starts playing sound through the audio manager. Only plays one sound at a time.
                    if (dialogue.conversations[conversationNumber].hasAudio)
                    {
                        audioManager.PlayDialogueAudio();
                    }
                    //plays the animation associated with the first sentence of dialogue.
                    if (NPCAnimator != null)
                    {
                        NPCAnimator.Play(currentAnimations.Dequeue());
                    }
                }
                else
                {
                    if (NPCAnimator != null)
                    {
                        NPCAnimator.Play(dialogue.conversations[conversationNumber].closingAnimation);
                    }
                    dialogueManager.StartDialogue(dialogue.characterName, newSentences);
                    PlayDialogue();
                }


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
                playerController.canMove = true;
            }
        }

    }

    IEnumerator ResponseTimer(float timerValue)
    {
        yield return new WaitForSeconds(timerValue);
        timerEnded = true;
        PlayDialogue();
    }
}
