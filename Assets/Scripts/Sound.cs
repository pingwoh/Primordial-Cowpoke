using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
//so that you can put everything in in the editor!
// names, the clip u want, the volume, and if it loops or not!
public class Sound
{

    public string name;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume;
    public bool loop;
    [HideInInspector] public AudioSource source;
}
