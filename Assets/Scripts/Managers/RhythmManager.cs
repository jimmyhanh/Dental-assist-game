using UnityEngine;
using System.Collections;

/// <summary>
/// Drives the taiko-style suction rhythm mini-game.
/// Each room shares this singleton; only one game runs per room at a time
/// (two rooms can overlap, each with its own RhythmTrack instance).
/// </summary>
public class RhythmManager : MonoBehaviour
{
    public static RhythmManager Instance { get; private set; }

    [Header("Accuracy Windows (seconds)")]
    public float perfectWindow = 0.08f;
    public float goodWindow    = 0.15f;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    // ─── Game loop ────────────────────────────────────────────────────────

    /// <summary>
    /// Runs the full rhythm game for one procedure on the given track.
    /// Yield-returns when the procedure music duration has elapsed.
    /// </summary>
    public IEnumerator RunRhythmGame(ProcedureData procedure, RhythmTrack track)
    {
        if (procedure.beatNotes == null || procedure.beatNotes.Length == 0)
        {
            // No beats defined — just wait out the duration
            yield return new WaitForSeconds(procedure.totalDuration);
            yield break;
        }

        track.Setup(procedure);

        // Schedule music ~0.1 s in the future for tight dspTime sync
        double dspStart = AudioSettings.dspTime + 0.1;
        AudioManager.Instance?.PlayProcedureMusicScheduled(procedure.procedureMusic, dspStart);
        track.StartTrack(dspStart);

        int noteIndex  = 0;
        int totalNotes = procedure.beatNotes.Length;

        while (noteIndex < totalNotes)
        {
            double elapsed = AudioSettings.dspTime - dspStart;

            // Auto-miss any note whose window has fully passed
            float noteTime = procedure.beatNotes[noteIndex].time;
            if (elapsed > noteTime + goodWindow)
            {
                ScoreTracker.Instance.RecordHit(AccuracyRating.Miss);
                UIManager.Instance?.ShowAccuracy(AccuracyRating.Miss);
                track.ShowNoteMiss(noteIndex);
                track.AdvanceNote();
                noteIndex++;
            }

            yield return null;
        }

        // Wait for the remaining music duration
        double remaining = procedure.totalDuration - (AudioSettings.dspTime - dspStart);
        if (remaining > 0)
            yield return new WaitForSeconds((float)remaining);

        track.Stop();
        AudioManager.Instance?.StopProcedureMusic();
    }

    // ─── Player input (called by RhythmTrack button) ─────────────────────

    public void OnSuctionPressed(RhythmTrack track, double currentDsp, double dspStart)
    {
        double elapsed    = currentDsp - dspStart;
        int    noteIndex  = track.NextNoteIndex;

        if (noteIndex >= track.TotalNotes) return;

        float noteTime = track.GetNoteTime(noteIndex);
        float delta    = Mathf.Abs((float)(elapsed - noteTime));

        // Ignore presses that arrive before the earliest good window
        if (elapsed < noteTime - goodWindow) return;

        AccuracyRating rating;
        if      (delta <= perfectWindow) rating = AccuracyRating.Perfect;
        else if (delta <= goodWindow)    rating = AccuracyRating.Good;
        else                             rating = AccuracyRating.Miss;

        ScoreTracker.Instance.RecordHit(rating);
        UIManager.Instance?.ShowAccuracy(rating);
        track.ShowNoteHit(noteIndex, rating);
        track.AdvanceNote();
    }
}
