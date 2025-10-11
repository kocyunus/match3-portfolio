using System;
using System.Collections.Generic;
using UnityEngine;

namespace Yunus.Match3
{
    /// <summary>
    /// Oyun yaşam döngüsünü yöneten merkezi durum makinesi.
    /// Tüm sistemlerin aynı kaynaktan game state okumasını sağlar ve geçiş kurallarını doğrular.
    /// </summary>
    public sealed class GameStateService : IService
    {
        public event Action<GameStateChangedEventArgs> OnStateChanged;

        public GameStateType CurrentState { get; private set; } = GameStateType.Booting;
        public GameStateType PreviousState { get; private set; } = GameStateType.Booting;

        /// <summary>
        /// Güncel durumda oyuncu input'una izin veriliyor mu?
        /// </summary>
        public bool IsPlayerInputAllowed => TryGetDefinition(CurrentState, out var definition) && definition.AllowsPlayerInput;

        private readonly Dictionary<GameStateType, GameStateDefinition> stateDefinitions;
        private GameStateType? stateBeforePause;
        private bool hasGameStarted;

        public GameStateService()
        {
            stateDefinitions = new Dictionary<GameStateType, GameStateDefinition>
            {
                { GameStateType.Booting, new GameStateDefinition(allowsPlayerInput: false, GameStateType.Loading) },
                { GameStateType.Loading, new GameStateDefinition(allowsPlayerInput: false, GameStateType.Ready, GameStateType.GameOver) },
                { GameStateType.Ready, new GameStateDefinition(allowsPlayerInput: true, GameStateType.Processing, GameStateType.Paused, GameStateType.GameOver) },
                { GameStateType.Processing, new GameStateDefinition(allowsPlayerInput: false, GameStateType.Ready, GameStateType.Paused, GameStateType.GameOver) },
                { GameStateType.Paused, new GameStateDefinition(allowsPlayerInput: false, GameStateType.Ready, GameStateType.GameOver) },
                { GameStateType.GameOver, new GameStateDefinition(allowsPlayerInput: false, GameStateType.Loading) }
            };
        }

        public void Initialize()
        {
            hasGameStarted = false;
            stateBeforePause = null;
            ForceSetState(GameStateType.Loading, GameStateTransitionSource.System);
        }

        public void Tick()
        {
            // Frame başına bir işlem yapılmıyor. Gerekirse buraya timer/logik eklenebilir.
        }

        public void Cleanup()
        {
            OnStateChanged = null;
            PreviousState = GameStateType.Booting;
            CurrentState = GameStateType.Booting;
            hasGameStarted = false;
            stateBeforePause = null;
        }

        /// <summary>
        /// Uygun ise hedef state'e geçiş yapar.
        /// </summary>
        public bool TrySetState(GameStateType targetState, GameStateTransitionSource source = GameStateTransitionSource.System)
        {
            if (CurrentState == targetState)
            {
                return true;
            }

            if (!CanTransition(targetState))
            {
                Debug.LogWarning($"[GameStateService] Geçersiz geçiş: {CurrentState} → {targetState} (kaynak: {source})");
                return false;
            }

            ApplyState(targetState, source);
            return true;
        }

        /// <summary>
        /// Aktif durumu pause'a alır. Zaten pause'da ise false döner.
        /// </summary>
        public bool TryPauseGame(GameStateTransitionSource source = GameStateTransitionSource.System)
        {
            if (CurrentState == GameStateType.Paused || !CanTransition(GameStateType.Paused))
            {
                return false;
            }

            stateBeforePause = CurrentState;

            if (TrySetState(GameStateType.Paused, source))
            {
                return true;
            }

            stateBeforePause = null;
            return false;
        }

        /// <summary>
        /// Pause'dan çıkarak önceki duruma döner.
        /// </summary>
        public bool TryResumeGame(GameStateTransitionSource source = GameStateTransitionSource.System)
        {
            if (CurrentState != GameStateType.Paused || !stateBeforePause.HasValue)
            {
                return false;
            }

            GameStateType resumeTarget = stateBeforePause.Value == GameStateType.Paused
                ? GameStateType.Ready
                : stateBeforePause.Value;

            if (TrySetState(resumeTarget, source))
            {
                stateBeforePause = null;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Mevcut durumdan hedef duruma geçiş mümkün mü?
        /// </summary>
        public bool CanTransition(GameStateType targetState)
        {
            if (!TryGetDefinition(CurrentState, out var definition))
            {
                return false;
            }

            return definition.CanTransitionTo(targetState);
        }

        private void ApplyState(GameStateType targetState, GameStateTransitionSource source)
        {
            var previous = CurrentState;
            PreviousState = previous;
            CurrentState = targetState;

            HandleLifecycleEvents(previous, targetState, source);

            var allowsInput = IsPlayerInputAllowed;
            OnStateChanged?.Invoke(new GameStateChangedEventArgs(previous, targetState, source, allowsInput));
        }

        private void ForceSetState(GameStateType targetState, GameStateTransitionSource source)
        {
            var previous = CurrentState;
            PreviousState = previous;
            CurrentState = targetState;

            HandleLifecycleEvents(previous, targetState, source);

            var allowsInput = IsPlayerInputAllowed;
            OnStateChanged?.Invoke(new GameStateChangedEventArgs(previous, targetState, source, allowsInput));
        }

        private void HandleLifecycleEvents(GameStateType previous, GameStateType current, GameStateTransitionSource source)
        {
            if (previous == GameStateType.Paused && current != GameStateType.Paused)
            {
                stateBeforePause = null;
                GameEvents.OnGamePaused?.Invoke(false);
            }

            switch (current)
            {
                case GameStateType.Ready:
                    if (!hasGameStarted)
                    {
                        hasGameStarted = true;
                        GameEvents.OnGameStarted?.Invoke();
                    }
                    break;
                case GameStateType.Paused:
                    GameEvents.OnGamePaused?.Invoke(true);
                    break;
                case GameStateType.GameOver:
                    GameEvents.OnGameEnded?.Invoke(false);
                    break;
            }
        }

        private bool TryGetDefinition(GameStateType state, out GameStateDefinition definition)
        {
            if (!stateDefinitions.TryGetValue(state, out definition))
            {
                Debug.LogError($"[GameStateService] {state} için durum tanımı bulunamadı.");
                definition = default;
                return false;
            }

            return true;
        }

        private readonly struct GameStateDefinition
        {
            private readonly HashSet<GameStateType> allowedTransitions;

            public bool AllowsPlayerInput { get; }

            public GameStateDefinition(bool allowsPlayerInput, params GameStateType[] transitions)
            {
                AllowsPlayerInput = allowsPlayerInput;
                allowedTransitions = transitions != null && transitions.Length > 0
                    ? new HashSet<GameStateType>(transitions)
                    : new HashSet<GameStateType>();
            }

            public bool CanTransitionTo(GameStateType target)
            {
                return allowedTransitions != null && allowedTransitions.Contains(target);
            }
        }
    }

    public readonly struct GameStateChangedEventArgs
    {
        public GameStateType Previous { get; }
        public GameStateType Current { get; }
        public GameStateTransitionSource Source { get; }
        public bool AllowsPlayerInput { get; }

        public GameStateChangedEventArgs(GameStateType previous, GameStateType current, GameStateTransitionSource source, bool allowsPlayerInput)
        {
            Previous = previous;
            Current = current;
            Source = source;
            AllowsPlayerInput = allowsPlayerInput;
        }
    }

    public enum GameStateTransitionSource
    {
        System,
        GameplayLoop,
        UserInterface,
        Debug
    }

    public enum GameStateType
    {
        Booting,
        Loading,
        Ready,
        Processing,
        Paused,
        GameOver
    }
}
