using System.Collections;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    [Header("Objetos da partida")]
    [SerializeField] private Rigidbody2D ballRigidbody;
    [SerializeField] private Rigidbody2D player1Rigidbody;
    [SerializeField] private Rigidbody2D player2Rigidbody;
    [SerializeField] private Transform ballSpawn;

    [Header("Partida")]
    [SerializeField] private int maxScore = 5;
    [SerializeField] private float resetDelay = 1f;

    private Vector2 player1StartPosition;
    private Vector2 player2StartPosition;

    private int player1Score;
    private int player2Score;

    private bool resettingRound;
    private bool matchFinished;

    private void Awake()
    {
        if (player1Rigidbody != null)
        {
            player1StartPosition = player1Rigidbody.position;
        }

        if (player2Rigidbody != null)
        {
            player2StartPosition = player2Rigidbody.position;
        }

        if (ballRigidbody == null)
        {
            Debug.LogError(
                "O Rigidbody 2D da bola não foi configurado."
            );
        }

        if (ballSpawn == null)
        {
            Debug.LogError(
                "O BallSpawn não foi configurado."
            );
        }
    }

    public void RegisterGoal(int scoringPlayer)
    {
        if (resettingRound || matchFinished)
        {
            return;
        }

        resettingRound = true;

        if (scoringPlayer == 1)
        {
            player1Score++;
        }
        else if (scoringPlayer == 2)
        {
            player2Score++;
        }
        else
        {
            resettingRound = false;
            return;
        }

        Debug.Log(
            "Gol do Jogador " + scoringPlayer +
            "! Placar: " +
            player1Score + " x " + player2Score
        );

        if (
            player1Score >= maxScore ||
            player2Score >= maxScore
        )
        {
            FinishMatch(scoringPlayer);
            return;
        }

        StartCoroutine(ResetRound());
    }

    private IEnumerator ResetRound()
    {
        // Faz a bola desaparecer imediatamente e impede
        // qualquer nova colisão com o gol.
        if (ballRigidbody != null)
        {
            StopBody(ballRigidbody);
            ballRigidbody.gameObject.SetActive(false);
        }

        StopBody(player1Rigidbody);
        StopBody(player2Rigidbody);

        yield return new WaitForSeconds(resetDelay);

        ResetBody(
            player1Rigidbody,
            player1StartPosition
        );

        ResetBody(
            player2Rigidbody,
            player2StartPosition
        );

        ResetBall();

        resettingRound = false;
    }

    private void ResetBall()
    {
        if (
            ballRigidbody == null ||
            ballSpawn == null
        )
        {
            return;
        }

        // Reposiciona o GameObject antes de reativar a física.
        ballRigidbody.transform.position =
            ballSpawn.position;

        ballRigidbody.transform.rotation =
            Quaternion.identity;

        ballRigidbody.gameObject.SetActive(true);

        // Garante que Rigidbody e Transform fiquem sincronizados.
        ballRigidbody.position = ballSpawn.position;
        ballRigidbody.rotation = 0f;
        ballRigidbody.linearVelocity = Vector2.zero;
        ballRigidbody.angularVelocity = 0f;

        Physics2D.SyncTransforms();
        ballRigidbody.WakeUp();
    }

    private void FinishMatch(int winningPlayer)
    {
        matchFinished = true;

        StopBody(ballRigidbody);
        StopBody(player1Rigidbody);
        StopBody(player2Rigidbody);

        if (ballRigidbody != null)
        {
            ballRigidbody.gameObject.SetActive(false);
        }

        Debug.Log(
            "Jogador " + winningPlayer +
            " venceu a partida!"
        );
    }

    private void StopBody(Rigidbody2D body)
    {
        if (body == null)
        {
            return;
        }

        body.linearVelocity = Vector2.zero;
        body.angularVelocity = 0f;
    }

    private void ResetBody(
        Rigidbody2D body,
        Vector2 startPosition
    )
    {
        if (body == null)
        {
            return;
        }

        body.position = startPosition;
        body.transform.position = startPosition;
        body.rotation = 0f;
        body.linearVelocity = Vector2.zero;
        body.angularVelocity = 0f;
        body.WakeUp();
    }
}