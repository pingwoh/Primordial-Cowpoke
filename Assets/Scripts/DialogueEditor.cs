using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.AnimatedValues;

[CustomEditor(typeof(NPCDialogue))]
[CanEditMultipleObjects]
public class DialogueEditor : Editor
{
    NPCDialogue NPCDialogueScript;
    Dialogue dialogue;

    static string characterName;
    static bool useCustomName;

    static string[] animationNames;
    static Dictionary<string, bool> dialogueFlags = DialogueFlagDatabase.dialogueFlags;

    List<Conversation> conversations;
    static string[] flagNames;

    static bool hasAnimator;

    GUIStyle g;
    GUIStyle r;

    int numberOfConversations = 0;
    void OnEnable()
    {
        NPCDialogueScript = (NPCDialogue)target;
        dialogue = NPCDialogueScript.dialogue;

        if (dialogue.characterName == null)
        {
            dialogue.characterName = "";
        }
        if (dialogue.conversations == null)
        {
            dialogue.conversations = new List<Conversation>();
        }

        characterName = dialogue.characterName;
        useCustomName = dialogue.customNameToggle;
        conversations = dialogue.conversations;
    }

    public override void OnInspectorGUI()
    {
        AnimationClip[] NPCAnimations = new AnimationClip[0];
        if (NPCDialogueScript.gameObject.GetComponent<Animator>() != null)
        {
            if(NPCDialogueScript.gameObject.GetComponent<Animator>().runtimeAnimatorController != null)
            {
                hasAnimator = true;
                NPCAnimations = NPCDialogueScript.gameObject.GetComponentInChildren<Animator>().runtimeAnimatorController.animationClips;
            }
            else
            {
                hasAnimator = false;
            }
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

        flagNames = new string[dialogueFlags.Count];
        int dictCount = 0;
        foreach (KeyValuePair<string, bool> entry in dialogueFlags)
        {
            flagNames[dictCount] = entry.Key;
            dictCount++;
        }

        g = new GUIStyle(EditorStyles.textField);
        g.normal.background = default;
        g.normal.textColor = Color.green;
        r = new GUIStyle(EditorStyles.textField);
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
            characterName = NPCDialogueScript.NPCName;
            EditorGUILayout.LabelField("Character Name: " + characterName);
        }
        dialogue.characterName = characterName;

        if (hasAnimator)
        {
            EditorGUILayout.LabelField("Character has animator.", g);
        }
        else
        {
            EditorGUILayout.LabelField("Character has no animator or has no Controller!", r);
        }

        //how many conversations of dialogue this npc has
        numberOfConversations = conversations.Count;

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
                    showConversation = new DisplayToggle
                    {
                        toggleBool = false,
                        toggleAnim = new AnimBool(false),
                    },
                    conversationName = "Conversation " + (conversations.Count + 1),
                    requiredFlag = "None",
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
                    outcomes = new Outcomes
                    {
                        setFlag = "None",
                        queueNextConversation = false,
                        triggerEvent = new DisplayToggle
                        {
                            toggleBool = false,
                            toggleAnim = new AnimBool(false),
                        },
                        eventObject = null,
                        eventScript = null,
                        eventMethod = null,
                    },
                    hasResponse = new DisplayToggle
                    {
                        toggleBool = false,
                        toggleAnim = new AnimBool(false),
                    },
                    responses = new List<Response> {
                        new Response {
                            outcomes = new Outcomes
                            {
                                setFlag = "None",
                                queueNextConversation = false,
                                triggerEvent = new DisplayToggle
                                {
                                    toggleBool = false,
                                    toggleAnim = new AnimBool(false),
                                },
                                eventObject = null,
                                eventScript = null,
                                eventMethod = null,
                            },
                            responseText = "Response",
                            triggerNextConversation = false,
                        }
                    },
                    timerToggle = new DisplayToggle
                    {
                        toggleBool = false,
                        toggleAnim = new AnimBool(false),
                    },
                    timerValue = 0f,
                    timerTriggerNextConversation = false,
                    timerOutcomes = new Outcomes
                    {
                        setFlag = "None",
                        queueNextConversation = false,
                        triggerEvent = new DisplayToggle
                        {
                            toggleBool = false,
                            toggleAnim = new AnimBool(false),
                        },
                        eventObject = null,
                        eventScript = null,
                        eventMethod = null,
                    },
                }
            );
        }
        while (numberOfConversations < conversations.Count)
        {
            conversations.RemoveAt(conversations.Count - 1);
        }

        int conversationCounter = 0;
        foreach (Conversation conversation in conversations)
        {
            EditorGUI.indentLevel = 0;

            conversation.showConversation.toggleBool = EditorGUILayout.BeginFoldoutHeaderGroup(conversation.showConversation.toggleBool, conversation.conversationName);
            conversation.showConversation.toggleAnim.target = conversation.showConversation.toggleBool;
            if (EditorGUILayout.BeginFadeGroup(conversation.showConversation.toggleAnim.faded))
            {
                conversation.conversationName = EditorGUILayout.TextField("Conversation Name:", conversation.conversationName);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Required Flag:");
                conversation.requiredFlag = checkFlags(conversation.requiredFlag);
                conversation.requiredFlag = flagNames[EditorGUILayout.Popup(System.Array.IndexOf(flagNames, conversation.requiredFlag), flagNames)];
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

                    EditorGUILayout.LabelField("Statement Text:");
                    statement.statementText = EditorGUILayout.TextArea(statement.statementText);

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
                EditorGUILayout.LabelField("Set Player Dialogue Flag:");
                conversation.outcomes.setFlag = checkFlags(conversation.outcomes.setFlag);
                conversation.outcomes.setFlag = flagNames[EditorGUILayout.Popup(System.Array.IndexOf(flagNames, conversation.outcomes.setFlag), flagNames)];
                EditorGUILayout.EndHorizontal();

                if(conversation.outcomes.triggerEvent.toggleBool || !conversation.hasResponse.toggleBool)
                {
                    conversation.outcomes.queueNextConversation = EditorGUILayout.Toggle("Queue Next Conversation?", conversation.outcomes.queueNextConversation);
                }
                else
                {
                    EditorGUILayout.LabelField("Queue Next Conversation?", "Set Queue in Responses!", r);
                }

                conversation.outcomes.triggerEvent.toggleBool = EditorGUILayout.Toggle("Trigger Event?", conversation.outcomes.triggerEvent.toggleBool);
                conversation.outcomes.triggerEvent.toggleAnim.target = conversation.outcomes.triggerEvent.toggleBool;

                if (EditorGUILayout.BeginFadeGroup(conversation.outcomes.triggerEvent.toggleAnim.faded))
                {
                    EditorGUI.indentLevel++;

                    EventTriggerSelection(conversation.outcomes);
                }
                EditorGUILayout.EndFadeGroup();
                EditorGUI.indentLevel = 0;

                if (conversation.outcomes.triggerEvent.toggleBool)
                {
                    conversation.hasResponse.toggleAnim.target = !conversation.outcomes.triggerEvent.toggleBool;
                    EditorGUILayout.LabelField("\n");
                    EditorGUILayout.LabelField("Has Response?", "Can't Have Response if Triggering an Event!", r);
                }
                else
                {
                    EditorGUILayout.LabelField("\n");

                    conversation.hasResponse.toggleBool = EditorGUILayout.Toggle("Has Response?", conversation.hasResponse.toggleBool);
                    conversation.hasResponse.toggleAnim.target = conversation.hasResponse.toggleBool;
                }

                if (EditorGUILayout.BeginFadeGroup(conversation.hasResponse.toggleAnim.faded))
                {
                    int numberOfResponses = conversation.responses.Count;
                    numberOfResponses = EditorGUILayout.IntField("Number of Responses:", numberOfResponses);
                    EditorGUILayout.LabelField("\n");
                    if (numberOfResponses < 1)
                    {
                        numberOfResponses = 1;
                    }
                    while (numberOfResponses > conversation.responses.Count)
                    {
                        conversation.responses.Add(
                            new Response
                            {
                                outcomes = new Outcomes
                                {
                                    setFlag = "None",
                                    triggerEvent = new DisplayToggle
                                    {
                                        toggleBool = false,
                                        toggleAnim = new AnimBool(false),
                                    },
                                    queueNextConversation = false,
                                    eventObject = null,
                                    eventScript = null,
                                    eventMethod = null,
                                },
                                responseText = "Response",
                            }
                        );
                    }
                    while (numberOfResponses < conversation.responses.Count)
                    {
                        conversation.responses.RemoveAt(conversation.responses.Count - 1);
                    }

                    int responseCounter = 0;
                    foreach (Response response in conversation.responses)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.LabelField("Response " + (responseCounter + 1));
                        EditorGUI.indentLevel++;

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Set Player Dialogue Flag:");
                        response.outcomes.setFlag = checkFlags(response.outcomes.setFlag);
                        response.outcomes.setFlag = flagNames[EditorGUILayout.Popup(System.Array.IndexOf(flagNames, response.outcomes.setFlag), flagNames)];
                        EditorGUILayout.EndHorizontal();

                        if(!response.triggerNextConversation || response.outcomes.triggerEvent.toggleBool)
                        {
                            response.outcomes.queueNextConversation = EditorGUILayout.Toggle("Queue Next Conversation?", response.outcomes.queueNextConversation);
                        }
                        else
                        {
                            EditorGUILayout.LabelField("Queue Next Conversation?", "Triggering Next Conversation Automatically Queues Next Conversation", r);
                        }

                        response.outcomes.triggerEvent.toggleBool = EditorGUILayout.Toggle("Trigger Event?", response.outcomes.triggerEvent.toggleBool);
                        response.outcomes.triggerEvent.toggleAnim.target = response.outcomes.triggerEvent.toggleBool;
                        if (EditorGUILayout.BeginFadeGroup(response.outcomes.triggerEvent.toggleAnim.faded))
                        {
                            EventTriggerSelection(response.outcomes);
                        }
                        EditorGUILayout.EndFadeGroup();
                        if (response.outcomes.triggerEvent.toggleBool)
                        {

                            EditorGUILayout.LabelField("\n");
                            EditorGUILayout.LabelField("Trigger Next Conversation?", "Can't Trigger Next Conversation if Triggering Event!", r);
                        }
                        else
                        {
                            response.triggerNextConversation = EditorGUILayout.Toggle("Trigger Next Conversation?", response.triggerNextConversation);
                        }

                        EditorGUILayout.LabelField("Response Text:");
                        response.responseText = EditorGUILayout.TextArea(response.responseText);

                        EditorGUILayout.LabelField("\n");
                        EditorGUI.indentLevel--;
                        EditorGUI.indentLevel--;
                        responseCounter++;
                    }
                }
                EditorGUILayout.EndFadeGroup();
                //runs if dialogue isn't triggering an event
                EditorGUILayout.LabelField("\n");
                conversation.timerToggle.toggleBool = EditorGUILayout.Toggle("Use Timer?", conversation.timerToggle.toggleBool);
                conversation.timerToggle.toggleAnim.target = conversation.timerToggle.toggleBool;

                if (EditorGUILayout.BeginFadeGroup(conversation.timerToggle.toggleAnim.faded))
                {
                    conversation.timerValue = EditorGUILayout.FloatField("Time Limit:", conversation.timerValue);
                    EditorGUILayout.LabelField("\n");
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Set Player Dialogue Flag:");
                    conversation.timerOutcomes.setFlag = checkFlags(conversation.timerOutcomes.setFlag);
                    conversation.timerOutcomes.setFlag = flagNames[EditorGUILayout.Popup(System.Array.IndexOf(flagNames, conversation.timerOutcomes.setFlag), flagNames)];
                    EditorGUILayout.EndHorizontal();
                    conversation.timerOutcomes.queueNextConversation = EditorGUILayout.Toggle("Queue Next Conversation?", conversation.timerOutcomes.queueNextConversation);



                    conversation.timerOutcomes.triggerEvent.toggleBool = EditorGUILayout.Toggle("Trigger Event?", conversation.timerOutcomes.triggerEvent.toggleBool);
                    conversation.timerOutcomes.triggerEvent.toggleAnim.target = conversation.timerOutcomes.triggerEvent.toggleBool;

                    if (EditorGUILayout.BeginFadeGroup(conversation.timerOutcomes.triggerEvent.toggleAnim.faded))
                    {
                        EditorGUI.indentLevel++;

                        EventTriggerSelection(conversation.timerOutcomes);
                    }
                    EditorGUILayout.EndFadeGroup();
                }
                EditorGUILayout.EndFadeGroup();

                EditorGUI.indentLevel = 0;
                if (conversation.timerOutcomes.triggerEvent.toggleBool)
                {
                    EditorGUILayout.LabelField("Trigger Next Conversation?", "Can't Trigger Next Conversation if Triggering Event!", r);
                }
                else
                {
                    conversation.timerTriggerNextConversation = EditorGUILayout.Toggle("Trigger Next Conversation?", conversation.timerTriggerNextConversation);
                }
            }

            EditorGUI.indentLevel = 0;

            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.EndFadeGroup();
            conversationCounter++;
        }
    }

    private void EventTriggerSelection(Outcomes outcomes)
    {
        outcomes.eventObject = EditorGUILayout.ObjectField("Event Script Object:", outcomes.eventObject, typeof(GameObject), true) as GameObject;
        if (outcomes.eventObject == null)
        {
            EditorGUILayout.LabelField("Event Script:", "No Event Object Selected!", r);
            EditorGUILayout.LabelField("Event Script Method:", "No Event Script Selected!", r);
        }
        else
        {
            if (outcomes.eventObject.GetComponent<MonoBehaviour>() == null)
            {
                EditorGUILayout.LabelField("Event Script:", "No Scripts Attached To Event Object!", r);
                EditorGUILayout.LabelField("Event Script Method:", "No Event Script Selected!", r);
            }
            else
            {
                if (outcomes.eventScript == null)
                {
                    outcomes.eventScript = outcomes.eventObject.GetComponent<MonoBehaviour>();
                }
                MonoBehaviour[] scripts = outcomes.eventObject.GetComponents<MonoBehaviour>();
                if (scripts.ToList().IndexOf(outcomes.eventScript) < 0)
                {
                    outcomes.eventScript = scripts[0];
                }
                string[] scriptNames = scripts.Select(s => s.GetType().Name).ToArray();
                int scriptIndex = EditorGUILayout.Popup("Event Script:", scripts.ToList().IndexOf(outcomes.eventScript), scriptNames);
                if (scriptIndex >= 0)
                {
                    outcomes.eventScript = scripts[scriptIndex];
                }
                if (outcomes.eventScript == null)
                {
                    EditorGUILayout.LabelField("Event Script Method:", "No Event Script Selected!", r);
                }
                else
                {
                    MethodInfo[] methods = outcomes.eventScript.GetType().GetMethods().Where(m => m.DeclaringType == outcomes.eventScript.GetType()).ToArray();
                    if (methods.Length == 0)
                    {
                        EditorGUILayout.LabelField("Event Script Method:", "Selected Event Script Doesn't Have Any Methods!", r);
                    }
                    else
                    {
                        string[] methodNames = methods.Select(m => m.Name).ToArray();
                        if (methodNames.ToList().IndexOf(outcomes.eventMethod) < 0 || outcomes.eventMethod == null)
                        {
                            outcomes.eventMethod = methodNames[0];
                        }
                        int methodIndex = EditorGUILayout.Popup("Event Script Method:", methodNames.ToList().IndexOf(outcomes.eventMethod), methodNames);
                        if (methodIndex >= 0)
                        {
                            outcomes.eventMethod = methodNames[methodIndex];
                        }
                    }
                }
            }
        }
    }

    private string checkFlags(string flag)
    {
        bool match = false;
        foreach (string name in flagNames)
        {
            if (flag == name)
            {
                match = true;
            }
        }
        if (match)
        {
            return flag;
        }
        else
        {
            return flagNames[0];
        }
    }
}
