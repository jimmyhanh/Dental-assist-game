using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Minimal boot scene initialiser. Transitions to MainMenu after any
/// first-frame setup (e.g. async asset loading) is complete.
/// </summary>
public class BootLoader : MonoBehaviour
{
    void Start()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
