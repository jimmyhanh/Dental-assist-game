using UnityEngine;

/// <summary>
/// Thin wrapper around the doctor's Animator. Exposes working/idle state
/// and the speech-bubble helper used during tool hand-off.
/// </summary>
public class DoctorAnimator : MonoBehaviour
{
    private Animator animator;

    // Animator parameter hashes (set in the Animator Controller)
    private static readonly int IsWorkingHash = Animator.StringToHash("IsWorking");

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetWorking(bool working)
    {
        if (animator != null)
            animator.SetBool(IsWorkingHash, working);
    }

    /// <summary>Activates the speech bubble showing the requested tool icon.</summary>
    public void ShowSpeechBubble(SpeechBubble bubble, ToolType tool)
    {
        bubble?.Show(tool);
    }
}
