using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public bool showConversation;
    public string conversationName;
    public string requiredTrigger;
    public bool hasAudio;
    public List<Statement> statements;
    public int closingAnimation;
    public string setTrigger;
    public bool hasResponse;
    public List<Response> responses;
    public bool triggerNextConversation;
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
    public string setTrigger;
    public string responseText;
}