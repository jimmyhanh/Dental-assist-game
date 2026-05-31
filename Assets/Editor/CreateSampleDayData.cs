using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Editor utility: DentalGame > Create Sample Day Data
/// Generates ready-to-use ProcedureData, PatientData, and DayData assets
/// so you can hit Play immediately after opening the project.
/// </summary>
public static class CreateSampleDayData
{
    private const string OutputPath = "Assets/ScriptableObjects/SampleDay";

    [MenuItem("DentalGame/Create Sample Day Data")]
    public static void Create()
    {
        Directory.CreateDirectory(Application.dataPath + "/../" +
                                  OutputPath.Replace("Assets/", "Assets/"));
        AssetDatabase.Refresh();

        // ── Procedures ────────────────────────────────────────────────────

        var cleaning = CreateProcedure(
            "Procedure_Cleaning",
            ProcedureType.Cleaning,
            "Routine Cleaning",
            new[] { 1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f },
            new[] { ToolType.Mirror, ToolType.Probe, ToolType.Scaler },
            dirtySpots: 4,
            duration: 10f);

        var filling = CreateProcedure(
            "Procedure_Filling",
            ProcedureType.Filling,
            "Tooth Filling",
            new[] { 0.8f, 1.6f, 2.4f, 3.2f, 4.0f, 4.8f, 5.6f },
            new[] { ToolType.Mirror, ToolType.Drill, ToolType.FillingSyringe, ToolType.Suction },
            dirtySpots: 5,
            duration: 12f);

        var extraction = CreateProcedure(
            "Procedure_Extraction",
            ProcedureType.Extraction,
            "Tooth Extraction",
            new[] { 1.0f, 2.0f, 2.5f, 3.5f, 4.5f, 5.0f, 6.0f, 7.0f },
            new[] { ToolType.Mirror, ToolType.Elevator, ToolType.Forceps, ToolType.Suction },
            dirtySpots: 6,
            duration: 14f);

        var rootCanal = CreateProcedure(
            "Procedure_RootCanal",
            ProcedureType.RootCanal,
            "Root Canal",
            new[] { 0.5f, 1.0f, 1.5f, 2.5f, 3.0f, 3.5f, 4.5f, 5.5f, 6.5f },
            new[] { ToolType.Mirror, ToolType.Probe, ToolType.Drill, ToolType.Suction, ToolType.FillingSyringe },
            dirtySpots: 7,
            duration: 16f);

        // ── Patients (5 per doctor = 10 total) ───────────────────────────

        // Dr. Pak patients
        var p0 = CreatePatient("Patient_Mia",     "Mia",     0, RoomId.DrPak,  cleaning);
        var p1 = CreatePatient("Patient_Jun",     "Jun",     1, RoomId.DrPak,  filling);
        var p2 = CreatePatient("Patient_Sara",    "Sara",    2, RoomId.DrPak,  extraction);
        var p3 = CreatePatient("Patient_Tom",     "Tom",     3, RoomId.DrPak,  rootCanal);
        var p4 = CreatePatient("Patient_Lily",    "Lily",    4, RoomId.DrPak,  cleaning);

        // Dr. Seol patients
        var p5 = CreatePatient("Patient_Kevin",   "Kevin",   1, RoomId.DrSeol, filling);
        var p6 = CreatePatient("Patient_Emma",    "Emma",    2, RoomId.DrSeol, cleaning);
        var p7 = CreatePatient("Patient_James",   "James",   0, RoomId.DrSeol, extraction);
        var p8 = CreatePatient("Patient_Nina",    "Nina",    3, RoomId.DrSeol, rootCanal);
        var p9 = CreatePatient("Patient_Carlos",  "Carlos",  4, RoomId.DrSeol, filling);

        // ── Day Data ──────────────────────────────────────────────────────
        // Interleave Dr Pak and Dr Seol patients so both rooms stay busy

        var day = ScriptableObject.CreateInstance<DayData>();
        day.patients = new PatientData[]
        {
            p0, p5,   // round 1: one per doctor
            p1, p6,
            p2, p7,
            p3, p8,
            p4, p9
        };
        SaveAsset(day, "Day_01");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("[DentalGame] Sample day data created at " + OutputPath);
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = day;
    }

    // ─── Helpers ──────────────────────────────────────────────────────────

    private static ProcedureData CreateProcedure(
        string assetName,
        ProcedureType type,
        string displayName,
        float[] beatTimes,
        ToolType[] tools,
        int dirtySpots,
        float duration)
    {
        var p          = ScriptableObject.CreateInstance<ProcedureData>();
        p.type         = type;
        p.displayName  = displayName;
        p.totalDuration = duration;
        p.dirtySpotCount = dirtySpots;

        var notes = new BeatNote[beatTimes.Length];
        for (int i = 0; i < beatTimes.Length; i++)
            notes[i] = new BeatNote { time = beatTimes[i] };
        p.beatNotes = notes;

        p.toolSequence = tools;
        SaveAsset(p, assetName);
        return p;
    }

    private static PatientData CreatePatient(
        string assetName,
        string patientName,
        int spriteIndex,
        RoomId room,
        ProcedureData procedure)
    {
        var p          = ScriptableObject.CreateInstance<PatientData>();
        p.patientName  = patientName;
        p.spriteIndex  = spriteIndex;
        p.assignedRoom = room;
        p.procedure    = procedure;
        SaveAsset(p, assetName);
        return p;
    }

    private static void SaveAsset(ScriptableObject obj, string name)
    {
        string path = $"{OutputPath}/{name}.asset";
        AssetDatabase.CreateAsset(obj, path);
    }
}
