using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FancyToolkit
{
    public class AudioRandomMusic : MonoBehaviour
    {
        [SerializeField] List<AudioClip> options;

        void Start()
        {
            var source = GetComponent<AudioSource>();
            source.Play();
        }
    }
}
