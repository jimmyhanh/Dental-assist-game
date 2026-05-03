using UnityEngine;
using System.Collections;

/// <summary>
/// Per-room orchestrator. Runs the full patient procedure state machine:
/// PatientEntering → Suctioning → ToolHandoff → PatientLeaving → Cleaning → Idle
/// </summary>
public class RoomController : MonoBehaviour
{
    [Header("Room Config")]
    public RoomId roomId;

    [Header("Scene References")]
    public Transform       patientSeat;
    public DoctorAnimator  doctorAnimator;
    public RhythmTrack     rhythmTrack;
    public ToolTray        toolTray;
    public CleaningZone    cleaningZone;

    // ─── State ────────────────────────────────────────────────────────────

    public RoomPhase Phase      { get; private set; } = RoomPhase.Idle;
    public bool      IsAvailable => Phase == RoomPhase.Idle;

    private PatientController currentPatient;

    // ─── Lifecycle ────────────────────────────────────────────────────────

    void Start()
    {
        GameManager.Instance.RegisterRoom(this);
        rhythmTrack.gameObject.SetActive(false);
        toolTray.gameObject.SetActive(false);
    }

    // ─── Entry point ──────────────────────────────────────────────────────

    public void ReceivePatient(PatientController patient)
    {
        currentPatient = patient;
        Phase = RoomPhase.PatientEntering;
        StartCoroutine(RunProcedure(patient.Data));
    }

    // ─── Procedure coroutine ──────────────────────────────────────────────

    private IEnumerator RunProcedure(PatientData data)
    {
        // ── Walk patient to seat ──────────────────────────────────────────
        UIManager.Instance?.ShowPhaseLabel(roomId, $"{data.patientName} incoming...");
        yield return currentPatient.WalkTo(patientSeat.position);

        doctorAnimator.SetWorking(true);

        // ── Suction / Rhythm phase ────────────────────────────────────────
        Phase = RoomPhase.Suctioning;
        UIManager.Instance?.ShowPhaseLabel(roomId, "Suction!");
        rhythmTrack.gameObject.SetActive(true);
        yield return RhythmManager.Instance.RunRhythmGame(data.procedure, rhythmTrack);
        rhythmTrack.gameObject.SetActive(false);

        // ── Tool hand-off phase ───────────────────────────────────────────
        Phase = RoomPhase.ToolHandoff;
        UIManager.Instance?.ShowPhaseLabel(roomId, "Hand the tools!");
        toolTray.gameObject.SetActive(true);
        yield return ToolManager.Instance.RunToolHandoff(
            data.procedure, toolTray, doctorAnimator);
        toolTray.gameObject.SetActive(false);

        // ── Patient leaves ────────────────────────────────────────────────
        Phase = RoomPhase.PatientLeaving;
        doctorAnimator.SetWorking(false);
        UIManager.Instance?.ShowPhaseLabel(roomId, "Goodbye!");
        yield return currentPatient.WalkTo(PatientManager.Instance.ExitPoint);
        Destroy(currentPatient.gameObject);
        currentPatient = null;

        // ── Cleaning phase ────────────────────────────────────────────────
        Phase = RoomPhase.Cleaning;
        UIManager.Instance?.ShowPhaseLabel(roomId, "Clean the room!");
        cleaningZone.ActivateDirtySpots(data.procedure.type);
        yield return new WaitUntil(() => cleaningZone.IsClean);
        cleaningZone.ClearAll();

        // ── Room ready ────────────────────────────────────────────────────
        UIManager.Instance?.ShowPhaseLabel(roomId, "Ready!");
        Phase = RoomPhase.Idle;

        GameManager.Instance.OnPatientCompleted();
        PatientManager.Instance.TryAssignNextPatient(roomId);
    }
}
