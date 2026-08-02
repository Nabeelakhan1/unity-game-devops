using CandyClicker.Core;
using CandyClicker.UI;
using UnityEngine;

namespace CandyClicker.Gameplay
{
    /// <summary>
    /// The big candy in the middle. On tap: score, sound, particles, floating text, punch scale.
    /// Dependencies: Collider2D on the clickable layer, ScoreSystem, AudioService, FloatingTextSpawner.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class MainCandy : MonoBehaviour, IClickable
    {
        [Header("References")]
        [SerializeField] private ScoreSystem _scoreSystem;
        [SerializeField] private AudioService _audio;
        [SerializeField] private FloatingTextSpawner _textSpawner;
        [SerializeField] private ParticleSystem _clickParticles;
        [SerializeField] private AudioClip _clickClip;

        [Header("Settings")]
        [SerializeField, Min(1)] private int _pointsPerClick = 1;
        [SerializeField] private Color _textColor = Color.white;
        [SerializeField] private float _punchScale = 0.12f;
        [SerializeField] private float _punchRecoverySpeed = 10f;

        private Vector3 _baseScale;

        private void Awake()
        {
            _baseScale = transform.localScale;

            if (_scoreSystem == null)
            {
                Debug.LogError($"[{nameof(MainCandy)}] Missing ScoreSystem reference.", this);
                enabled = false;
            }
        }

        private void Update()
        {
            transform.localScale = Vector3.Lerp(transform.localScale, _baseScale, _punchRecoverySpeed * Time.deltaTime);
        }

        public void OnClicked(Vector2 worldPoint)
        {
            _scoreSystem.Add(_pointsPerClick);

            if (_audio != null) _audio.Play(_clickClip);
            if (_textSpawner != null) _textSpawner.Spawn(worldPoint, $"+{_pointsPerClick}", _textColor);

            if (_clickParticles != null)
            {
                _clickParticles.transform.position = worldPoint;
                _clickParticles.Play();
            }

            transform.localScale = _baseScale * (1f + _punchScale);
        }
    }
}
