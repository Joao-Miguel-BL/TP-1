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

    private float cooldownTimer;

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
            Kick();
            cooldownTimer = kickCooldown;
        }
    }

    private void Kick()
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

        // Reduz parte da velocidade anterior para o chute
        // conseguir mudar a direção da bola com mais facilidade.
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