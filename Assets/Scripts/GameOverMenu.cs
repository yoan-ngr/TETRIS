using TMPro;
using UnityEngine;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField] private CanvasGroup gameOverMenu;
    [SerializeField] private RectTransform gameOverPanel;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Board board;
    [SerializeField] private Game game;
    
    private const float FadeTime = 0.1f;
    
    public void Restart()
    {
        LeanTween.alphaCanvas(gameOverMenu, 0f, FadeTime);
        LeanTween.scale(gameOverPanel, Vector3.one * 0.8f, FadeTime).setOnComplete(() =>
        {
            gameOverMenu.gameObject.SetActive(false);
            board.StartGame();
        });
    }

    public void Home()
    {
        game.Hide();
        LeanTween.alphaCanvas(gameOverMenu, 0f, FadeTime);
        LeanTween.scale(gameOverPanel, Vector3.one * 0.8f, FadeTime).setOnComplete(() =>
        {
            gameOverMenu.gameObject.SetActive(false);
            board.paused = false;
        });
    }

    public void Show(int score)
    {
        gameOverMenu.gameObject.SetActive(true);
        scoreText.text = "Score " + score;
        LeanTween.alphaCanvas(gameOverMenu, 1f, FadeTime);
        LeanTween.scale(gameOverPanel, Vector3.one, FadeTime).setEase(LeanTweenType.easeOutElastic);
    }

    private void Hide()
    {
        LeanTween.alphaCanvas(gameOverMenu, 0f, FadeTime);
        LeanTween.scale(gameOverPanel, Vector3.one * 0.8f, FadeTime).setOnComplete(() =>
        {
            
        });
    }
}
