using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// One selectable tool button on the ToolTray.
/// Wire the Button component's OnClick to ToolSlot.OnClick().
/// </summary>
public class ToolSlot : MonoBehaviour
{
    [Header("Visuals")]
    public Image  toolIcon;
    public Image  highlightBorder;
    [Tooltip("Sprites indexed by ToolType enum value.")]
    public Sprite[] toolSprites;

    private ToolType toolType;
    private ToolTray parentTray;

    // ─── Setup ────────────────────────────────────────────────────────────

    public void Setup(ToolType type, ToolTray tray)
    {
        toolType   = type;
        parentTray = tray;

        int idx = (int)type;
        if (toolSprites != null && idx < toolSprites.Length && toolIcon != null)
            toolIcon.sprite = toolSprites[idx];

        SetHighlight(false);
    }

    public void SetHighlight(bool on)
    {
        if (highlightBorder != null)
            highlightBorder.gameObject.SetActive(on);
    }

    // ─── Button callback (wire in Inspector) ─────────────────────────────

    public void OnClick()
    {
        parentTray?.NotifyToolSelected(toolType);
    }
}
