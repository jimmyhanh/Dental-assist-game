using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// The tool tray displayed during the tool hand-off mini-game.
/// Displays one ToolSlot per ToolType and relays selections to ToolManager.
/// </summary>
public class ToolTray : MonoBehaviour
{
    [Header("References")]
    public SpeechBubble speechBubble;
    public Image        timerBar;
    public ToolSlot[]   slots;

    /// <summary>Called by ToolManager when the player selects a tool slot.</summary>
    public Action<ToolType> OnToolSelected;

    // ─── Setup ────────────────────────────────────────────────────────────

    public void Setup()
    {
        var allTools = (ToolType[])Enum.GetValues(typeof(ToolType));
        for (int i = 0; i < slots.Length && i < allTools.Length; i++)
        {
            slots[i].Setup(allTools[i], this);
            slots[i].gameObject.SetActive(true);
        }
        UpdateTimerBar(1f);
    }

    // ─── Timer bar ────────────────────────────────────────────────────────

    public void UpdateTimerBar(float fillAmount)
    {
        if (timerBar != null)
            timerBar.fillAmount = Mathf.Clamp01(fillAmount);
    }

    // ─── Internal callback from ToolSlot ─────────────────────────────────

    public void NotifyToolSelected(ToolType tool)
    {
        OnToolSelected?.Invoke(tool);
    }
}
