using UnityEngine;

/// <summary>
/// Controls the MainMenu scene. Assign a DayData asset in the Inspector
/// to define which workday the player starts.
/// </summary>
public class MainMenuController : MonoBehaviour
{
    [Header("Day to play")]
    public DayData day;

    void Start()
    {
        AudioManager.Instance?.PlayBGM(AudioManager.Instance.mainMenuBGM);
    }

    // ─── Button callbacks ─────────────────────────────────────────────────

    public void OnStartGameClicked()
    {
        if (day == null)
        {
            Debug.LogError("[MainMenuController] No DayData assigned in the Inspector!");
            return;
        }
        GameManager.Instance.StartDay(day);
    }

    public void OnQuitClicked()
    {
        Application.Quit();
    }
}
