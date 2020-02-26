using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.AnimatedValues;

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
    public bool hasAudio;
    public List<Statement> statements;
    public int closingAnimation;
    public Outcomes outcomes;
    public DisplayToggle hasResponse;
    public List<Response> responses;
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
    public AnimBool toggleAnim;
}