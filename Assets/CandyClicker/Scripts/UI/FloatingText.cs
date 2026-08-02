using System;
using TMPro;
using UnityEngine;

namespace CandyClicker.UI
{
    /// <summary>
    /// One pooled "+1" label: rises, fades, then returns itself to the pool.
    /// Dependencies: a world-space TextMeshPro (3D) component on the same object.
    /// Usage: spawned by <see cref="FloatingTextSpawner"/>, never placed by hand.
    /// </summary>
    [RequireComponent(typeof(TextMeshPro))]
    public class FloatingText : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _lifetime = 0.7f;
        [SerializeField] private float _riseSpeed = 2f;
        [SerializeField] private float _horizontalDrift = 0.4f;

        private TextMeshPro _label;
        private Action<FloatingText> _onFinished;
        private float _elapsed;
        private Vector3 _velocity;
        private Color _baseColor;

        private void Awake()
        {
            _label = GetComponent<TextMeshPro>();
            _baseColor = _label.color;
        }

        /// <summary>Starts the animation. <paramref name="onFinished"/> returns it to the pool.</summary>
        public void Play(Vector3 worldPosition, string text, Color color, Action<FloatingText> onFinished)
        {
            transform.position = worldPosition;
            _label.text = text;
            _baseColor = color;
            _label.color = color;

            _onFinished = onFinished;
            _elapsed = 0f;
            _velocity = new Vector3(UnityEngine.Random.Range(-_horizontalDrift, _horizontalDrift), _riseSpeed, 0f);
        }

        private void Update()
        {
            _elapsed += Time.deltaTime;

            if (_elapsed >= _lifetime)
            {
                _onFinished?.Invoke(this);
                return;
            }

            transform.position += _velocity * Time.deltaTime;

            float alpha = 1f - (_elapsed / _lifetime);
            _label.color = new Color(_baseColor.r, _baseColor.g, _baseColor.b, alpha);
        }
    }
}
