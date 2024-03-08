using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The <b>PauseMenu</b> class is responsible for managing the pause menu.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    [SerializeField] private CanvasGroup pauseMenu;
    [SerializeField] private RectTransform pauseMenuPanel;
    [SerializeField] private Board board;
    [SerializeField] private Game game;

    [Space(10)] 
    [SerializeField] private Image musicIcon;
    [SerializeField] private Image soundIcon;
    [SerializeField] private Sprite musicOnSprite, musicOffSprite, soundOnSprite, soundOffSprite;

    private const float FadeTime = 0.1f;

    private void Start()
    {
        AudioManager.Instance.onUpdateMusic += UpdateMusicSprite;
        AudioManager.Instance.onUpdateSound += UpdateSoundSprite;
    }

    /// <summary>
    /// Shows the pause menu.
    /// </summary>
    public void Show()
    {
        pauseMenu.gameObject.SetActive(true);
        LeanTween.alphaCanvas(pauseMenu, 1f, FadeTime);
        LeanTween.scale(pauseMenuPanel, Vector3.one, FadeTime).setEase(LeanTweenType.easeOutElastic);
        board.paused = true;
    }

    /// <summary>
    /// Hides the pause menu.
    /// </summary>
    public void Hide()
    {
        LeanTween.alphaCanvas(pauseMenu, 0f, FadeTime);
        LeanTween.scale(pauseMenuPanel, Vector3.one * 0.8f, FadeTime).setOnComplete(() =>
        {
            pauseMenu.gameObject.SetActive(false);
            board.paused = false;
        });
    }

    /// <summary>
    /// Button method for toggling music on or off.
    /// </summary>
    public void ToggleMusic()
    {
        AudioManager.Instance.SetMusic(!AudioManager.Instance.MusicActive);
    }

    /// <summary>
    /// Button method for toggling sound on or off.
    /// </summary>
    public void ToggleSound()
    {
        AudioManager.Instance.SetSound(!AudioManager.Instance.SoundActive);
    }

    /// <summary>
    /// Hides the pause menu, the game and shows the main menu.
    /// </summary>
    public void Home()
    {
        game.Hide();
        LeanTween.scale(pauseMenuPanel, Vector3.one * 0.8f, FadeTime);
        LeanTween.alphaCanvas(pauseMenu, 0f, FadeTime).setOnComplete(() =>
        {
            pauseMenu.gameObject.SetActive(false);
            board.paused = false;
        });
    }

    /// <summary>
    /// Updates the music button sprite.
    /// </summary>
    private void UpdateMusicSprite()
    {
        musicIcon.sprite = AudioManager.Instance.MusicActive ? musicOnSprite : musicOffSprite;
    }

    /// <summary>
    /// Updates the sound button sprite.
    /// </summary>
    private void UpdateSoundSprite()
    {
        soundIcon.sprite = AudioManager.Instance.SoundActive ? soundOnSprite : soundOffSprite;
    }
}
