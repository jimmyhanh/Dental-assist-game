using UnityEngine;

/// <summary>
/// Central audio facade. Persists across scene loads.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    public AudioSource procedureSource;

    [Header("BGM Clips")]
    public AudioClip mainMenuBGM;
    public AudioClip waitingRoomBGM;

    [Header("SFX Clips")]
    public AudioClip perfectSFX;
    public AudioClip goodSFX;
    public AudioClip missSFX;
    public AudioClip toolCorrectSFX;
    public AudioClip toolWrongSFX;
    public AudioClip cleanSFX;
    public AudioClip patientArriveSFX;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ─── BGM ──────────────────────────────────────────────────────────────

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null || bgmSource.clip == clip) return;
        bgmSource.clip  = clip;
        bgmSource.loop  = true;
        bgmSource.Play();
    }

    public void StopBGM() => bgmSource.Stop();

    // ─── Procedure Music ─────────────────────────────────────────────────

    /// <summary>
    /// Schedules the procedure clip to start at <paramref name="dspTime"/>.
    /// Returns the actual dspTime used so the caller can sync note positions.
    /// </summary>
    public double PlayProcedureMusicScheduled(AudioClip clip, double dspTime)
    {
        if (clip == null) return dspTime;
        procedureSource.clip = clip;
        procedureSource.PlayScheduled(dspTime);
        return dspTime;
    }

    public void StopProcedureMusic() => procedureSource.Stop();

    // ─── SFX ─────────────────────────────────────────────────────────────

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }

    public void PlayAccuracySFX(AccuracyRating rating)
    {
        switch (rating)
        {
            case AccuracyRating.Perfect: PlaySFX(perfectSFX); break;
            case AccuracyRating.Good:    PlaySFX(goodSFX);    break;
            case AccuracyRating.Miss:    PlaySFX(missSFX);    break;
        }
    }
}
