using System;
using UnityEngine;

namespace SW
{
    public class AudioManager : MonoBehaviour
    {
        private AudioSource _active;

        public AudioSource active
        {
            get
            {
                if (_active == null)
                {
                    _active = gameObject.AddComponent<AudioSource>();
                    _active.loop = true;
                    _active.playOnAwake = false;
                }

                return _active;
            }
        }

        public float fadeTime = 0.5f;


        public void Play(AudioClip clip = null)
        {
            if (active.clip == clip) return;
            FadeAudio(active.volume, 0, () =>
            {
                active.Stop();
                if (clip != null)
                {
                    active.clip = clip;
                    active.volume = 0;
                    active.Play();
                    FadeAudio(0, 1);
                }
            });
        }

        void FadeAudio(float from, float to, Action onComplete = null)
        {
            Tween
                .Value(fadeTime)
                .From(from)
                .To(to)
                .OnUpdate(f => { active.volume = f; })
                .OnComplete(onComplete)
                .Start();
        }
    }
}