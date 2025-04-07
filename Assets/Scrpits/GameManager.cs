using TMPro;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("球的预制体")]
    public GameObject ballPrefab;
    [Header("发球点")]
    public Transform playerServePoint;
    public Transform aiServePoint;
    private bool isPlayerServe = true;


    public bool isLeft = false; // 添加左右方向判断
    public GameObject leftImage;
    public GameObject rightImage;

    public int playerScore = 0;
    public int aiScore = 0;
    public TMP_Text PlayerS;
    public TMP_Text AiS;

    public void SetActiveImage(bool leftB,bool rightB)
    {
        leftImage.SetActive(leftB);
        rightImage.SetActive(rightB);
    }

    public void StartNewRound()
    {
        if (isPlayerServe)
        {
            SpawnBallAtPlayer();
        }
        else
        {
            SpawnBallAtAI();
        }
        isPlayerServe = !isPlayerServe; // 切换发球方
    }

    private void SpawnBallAtPlayer()
    {
        GameObject ball = Instantiate(ballPrefab, playerServePoint.position, Quaternion.identity);
        ball.transform.SetParent(playerServePoint);
        ball.GetComponent<NewBallMovement>().SetServeMode(true);
    }

    private void SpawnBallAtAI()
    {
        AIBotController.Instance.targetPosition = AIBotController.Instance.transform.position;
        GameObject ball = Instantiate(ballPrefab, aiServePoint.position, Quaternion.identity);
        ball.transform.SetParent(aiServePoint);
        ball.GetComponent<NewBallMovement>().SetServeMode(false);
        //ball.GetComponent<NewBallMovement>().LaunchToPlayerCourt();
    }

    void Start()
    {
        StartNewRound();
        PlayerS.text = "PlayerScore:" + playerScore.ToString();
        AiS.text = "AiScore:" + aiScore.ToString();
    }

    public void AddPlayerScore()
    {
        playerScore++;
        PlayerS.text="PlayerScore:"+playerScore.ToString();
    }

    public void AddAIScore()
    {
        // AI得分
        aiScore++;
        AiS.text = "AiScore:" + aiScore.ToString();
    }
}