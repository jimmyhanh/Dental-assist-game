using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Doctor's speech bubble that shows which tool is being requested.
/// Attach to a UI panel that sits above the doctor sprite.
/// </summary>
public class SpeechBubble : MonoBehaviour
{
    [Header("Icon")]
    public Image toolIcon;
    [Tooltip("Sprites indexed by ToolType enum value.")]
    public Sprite[] toolSprites;

    void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Show(ToolType tool)
    {
        int idx = (int)tool;
        if (toolSprites != null && idx < toolSprites.Length && toolIcon != null)
            toolIcon.sprite = toolSprites[idx];
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
