using UnityEngine;
using UnityEngine.EventSystems;

namespace CandyClicker.Core
{
    /// <summary>
    /// Single entry point for taps. Converts screen position to world space, finds a
    /// Collider2D under it and forwards to <see cref="IClickable"/>.
    /// Dependencies: a Camera (orthographic), colliders on the clickable layer.
    /// Usage: put on one persistent GameObject in the scene, assign the camera + mask.
    /// </summary>
    public class ClickInput : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Camera _camera;

        [Header("Settings")]
        [SerializeField] private LayerMask _clickableMask = ~0;
        [SerializeField] private bool _blockWhenOverUI = true;

        private void Reset() => _camera = Camera.main;

        private void Awake()
        {
            if (_camera == null) _camera = Camera.main;

            if (_camera == null)
            {
                Debug.LogError($"[{nameof(ClickInput)}] No camera assigned.", this);
                enabled = false;
            }
        }

        private void Update()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            if (_blockWhenOverUI && IsPointerOverUI()) return;

            Vector2 worldPoint = _camera.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(worldPoint, _clickableMask);

            if (hit != null && hit.TryGetComponent(out IClickable clickable))
                clickable.OnClicked(worldPoint);
        }

        // Mouse button 0 also covers the first touch on WebGL/mobile builds.
        private static bool IsPointerOverUI()
        {
            if (EventSystem.current == null) return false;

            if (Input.touchCount > 0)
                return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);

            return EventSystem.current.IsPointerOverGameObject();
        }
    }
}
