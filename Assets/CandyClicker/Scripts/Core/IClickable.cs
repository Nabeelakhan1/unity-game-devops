using UnityEngine;

namespace CandyClicker.Core
{
    /// <summary>
    /// Contract for anything the player can tap in world space.
    /// Implemented by MainCandy and BonusCandy; dispatched by <see cref="ClickInput"/>.
    /// </summary>
    public interface IClickable
    {
        /// <summary>Called once per valid tap. <paramref name="worldPoint"/> is where the tap landed.</summary>
        void OnClicked(Vector2 worldPoint);
    }
}
