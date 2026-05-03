using UnityEngine;
using TMPro;

/// <summary>
/// Persistent HUD overlay: score, patient progress, combo counter.
/// Refreshed by UIManager.UpdateHUD().
/// </summary>
public class HUD : MonoBehaviour
{
    [Header("Labels")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI progressText;
    public TextMeshProUGUI comboText;

    private int combo;
    private int lastMissCount;

    public void Refresh()
    {
        if (ScoreTracker.Instance == null || GameManager.Instance == null) return;

        var s = ScoreTracker.Instance;
        var g = GameManager.Instance;

        if (scoreText    != null) scoreText.text    = $"Score: {s.TotalScore}";
        if (progressText != null) progressText.text =
            $"Patients: {g.CompletedPatients} / {g.TotalPatients}";

        // Combo: increments on each non-miss event; resets on new miss
        if (s.Misses > lastMissCount)
        {
            combo         = 0;
            lastMissCount = s.Misses;
        }
        else
        {
            combo++;
        }

        if (comboText != null)
        {
            comboText.gameObject.SetActive(combo > 1);
            comboText.text = combo > 1 ? $"Combo ×{combo}!" : "";
        }
    }
}
