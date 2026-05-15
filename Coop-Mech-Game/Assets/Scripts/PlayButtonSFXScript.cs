using System;
using UnityEngine;

// Require AudioSource component on the same GameObject
[RequireComponent(typeof(AudioSource))]
public class PlayButtonSFXScript : MonoBehaviour
{
    private AudioSource source;
    [SerializeField] private AudioClip hoverClip;
    [SerializeField] private AudioClip clickClip;

    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void PlayHoverClip()
    {
        source.PlayOneShot(hoverClip);
    }

    public void PlayClickClip()
    {
        source.PlayOneShot(clickClip);
    }
}
