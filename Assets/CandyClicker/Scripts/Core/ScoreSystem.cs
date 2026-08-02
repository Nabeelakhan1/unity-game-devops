using System;
using UnityEngine;

namespace CandyClicker.Core
{
    /// <summary>
    /// Owns score, level and level progress. Everything else reads it through events.
    /// Dependencies: none. Usage: assign this instance to MainCandy, BonusCandy spawner and HUDView.
    /// </summary>
    public class ScoreSystem : MonoBehaviour
    {
        private const string ScoreKey = "candy_score";

        [Header("Settings")]
        [SerializeField, Min(1)] private int _pointsPerLevel = 25;
        [SerializeField] private bool _persist = true;

        private int _score;

        /// <summary>Total points collected.</summary>
        public int Score => _score;

        /// <summary>Current level, starting at 1.</summary>
        public int Level => (_score / _pointsPerLevel) + 1;

        /// <summary>Progress through the current level, 0..1.</summary>
        public float LevelProgress => (_score % _pointsPerLevel) / (float)_pointsPerLevel;

        /// <summary>Raised on every score change with the new score.</summary>
        public event Action<int> ScoreChanged;

        /// <summary>Raised when the level rolls over, with the new level.</summary>
        public event Action<int> LevelChanged;

        private void Awake()
        {
            if (_persist) _score = PlayerPrefs.GetInt(ScoreKey, 0);
        }

        private void Start()
        {
            // Fire once so the HUD paints the loaded state.
            ScoreChanged?.Invoke(_score);
            LevelChanged?.Invoke(Level);
        }

        /// <summary>Adds points and raises the matching events.</summary>
        public void Add(int amount)
        {
            if (amount <= 0) return;

            int previousLevel = Level;
            _score += amount;

            ScoreChanged?.Invoke(_score);
            if (Level != previousLevel) LevelChanged?.Invoke(Level);

            if (_persist) PlayerPrefs.SetInt(ScoreKey, _score);
        }

        /// <summary>Wipes the saved run. Handy for testing.</summary>
        public void ResetScore()
        {
            _score = 0;
            if (_persist) PlayerPrefs.DeleteKey(ScoreKey);

            ScoreChanged?.Invoke(_score);
            LevelChanged?.Invoke(Level);
        }

        private void OnApplicationQuit()
        {
            // WebGL flushes PlayerPrefs to IndexedDB on Save().
            if (_persist) PlayerPrefs.Save();
        }
    }
}
