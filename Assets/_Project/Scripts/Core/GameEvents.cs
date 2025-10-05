using System;
using UnityEngine;

namespace Yunus.Match3
{
    /// <summary>
    /// Oyun event'lerini yöneten merkezi sınıf (Observer Pattern)
    /// </summary>
    public static class GameEvents
    {
        #region Score Events
        public static Action<int> OnScoreChanged;
        public static Action<int> OnComboTriggered;
        #endregion

        #region Match Events
        public static Action<int> OnMatchFound;
        public static Action<string> OnSpecialMatchFound;
        #endregion

        #region Game State Events
        public static Action OnGameStarted;
        public static Action<bool> OnGameEnded;
        public static Action<bool> OnGamePaused;
        #endregion

        #region Level Events
        public static Action<int> OnLevelLoaded;
        public static Action<int> OnLevelCompleted;
        #endregion

        #region Move Events
        public static Action<TileView> OnTileClicked;
        public static Action<TileView, Vector2Int> OnTileSwiped; // Yeni: Swipe direction ile
        public static Action OnMoveExecuted;
        public static Action OnInvalidMove;
        #endregion

        #region Power-Up Events
        public static Action<string> OnPowerUpCreated;
        public static Action<string> OnPowerUpActivated;
        #endregion

        #region UI Events
        public static Action<string> OnButtonClicked;
        #endregion

        #region Debug Events
        public static Action<string> OnDebugMessage;
        #endregion

        #region Helper Methods

        /// <summary>
        /// Tüm event'leri temizle (memory leak önleme)
        /// </summary>
        public static void ClearAllEvents()
        {
            OnScoreChanged = null;
            OnComboTriggered = null;
            OnMatchFound = null;
            OnSpecialMatchFound = null;
            OnGameStarted = null;
            OnGameEnded = null;
            OnGamePaused = null;
            OnLevelLoaded = null;
            OnLevelCompleted = null;
            OnTileClicked = null;
            OnMoveExecuted = null;
            OnInvalidMove = null;
            OnPowerUpCreated = null;
            OnPowerUpActivated = null;
            OnButtonClicked = null;
            OnDebugMessage = null;
        }

        #endregion
    }
}
