using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Central UI facade for the GameScene.
/// Scene-specific — does NOT use DontDestroyOnLoad.
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("HUD")]
    public HUD hud;

    [Header("Accuracy Popup")]
    public AccuracyPopup accuracyPopupPrefab;
    public RectTransform accuracyPopupParent;

    [Header("Room Phase Labels")]
    public TextMeshProUGUI drPakPhaseLabel;
    public TextMeshProUGUI drSeolPhaseLabel;

    [Header("Feedback Banner")]
    public TextMeshProUGUI feedbackText;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    // ─── HUD ──────────────────────────────────────────────────────────────

    public void UpdateHUD() => hud?.Refresh();

    // ─── Accuracy popup ───────────────────────────────────────────────────

    public void ShowAccuracy(AccuracyRating rating)
    {
        if (accuracyPopupPrefab != null)
        {
            var popup = Instantiate(accuracyPopupPrefab, accuracyPopupParent);
            popup.Show(rating);
        }
        AudioManager.Instance?.PlayAccuracySFX(rating);
    }

    // ─── Phase labels ─────────────────────────────────────────────────────

    public void ShowPhaseLabel(RoomId room, string text)
    {
        var label = room == RoomId.DrPak ? drPakPhaseLabel : drSeolPhaseLabel;
        if (label != null) label.text = text;
    }

    // ─── Feedback banner ──────────────────────────────────────────────────

    public void ShowFeedback(string message, Color color)
    {
        if (feedbackText == null) return;
        feedbackText.text  = message;
        feedbackText.color = color;
        StopCoroutine(nameof(ClearFeedback));
        StartCoroutine(nameof(ClearFeedback));
    }

    private IEnumerator ClearFeedback()
    {
        yield return new WaitForSeconds(2f);
        if (feedbackText != null) feedbackText.text = "";
    }
}
