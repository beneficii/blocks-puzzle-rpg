using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FancyToolkit
{
    [DefaultExecutionOrder(-14)]
    public class AudioCtrl : MonoBehaviour
    {
        const string prefsKeyMusicVolume = "audio_musicVolume";
        const string prefsKeySoundVolume = "audio_soundVolume";

        public static AudioCtrl current;

        public AudioSource music;
        public AudioSource effects;

        public AudioClip clipTakeCard;
        public AudioClip clipDrawPile;
        public AudioClip clipUseCard;
        public AudioClip clipCollectMoney;
        public AudioClip clipPayRent;
        public AudioClip clipBuyCards;
        public AudioClip clipSellCards;
        public AudioClip clipGameOver;
        public AudioClip clipVictory;

        public float VolumeMusic
        {
            get => music.volume;
            set => music.volume = value;
        }

        public float VolumeSound
        {
            get => effects.volume;
            set => effects.volume = value;
        }

        private void Awake()
        {
            /*
            if(current)
            {
                current.SetMusic(music.clip);
                Destroy(gameObject);
                return;
            }
            */
            current = this;
            //DontDestroyOnLoad(gameObject);

            VolumeMusic = PlayerPrefs.GetFloat(prefsKeyMusicVolume, 1f);
            VolumeSound = PlayerPrefs.GetFloat(prefsKeySoundVolume, 1f);
        }

        void SavePrefsOnClose()
        {
            PlayerPrefs.SetFloat(prefsKeyMusicVolume, VolumeMusic);
            PlayerPrefs.SetFloat(prefsKeySoundVolume, VolumeSound);
        }

        private void OnDestroy()
        {
            SavePrefsOnClose();
        }

        public void Play(AudioClip clip, float volumeScale = 1f)
        {
            if (!clip) return;
            effects.PlayOneShot(clip, volumeScale);
        }

        public void PlayRandomPitch(AudioClip clip, float range)
        {
            if (!clip) return;

            var newSource = gameObject.AddComponent<AudioSource>();
            newSource.pitch = 1f + Random.Range(-range, range);
            newSource.PlayOneShot(clip);
            Destroy(newSource, clip.length + 0.1f);
        }

        public void GameOver()
        {
            music.Stop();
            clipGameOver.PlayNow();
        }

        public void SetMusic(AudioClip clip)
        {
            if(music.clip == clip) return;

            music.clip = clip;
            music.Play();
        }

        void Reset()
        {
            music = gameObject.AddComponent<AudioSource>();
            effects = gameObject.AddComponent<AudioSource>();
            effects.playOnAwake = false;
            music.loop = true;
        }
    }

    public static class AudioExtensions
    {
        public static void PlayNow(this AudioClip clip, float volume = 1f)
        {
            AudioCtrl.current?.Play(clip);
        }

        public static void PlayWithRandomPitch(this AudioClip clip, float range = 0.2f)
        {
            AudioCtrl.current?.PlayRandomPitch(clip, range);
        }
    }
}
