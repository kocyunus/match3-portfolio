/// <summary>
/// Oyun sabitleri - Magic number kullanımını önler
/// </summary>
public static class GameConstants
{
    #region Board Settings
    public const int BOARD_WIDTH = 8;
    public const int BOARD_HEIGHT = 8;
    public const float TILE_SPACING = 1.0f;
    #endregion

    #region Match Rules
    public const int MIN_MATCH_COUNT = 3;
    public const int SPECIAL_MATCH_4 = 4;
    public const int SPECIAL_MATCH_5 = 5;
    #endregion

    #region Scoring
    public const int POINTS_PER_TILE = 10;
    public const int MAX_COMBO_MULTIPLIER = 5;
    public const int ONE_STAR_SCORE = 1000;
    public const int TWO_STAR_SCORE = 2000;
    public const int THREE_STAR_SCORE = 3000;
    #endregion

    #region Animation Durations
    public const float TILE_SWAP_DURATION = 0.3f;
    public const float TILE_MATCH_DURATION = 0.25f;
    public const float TILE_FALL_DURATION = 0.4f;
    public const float UI_TRANSITION_DURATION = 0.3f;
    #endregion

    #region Input Settings
    public const float MIN_SWIPE_DISTANCE = 50f;
    public const float MAX_SWIPE_TIME = 0.5f;
    #endregion

    #region Audio Settings
    public const int MAX_AUDIO_SOURCES = 10;
    public const float DEFAULT_SFX_VOLUME = 0.7f;
    public const float DEFAULT_MUSIC_VOLUME = 0.5f;
    #endregion

    #region Performance Settings
    public const int INITIAL_POOL_SIZE = 64;
    public const int PARTICLE_POOL_SIZE = 20;
    public const int TARGET_FPS = 60;
    #endregion

    #region Game Rules
    public const int DEFAULT_MOVE_LIMIT = 20;
    public const float COMBO_TIMEOUT = 2.0f;
    #endregion

    #region Layer & Tag Names
    public const string LAYER_TILE = "Tile";
    public const string LAYER_UI = "UI";
    public const string TAG_PLAYER = "Player";
    public const string TAG_TILE = "Tile";
    #endregion

    #region Scene Names
    public const string SCENE_MAIN_MENU = "MainMenu";
    public const string SCENE_GAME = "Game";
    public const string SCENE_LOADING = "Loading";
    #endregion
}
