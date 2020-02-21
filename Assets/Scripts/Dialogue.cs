using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public GameObject character;

    //remember to add conversationNames!!
    public List<string> conversationNames;
    public List<bool> showConversation;

    public List<AnimationSelectionsList> animationSelections;

    public List<string> triggerSelections;
    public List<string> triggerActivations;

    public bool customNameToggle;
    public string characterName;

    public bool useAudioFile;
    public string audioFileName;

    //[TextArea(3, 10)]
    public List<SentenceList> conversationList;

    public List<bool> responseToggles;
    public List<int> numberOfResponses;
    public List<ResponseList> responseList;
    public List<ResponseActiviationsList> responseActivationsList;

    public List<bool> hasAudio;
    public List<CustomAudioList> customAudioList;
    public List<AudioClipsList> audioClipsList;

    public List<bool> triggerNextConversation;

}
[System.Serializable]
public class SentenceList : IEnumerable
{
    [SerializeField]
    public List<string> sentences;
    public IEnumerator GetEnumerator()
    {
        foreach(string sent in sentences)
        {
            yield return sent;
        }
    }
    public string this[int i]
    {
        get
        {
            return this.sentences[i];
        }
        set
        {
            this.sentences[i] = value;
        }
    }
}

[System.Serializable]
public class ResponseList : IEnumerable
{
    [SerializeField]
    public List<string> responses;
    public IEnumerator GetEnumerator()
    {
        foreach(string resp in responses)
        {
            yield return responses;
        }
    }
    public string this[int i]
    {
        get
        {
            return this.responses[i];
        }
        set
        {
            this.responses[i] = value;
        }
    }
}

[System.Serializable]
public class ResponseActiviationsList : IEnumerable
{
    [SerializeField]
    public List<string> activations;
    public IEnumerator GetEnumerator()
    {
        foreach(string actv in activations)
        {
            yield return activations;
        }
    }
    public string this[int i]
    {
        get
        {
            return this.activations[i];
        }
        set
        {
            this.activations[i] = value;
        }
    }
}

[System.Serializable]
public class AnimationSelectionsList : IEnumerable
{
    [SerializeField]
    public List<int> animationSelections;
    public IEnumerator GetEnumerator()
    {
        foreach(int anim in animationSelections)
        {
            yield return anim;
        }
    }
    public int this[int i]
    {
        get
        {
            return this.animationSelections[i];
        }
        set
        {
            this.animationSelections[i] = value;
        }
    }
}

[System.Serializable]
public class CustomAudioList : IEnumerable
{
    [SerializeField]
    public List<bool> toggle;
    public IEnumerator GetEnumerator()
    {
        foreach (bool tog in toggle)
        {
            yield return toggle;
        }
    }
    public bool this[int i]
    {
        get
        {
            return this.toggle[i];
        }
        set
        {
            this.toggle[i] = value;
        }
    }
}

[System.Serializable]
public class AudioClipsList : IEnumerable
{
    [SerializeField]
    public List<AudioClip> clips;
    public IEnumerator GetEnumerator()
    {
        foreach (AudioClip clip in clips)
        {
            yield return clips;
        }
    }
    public AudioClip this[int i]
    {
        get
        {
            return this.clips[i];
        }
        set
        {
            this.clips[i] = value;
        }
    }
}