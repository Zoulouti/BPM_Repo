using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    
    protected AudioClip GetAudioFromArray(AudioClip[] audios)
    {
        if (audios.Length == 0)
        {
            Debug.LogWarning("No audioClip in the array!");
            return null;
        }
        
        return audios[Random.Range(0, audios.Length)];
    }

    protected float GetRandomValue(float baseValue, float randomizerRange)
    {
		return baseValue - Random.Range(-randomizerRange, randomizerRange);
    }

    protected void StartSoundFromArray(AudioSource audioSource, AudioClip[] audioClip, float volume, float volumeRandomizer, float pitch, float pitchRandomizer)
    {
        if (audioClip.Length == 0)
        {
            Debug.LogWarning("No audioClip in the array!");
            return;
        }

        AudioClip sound = GetAudioFromArray(audioClip);
        float volumeValue = GetRandomValue(volume, volumeRandomizer);
        float pitchValue = GetRandomValue(pitch, pitchRandomizer);

        audioSource.volume = volumeValue;
        audioSource.pitch = pitchValue;

        audioSource.PlayOneShot(sound);
    }
    
}
