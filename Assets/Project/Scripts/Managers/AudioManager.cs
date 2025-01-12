using UnityEngine;
using System.Collections.Generic;
using Core;

namespace Core.Managers
{
    /// <summary>
    /// 音频管理器
    /// 处理游戏音频的播放和管理
    /// </summary>
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

        /// <summary>
        /// 初始化音频系统
        /// </summary>
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

        /// <summary>
        /// 播放指定名称的音频
        /// </summary>
        /// <param name="name">音频名称</param>
        public void PlaySound(string name)
        {
            if (soundDictionary.TryGetValue(name, out Sound sound))
            {
                sound.source.Play();
            }
            else
            {
                Debug.LogWarning($"[音频系统] 未找到音频 {name}！");
            }
        }

        /// <summary>
        /// 停止指定名称的音频
        /// </summary>
        /// <param name="name">音频名称</param>
        public void StopSound(string name)
        {
            if (soundDictionary.TryGetValue(name, out Sound sound))
            {
                sound.source.Stop();
            }
        }

        /// <summary>
        /// 设置指定音频的音量
        /// </summary>
        /// <param name="name">音频名称</param>
        /// <param name="volume">音量值（0-1）</param>
        public void SetVolume(string name, float volume)
        {
            if (soundDictionary.TryGetValue(name, out Sound sound))
            {
                sound.source.volume = Mathf.Clamp01(volume);
            }
        }

        /// <summary>
        /// 暂停所有音频
        /// </summary>
        public void PauseAll()
        {
            foreach (var sound in soundDictionary.Values)
            {
                sound.source.Pause();
            }
        }

        /// <summary>
        /// 恢复所有音频
        /// </summary>
        public void ResumeAll()
        {
            foreach (var sound in soundDictionary.Values)
            {
                sound.source.UnPause();
            }
        }

        /// <summary>
        /// 停止所有音频
        /// </summary>
        public void StopAll()
        {
            foreach (var sound in soundDictionary.Values)
            {
                sound.source.Stop();
            }
        }
    }
}
