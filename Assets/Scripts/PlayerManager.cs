using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    Dictionary<string, bool> dialogueTriggerDatabase = DialogueTriggerDatabase.dialogueTriggers;

    public Dictionary<string, bool> playerTriggers = new Dictionary<string, bool>();

    void Start()
    {
        foreach (KeyValuePair<string, bool> entry in dialogueTriggerDatabase)
        {
            bool triggerBool;
            if (PlayerPrefs.GetInt(entry.Key) > 0)
            {
                triggerBool = true;
            }
            else
            {
                triggerBool = false;
            }
            playerTriggers.Add(entry.Key, triggerBool);
        }
        playerTriggers["None"] = true;
    }

    void Update()
    {
        
    }

    void Save()
    {
        foreach(KeyValuePair<string, bool> entry in playerTriggers)
        {
            int triggerBool;
            if (entry.Value)
            {
                triggerBool = 1;
            }
            else
            {
                triggerBool = 0;
            }
            PlayerPrefs.SetInt(entry.Key, triggerBool);
        }
    }
}
