using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSFXManager : MonoBehaviour
{
    [SerializeField] private List<AudioSource> shootAudioSources;
    [SerializeField] private List<AudioSource> punchAudioSources;

    private void Start()
    {
        shootAudioSources = new List<AudioSource>(GetComponents<AudioSource>());
        punchAudioSources = new List<AudioSource>(GetComponents<AudioSource>());
    }

    public void PlayShootSound()
    {
        if (shootAudioSources.Count == 0) return;

        foreach (AudioSource source in shootAudioSources)
        {
            if (!source.isPlaying)
            {
                source.Play();
            }
            // else if (source.isPlaying)
            // {
            //     source.Stop();
            //     source.Play();
            // }
        }
    }

    public void PlayPunchSound()
    {
        if (punchAudioSources.Count == 0) return;

        foreach (AudioSource source in punchAudioSources)
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
