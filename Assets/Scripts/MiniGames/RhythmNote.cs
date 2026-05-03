using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A single scrolling note indicator on the RhythmTrack.
/// Moves from right (spawn) to left (hit zone) over travelTime seconds.
/// </summary>
public class RhythmNote : MonoBehaviour
{
    [Header("Visual")]
    public Image noteImage;

    // ─── Runtime data ─────────────────────────────────────────────────────

    public float BeatTime  { get; private set; }
    public bool  Consumed  { get; private set; }

    private float startX;
    private float hitZoneX;
    private float travelTime;
    private RectTransform rectTransform;

    // ─── Lifecycle ────────────────────────────────────────────────────────

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // ─── Setup ────────────────────────────────────────────────────────────

    public void Setup(float beatTime, float spawnX, float zoneX, float travel)
    {
        BeatTime   = beatTime;
        startX     = spawnX;
        hitZoneX   = zoneX;
        travelTime = travel;
        rectTransform.anchoredPosition = new Vector2(spawnX, 0f);
    }

    // ─── Per-frame update (called by RhythmTrack.Update) ─────────────────

    public void UpdatePosition(double elapsed)
    {
        if (Consumed) return;
        float timeUntilBeat = BeatTime - (float)elapsed;
        float fraction      = 1f - (timeUntilBeat / travelTime);
        float x             = Mathf.Lerp(startX, hitZoneX, fraction);
        rectTransform.anchoredPosition = new Vector2(x, 0f);
    }

    // ─── Hit / Miss feedback ──────────────────────────────────────────────

    public void MarkConsumed()
    {
        Consumed = true;
        gameObject.SetActive(false);
    }

    public void ShowMiss()
    {
        Consumed = true;
        if (noteImage != null) noteImage.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
        Destroy(gameObject, 0.3f);
    }
}
