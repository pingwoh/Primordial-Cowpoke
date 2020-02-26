using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.AnimatedValues;
#endif

[System.Serializable]
public class Dialogue
{
    public bool customNameToggle;
    public string characterName;
    public List<Conversation> conversations;
}

[System.Serializable]
public class Conversation
{
    [SerializeField]
    public DisplayToggle showConversation;
    public string conversationName;
    public string requiredFlag;
    public string skipFlag;
    public bool hasAudio;
    public List<Statement> statements;
    public int closingAnimation;
    public Outcomes outcomes;
    public DisplayToggle hasResponse;
    public List<Response> responses;
    public DisplayToggle timerToggle;
    public float timerValue;
    public bool timerTriggerNextConversation;
    public Outcomes timerOutcomes;
}

[System.Serializable]
public class Statement
{
    public int animationSelection;
    public bool hasCustomAudio;
    public Sound audioProperties;
    public string statementText;
}

[System.Serializable]
public class Response
{
    public Outcomes outcomes;
    public string responseText;
    public bool triggerNextConversation;
}

[System.Serializable]
public class Outcomes
{
    public string setFlag;
    public bool queueNextConversation;
    public DisplayToggle triggerEvent;
    public GameObject eventObject;
    public MonoBehaviour eventScript;
    public string eventMethod;
}

[System.Serializable]
public class DisplayToggle
{
    public bool toggleBool;
#if UNITY_EDITOR
    public AnimBool toggleAnim;
#endif
}