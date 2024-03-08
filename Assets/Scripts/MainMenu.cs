using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The <b>MainMenu</b> class is responsible for managing the main menu.
/// </summary>
public class MainMenu : MonoBehaviour
{
    [SerializeField] private Image musicButtonIcon, soundButtonIcon;
    [SerializeField] private Sprite musicOnSprite, musicOffSprite, soundOnSprite, soundOffSprite;
    [SerializeField] private CanvasGroup titleCanvasGroup;
    [SerializeField] private GameObject buttons;
    [SerializeField] private Game _game;

    private float _originalButtonsPosition;
    
    private void Start()
    {
        AudioManager.Instance.onUpdateMusic += UpdateMusic;
        AudioManager.Instance.onUpdateSound += UpdateSound;
    }

    /// <summary>
    /// Shows back the main menu.
    /// </summary>
    public void Show()
    {
        LeanTween.alphaCanvas(titleCanvasGroup, 1f, .5f);
        LeanTween.moveY(buttons, _originalButtonsPosition, .5f).setEase(LeanTweenType.easeOutBack);
    }

    /// <summary>
    /// Hides the main menu and starts the game.
    /// </summary>
    public void Play()
    {
        LeanTween.alphaCanvas(titleCanvasGroup, 0f, .5f);
        _originalButtonsPosition = buttons.transform.position.y;
        LeanTween.moveY(buttons, -200f, .5f).setEase(LeanTweenType.easeInBack).setOnComplete(() => _game.Show());
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
    /// Updates the music button sprite.
    /// </summary>
    private void UpdateMusic()
    {
        musicButtonIcon.sprite = AudioManager.Instance.MusicActive ? musicOnSprite : musicOffSprite;
    }

    /// <summary>
    /// Updates the sound button sprite.
    /// </summary>
    private void UpdateSound()
    {
        soundButtonIcon.sprite = AudioManager.Instance.SoundActive ? soundOnSprite : soundOffSprite;
    }
}
