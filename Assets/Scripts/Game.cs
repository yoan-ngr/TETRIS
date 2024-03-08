using UnityEngine;

/// <summary>
/// The <b>Game</b> class is responsible for showing or hiding the game.
/// </summary>
public class Game : MonoBehaviour
{
    [SerializeField] private GameObject border, background;
    [SerializeField] private Color finalColor = Color.white, defaultColor = Color.white;
    [SerializeField] private Board board;
    [SerializeField] private GameObject footer, header;
    [SerializeField] private MainMenu mainMenu;

    private const float FadeTime = 0.5f;
    private float _originalHeaderY;

    private void Start()
    {
        _originalHeaderY = header.transform.position.y;
    }

    /// <summary>
    /// Shows the game.
    /// </summary>
    public void Show()
    {
        LeanTween.color(border, finalColor, FadeTime);
        LeanTween.color(background, finalColor, FadeTime);
        
        footer.transform.position = new Vector3(footer.transform.position.x, -200f, footer.transform.position.z);
        footer.SetActive(true);
        LeanTween.moveY(footer, 208f, FadeTime).setEase(LeanTweenType.easeOutBack);
        
        header.transform.position = new Vector3(header.transform.position.x, _originalHeaderY + 200f, header.transform.position.z);
        header.SetActive(true);
        LeanTween.moveY(header, _originalHeaderY, FadeTime).setEase(LeanTweenType.easeOutBack).setOnComplete(() => board.StartGame());
    }

    /// <summary>
    /// Hides the game.
    /// </summary>
    public void Hide()
    {
        LeanTween.color(border, defaultColor, FadeTime);
        LeanTween.color(background, defaultColor, FadeTime);
        
        LeanTween.moveY(footer, -200f, FadeTime).setEase(LeanTweenType.easeInBack).setOnComplete(() => footer.SetActive(true));
        
        LeanTween.moveY(header, header.transform.position.y + 200f, FadeTime).setEase(LeanTweenType.easeInBack).setOnComplete(() =>
        {
            header.SetActive(false);
            board.StopGame();
            mainMenu.Show();
        });
    }
}
