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

        public List<AudioClip> randomMusic = new();

        public AudioClip clipTakeCard;
        public AudioClip clipDrawPile;
        public AudioClip clipUseCard;
        public AudioClip clipCollectMoney;
        public AudioClip clipPayRent;
        public AudioClip clipBuyCards;
        public AudioClip clipSellCards;
        public AudioClip clipGameOver;
        public AudioClip clipVictory;

        public AudioClip clipPop;
        public AudioClip clipArmor;

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
            var randClip = randomMusic.Rand();
            if (randClip != null)
            {
                music.clip = randClip;
            }

            music.Play();
        }

        void SavePrefsOnClose()
        {
            PlayerPrefs.SetFloat(prefsKeyMusicVolume, VolumeMusic);
            PlayerPrefs.SetFloat(prefsKeySoundVolume, VolumeSound);
            //PlayerPrefs.Save();
        }

        private void OnDestroy()
        {
            SavePrefsOnClose();
        }

        public void Play(AudioClip clip)
        {
            if (!clip) return;
            effects.PlayOneShot(clip, VolumeSound);
        }

        public void PlayRandomPitch(AudioClip clip, float range)
        {
            if (!clip) return;

            var newSource = gameObject.AddComponent<AudioSource>();
            newSource.pitch = 1f + Random.Range(-range, range);
            newSource.PlayOneShot(clip, VolumeSound);
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
        public static void PlayNow(this AudioClip clip)
        {
            AudioCtrl.current?.Play(clip);
        }

        public static void PlayWithRandomPitch(this AudioClip clip, float range = 0.2f)
        {
            AudioCtrl.current?.PlayRandomPitch(clip, range);
        }
    }
}
