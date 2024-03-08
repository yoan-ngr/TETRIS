using TMPro;
using UnityEngine;

/// <summary>
/// The <b>Difficulty</b> class is responsible for managing the game's difficulty, and displaying it on the screen.
/// </summary>
public class Difficulty : MonoBehaviour
{
    [SerializeField] private float[] delays;
    [SerializeField] private Gradient levelColorGradient;
    [SerializeField] private ScoreCounter scoreCounter;
    [SerializeField] private TextMeshProUGUI difficultyText;

    /// <summary>
    /// The current difficulty, calculated from the score.
    /// </summary>
    private int CurrentDifficulty => Mathf.Min(scoreCounter.Score / 600, delays.Length - 1);

    /// <summary>
    /// The current time between each "move down" command on the piece.
    /// </summary>
    public float CurrentDelay => delays[CurrentDifficulty];

    private void Start()
    {
        scoreCounter.onUpdateScore += UpdateDifficulty;
        UpdateDifficulty();
    }

    private void UpdateDifficulty()
    {
        difficultyText.text = CurrentDifficulty.ToString();
        difficultyText.color = levelColorGradient.Evaluate((float)CurrentDifficulty / delays.Length);
    }
}
