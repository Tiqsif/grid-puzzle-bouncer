using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntryAudio : MonoBehaviour
{
    public AudioPackSO[] landAudioPacks;
    

    public void PlayLandAudio() // animation event will call this
    {
        foreach (AudioClip clip in AudioPackManager.GetRandomClipFromEachPack(landAudioPacks))
        {
            AudioManager.Instance.KillSFX(clip);
            AudioManager.Instance.PlaySFX(clip);
        }
    }

}
