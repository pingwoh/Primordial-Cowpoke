using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds; //create an array of sounds to play

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

        foreach (Sound s in sounds) //populate the array w/ sound clips
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.loop = s.loop;
        }
    }

    public void PlayAudio(string SoundName)
    {
        //find the sound specified and play it back
        Sound s = Array.Find(sounds, sound => sound.name == SoundName);
        if (s == null) return;
        s.source.Play();
    }

    public void Stop(string SoundName)
    {
        //find the sound specified and stop it
        Sound s = Array.Find(sounds, sound => sound.name == SoundName);
        s.source.Stop();
    }

}
