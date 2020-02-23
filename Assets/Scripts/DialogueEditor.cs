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

    static string characterName;
    static bool useCustomName;

    static string[] animationNames;
    static Dictionary<string, bool> dialogueTriggers = DialogueTriggerDatabase.dialogueTriggers;

    List<Conversation> conversations;
    static string[] triggerNames;

    static bool hasAnimator;
    void OnEnable()
    {
        NPCDialogueScript = (NPCDialogue)target;
        dialogue = NPCDialogueScript.dialogue;

        characterName = dialogue.characterName;
        useCustomName = dialogue.customNameToggle;
        conversations = dialogue.conversations;

        AnimationClip[] NPCAnimations = new AnimationClip[0];
        if (NPCDialogueScript.gameObject.GetComponent<Animator>() != null)
        {
            hasAnimator = true;
            NPCAnimations = NPCDialogueScript.gameObject.GetComponentInChildren<Animator>().runtimeAnimatorController.animationClips;
        }
        else
        {
            hasAnimator = false;
        }

        animationNames = new string[NPCAnimations.Length];
        for (int i = 0; i < NPCAnimations.Length; i++)
        {
            animationNames[i] = NPCAnimations[i].name;
        }

        triggerNames = new string[dialogueTriggers.Count];
        int dictCount = 0;
        foreach (KeyValuePair<string, bool> entry in dialogueTriggers)
        {
            triggerNames[dictCount] = entry.Key;
            dictCount++;
        }
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

        if (hasAnimator)
        {
            EditorGUILayout.LabelField("Character has animator.", g);
        }
        else
        {
            EditorGUILayout.LabelField("Character has no animator!", r);
        }

        //how many conversations of dialogue this npc has
        int numberOfConversations = conversations.Count;
        numberOfConversations = EditorGUILayout.IntField("Number of Conversations:", numberOfConversations);

        if (numberOfConversations < 0)
        {
            numberOfConversations = 0;
        }

        while (numberOfConversations > conversations.Count)
        {
            conversations.Add(
                new Conversation
                {
                    showConversation = false,
                    conversationName = "Conversation " + (conversations.Count + 1),
                    requiredTrigger = "None",
                    hasAudio = false,
                    statements = new List<Statement> {
                        new Statement {
                            animationSelection = 0,
                            hasCustomAudio = false,
                            audioProperties = new Sound {
                                name = "Nothing",
                                clip = null,
                                volume = 1,
                                loop = false,
                            },
                            statementText = "Statement",
                        }
                    },
                    closingAnimation = 0,
                    setTrigger = "None",
                    hasResponse = false,
                    responses = new List<Response> {
                        new Response {
                            setTrigger = "None",
                            responseText = "Response",
                        }
                    },
                    triggerNextConversation = false,
                }
            );
        }
        while (numberOfConversations < conversations.Count)
        {
            conversations.RemoveAt(conversations.Count - 1);
        }

        foreach (Conversation conversation in conversations)
        {
            EditorGUI.indentLevel = 0;

            conversation.showConversation = EditorGUILayout.BeginFoldoutHeaderGroup(conversation.showConversation, conversation.conversationName);

            if (conversation.showConversation)
            {
                conversation.conversationName = EditorGUILayout.TextField("Conversation Name:", conversation.conversationName);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Required Trigger:");
                conversation.requiredTrigger = checkTriggers(conversation.requiredTrigger);
                conversation.requiredTrigger = triggerNames[EditorGUILayout.Popup(System.Array.IndexOf(triggerNames, conversation.requiredTrigger), triggerNames)];
                EditorGUILayout.EndHorizontal();

                conversation.hasAudio = EditorGUILayout.Toggle("Has Audio?", conversation.hasAudio);

                int numberOfStatements = conversation.statements.Count;
                numberOfStatements = EditorGUILayout.IntField("Number of Statements:", numberOfStatements);
                if (numberOfStatements < 0)
                {
                    numberOfStatements = 0;
                }
                while (numberOfStatements > conversation.statements.Count)
                {
                    conversation.statements.Add(
                        new Statement
                        {
                            animationSelection = 0,
                            hasCustomAudio = false,
                            audioProperties = new Sound
                            {
                                name = "Nothing",
                                clip = null,
                                volume = 1,
                                loop = false,
                            },
                            statementText = "Statement",
                        }
                    );
                }
                while (numberOfStatements < conversation.statements.Count)
                {
                    conversation.statements.RemoveAt(conversation.statements.Count - 1);
                }

                int statementCounter = 0;
                foreach (Statement statement in conversation.statements)
                {
                    EditorGUI.indentLevel++;

                    EditorGUILayout.LabelField("Statement " + (statementCounter + 1));

                    EditorGUI.indentLevel++;

                    if (hasAnimator)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Statement Animation:");
                        statement.animationSelection = EditorGUILayout.Popup(statement.animationSelection, animationNames);
                        EditorGUILayout.EndHorizontal();
                    }

                    if (conversation.hasAudio)
                    {
                        statement.hasCustomAudio = EditorGUILayout.Toggle("Has Custom Audio?", statement.hasCustomAudio);

                        if (statement.hasCustomAudio)
                        {
                            statement.audioProperties.clip = EditorGUILayout.ObjectField("Custom Audio Clip:", statement.audioProperties.clip, typeof(AudioClip), false) as AudioClip;
                        }
                        else
                        {
                            string mp3 = ".mp3";
                            string wav = ".wav";
                            string filePath = characterName.Replace(" ", "").Replace("/", "") + "_" + conversation.conversationName.Replace(" ", "") + "_" + (statementCounter + 1);

                            if (System.IO.File.Exists("Assets/Resources/Audio/" + filePath + wav) != false || System.IO.File.Exists("Assets/Resources/Audio/" + filePath + mp3) != false)
                            {
                                EditorGUILayout.LabelField("Using: " + filePath, g);
                                statement.audioProperties.clip = Resources.Load("Audio/" + filePath) as AudioClip;
                            }
                            else
                            {
                                EditorGUILayout.SelectableLabel("Couldn't find file at path: Resources/Audio/" + filePath + mp3, r);
                                statement.audioProperties.clip = null;
                            }
                        }

                        statement.audioProperties.volume = EditorGUILayout.Slider("Audio Volume:", statement.audioProperties.volume, 0, 1);
                    }

                    statement.statementText = EditorGUILayout.TextField("Statement Text:", statement.statementText);

                    EditorGUI.indentLevel = 0;
                    EditorGUILayout.LabelField("\n");
                    statementCounter++;
                }

                EditorGUI.indentLevel++;

                if (hasAnimator)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Closing Animation:");
                    conversation.closingAnimation = EditorGUILayout.Popup(conversation.closingAnimation, animationNames);
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Set Player Trigger:");
                conversation.setTrigger = checkTriggers(conversation.setTrigger);
                conversation.setTrigger = triggerNames[EditorGUILayout.Popup(System.Array.IndexOf(triggerNames, conversation.setTrigger), triggerNames)];
                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel--;
                EditorGUILayout.LabelField("\n");

                conversation.hasResponse = EditorGUILayout.Toggle("Has Response?", conversation.hasResponse);

                if (conversation.hasResponse)
                {
                    int numberOfResponses = conversation.responses.Count;
                    numberOfResponses = EditorGUILayout.IntField("Number of Responses:", numberOfResponses);
                    if (numberOfResponses < 1)
                    {
                        numberOfResponses = 1;
                    }
                    while (numberOfResponses > conversation.responses.Count)
                    {
                        conversation.responses.Add(
                            new Response
                            {
                                setTrigger = "None",
                                responseText = "Response",
                            }
                        );
                    }
                    while (numberOfResponses < conversation.responses.Count)
                    {
                        conversation.responses.RemoveAt(conversation.responses.Count - 1);
                    }

                    foreach (Response response in conversation.responses)
                    {
                        EditorGUI.indentLevel++;

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Set Trigger:");
                        response.setTrigger = checkTriggers(response.setTrigger);
                        response.setTrigger = triggerNames[EditorGUILayout.Popup(System.Array.IndexOf(triggerNames, response.setTrigger), triggerNames)];
                        EditorGUILayout.EndHorizontal();

                        response.responseText = EditorGUILayout.TextField("Response Text:", response.responseText);

                        EditorGUI.indentLevel--;
                    }
                }

                EditorGUILayout.LabelField("\n");
                conversation.triggerNextConversation = EditorGUILayout.Toggle("Trigger Next Conversation?", conversation.triggerNextConversation);

            }

            EditorGUI.indentLevel = 0;

            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }

    private string checkTriggers(string trigger)
    {
        bool match = false;
        foreach(string name in triggerNames)
        {
            if(trigger == name)
            {
                match = true;
            }
        }
        if(match)
        {
            return trigger;
        }
        else
        {
            return triggerNames[0];
        }
    }
}
