using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Short-lived popup that floats upward and fades out displaying the accuracy rating.
/// Requires a CanvasGroup component on the same GameObject.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class AccuracyPopup : MonoBehaviour
{
    public TextMeshProUGUI label;

    private static readonly Color PerfectColor = new Color(1f, 0.84f, 0f);  // gold
    private static readonly Color GoodColor    = Color.green;
    private static readonly Color MissColor    = new Color(0.6f, 0.6f, 0.6f);

    public void Show(AccuracyRating rating)
    {
        label.text  = rating.ToString().ToUpper();
        label.color = rating switch
        {
            AccuracyRating.Perfect => PerfectColor,
            AccuracyRating.Good    => GoodColor,
            _                      => MissColor
        };
        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        var cg       = GetComponent<CanvasGroup>();
        var startPos = transform.localPosition;
        float t      = 0f;

        while (t < 1f)
        {
            t                        += Time.deltaTime * 2f;
            transform.localPosition   = startPos + Vector3.up * (t * 80f);
            if (cg != null) cg.alpha  = 1f - t;
            yield return null;
        }

        Destroy(gameObject);
    }
}
