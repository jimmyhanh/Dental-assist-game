using UnityEngine;

[CreateAssetMenu(fileName = "NewDay", menuName = "DentalGame/Day Data")]
public class DayData : ScriptableObject
{
    [Tooltip("Ordered list of all patients for this workday. 10-12 for a full day.")]
    public PatientData[] patients;
}
