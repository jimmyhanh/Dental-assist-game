using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Shown at the end of a workday. Reads from ScoreTracker and displays
/// star rating plus per-category stats.
/// </summary>
public class ResultsUI : MonoBehaviour
{
    [Header("Stars")]
    public Image  star1;
    public Image  star2;
    public Image  star3;
    public Sprite filledStar;
    public Sprite emptyStar;

    [Header("Stats")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI perfectText;
    public TextMeshProUGUI goodText;
    public TextMeshProUGUI missText;
    public TextMeshProUGUI toolCorrectText;
    public TextMeshProUGUI toolMissText;
    public TextMeshProUGUI wrongRoomText;

    // ─── Lifecycle ────────────────────────────────────────────────────────

    void Start()
    {
        if (ScoreTracker.Instance == null) return;

        var s     = ScoreTracker.Instance;
        int stars = s.CalculateStars();

        SetText(scoreText,      $"Total Score:     {s.TotalScore}");
        SetText(perfectText,    $"Perfect Hits:    {s.PerfectHits}");
        SetText(goodText,       $"Good Hits:       {s.GoodHits}");
        SetText(missText,       $"Misses:          {s.Misses}");
        SetText(toolCorrectText,$"Tools Correct:   {s.ToolCorrect}");
        SetText(toolMissText,   $"Tools Missed:    {s.ToolMisses}");
        SetText(wrongRoomText,  $"Wrong Rooms:     {s.WrongRoomCount}");

        SetStar(star1, stars >= 1);
        SetStar(star2, stars >= 2);
        SetStar(star3, stars >= 3);
    }

    // ─── Helpers ──────────────────────────────────────────────────────────

    private void SetText(TextMeshProUGUI label, string text)
    {
        if (label != null) label.text = text;
    }

    private void SetStar(Image img, bool filled)
    {
        if (img == null) return;
        img.sprite = filled ? filledStar : emptyStar;
    }

    // ─── Button handlers ──────────────────────────────────────────────────

    public void OnPlayAgainClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnQuitClicked()
    {
        Application.Quit();
    }
}
