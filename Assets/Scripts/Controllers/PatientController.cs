using UnityEngine;
using System.Collections;

/// <summary>
/// Handles a patient GameObject: sprite initialisation, walk animation,
/// and mouse-click routing to PatientManager.
/// </summary>
public class PatientController : MonoBehaviour
{
    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;
    [Tooltip("Sprite sheet indexed by PatientData.spriteIndex.")]
    public Sprite[] patientSprites;

    [Header("Movement")]
    [Tooltip("Walk speed in world units per second.")]
    public float walkSpeed = 3f;

    public PatientData Data { get; private set; }

    private Coroutine walkCoroutine;

    // ─── Initialisation ───────────────────────────────────────────────────

    public void Initialize(PatientData data)
    {
        Data = data;
        if (patientSprites != null && data.spriteIndex < patientSprites.Length)
            spriteRenderer.sprite = patientSprites[data.spriteIndex];
    }

    // ─── Movement ─────────────────────────────────────────────────────────

    /// <summary>Starts walking toward <paramref name="target"/> and returns the Coroutine.</summary>
    public Coroutine WalkTo(Vector3 target)
    {
        if (walkCoroutine != null) StopCoroutine(walkCoroutine);
        walkCoroutine = StartCoroutine(WalkCoroutine(target));
        return walkCoroutine;
    }

    private IEnumerator WalkCoroutine(Vector3 target)
    {
        target.z = transform.position.z;
        while (Vector3.Distance(transform.position, target) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, target, walkSpeed * Time.deltaTime);

            // Flip sprite to face direction of travel
            if (spriteRenderer != null)
                spriteRenderer.flipX = target.x < transform.position.x;

            yield return null;
        }
        transform.position = target;
    }

    // ─── Input ────────────────────────────────────────────────────────────

    void OnMouseDown()
    {
        PatientManager.Instance?.OnPatientClicked(this);
    }
}
