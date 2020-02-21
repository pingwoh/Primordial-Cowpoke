using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(NPCDialogue))]
[CanEditMultipleObjects]
public class DialogueEditor : Editor
{
    NPCDialogue NPCDialogueScript;
    Dialogue dialogue;

    static string filePath;

    static string characterName;
    static bool useCustomName;

    static List<SentenceList> conversations;
    static List<string> conversationNames;
    static string[] animationTriggers;
    static List<AnimationSelectionsList> animationSelections;
    static int numberOfConversations;
    static int[] numberOfSentences;
    static string[][] sentenceText;
    static string[] animationNames;
    static int[][] selectedAnimation;

    static bool hasAudioFile;
    static string audioFileName;

    static Dictionary<string, bool> dialogueTriggers = DialogueTriggerDatabase.dialogueTriggers;
    static List<string> triggerSelections;
    static List<string> triggerActivations;
    static string[] triggerNames;
    static int[] selectedTrigger;
    static int[] selectedActivation;

    static List<bool> responseToggles;
    static List<int> numberOfResponses;
    static List<ResponseList> responses;
    static List<ResponseActiviationsList> responseActivations;
    static string[][] responseText;
    static int[][] selectedResponseActivations;

    static List<bool> showConversation;
    static bool[] selectedShow;

    static List<bool> hasAudio;
    static List<CustomAudioList> customAudioList;
    static List<AudioClipsList> audioClipsLists;
    static bool[][] selectedCustomAudio;
    static Object[][] selectedClip;

    static List<bool> triggerNextConversation;

    void OnEnable()
    {
        NPCDialogueScript = (NPCDialogue)target;
        dialogue = NPCDialogueScript.dialogue;
        AnimationClip[] NPCAnimations = null;
        if (NPCDialogueScript.gameObject.GetComponent<Animator>() != null)
        {
            NPCAnimations = NPCDialogueScript.gameObject.GetComponentInChildren<Animator>().runtimeAnimatorController.animationClips;
        }
        else
        {
            Debug.Log("no animator");
            //need to setup failsafe for if no animator attached to gameobject.
        }

        characterName = dialogue.characterName;
        useCustomName = dialogue.customNameToggle;

        animationSelections = dialogue.animationSelections;

        animationNames = new string[NPCAnimations.Length];
        for (int i = 0; i < NPCAnimations.Length; i++)
        {
            animationNames[i] = NPCAnimations[i].name;
        }

        conversations = dialogue.conversationList;
        conversationNames = dialogue.conversationNames;

        triggerNames = new string[dialogueTriggers.Count];
        int dictCount = 0;
        foreach (KeyValuePair<string, bool> entry in dialogueTriggers)
        {
            triggerNames[dictCount] = entry.Key;
            dictCount++;
        }

        triggerSelections = dialogue.triggerSelections;
        triggerActivations = dialogue.triggerActivations;

        responseToggles = dialogue.responseToggles;
        numberOfResponses = dialogue.numberOfResponses;
        responses = dialogue.responseList;
        responseActivations = dialogue.responseActivationsList;

        showConversation = dialogue.showConversation;

        hasAudio = dialogue.hasAudio;
        customAudioList = dialogue.customAudioList;
        audioClipsLists = dialogue.audioClipsList;

        triggerNextConversation = dialogue.triggerNextConversation;
    }

    public override void OnInspectorGUI()
    {
        GUIStyle g = new GUIStyle(EditorStyles.textField);
        g.normal.background = default;
        g.normal.textColor = Color.green;
        GUIStyle r = new GUIStyle(EditorStyles.textField);
        r.normal.background = default;
        r.normal.textColor = Color.red;
        
        
        //sets up ability to undo changes
        Undo.RecordObject(NPCDialogueScript, "Changed NPC Dialogue");
        //start updating the custom editor gui
        serializedObject.Update();
        EditorGUI.indentLevel = 0;
        //section that handles the NPC's name and if a custom name should be used
        useCustomName = EditorGUILayout.Toggle("Custom Name?", useCustomName);
        dialogue.customNameToggle = useCustomName;

        if (useCustomName)
        {
            characterName = EditorGUILayout.TextField("Character Name: ", characterName);
        }
        else
        {
            characterName = NPCDialogueScript.gameObject.name;
            EditorGUILayout.LabelField("Character Name: " + characterName);
        }
        dialogue.characterName = characterName;

        EditorGUILayout.LabelField("\n");

        //how many conversations of dialogue this npc has
        numberOfConversations = conversations.Count;
        numberOfConversations = EditorGUILayout.IntField("Number of Conversations", numberOfConversations);
        EditorGUILayout.LabelField("\n");
        while (numberOfConversations > conversations.Count )
        {
            conversations.Add(new SentenceList());
        }
        while (numberOfConversations > animationSelections.Count )
        {
            animationSelections.Add(new AnimationSelectionsList());
        }
        //triggerSelections.Clear();
        while (numberOfConversations > triggerSelections.Count )
        {
            triggerSelections.Add(triggerNames[0]);
        }
        while (numberOfConversations > triggerActivations.Count )
        {
            triggerActivations.Add(triggerNames[0]);
        }
        while (numberOfConversations > responseToggles.Count )
        {
            responseToggles.Add(false);
        }
        while (numberOfConversations > numberOfResponses.Count )
        {
            numberOfResponses.Add(1);
        }
        while (numberOfConversations > responses.Count )
        {
            responses.Add(new ResponseList());
        }
        //responseActivations.Clear();
        while (numberOfConversations > responseActivations.Count )
        {
            responseActivations.Add(new ResponseActiviationsList());
        }
        while (numberOfConversations > showConversation.Count )
        {
            showConversation.Add(true);
        }
        while (numberOfConversations > hasAudio.Count)
        {
            hasAudio.Add(false);
        }
        while (numberOfConversations > customAudioList.Count )
        {
            customAudioList.Add(new CustomAudioList());
        }
        while (numberOfConversations > audioClipsLists.Count )
        {
            audioClipsLists.Add(new AudioClipsList());
        }
        while (numberOfConversations > conversationNames.Count )
        {
            conversationNames.Add("Conversation " + (conversationNames.Count + 1));
        }
        while (numberOfConversations > triggerNextConversation.Count )
        {
            triggerNextConversation.Add(false);
        }


        while(numberOfConversations < conversations.Count)
        {
            conversations.RemoveAt(conversations.Count - 1);
        }
        while(numberOfConversations < animationSelections.Count)
        {
            animationSelections.RemoveAt(animationSelections.Count - 1);
        }
        while(numberOfConversations < triggerSelections.Count)
        {
            triggerSelections.RemoveAt(triggerSelections.Count - 1);
        }
        while (numberOfConversations < triggerActivations.Count)
        {
            triggerActivations.RemoveAt(triggerActivations.Count - 1);
        }
        while (numberOfConversations < responseToggles.Count)
        {
            responseToggles.RemoveAt(responseToggles.Count - 1);
        }
        while (numberOfConversations < numberOfResponses.Count)
        {
            numberOfResponses.RemoveAt(numberOfResponses.Count - 1);
        }
        while (numberOfConversations < responses.Count)
        {
            responses.RemoveAt(responses.Count - 1);
        }
        while (numberOfConversations < responseActivations.Count)
        {
            responseActivations.RemoveAt(responseActivations.Count - 1);
        }
        while (numberOfConversations < showConversation.Count)
        {
            showConversation.RemoveAt(showConversation.Count - 1);
        }
        while (numberOfConversations < hasAudio.Count)
        {
            hasAudio.RemoveAt(hasAudio.Count - 1);
        }
        while (numberOfConversations < customAudioList.Count)
        {
            customAudioList.RemoveAt(customAudioList.Count - 1);
        }
        while (numberOfConversations < audioClipsLists.Count)
        {
            audioClipsLists.RemoveAt(audioClipsLists.Count - 1);
        }
        while (numberOfConversations < conversationNames.Count)
        {
            conversationNames.RemoveAt(conversationNames.Count - 1);
        }
        while (numberOfConversations < triggerNextConversation.Count)
        {
            triggerNextConversation.RemoveAt(triggerNextConversation.Count - 1);
        }

        sentenceText = new string[numberOfConversations][];
        selectedAnimation = new int[numberOfConversations][];
        selectedTrigger = new int[numberOfConversations];
        selectedActivation = new int[numberOfConversations];
        responseText = new string[numberOfConversations][];
        selectedResponseActivations = new int[numberOfConversations][];
        selectedShow = new bool[numberOfConversations];
        selectedCustomAudio = new bool[numberOfConversations][];
        selectedClip = new Object[numberOfConversations][];

        if (numberOfConversations < 0)
        {
            numberOfConversations = 0;
        }
        numberOfSentences = new int[numberOfConversations];

        //runs for each conversation in number of conversations
        for (int i = 0; i < numberOfConversations; i++)
        {
            EditorGUI.indentLevel = 0;
            if (conversations[i].sentences == null)
            {
                conversations[i].sentences = new List<string> { "Sentence" };
            }

            selectedShow[i] = showConversation[i];
            //selectedShow[i] = EditorGUILayout.BeginFoldoutHeaderGroup(selectedShow[i], "Conversation " + (i + 1));
            selectedShow[i] = EditorGUILayout.BeginFoldoutHeaderGroup(selectedShow[i], conversationNames[i]);
            showConversation[i] = selectedShow[i];
            if (selectedShow[i])
            {
                EditorGUILayout.BeginHorizontal();

                conversationNames[i] = EditorGUILayout.TextField("Conversation Name:", conversationNames[i]);

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Required Trigger:");
                selectedTrigger[i] = System.Array.IndexOf(triggerNames, triggerSelections[i]);
                selectedTrigger[i] = EditorGUILayout.Popup(selectedTrigger[i], triggerNames);
                triggerSelections[i] = triggerNames[selectedTrigger[i]];

                EditorGUILayout.EndHorizontal();

                hasAudio[i] = EditorGUILayout.Toggle("Has Audio?", hasAudio[i]);

                numberOfSentences[i] = conversations[i].sentences.Count;
                numberOfSentences[i] = EditorGUILayout.IntField("Number of Sentences", numberOfSentences[i]);
                if (numberOfSentences[i] < 1)
                {
                    numberOfSentences[i] = 1;
                }
                while (numberOfSentences[i] > conversations[i].sentences.Count)
                {
                    conversations[i].sentences.Add("Sentence");
                }
                while (numberOfSentences[i] < conversations[i].sentences.Count)
                {
                    conversations[i].sentences.RemoveAt(conversations[i].sentences.Count - 1);
                }
                sentenceText[i] = new string[numberOfSentences[i]];

                if (animationSelections[i].animationSelections == null)
                {
                    animationSelections[i].animationSelections = new List<int> { 0 };
                }
                //add one to number of sentences to account for closing animation
                while (numberOfSentences[i] + 1 > animationSelections[i].animationSelections.Count)
                {
                    animationSelections[i].animationSelections.Add(0);
                }
                while (numberOfSentences[i] + 1 < animationSelections[i].animationSelections.Count)
                {
                    animationSelections[i].animationSelections.RemoveAt(animationSelections[i].animationSelections.Count - 1);
                }
                selectedAnimation[i] = new int[numberOfSentences[i] + 1];


                if (customAudioList[i].toggle == null)
                {
                    customAudioList[i].toggle = new List<bool> { false };
                }
                while (numberOfSentences[i] > customAudioList[i].toggle.Count)
                {
                    customAudioList[i].toggle.Add(false);
                }
                while (numberOfSentences[i] < customAudioList[i].toggle.Count)
                {
                    customAudioList[i].toggle.RemoveAt(customAudioList[i].toggle.Count - 1);
                }
                selectedCustomAudio[i] = new bool[numberOfSentences[i]];

                if (audioClipsLists[i].clips == null)
                {
                    audioClipsLists[i].clips = new List<AudioClip> { null };
                }
                while (numberOfSentences[i] > audioClipsLists[i].clips.Count)
                {
                    audioClipsLists[i].clips.Add(null);
                }
                while (numberOfSentences[i] < audioClipsLists[i].clips.Count)
                {
                    audioClipsLists[i].clips.RemoveAt(audioClipsLists[i].clips.Count - 1);
                }
                selectedClip[i] = new Object[numberOfSentences[i]];

                //runs for each sentence within each conversation
                for (int j = 0; j < numberOfSentences[i]; j++)
                {
                    selectedAnimation[i][j] = animationSelections[i].animationSelections[j];
                    selectedCustomAudio[i][j] = customAudioList[i].toggle[j];
                    selectedClip[i][j] = audioClipsLists[i].clips[j];
                    sentenceText[i][j] = conversations[i][j];

                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField("Sentence " + (j + 1));

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Animation:");
                    selectedAnimation[i][j] = EditorGUILayout.Popup(selectedAnimation[i][j], animationNames);
                    animationSelections[i][j] = selectedAnimation[i][j];

                    EditorGUILayout.EndHorizontal();



                    if (hasAudio[i])
                    {
                        selectedCustomAudio[i][j] = EditorGUILayout.Toggle("Use Custom Audio", selectedCustomAudio[i][j]);
                        customAudioList[i][j] = selectedCustomAudio[i][j];
                        if (selectedCustomAudio[i][j])
                        {
                            selectedClip[i][j] = EditorGUILayout.ObjectField("Custom Audio Clip:", selectedClip[i][j], typeof(AudioClip), false);
                        }
                        else
                        {
                            //Debug.Log(System.IO.File.Exists("Assets/Resources/Audio/bling.mp3"));
                            filePath = characterName.Replace(" ", "") + "_" + conversationNames[i].Replace(" ", "") + "_" + (j + 1) + ".mp3";
                            if (System.IO.File.Exists("Assets/Resources/Audio/" + filePath) != false)
                            {
                                EditorGUILayout.LabelField("Using: " + filePath, g);
                                selectedClip[i][j] = Resources.Load("Audio/" + filePath);
                            }
                            else
                            {
                                EditorGUILayout.LabelField("Couldn't find file at path: Resources/Audio/" + filePath, r);
                                selectedClip[i][j] = null;
                            }
                        }
                    }
                    else
                    {
                        selectedClip[i][j] = null;
                    }

                    audioClipsLists[i][j] = selectedClip[i][j] as AudioClip;
      

                    sentenceText[i][j] = EditorGUILayout.TextArea(sentenceText[i][j]);
                    conversations[i][j] = sentenceText[i][j];

                    if (j == numberOfSentences[i] - 1)
                    {
                        selectedAnimation[i][j + 1] = animationSelections[i].animationSelections[j + 1];
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Closing Animation: ");

                        selectedAnimation[i][j + 1] = EditorGUILayout.Popup(selectedAnimation[i][j + 1], animationNames);
                        animationSelections[i][j + 1] = selectedAnimation[i][j + 1];

                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Set Player Trigger: ");

                        selectedTrigger[i] = System.Array.IndexOf(triggerNames, triggerActivations[i]);
                        selectedTrigger[i] = EditorGUILayout.Popup(selectedTrigger[i], triggerNames);
                        triggerActivations[i] = triggerNames[selectedTrigger[i]];

                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUI.indentLevel--;
                }

                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("\n");
                responseToggles[i] = EditorGUILayout.Toggle("Has Responses?", responseToggles[i]);


                if (responseToggles[i])
                {
                    //EditorGUILayout.LabelField("\n");
                    numberOfResponses[i] = responses[i].responses.Count;
                    numberOfResponses[i] = EditorGUILayout.IntField("Number of Responses", numberOfResponses[i]);
                    if (numberOfResponses[i] < 1)
                    {
                        numberOfResponses[i] = 1;
                    }
                    while (numberOfResponses[i] > responses[i].responses.Count)
                    {
                        responses[i].responses.Add("Response");
                    }
                    while (numberOfResponses[i] < responses[i].responses.Count)
                    {
                        responses[i].responses.RemoveAt(responses[i].responses.Count - 1);
                    }
                }
                else
                {
                    numberOfResponses[i] = 0;
                    EditorGUI.indentLevel = 0;
                }
                responseText[i] = new string[numberOfResponses[i]];


                //don't know if I need this
                if (responseActivations[i].activations == null)
                {
                    responseActivations[i].activations = new List<string> { "None" };
                }
                while (numberOfResponses[i] > responseActivations[i].activations.Count)
                {
                    responseActivations[i].activations.Add("None");
                }
                while (numberOfResponses[i] > responseActivations[i].activations.Count)
                {
                    responseActivations[i].activations.RemoveAt(responseActivations[i].activations.Count - 1);
                }
                selectedResponseActivations[i] = new int[numberOfResponses[i]];

                for (int j = 0; j < numberOfResponses[i]; j++)
                {
                    selectedResponseActivations[i][j] = System.Array.IndexOf(triggerNames, responseActivations[i][j]);
                    responseText[i][j] = responses[i][j];

                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField("Response " + (j + 1));
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Set Player Trigger:");
                    selectedResponseActivations[i][j] = EditorGUILayout.Popup(selectedResponseActivations[i][j], triggerNames);
                    responseActivations[i][j] = triggerNames[selectedResponseActivations[i][j]];

                    EditorGUILayout.EndHorizontal();
                    responseText[i][j] = EditorGUILayout.TextArea(responseText[i][j]);
                    responses[i][j] = responseText[i][j];
                    EditorGUI.indentLevel--;
                }
                if (responseToggles[i])
                {
                    EditorGUI.indentLevel++;
                    triggerNextConversation[i] = EditorGUILayout.Toggle("Trigger Next Conversation?", triggerNextConversation[i]);
                    EditorGUI.indentLevel--;
                }
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(NPCDialogueScript);
            EditorSceneManager.MarkSceneDirty(NPCDialogueScript.gameObject.scene);
            Undo.FlushUndoRecordObjects();
        }
    }
}
