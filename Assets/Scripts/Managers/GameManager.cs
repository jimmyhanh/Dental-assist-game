using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// Orchestrates the overall workday: patient queue, scene transitions,
/// and room registry. Persists across scenes.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Day Setup (assign in Inspector or via StartDay)")]
    public DayData currentDay;

    // ─── Runtime state ────────────────────────────────────────────────────

    private readonly Dictionary<RoomId, RoomController> rooms =
        new Dictionary<RoomId, RoomController>();

    private readonly Queue<PatientData> patientQueue = new Queue<PatientData>();
    private int totalPatients;
    private int completedPatients;

    public GamePhase Phase      { get; private set; } = GamePhase.MainMenu;
    public int PatientsRemaining => patientQueue.Count;
    public int CompletedPatients => completedPatients;
    public int TotalPatients     => totalPatients;

    // ─── Lifecycle ────────────────────────────────────────────────────────

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ─── Room registry (called by RoomController.Start) ──────────────────

    public void RegisterRoom(RoomController room)
    {
        rooms[room.roomId] = room;
    }

    public RoomController GetRoom(RoomId id)
    {
        rooms.TryGetValue(id, out var room);
        return room;
    }

    // ─── Day flow ─────────────────────────────────────────────────────────

    public void StartDay(DayData day)
    {
        currentDay = day;
        rooms.Clear();
        patientQueue.Clear();
        completedPatients = 0;

        ScoreTracker.Instance.Reset();

        foreach (var p in day.patients)
            patientQueue.Enqueue(p);
        totalPatients = patientQueue.Count;

        Phase = GamePhase.DayRunning;
        SceneManager.LoadScene("GameScene");
    }

    public PatientData DequeueNextPatient()
    {
        return patientQueue.Count > 0 ? patientQueue.Dequeue() : null;
    }

    public void OnPatientCompleted()
    {
        completedPatients++;
        UIManager.Instance?.UpdateHUD();

        if (completedPatients >= totalPatients)
            EndDay();
    }

    private void EndDay()
    {
        Phase = GamePhase.EndOfDay;
        SceneManager.LoadScene("ResultsScene");
    }
}
