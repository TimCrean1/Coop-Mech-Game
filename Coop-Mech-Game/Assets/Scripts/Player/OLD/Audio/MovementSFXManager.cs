using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSFXManager : MonoBehaviour
{
    [SerializeField] private List<AudioSource> audioSources;

    private void Start()
    {
        audioSources = new List<AudioSource>(GetComponents<AudioSource>());
    }

    public void PlayFootstepSound()
    {
        if (audioSources.Count == 0) return;

        foreach (AudioSource source in audioSources)
        {
            if (!source.isPlaying)
            {
                source.Play();
            }
            else if (source.isPlaying)
            {
                source.Stop();
                source.Play();
            }
        }
    }
}
