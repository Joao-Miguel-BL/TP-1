using System.Collections;
using UnityEngine;

public class PlayerKick : MonoBehaviour
{
    [Header("Controle")]
    [SerializeField] private KeyCode kickKey = KeyCode.F;

    [Header("Área do chute")]
    [SerializeField] private Transform kickPoint;
    [SerializeField] private float kickRadius = 0.75f;
    [SerializeField] private LayerMask ballLayer;

    [Header("Força")]
    [SerializeField] private float kickDirection = 1f;
    [SerializeField] private float horizontalForce = 9f;
    [SerializeField] private float upwardForce = 3.5f;

    [Header("Intervalo")]
    [SerializeField] private float kickCooldown = 0.35f;

    [Header("Animação do pé")]
    [SerializeField] private Transform footPivot;
    [SerializeField] private float swingDuration = 0.12f;
    [SerializeField] private float returnDuration = 0.12f;
    [SerializeField] private float kickAngle = 65f;
    [SerializeField] private float kickLift = 0.18f;
    [SerializeField] private float kickForward = 0.22f;

    private float cooldownTimer;
    private bool isAnimating;

    private Vector3 footStartPosition;
    private Quaternion footStartRotation;

    private void Awake()
    {
        if (footPivot == null)
        {
            return;
        }

        footStartPosition = footPivot.localPosition;
        footStartRotation = footPivot.localRotation;
    }

    private void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (
            Input.GetKeyDown(kickKey) &&
            cooldownTimer <= 0f
        )
        {
            TryKickBall();

            if (footPivot != null && !isAnimating)
            {
                StartCoroutine(AnimateKick());
            }

            cooldownTimer = kickCooldown;
        }
    }

    private void TryKickBall()
    {
        if (kickPoint == null)
        {
            return;
        }

        Collider2D ballCollider = Physics2D.OverlapCircle(
            kickPoint.position,
            kickRadius,
            ballLayer
        );

        if (ballCollider == null)
        {
            return;
        }

        Rigidbody2D ballRigidbody =
            ballCollider.attachedRigidbody;

        if (ballRigidbody == null)
        {
            return;
        }

        ballRigidbody.linearVelocity = new Vector2(
            ballRigidbody.linearVelocity.x * 0.25f,
            ballRigidbody.linearVelocity.y
        );

        Vector2 kickForce = new Vector2(
            kickDirection * horizontalForce,
            upwardForce
        );

        ballRigidbody.AddForce(
            kickForce,
            ForceMode2D.Impulse
        );
    }

    private IEnumerator AnimateKick()
    {
        isAnimating = true;

        float direction =
            kickDirection >= 0f ? 1f : -1f;

        float timer = 0f;

        while (timer < swingDuration)
        {
            timer += Time.deltaTime;

            float progress = Mathf.Clamp01(
                timer / swingDuration
            );

            float easedProgress = Mathf.SmoothStep(
                0f,
                1f,
                progress
            );

            // Movimento semelhante a um quarto de círculo.
            float arcAngle =
                easedProgress * Mathf.PI * 0.5f;

            float horizontalMovement =
                Mathf.Sin(arcAngle) * kickForward;

            float verticalMovement =
                (1f - Mathf.Cos(arcAngle)) * kickLift;

            footPivot.localPosition =
                footStartPosition +
                new Vector3(
                    direction * horizontalMovement,
                    verticalMovement,
                    0f
                );

            footPivot.localRotation =
                footStartRotation *
                Quaternion.Euler(
                    0f,
                    0f,
                    direction *
                    kickAngle *
                    easedProgress
                );

            yield return null;
        }

        Vector3 raisedPosition =
            footPivot.localPosition;

        Quaternion raisedRotation =
            footPivot.localRotation;

        timer = 0f;

        while (timer < returnDuration)
        {
            timer += Time.deltaTime;

            float progress = Mathf.Clamp01(
                timer / returnDuration
            );

            float easedProgress = Mathf.SmoothStep(
                0f,
                1f,
                progress
            );

            footPivot.localPosition = Vector3.Lerp(
                raisedPosition,
                footStartPosition,
                easedProgress
            );

            footPivot.localRotation =
                Quaternion.Slerp(
                    raisedRotation,
                    footStartRotation,
                    easedProgress
                );

            yield return null;
        }

        footPivot.localPosition = footStartPosition;
        footPivot.localRotation = footStartRotation;

        isAnimating = false;
    }

    private void OnDisable()
    {
        if (footPivot == null)
        {
            return;
        }

        footPivot.localPosition = footStartPosition;
        footPivot.localRotation = footStartRotation;

        isAnimating = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (kickPoint == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(
            kickPoint.position,
            kickRadius
        );
    }
}