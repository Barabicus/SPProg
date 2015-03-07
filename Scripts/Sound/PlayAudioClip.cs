using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class PlayAudioClip : MonoBehaviour {

    AudioSource audio;

    public AudioClip audioClip;
    public float playSoundfreq;

    private Timer timer;

    void Start()
    {
        audio = GetComponent<AudioSource>();
        timer = new Timer(playSoundfreq);
    }

    void Update()
    {
        if (timer.CanTickAndReset())
        {
            audio.PlayOneShot(audioClip);
        }
    }


}
