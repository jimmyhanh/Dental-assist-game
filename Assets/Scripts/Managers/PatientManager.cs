using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Spawns patients in the waiting room, handles player routing input,
/// and feeds the next batch after a room frees up.
/// </summary>
public class PatientManager : MonoBehaviour
{
    public static PatientManager Instance { get; private set; }

    [Header("Prefab")]
    public PatientController patientPrefab;

    [Header("Positions")]
    public Transform waitingRoomSpawn;
    public Transform exitPoint;
    [Tooltip("Queue slot positions in the waiting room (max waiting capacity).")]
    public Transform[] queuePositions;

    public Vector3 ExitPoint => exitPoint.position;

    private readonly List<PatientController> waitingPatients =
        new List<PatientController>();

    private PatientController selectedPatient;

    // ─── Lifecycle ────────────────────────────────────────────────────────

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        BeginSpawning();
    }

    // ─── Spawning ─────────────────────────────────────────────────────────

    public void BeginSpawning()
    {
        SpawnNextBatch();
    }

    private void SpawnNextBatch()
    {
        while (waitingPatients.Count < queuePositions.Length
               && GameManager.Instance.PatientsRemaining > 0)
        {
            var data = GameManager.Instance.DequeueNextPatient();
            if (data == null) break;
            SpawnPatient(data);
        }
    }

    private void SpawnPatient(PatientData data)
    {
        var patient = Instantiate(patientPrefab,
                                  waitingRoomSpawn.position,
                                  Quaternion.identity);
        patient.Initialize(data);

        int idx = waitingPatients.Count;
        if (idx < queuePositions.Length)
            patient.WalkTo(queuePositions[idx].position);

        waitingPatients.Add(patient);
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.patientArriveSFX);
    }

    // ─── Player routing input ─────────────────────────────────────────────

    public void OnPatientClicked(PatientController patient)
    {
        selectedPatient = patient;
        UIManager.Instance?.ShowFeedback(
            $"Select a room for {patient.Data.patientName}",
            UnityEngine.Color.cyan);
    }

    public void OnRoomDoorClicked(RoomId room)
    {
        if (selectedPatient == null) return;
        RoutePatient(selectedPatient, room);
        selectedPatient = null;
    }

    // ─── Routing logic ────────────────────────────────────────────────────

    private void RoutePatient(PatientController patient, RoomId targetRoom)
    {
        var room = GameManager.Instance.GetRoom(targetRoom);
        if (room == null || !room.IsAvailable)
        {
            UIManager.Instance?.ShowFeedback("That room is busy!", UnityEngine.Color.red);
            return;
        }

        if (patient.Data.assignedRoom != targetRoom)
        {
            UIManager.Instance?.ShowFeedback("Wrong room! -50 pts", UnityEngine.Color.red);
            ScoreTracker.Instance.AddPenalty(50);
        }

        waitingPatients.Remove(patient);
        room.ReceivePatient(patient);
        SpawnNextBatch();
    }

    /// <summary>Called by RoomController after a room becomes Idle again.</summary>
    public void TryAssignNextPatient(RoomId roomId)
    {
        var match = waitingPatients.Find(p => p.Data.assignedRoom == roomId);
        if (match != null)
            RoutePatient(match, roomId);
    }
}
