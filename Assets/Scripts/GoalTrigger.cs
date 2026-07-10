using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class GoalTrigger : MonoBehaviour
{
    [Header("Configuração do gol")]
    [SerializeField] private MatchManager matchManager;
    [SerializeField] private int scoringPlayer = 1;
    [SerializeField] private LayerMask ballLayer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        bool isBall =
            (ballLayer.value & (1 << other.gameObject.layer)) != 0;

        if (!isBall || matchManager == null)
        {
            return;
        }

        matchManager.RegisterGoal(scoringPlayer);
    }
}