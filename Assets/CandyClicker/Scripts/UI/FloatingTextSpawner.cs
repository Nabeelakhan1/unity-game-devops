using CandyClicker.Utilities;
using UnityEngine;

namespace CandyClicker.UI
{
    /// <summary>
    /// Owns the FloatingText pool. Dependencies: a FloatingText prefab.
    /// Usage: spawner.Spawn(worldPoint, "+1", Color.white);
    /// </summary>
    public class FloatingTextSpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private FloatingText _prefab;

        [Header("Settings")]
        [SerializeField, Min(1)] private int _initialPoolSize = 16;

        private ObjectPool<FloatingText> _pool;

        private void Awake()
        {
            if (_prefab == null)
            {
                Debug.LogError($"[{nameof(FloatingTextSpawner)}] Missing prefab.", this);
                enabled = false;
                return;
            }

            _pool = new ObjectPool<FloatingText>(_prefab, _initialPoolSize, transform);
        }

        /// <summary>Shows a label at a world position.</summary>
        public void Spawn(Vector3 worldPosition, string text, Color color)
        {
            if (!enabled) return;

            FloatingText instance = _pool.Get();
            instance.Play(worldPosition, text, color, _pool.Release);
        }
    }
}
