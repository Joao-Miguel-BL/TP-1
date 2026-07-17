using System.Collections;
using TMPro;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    [Header("Objetos da partida")]
    [SerializeField] private Rigidbody2D ballRigidbody;
    [SerializeField] private Rigidbody2D player1Rigidbody;
    [SerializeField] private Rigidbody2D player2Rigidbody;
    [SerializeField] private Transform ballSpawn;

    [Header("Configuração da partida")]
    [SerializeField] private int maxScore = 5;
    [SerializeField] private float resetDelay = 1f;
    [SerializeField] private float matchRestartDelay = 3f;

    [Header("Interface")]
    [SerializeField] private TMP_Text scoreText;

    [Header("Áudio")]
    [SerializeField] private GameAudio gameAudio;

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

        player1Score = 0;
        player2Score = 0;

        UpdateScoreText();
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
            Debug.LogWarning(
                "Número de jogador inválido ao registrar o gol."
            );

            resettingRound = false;
            return;
        }

        UpdateScoreText();

        Debug.Log(
            "Gol do Jogador " + scoringPlayer +
            "! Placar: " +
            player1Score + " x " + player2Score
        );

        bool reachedMaximumScore =
            player1Score >= maxScore ||
            player2Score >= maxScore;

        if (reachedMaximumScore)
        {
            if (gameAudio != null)
            {
                gameAudio.PlayVictory();
            }

            FinishMatch(scoringPlayer);
            return;
        }

        if (gameAudio != null)
        {
            gameAudio.PlayGoal();
        }

        StartCoroutine(ResetRound());
    }

    private IEnumerator ResetRound()
    {
        DisableBall();

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

    private void FinishMatch(int winningPlayer)
    {
        matchFinished = true;

        DisableBall();

        StopBody(player1Rigidbody);
        StopBody(player2Rigidbody);

        SetPlayerControls(
            player1Rigidbody,
            false
        );

        SetPlayerControls(
            player2Rigidbody,
            false
        );

        Debug.Log(
            "Jogador " + winningPlayer +
            " venceu a partida!"
        );

        StartCoroutine(RestartMatchAfterDelay());
    }

    private IEnumerator RestartMatchAfterDelay()
    {
        yield return new WaitForSeconds(matchRestartDelay);

        player1Score = 0;
        player2Score = 0;

        UpdateScoreText();

        ResetBody(
            player1Rigidbody,
            player1StartPosition
        );

        ResetBody(
            player2Rigidbody,
            player2StartPosition
        );

        ResetBall();

        SetPlayerControls(
            player1Rigidbody,
            true
        );

        SetPlayerControls(
            player2Rigidbody,
            true
        );

        matchFinished = false;
        resettingRound = false;

        Debug.Log("Nova partida iniciada.");
    }

    private void DisableBall()
    {
        if (ballRigidbody == null)
        {
            return;
        }

        StopBody(ballRigidbody);
        ballRigidbody.gameObject.SetActive(false);
    }

    private void ResetBall()
    {
        if (ballRigidbody == null || ballSpawn == null)
        {
            return;
        }

        ballRigidbody.transform.position =
            ballSpawn.position;

        ballRigidbody.transform.rotation =
            Quaternion.identity;

        ballRigidbody.gameObject.SetActive(true);

        ballRigidbody.position =
            ballSpawn.position;

        ballRigidbody.rotation = 0f;
        ballRigidbody.linearVelocity = Vector2.zero;
        ballRigidbody.angularVelocity = 0f;

        Physics2D.SyncTransforms();
        ballRigidbody.WakeUp();
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
        body.transform.rotation = Quaternion.identity;

        body.linearVelocity = Vector2.zero;
        body.angularVelocity = 0f;

        body.WakeUp();
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

    private void UpdateScoreText()
    {
        if (scoreText == null)
        {
            return;
        }

        scoreText.text =
            player1Score + "  x  " + player2Score;
    }

    private void SetPlayerControls(
        Rigidbody2D playerBody,
        bool controlsEnabled
    )
    {
        if (playerBody == null)
        {
            return;
        }

        PlayerController controller =
            playerBody.GetComponent<PlayerController>();

        PlayerKick kick =
            playerBody.GetComponent<PlayerKick>();

        if (controller != null)
        {
            controller.enabled = controlsEnabled;
        }

        if (kick != null)
        {
            kick.enabled = controlsEnabled;
        }
    }
}