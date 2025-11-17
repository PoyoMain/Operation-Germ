using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXPlayer : MonoBehaviour
{
    [Header("Clips")]
    [SerializeField] private AudioClip[] clips;

    private AudioSource audioSource;

    private void Awake()
    {
        TryGetComponent(out audioSource);
    }

    public void Play(bool looping = false)
    {
        if (clips == null) return;
        if (clips.Length == 0) return;

        AudioClip clipToPlay = clips[Random.Range(0, clips.Length)];

        if (audioSource == null)
        {
            if (!TryGetComponent(out audioSource)) return;
            if (!audioSource.isActiveAndEnabled) return;
        }

        if (clipToPlay == null) return;

        if (!looping) audioSource.PlayOneShot(clipToPlay);
        else
        {
            audioSource.clip = clipToPlay;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    public void PlayClipAtPoint()
    {
        if (clips == null) return;
        if (clips.Length == 0) return;

        AudioClip clipToPlay = clips[Random.Range(0, clips.Length)];

        if (clipToPlay != null) AudioSource.PlayClipAtPoint(clipToPlay, transform.position, 1);
    }

    public void Stop()
    {
        if (audioSource.clip == null) return;

        audioSource.Stop();
        audioSource.clip = null;
        audioSource.loop = false;
    }
}
