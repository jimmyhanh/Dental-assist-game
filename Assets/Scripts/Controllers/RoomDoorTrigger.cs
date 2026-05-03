using UnityEngine;

/// <summary>
/// Attach to each room-door collider in the scene.
/// Forwards mouse clicks to PatientManager for routing.
/// </summary>
public class RoomDoorTrigger : MonoBehaviour
{
    [Tooltip("Which room this door leads to.")]
    public RoomId roomId;

    void OnMouseDown()
    {
        PatientManager.Instance?.OnRoomDoorClicked(roomId);
    }
}
