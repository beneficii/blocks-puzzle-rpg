using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FancyToolkit
{
    [DefaultExecutionOrder(-14)]
    public class AudioCtrl : MonoBehaviour
    {
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
        }

        public void Play(AudioClip clip, float volumeScale = 1f)
        {
            if (!clip) return;
            effects.PlayOneShot(clip, volumeScale);
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
    }
}
