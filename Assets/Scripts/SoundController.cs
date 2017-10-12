using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour {

    public enum Sounds
    {
        TEST_SOUND0 = 0,
        TEST_SOUND1 = 1
    }

    [SerializeField]
    private AudioClip[] soundArray;

    public AudioClip AudioClipCreate(Sounds type)
    {
        int num = (int)type;
        AudioClip clip;
        clip = soundArray[num];
        return clip;
    }
}
