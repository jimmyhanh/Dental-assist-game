using UnityEngine;

/// <summary>
/// Drop this on any GameObject in GameScene to skip the menu flow
/// and instantly run a test day built entirely in code — no assets needed.
///
/// USAGE:
///   1. Open GameScene
///   2. Add an empty GameObject "TestBootstrap"
///   3. Attach this script
///   4. Press Play
/// </summary>
public class TestBootstrap : MonoBehaviour
{
#if UNITY_EDITOR
    void Awake()
    {
        // Only active in the Editor — remove the #if to test in builds too.
        var day = BuildTestDay();
        GameManager.Instance?.StartDay(day);
    }

    private static DayData BuildTestDay()
    {
        // ── Cleaning procedure (short, easy to test) ──────────────────────
        var cleaningProc          = ScriptableObject.CreateInstance<ProcedureData>();
        cleaningProc.type         = ProcedureType.Cleaning;
        cleaningProc.displayName  = "Routine Cleaning";
        cleaningProc.totalDuration = 8f;
        cleaningProc.dirtySpotCount = 3;
        cleaningProc.beatNotes = new BeatNote[]
        {
            new BeatNote { time = 1.0f },
            new BeatNote { time = 2.0f },
            new BeatNote { time = 3.0f },
            new BeatNote { time = 4.0f },
        };
        cleaningProc.toolSequence = new[] { ToolType.Mirror, ToolType.Probe };

        // ── Filling procedure ─────────────────────────────────────────────
        var fillingProc          = ScriptableObject.CreateInstance<ProcedureData>();
        fillingProc.type         = ProcedureType.Filling;
        fillingProc.displayName  = "Tooth Filling";
        fillingProc.totalDuration = 10f;
        fillingProc.dirtySpotCount = 4;
        fillingProc.beatNotes = new BeatNote[]
        {
            new BeatNote { time = 0.8f },
            new BeatNote { time = 1.6f },
            new BeatNote { time = 2.4f },
            new BeatNote { time = 3.6f },
        };
        fillingProc.toolSequence = new[] { ToolType.Drill, ToolType.FillingSyringe, ToolType.Suction };

        // ── Patients: 2 per doctor for a quick 4-patient test day ─────────
        PatientData[] patients =
        {
            MakePatient("Mia",    0, RoomId.DrPak,  cleaningProc),
            MakePatient("Kevin",  1, RoomId.DrSeol, fillingProc),
            MakePatient("Sara",   2, RoomId.DrPak,  fillingProc),
            MakePatient("Emma",   0, RoomId.DrSeol, cleaningProc),
        };

        var day      = ScriptableObject.CreateInstance<DayData>();
        day.patients = patients;
        return day;
    }

    private static PatientData MakePatient(
        string name, int sprite, RoomId room, ProcedureData proc)
    {
        var p          = ScriptableObject.CreateInstance<PatientData>();
        p.patientName  = name;
        p.spriteIndex  = sprite;
        p.assignedRoom = room;
        p.procedure    = proc;
        return p;
    }
#endif
}
