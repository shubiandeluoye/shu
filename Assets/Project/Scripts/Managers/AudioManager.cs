using UnityEngine;
using System.Collections.Generic;
using Core;

namespace Core.Managers
{
    public class AudioManager : Singleton<AudioManager>
    {
        [System.Serializable]
        public class Sound
        {
            public string name;
            public AudioClip clip;
            [Range(0f, 1f)]
            public float volume = 1f;
            [Range(0.1f, 3f)]
            public float pitch = 1f;
            public bool loop = false;

            [HideInInspector]
            public AudioSource source;
        }

        public Sound[] sounds;
        private Dictionary<string, Sound> soundDictionary;

        protected override void Awake()
        {
            base.Awake();
            InitializeAudio();
        }

        private void InitializeAudio()
        {
            soundDictionary = new Dictionary<string, Sound>();

            foreach (Sound s in sounds)
            {
                GameObject soundObject = new GameObject($"Sound_{s.name}");
                soundObject.transform.SetParent(transform);
                
                s.source = soundObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = s.loop;

                soundDictionary[s.name] = s;
            }
        }

        public void PlaySound(string name)
        {
            if (soundDictionary.TryGetValue(name, out Sound sound))
            {
                sound.source.Play();
            }
            else
            {
                Debug.LogWarning($"[AudioManager] 未找到音频 {name}");
            }
        }

        public void StopSound(string name)
        {
            if (soundDictionary.TryGetValue(name, out Sound sound))
            {
                sound.source.Stop();
            }
        }

        public void SetVolume(string name, float volume)
        {
            if (soundDictionary.TryGetValue(name, out Sound sound))
            {
                sound.source.volume = Mathf.Clamp01(volume);
            }
        }

        public void PauseAll()
        {
            foreach (var sound in soundDictionary.Values)
            {
                sound.source.Pause();
            }
        }

        public void ResumeAll()
        {
            foreach (var sound in soundDictionary.Values)
            {
                sound.source.UnPause();
            }
        }

        public void StopAll()
        {
            foreach (var sound in soundDictionary.Values)
            {
                sound.source.Stop();
            }
        }
    }
}
