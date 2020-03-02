using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomInputs
{
    public static Dictionary<string, InputValues> inputDictionary = new Dictionary<string, InputValues>
    {
        {
            "Right",
            new InputValues
            (
                KeyCode.RightArrow,
                KeyCode.D
            )
        },
        {
            "Left",
            new InputValues
            (
                KeyCode.LeftArrow,
                KeyCode.A
            )
        },
        {
            "Up",
            new InputValues
            (
                KeyCode.UpArrow,
                KeyCode.W
            )
        },
        {
            "Down",
            new InputValues
            (
                KeyCode.DownArrow,
                KeyCode.S
            )
        },
        {
            "Jump",
            new InputValues
            (
                KeyCode.Space,
                KeyCode.Space
            )
        },
        {
            "Interact",
            new InputValues
            (
                KeyCode.Return,
                KeyCode.E
            )
        },
    };
}

[System.Serializable]
public class InputValues
{
    public KeyCode defaultKey;
    public KeyCode customKey;
    //public KeyCode defaultButton;
    //public KeyCode customButton;


    public InputValues(KeyCode defaultKey, KeyCode customKey/*, KeyCode defaultButton, KeyCode customButton*/)
    {
        this.defaultKey = defaultKey;
        this.customKey = customKey;
        //this.defaultButton = defaultButton;
        //this.customButton = customButton;
    }
    
    /*
    public InputValues(InputValues inputValues)
    {
        this.defaultKey = inputValues.defaultKey;
        this.customKey = inputValues.customKey;
        //this.defaultButton = inputValues.defaultButton;
        //this.customButton = inputValues.customButton;
    }
    */
}
