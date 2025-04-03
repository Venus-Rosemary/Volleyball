using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public GameObject ballPrefab;
    public Transform playerServePoint;
    public Transform aiServePoint;
    private bool isPlayerServe = true;


    public bool isLeft = false; // 添加左右方向判断
    public GameObject leftImage;
    public GameObject rightImage;

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
        GameObject ball = Instantiate(ballPrefab, aiServePoint.position, Quaternion.identity);
        ball.transform.SetParent(aiServePoint);
        ball.GetComponent<NewBallMovement>().SetServeMode(false);
        //ball.GetComponent<NewBallMovement>().LaunchToPlayerCourt();
    }

    void Start()
    {
        StartNewRound();
    }
}