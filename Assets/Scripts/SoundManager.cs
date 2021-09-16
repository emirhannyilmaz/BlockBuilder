using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    public AudioClip[] sounds;

    private void Awake() {
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySound(string name, float volumeScale) {
        AudioClip audioClip = Array.Find(sounds, sound => sound.name == name);
        GetComponent<AudioSource>().PlayOneShot(audioClip, volumeScale);
    }

    public void PlaySound(AudioClip audioClip, float volumeScale) {
        GetComponent<AudioSource>().PlayOneShot(audioClip, volumeScale);
    }
}
