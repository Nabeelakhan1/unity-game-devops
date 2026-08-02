using System;
using CandyClicker.Core;
using CandyClicker.UI;
using UnityEngine;

namespace CandyClicker.Gameplay
{
    /// <summary>
    /// A candy that drifts across the screen and is worth more than a normal tap.
    /// Despawns on tap or when it passes the exit X. Pooled — never destroyed.
    /// Dependencies: Collider2D on the clickable layer, SpriteRenderer.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class BonusCandy : MonoBehaviour, IClickable
    {
        [Header("Settings")]
        [SerializeField, Min(1)] private int _points = 2;
        [SerializeField] private float _spinSpeed = 90f;

        private SpriteRenderer _renderer;
        private ScoreSystem _scoreSystem;
        private AudioService _audio;
        private FloatingTextSpawner _textSpawner;
        private AudioClip _clip;
        private ParticleSystem _particles;

        private Action<BonusCandy> _onDespawn;
        private float _speed;
        private float _exitX;

        private void Awake() => _renderer = GetComponent<SpriteRenderer>();

        /// <summary>Wires dependencies once, when the pool creates or reuses the instance.</summary>
        public void Launch(
            Vector3 startPosition,
            float speed,
            float exitX,
            Color color,
            ScoreSystem scoreSystem,
            AudioService audio,
            AudioClip clip,
            FloatingTextSpawner textSpawner,
            ParticleSystem particles,
            Action<BonusCandy> onDespawn)
        {
            transform.position = startPosition;
            transform.rotation = Quaternion.identity;

            _speed = speed;
            _exitX = exitX;
            _renderer.color = color;

            _scoreSystem = scoreSystem;
            _audio = audio;
            _clip = clip;
            _textSpawner = textSpawner;
            _particles = particles;
            _onDespawn = onDespawn;
        }

        private void Update()
        {
            transform.position += Vector3.right * (_speed * Time.deltaTime);
            transform.Rotate(0f, 0f, _spinSpeed * Time.deltaTime);

            if (transform.position.x > _exitX) Despawn();
        }

        public void OnClicked(Vector2 worldPoint)
        {
            _scoreSystem?.Add(_points);

            if (_audio != null) _audio.Play(_clip);
            if (_textSpawner != null) _textSpawner.Spawn(worldPoint, $"+{_points}", _renderer.color);

            if (_particles != null)
            {
                _particles.transform.position = worldPoint;
                _particles.Play();
            }

            Despawn();
        }

        private void Despawn() => _onDespawn?.Invoke(this);
    }
}
