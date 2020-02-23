using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    //made this general sounds to have one list of sounds for dialogue and one list for everything else
    public Sound[] generalSounds; //create an array of sounds to play 

    [HideInInspector]
    public Queue<Sound> dialogueSounds = new Queue<Sound>(); //create an array of sounds for dialogue
    private AudioSource dialogueSource;

    public static AudioManager Instance; //create an instance of the audio manager


    private void Awake()
    {
        if (Instance == null) //if there is none
            Instance = this; //set it equal to 'this'; note: hard to explain
        else
        {
            Destroy((gameObject)); //destroy the object
            return; //exit function
        }
        DontDestroyOnLoad(gameObject); //keep the manager from scene to scene

        foreach (Sound s in generalSounds) //populate the array w/ sound clips
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.loop = s.loop;
        }
        dialogueSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlayAudio(string SoundName)
    {
        //find the sound specified and play it back
        Sound s = Array.Find(generalSounds, sound => sound.name == SoundName);
        if (s == null) return;
        s.source.Play();
    }

    public void Stop(string SoundName)
    {
        //find the sound specified and stop it
        Sound s = Array.Find(generalSounds, sound => sound.name == SoundName);
        s.source.Stop();
    }

    public void PlayDialogueAudio()
    {
        Sound currentSound = dialogueSounds.Dequeue();
        if(currentSound == null)
        {
            return;
        }
        dialogueSource.clip = currentSound.clip;
        dialogueSource.volume = currentSound.volume;
        dialogueSource.loop = currentSound.loop;
        dialogueSource.Play();
    }

    public void StopDialogueAudio()
    {
        dialogueSource.Stop();
    }

}
