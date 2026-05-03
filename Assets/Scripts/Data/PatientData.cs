using UnityEngine;

[CreateAssetMenu(fileName = "NewPatient", menuName = "DentalGame/Patient Data")]
public class PatientData : ScriptableObject
{
    [Header("Identity")]
    public string patientName;

    [Tooltip("Index into the patient sprite sheet / sprite array on PatientController.")]
    public int spriteIndex;

    [Header("Assignment")]
    public RoomId assignedRoom;
    public ProcedureData procedure;
}
