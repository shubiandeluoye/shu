using UnityEngine;

namespace SkillModule.Utils
{
    public static class SkillAudioUtils
    {
        public static void PlaySkillSound(AudioClip clip, Vector3 position, float volume = 1f)
        {
            if (clip != null)
            {
                AudioSource.PlayClipAtPoint(clip, position, volume);
            }
        }

        public static AudioSource CreateAudioSource(GameObject obj, AudioClip clip, float volume = 1f, bool playOnAwake = false, bool loop = false)
        {
            var audioSource = SkillUtils.SafeGetComponent<AudioSource>(obj);
            if (audioSource == null)
            {
                audioSource = obj.AddComponent<AudioSource>();
            }

            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.playOnAwake = playOnAwake;
            audioSource.loop = loop;

            return audioSource;
        }
    }
} 