using UnityEngine;

/// <summary>
/// A single dirty spot in the room. Player clicks it to clean.
/// </summary>
public class CleanableSpot : MonoBehaviour
{
    [Header("Visuals")]
    public SpriteRenderer dirtySprite;
    [Tooltip("Random dirty sprite chosen on initialisation.")]
    public Sprite[] dirtyVariants;

    public bool IsCleaned { get; private set; }

    private CleaningZone zone;

    // ─── Initialisation ───────────────────────────────────────────────────

    public void Initialize(CleaningZone parentZone)
    {
        zone      = parentZone;
        IsCleaned = false;

        if (dirtyVariants != null && dirtyVariants.Length > 0)
            dirtySprite.sprite = dirtyVariants[Random.Range(0, dirtyVariants.Length)];
    }

    // ─── Input ────────────────────────────────────────────────────────────

    void OnMouseDown()
    {
        if (!IsCleaned)
            Clean();
    }

    // ─── Clean ────────────────────────────────────────────────────────────

    private void Clean()
    {
        IsCleaned          = true;
        dirtySprite.color  = Color.clear;   // fade out immediately; replace with animation later
        zone?.OnSpotCleaned();
    }
}
