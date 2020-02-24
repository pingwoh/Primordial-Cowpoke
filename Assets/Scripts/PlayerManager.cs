using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    Dictionary<string, bool> dialogueFlagDatabase = DialogueFlagDatabase.dialogueFlags;

    public Dictionary<string, bool> playerFlags = new Dictionary<string, bool>();

    void Start()
    {
        foreach (KeyValuePair<string, bool> entry in dialogueFlagDatabase)
        {
            bool flagBool;
            if (PlayerPrefs.GetInt(entry.Key) > 0)
            {
                flagBool = true;
            }
            else
            {
                flagBool = false;
            }
            playerFlags.Add(entry.Key, flagBool);
        }
        playerFlags["None"] = true;
    }

    void Update()
    {
        
    }

    void Save()
    {
        foreach(KeyValuePair<string, bool> entry in playerFlags)
        {
            int flagBool;
            if (entry.Value)
            {
                flagBool = 1;
            }
            else
            {
                flagBool = 0;
            }
            PlayerPrefs.SetInt(entry.Key, flagBool);
        }
    }
}
