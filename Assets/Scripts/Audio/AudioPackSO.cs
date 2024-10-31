using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioPack", menuName = "AudioPack")]
public class AudioPackSO : ScriptableObject
{
    public AudioClip[] clips;
}

public static class AudioPackManager
{
    public static AudioClip[] GetRandomClipFromEachPack(AudioPackSO[] packArray)
    {
        AudioClip[] ret = new AudioClip[packArray.Length];
        for (int i = 0; i < packArray.Length; i++)
        {
            if (packArray[i].clips.Length > 0)
            {
                ret[i] = packArray[i].clips[Random.Range(0, packArray[i].clips.Length)];
            }
        }
        //Debug.Log(ret.Length);
        return ret;
    }
}