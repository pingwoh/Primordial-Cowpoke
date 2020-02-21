using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTriggerDatabase
{

    public static Dictionary<string, bool> dialogueTriggers = new Dictionary<string, bool>()
    {
        {
            "None",
            true
        },
        {
            "Mad",
            false
        },
        {
            "Happy",
            false
        },
        {
            "Placeholder",
            false
        },
    };

}
