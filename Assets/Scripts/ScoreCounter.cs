using TMPro;
using UnityEngine;

/// <summary>
/// The <b>ScoreCounter</b> class is responsible for calculating and displaying the player's score.
/// </summary>
public class ScoreCounter : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestScoreText;

    public delegate void OnUpdateScore();
    public OnUpdateScore onUpdateScore;

    private int _score;

    private void Start()
    {
        bestScoreText.text = "Best " + PlayerPrefs.GetInt("BestScore");
    }

    /// <summary>
    /// Adds a given point amount to the score. 
    /// </summary>
    /// <param name="amount">How much points need to be added.</param>
    public void AddScore(int amount)
    {
        _score += amount;
        scoreText.text = _score.ToString();
        onUpdateScore?.Invoke();

        float intensity = amount switch
        {
            100 => 0.25f,
            300 => 0.5f,
            500 => 0.75f,
            _ => 1f
        };
        
        LeanTween.scale(scoreText.gameObject, Vector3.one * (1 + .3f * intensity), 0.1f).setOnComplete(
            () => LeanTween.scale(scoreText.gameObject, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutElastic)
            );
        
        LeanTween.rotateZ(scoreText.gameObject, Mathf.Sign(Random.Range(-1f, 1f)) * 15f * intensity, 0.1f).setOnComplete(
            () => LeanTween.rotateZ(scoreText.gameObject, 0f, 0.1f)
        );
        
        
    }

    /// <summary>
    /// Resets the score and saves it as the best score if it is better than the last one.
    /// </summary>
    public void Reset()
    {
        if (_score > PlayerPrefs.GetInt("BestScore"))
        {
            bestScoreText.text = "Best " + _score;
            PlayerPrefs.SetInt("BestScore", _score);
        }
        _score = 0;
        scoreText.text = _score.ToString();
        onUpdateScore?.Invoke();
    }

    /// <summary>
    /// The current score.
    /// </summary>
    public int Score => _score;
}
