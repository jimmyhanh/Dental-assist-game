using UnityEngine;

[CreateAssetMenu(fileName = "NewProcedure", menuName = "DentalGame/Procedure Data")]
public class ProcedureData : ScriptableObject
{
    [Header("Procedure Info")]
    public ProcedureType type;
    public string displayName;

    [Header("Rhythm Phase")]
    public AudioClip procedureMusic;

    [Tooltip("Beat timestamps (seconds from audio start) the player must suction on.")]
    public BeatNote[] beatNotes;
    public float totalDuration = 30f;

    [Header("Tool Phase")]
    [Tooltip("Ordered list of tools the doctor will request during the procedure.")]
    public ToolType[] toolSequence;

    [Header("Cleaning Phase")]
    [Tooltip("How many dirty spots this procedure leaves in the room.")]
    public int dirtySpotCount = 5;
}
