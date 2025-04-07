using TMPro;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("球的预制体")]
    public GameObject ballPrefab;
    private GameObject ballG;

    [Header("发球点")]
    public Transform playerServePoint;
    public Transform aiServePoint;
    private bool isPlayerServe = true;

    [Header("左右手判断")]
    public bool isLeft = false; // 添加左右方向判断
    public GameObject leftImage;
    public GameObject rightImage;

    [Header("分数")]
    public int playerScore = 0;
    public int aiScore = 0;
    public TMP_Text PlayerS;
    public TMP_Text AiS;

    [Header("玩家和ai")]
    public GameObject PlayerG;
    public GameObject AIBotG;

    [Header("UI界面管理")]
    public GameObject StartUI;
    public GameObject GameUI;
    public GameObject EndUI;

    public void SetActiveImage(bool leftB,bool rightB)//左右UI显示
    {
        leftImage.SetActive(leftB);
        rightImage.SetActive(rightB);
    }

    public void StartNewRound()//双方发球
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

    private void SpawnBallAtPlayer()//玩家发球
    {
        AIBotController.Instance.targetPosition = AIBotG.transform.position;
        ballG = Instantiate(ballPrefab, playerServePoint.position, Quaternion.identity);
        ballG.transform.SetParent(playerServePoint);
        ballG.GetComponent<NewBallMovement>().SetServeMode(true);
    }

    private void SpawnBallAtAI()//Ai发球
    {
        AIBotController.Instance.targetPosition = AIBotG.transform.position;
        ballG = Instantiate(ballPrefab, aiServePoint.position, Quaternion.identity);
        ballG.transform.SetParent(aiServePoint);
        ballG.GetComponent<NewBallMovement>().SetServeMode(false);
    }

    void Start()
    {
        PlayerG.SetActive(false);
        AIBotG.SetActive(false);
        //StartNewRound();
        //PlayerS.text = "PlayerScore:" + playerScore.ToString();
        //AiS.text = "AiScore:" + aiScore.ToString();
    }

    private void Update()
    {
        if (playerScore>=10||aiScore>=10)
        {
            GameEnd();
        }
    }

    public void AddPlayerScore()//玩家得分
    {
        playerScore++;
        PlayerS.text="PlayerScore:"+playerScore.ToString();
    }

    public void AddAIScore()//AI得分
    {
        aiScore++;
        AiS.text = "AiScore:" + aiScore.ToString();
    }

    void UIController(bool startUI,bool gameUI,bool endUI)
    {
        StartUI.SetActive(startUI);
        GameUI.SetActive(gameUI);
        EndUI.SetActive(endUI);
    }

    public void InitialSetUp()//初始化设置
    {
        isPlayerServe = true;
        SetActiveImage(false, false);
        playerScore = 0;
        aiScore = 0;
        if (ballG!=null)
        {
            ballG.GetComponent<NewBallMovement>().ClearAllIndicator();
        }
        Destroy(ballG);
        PlayerG.SetActive(false);
        AIBotG.SetActive(false);
        PlayerG.GetComponent<PlayerController>().SetPlayerStartPos();
        AIBotG.GetComponent<AIBotController>().SetAIStartPos();
    }

    public void StartGame()//开始游戏
    {
        UIController(false, true, false);
        InitialSetUp();
        PlayerG.SetActive(true);
        AIBotG.SetActive(true);
        StartNewRound();
        PlayerS.text = "PlayerScore:" + playerScore.ToString();
        AiS.text = "AiScore:" + aiScore.ToString();
    }

    public void GameEnd()//游戏结束操作
    {
        InitialSetUp();
        UIController(false, false, true);
    }

    public void EndPanel()//返回按钮操作
    {
        UIController(true, false, false);
    }

    //退出游戏
    public void ExitClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}