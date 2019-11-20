using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Question : MonoBehaviour
{
    public GameObject panelWhenCorrect;
    public Waypoint   waypointToDisable;
    public AudioClip  rightAnswerSound;
    public AudioClip  wrongAnswerSound;

    private AudioSource source;

    protected void Start()
    {
        source = gameObject.AddComponent<AudioSource>();
    }

    protected void PlayCorrectSound()
    {
        source.Stop();
        if (!rightAnswerSound) return;

        source.PlayOneShot(rightAnswerSound);
    }

    protected void PlayWrongSound()
    {
        source.Stop();
        if (!wrongAnswerSound) return;

        source.PlayOneShot(wrongAnswerSound);
    }

    public delegate void Correct();

    public virtual void Init(Correct onCorrect) { }
}
