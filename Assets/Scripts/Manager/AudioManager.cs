using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : SingletonBase<AudioManager>
{        
    public void PlaySound(string soundName)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        AudioClip audioClip = Resources.Load("Sounds/" + soundName) as AudioClip;
        audioSource.clip = audioClip;
        audioSource.SetScheduledEndTime(AudioSettings.dspTime + audioClip.length);
        audioSource.PlayScheduled(AudioSettings.dspTime);

        StartCoroutine(RemoveAudioSource(audioSource));
    }
    private IEnumerator RemoveAudioSource(AudioSource audioSource)
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        Destroy(audioSource);
    }
}
