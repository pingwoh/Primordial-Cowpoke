using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{

    public Text nameText;
    public Text enterText;
    public Text dialogueText;
    //public Text preLoadText;
    public Image dialogueBox;

    public Text responsesHeaderText;

    public GameObject responseScrollContent;
    public GameObject responseButtonPrefab;
    private GameObject[] responseButtons;

    //private Vector2 currentBoxSize;
    //private Vector2 futureBoxSize;

    //private Animator character;
    //private string[] animationTriggers;
    //private Queue<int> animationSelections;
    private Queue<string> sentences;

    void Start()
    {
        //animationSelections = new Queue<int>();
        sentences = new Queue<string>();
    }

    public void StartDialogue(string NPCName, string[] newSentences)
    {
        StopAllCoroutines();

        dialogueBox.enabled = true;
        nameText.enabled = false;
        enterText.enabled = false;
        nameText.text = NPCName;
        enterText.text = "Press N";
        sentences.Clear();

        foreach (string sentence in newSentences)
        {
            sentences.Enqueue(sentence);
        }

        ContinueDialogue();
    }

    public bool ContinueDialogue()
    {
        if (sentences.Count == 0)
        {
            return false;
        }
        string sentence = sentences.Dequeue();
        StopAllCoroutines();

        StartCoroutine(TypeSentence(sentence));
        return true;
    }

    IEnumerator TypeSentence(string sentence)
    {

        dialogueText.text = "";
        //enterText.GetComponent<Animator>().SetTrigger("Off");
        //yield return null;

        if (nameText.enabled == false)
        {
            nameText.enabled = true;
            enterText.enabled = true;
        }
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
        //enterText.GetComponent<Animator>().SetTrigger("Flashing");
        yield break;
    }

    public void EndDialogue()
    {
        enterText.enabled = false;
        nameText.enabled = false;
        dialogueBox.enabled = false;
        dialogueText.text = "";
    }

    public void StartResponse(string[] responses, string[] triggers, NPCDialogue npcDialogue)
    {
        responsesHeaderText.enabled = true;
        enterText.enabled = false;
        nameText.enabled = false;
        dialogueText.text = "";

        responseButtons = new GameObject[responses.Length];
        for (int i = 0; i < responses.Length; i++)
        {
            responseButtons[i] = Instantiate(responseButtonPrefab, responseScrollContent.transform);
            responseButtons[i].GetComponentInChildren<Text>().text = responses[i];
            responseButtons[i].GetComponent<ResponseButton>().playerTrigger = triggers[i];
            responseButtons[i].GetComponent<ResponseButton>().npcDialogue = npcDialogue;
        }
    }

    public bool EndResponse()
    {
        responsesHeaderText.enabled = false;
        foreach (GameObject button in responseButtons)
        {
            Destroy(button);
        }
        return false;
    }
}
