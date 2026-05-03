using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Visual and input component for one room's rhythm mini-game lane.
/// Spawns/scrolls RhythmNote objects and forwards button presses to RhythmManager.
/// </summary>
public class RhythmTrack : MonoBehaviour
{
    [Header("Layout")]
    public RectTransform trackArea;
    [Tooltip("X position (anchored) where notes are spawned (off-screen right).")]
    public float spawnX    =  600f;
    [Tooltip("X position (anchored) of the hit-zone circle.")]
    public float hitZoneX  = -300f;
    [Tooltip("Seconds it takes a note to travel from spawn to hit zone.")]
    public float travelTime = 2f;

    [Header("Prefabs & Visuals")]
    public RhythmNote notePrefab;
    public Image      hitZoneCircle;
    public Button     suctionButton;

    // ─── Runtime state ────────────────────────────────────────────────────

    private readonly List<RhythmNote> notes = new List<RhythmNote>();
    private int    nextNoteIndex;
    private bool   running;
    private double dspStart;

    public int   NextNoteIndex              => nextNoteIndex;
    public int   TotalNotes                 => notes.Count;
    public float GetNoteTime(int i)         => i < notes.Count ? notes[i].BeatTime : float.MaxValue;

    // ─── Setup ────────────────────────────────────────────────────────────

    public void Setup(ProcedureData procedure)
    {
        foreach (var n in notes)
            if (n != null) Destroy(n.gameObject);
        notes.Clear();
        nextNoteIndex = 0;
        running       = false;

        if (procedure.beatNotes == null) return;

        foreach (var beat in procedure.beatNotes)
        {
            var note = Instantiate(notePrefab, trackArea);
            note.Setup(beat.time, spawnX, hitZoneX, travelTime);
            note.gameObject.SetActive(false);
            notes.Add(note);
        }
    }

    public void StartTrack(double dspStartTime)
    {
        dspStart = dspStartTime;
        running  = true;

        suctionButton.onClick.RemoveAllListeners();
        suctionButton.onClick.AddListener(OnSuctionButtonPressed);
    }

    // ─── Per-frame update ─────────────────────────────────────────────────

    void Update()
    {
        if (!running) return;

        double elapsed = AudioSettings.dspTime - dspStart;

        // Activate notes whose travel-start time has arrived
        for (int i = nextNoteIndex; i < notes.Count; i++)
        {
            double spawnTime = notes[i].BeatTime - travelTime;
            if (elapsed >= spawnTime)
                notes[i].gameObject.SetActive(true);
            else
                break; // notes are sorted chronologically
        }

        // Update all active, unconsumed notes
        foreach (var note in notes)
        {
            if (note != null && note.gameObject.activeSelf && !note.Consumed)
                note.UpdatePosition(elapsed);
        }

        // Subtle pulse on the hit-zone circle
        float pulse = 1f + 0.05f * Mathf.Sin((float)elapsed * Mathf.PI * 4f);
        hitZoneCircle.transform.localScale = Vector3.one * pulse;
    }

    // ─── Hit / Miss visual feedback ───────────────────────────────────────

    public void ShowNoteHit(int index, AccuracyRating rating)
    {
        if (index < notes.Count) notes[index].MarkConsumed();
        FlashHitZone(rating == AccuracyRating.Perfect ? Color.yellow : Color.green);
    }

    public void ShowNoteMiss(int index)
    {
        if (index < notes.Count) notes[index].ShowMiss();
        FlashHitZone(Color.red);
    }

    private void FlashHitZone(Color color)
    {
        hitZoneCircle.color = color;
        CancelInvoke(nameof(ResetHitZoneColor));
        Invoke(nameof(ResetHitZoneColor), 0.15f);
    }

    private void ResetHitZoneColor() => hitZoneCircle.color = Color.white;

    // ─── Note advance ─────────────────────────────────────────────────────

    public void AdvanceNote() => nextNoteIndex++;

    // ─── Stop ─────────────────────────────────────────────────────────────

    public void Stop()
    {
        running = false;
        suctionButton.onClick.RemoveAllListeners();
    }

    // ─── Button handler ───────────────────────────────────────────────────

    private void OnSuctionButtonPressed()
    {
        RhythmManager.Instance?.OnSuctionPressed(
            this, AudioSettings.dspTime, dspStart);
    }
}
