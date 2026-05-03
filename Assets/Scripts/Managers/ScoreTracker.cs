using UnityEngine;

/// <summary>
/// Tracks all scoring events across a full workday.
/// Persists across scene loads (DontDestroyOnLoad).
/// </summary>
public class ScoreTracker : MonoBehaviour
{
    public static ScoreTracker Instance { get; private set; }

    public int TotalScore     { get; private set; }
    public int PerfectHits    { get; private set; }
    public int GoodHits       { get; private set; }
    public int Misses         { get; private set; }
    public int ToolCorrect    { get; private set; }
    public int ToolMisses     { get; private set; }
    public int WrongRoomCount { get; private set; }

    private const int PerfectPoints     = 100;
    private const int GoodPoints        = 50;
    private const int ToolCorrectPoints = 75;
    private const int WrongRoomPenalty  = 50;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Reset()
    {
        TotalScore     = 0;
        PerfectHits    = 0;
        GoodHits       = 0;
        Misses         = 0;
        ToolCorrect    = 0;
        ToolMisses     = 0;
        WrongRoomCount = 0;
    }

    public void RecordHit(AccuracyRating rating)
    {
        switch (rating)
        {
            case AccuracyRating.Perfect:
                TotalScore += PerfectPoints;
                PerfectHits++;
                break;
            case AccuracyRating.Good:
                TotalScore += GoodPoints;
                GoodHits++;
                break;
            case AccuracyRating.Miss:
                Misses++;
                break;
        }
        UIManager.Instance?.UpdateHUD();
    }

    public void RecordToolResult(bool correct)
    {
        if (correct)
        {
            TotalScore += ToolCorrectPoints;
            ToolCorrect++;
        }
        else
        {
            ToolMisses++;
        }
        UIManager.Instance?.UpdateHUD();
    }

    public void AddPenalty(int points)
    {
        TotalScore = Mathf.Max(0, TotalScore - points);
        WrongRoomCount++;
        UIManager.Instance?.UpdateHUD();
    }

    /// <summary>Returns 1-3 stars based on overall performance.</summary>
    public int CalculateStars()
    {
        int totalEvents = PerfectHits + GoodHits + Misses + ToolCorrect + ToolMisses;
        if (totalEvents == 0) return 1;

        // Weight: perfect=2pts, good=1pt, toolCorrect=1pt, max possible = (hits*2 + toolCorrect)
        float successRate = (float)(PerfectHits * 2 + GoodHits + ToolCorrect)
                          / (float)(totalEvents * 2);

        if (successRate >= 0.85f && WrongRoomCount == 0) return 3;
        if (successRate >= 0.60f) return 2;
        return 1;
    }
}
