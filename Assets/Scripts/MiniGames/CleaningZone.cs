using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages the set of CleanableSpots spawned in a room after a procedure.
/// RoomController polls <see cref="IsClean"/> via WaitUntil.
/// </summary>
public class CleaningZone : MonoBehaviour
{
    [Header("Prefab")]
    public CleanableSpot spotPrefab;

    [Header("Anchor positions for dirty spots in this room")]
    public Transform[] spotAnchors;

    private readonly List<CleanableSpot> activeSpots = new List<CleanableSpot>();

    // ─── State ────────────────────────────────────────────────────────────

    /// <summary>True when all spots have been cleaned (or none were spawned).</summary>
    public bool IsClean => activeSpots.Count == 0
                        || activeSpots.TrueForAll(s => s.IsCleaned);

    // ─── Activate ─────────────────────────────────────────────────────────

    public void ActivateDirtySpots(ProcedureType procedureType)
    {
        ClearAll();

        int count = procedureType switch
        {
            ProcedureType.Extraction => 6,
            ProcedureType.RootCanal  => 7,
            ProcedureType.Filling    => 5,
            _                        => 4   // Cleaning
        };
        count = Mathf.Min(count, spotAnchors.Length);

        for (int i = 0; i < count; i++)
        {
            var spot = Instantiate(spotPrefab,
                                   spotAnchors[i].position,
                                   Quaternion.identity,
                                   transform);
            spot.Initialize(this);
            activeSpots.Add(spot);
        }
    }

    // ─── Callback from CleanableSpot ─────────────────────────────────────

    public void OnSpotCleaned()
    {
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.cleanSFX);
    }

    // ─── Clear ────────────────────────────────────────────────────────────

    public void ClearAll()
    {
        foreach (var s in activeSpots)
            if (s != null) Destroy(s.gameObject);
        activeSpots.Clear();
    }
}
