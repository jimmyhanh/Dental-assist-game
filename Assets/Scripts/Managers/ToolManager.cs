using UnityEngine;
using System.Collections;

/// <summary>
/// Orchestrates the tool hand-off mini-game phase for a procedure.
/// </summary>
public class ToolManager : MonoBehaviour
{
    public static ToolManager Instance { get; private set; }

    [Header("Settings")]
    [Tooltip("Seconds the player has to select the correct tool per request.")]
    public float toolTimeWindow = 3f;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    // ─── Game loop ────────────────────────────────────────────────────────

    /// <summary>
    /// Iterates through the procedure's tool sequence, prompting the player
    /// each time and scoring the result.
    /// </summary>
    public IEnumerator RunToolHandoff(ProcedureData procedure,
                                      ToolTray tray,
                                      DoctorAnimator doctor)
    {
        if (procedure.toolSequence == null || procedure.toolSequence.Length == 0)
            yield break;

        tray.Setup();

        foreach (var requiredTool in procedure.toolSequence)
        {
            // Show doctor speech bubble with the required tool
            doctor.ShowSpeechBubble(tray.speechBubble, requiredTool);

            float   timer    = 0f;
            bool    answered = false;
            ToolType selected = default;

            tray.OnToolSelected = (tool) =>
            {
                selected = tool;
                answered = true;
            };

            // Wait for input or timeout
            while (!answered && timer < toolTimeWindow)
            {
                timer += Time.deltaTime;
                tray.UpdateTimerBar(1f - timer / toolTimeWindow);
                yield return null;
            }

            tray.OnToolSelected = null;
            tray.speechBubble.Hide();
            tray.UpdateTimerBar(0f);

            bool correct = answered && selected == requiredTool;
            ScoreTracker.Instance.RecordToolResult(correct);

            if (correct)
            {
                UIManager.Instance?.ShowFeedback("Correct!", UnityEngine.Color.green);
                AudioManager.Instance?.PlaySFX(AudioManager.Instance.toolCorrectSFX);
            }
            else
            {
                string msg = answered ? "Wrong tool!" : "Too slow!";
                UIManager.Instance?.ShowFeedback(msg, UnityEngine.Color.red);
                AudioManager.Instance?.PlaySFX(AudioManager.Instance.toolWrongSFX);
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
}
