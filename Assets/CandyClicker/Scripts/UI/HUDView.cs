using CandyClicker.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CandyClicker.UI
{
    /// <summary>
    /// Paints the top progress bar, level and score. Read-only view over <see cref="ScoreSystem"/>.
    /// Dependencies: a filled Image for the bar, TMP labels.
    /// </summary>
    public class HUDView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ScoreSystem _scoreSystem;
        [SerializeField] private Image _progressBar;
        [SerializeField] private TextMeshProUGUI _levelLabel;
        [SerializeField] private TextMeshProUGUI _scoreLabel;

        [Header("Settings")]
        [SerializeField] private float _barLerpSpeed = 8f;

        private float _targetFill;

        private void Awake()
        {
            if (_scoreSystem == null)
            {
                Debug.LogError($"[{nameof(HUDView)}] Missing ScoreSystem reference.", this);
                enabled = false;
            }
        }

        private void OnEnable()
        {
            _scoreSystem.ScoreChanged += HandleScoreChanged;
            _scoreSystem.LevelChanged += HandleLevelChanged;
        }

        private void OnDisable()
        {
            _scoreSystem.ScoreChanged -= HandleScoreChanged;
            _scoreSystem.LevelChanged -= HandleLevelChanged;
        }

        private void Update()
        {
            if (_progressBar == null) return;

            _progressBar.fillAmount = Mathf.Lerp(_progressBar.fillAmount, _targetFill, _barLerpSpeed * Time.deltaTime);
        }

        private void HandleScoreChanged(int score)
        {
            _targetFill = _scoreSystem.LevelProgress;

            if (_scoreLabel != null) _scoreLabel.text = score.ToString();
        }

        private void HandleLevelChanged(int level)
        {
            // Snap back to empty instead of lerping down through the whole bar.
            if (_progressBar != null) _progressBar.fillAmount = 0f;

            if (_levelLabel != null) _levelLabel.text = $"Level {level}";
        }
    }
}
