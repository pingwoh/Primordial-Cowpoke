﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueFlagDatabase
{

    public static Dictionary<string, bool> dialogueFlags = new Dictionary<string, bool>()
    {
        {
            "None",
            true
        },
        {
            "Red",
            false
        },
        {
            "NextConvo",
            false
        },
        {
            "Placeholder",
            false
        },
    };

}