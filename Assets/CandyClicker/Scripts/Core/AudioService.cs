using UnityEngine;

namespace CandyClicker.Core
{
    /// <summary>
    /// Thin SFX wrapper over a single AudioSource with slight pitch variation so
    /// rapid clicks don't sound robotic. Dependencies: an AudioSource on the same object.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioService : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField, Range(0f, 0.3f)] private float _pitchVariation = 0.08f;

        private AudioSource _source;

        private void Awake()
        {
            _source = GetComponent<AudioSource>();
            _source.playOnAwake = false;
        }

        /// <summary>Plays a one-shot clip. Null clips are ignored.</summary>
        public void Play(AudioClip clip, float volume = 1f)
        {
            if (clip == null) return;

            _source.pitch = 1f + Random.Range(-_pitchVariation, _pitchVariation);
            _source.PlayOneShot(clip, volume);
        }
    }
}
