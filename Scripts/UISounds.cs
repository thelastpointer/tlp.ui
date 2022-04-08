using System.Collections.Generic;
using UnityEngine;

namespace TLP.UI
{
    public class UISounds : MonoBehaviour
    {
#pragma warning disable 649

        private DefaultSoundAssignment[] defaultSounds;

#pragma warning restore 649

        [System.Serializable]
        public struct DefaultSoundAssignment
        {
            public UISoundType Sound;
            public AudioClip Clip;
        }

        public static void Play(UISound sound)
        {
            if (sound.CustomSound != null)
                instance.audio.PlayOneShot(sound.CustomSound);
        }

        public static void Play(UISoundType sound)
        {
            if (instance.defaultSoundsDict.TryGetValue(sound, out var clip))
                instance.audio.PlayOneShot(clip);
        }

        public static UISounds Instance
        {
            get
            {
                if (instance != null)
                    return instance;

                // No instance; add to WindowManager
                if (WindowManager.Instance != null)
                {
                    instance = WindowManager.Instance.gameObject.AddComponent<UISounds>();
                    instance.Setup(instance.gameObject.AddComponent<AudioSource>());
                }
                // No WindowManager either, create a new GameObject
                else
                {
                    var go = new GameObject("UISounds");
                    instance = go.AddComponent<UISounds>();
                    instance.Setup(instance.gameObject.AddComponent<AudioSource>());
                }

                return instance;
            }
            private set { instance = value; }
        }

        private static UISounds instance;

        private void Setup(AudioSource source)
        {
            audio = source;

            defaultSoundsDict = new Dictionary<UISoundType, AudioClip>();
            foreach (var sound in defaultSounds)
            {
                if (sound.Clip != null)
                    defaultSoundsDict[sound.Sound] = sound.Clip;
            }
        }

        private new AudioSource audio;
        private Dictionary<UISoundType, AudioClip> defaultSoundsDict;
    }

    [System.Serializable]
    public class UISound
    {
        public UISoundType Sound;
        public AudioClip CustomSound;

        public UISound(UISoundType sound)
        {
            Sound = sound;
        }

        public UISound()
        {
            Sound = UISoundType.None;
        }
    }

    public enum UISoundType
    {
        None,

        Click,
        Hover,

        Confirm,
        Cancel,

        Error,
        Success
    }
}