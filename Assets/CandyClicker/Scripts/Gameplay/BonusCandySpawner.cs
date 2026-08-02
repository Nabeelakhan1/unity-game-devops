using CandyClicker.Core;
using CandyClicker.UI;
using CandyClicker.Utilities;
using UnityEngine;

namespace CandyClicker.Gameplay
{
    /// <summary>
    /// Releases a bonus candy from off-screen left every few seconds and recycles it
    /// once tapped or once it leaves on the right.
    /// Dependencies: BonusCandy prefab, camera (for screen edges), ScoreSystem, AudioService.
    /// </summary>
    public class BonusCandySpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private BonusCandy _prefab;
        [SerializeField] private Camera _camera;
        [SerializeField] private ScoreSystem _scoreSystem;
        [SerializeField] private AudioService _audio;
        [SerializeField] private FloatingTextSpawner _textSpawner;
        [SerializeField] private ParticleSystem _bonusParticles;
        [SerializeField] private AudioClip _bonusClip;

        [Header("Spawn Timing")]
        [SerializeField] private float _minInterval = 4f;
        [SerializeField] private float _maxInterval = 8f;

        [Header("Movement")]
        [SerializeField] private float _minSpeed = 2f;
        [SerializeField] private float _maxSpeed = 4f;
        [SerializeField] private float _edgePadding = 1.5f;
        [SerializeField, Range(0f, 0.5f)] private float _verticalMargin = 0.2f;

        [Header("Look")]
        [SerializeField]
        private Color[] _colors =
        {
            Color.blue, 
            Color.deepPink, 
           Color.darkOrange, 
            Color.yellowNice, 
        };

        private ObjectPool<BonusCandy> _pool;
        private float _timer;

        private void Awake()
        {
            if (_camera == null) _camera = Camera.main;

            if (_prefab == null || _camera == null || _scoreSystem == null)
            {
                Debug.LogError($"[{nameof(BonusCandySpawner)}] Missing references.", this);
                enabled = false;
                return;
            }

            _pool = new ObjectPool<BonusCandy>(_prefab, 4, transform);
            _timer = Random.Range(_minInterval, _maxInterval);
        }

        private void Update()
        {
            _timer -= Time.deltaTime;
            if (_timer > 0f) return;

            Spawn();
            _timer = Random.Range(_minInterval, _maxInterval);
        }

        private void Spawn()
        {
            float halfHeight = _camera.orthographicSize;
            float halfWidth = halfHeight * _camera.aspect;

            float usableHeight = halfHeight * (1f - _verticalMargin * 2f);
            float y = Random.Range(-usableHeight, usableHeight);

            Vector3 start = new Vector3(-halfWidth - _edgePadding, y, 0f);
            float exitX = halfWidth + _edgePadding;
            Color color = _colors.Length > 0 ? _colors[Random.Range(0, _colors.Length)] : Color.white;

            BonusCandy candy = _pool.Get();
            candy.Launch(
                start,
                Random.Range(_minSpeed, _maxSpeed),
                exitX,
                color,
                _scoreSystem,
                _audio,
                _bonusClip,
                _textSpawner,
                _bonusParticles,
                _pool.Release);
        }
    }
}
