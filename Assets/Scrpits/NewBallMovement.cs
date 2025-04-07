using System.Collections;
using UnityEngine;

public class NewBallMovement : MonoBehaviour
{
    public GameObject targetIndicatorPrefab; // 目标点指示器预制体
    private GameObject currentIndicator; // 当前的落点指示器
    public GameObject VFXfallPosPrefab;//落点特效指示器
    private GameObject currentVFXfallPos;

    private Vector3 startPos; // 击球起始位置
    private Vector3 targetPos; // 目标位置
    private float moveTime = 0f; // 当前移动时间
    public  float totalTime = 3f; // 总移动时间
    private bool isMoving = false; // 是否在移动
    private Vector3 initialVelocity; // 初始速度
    public bool isServing = false;
    
    // AI场地和玩家场地的范围
    private readonly Vector2 aiFieldXRange = new Vector2(-6f, 6f);
    private readonly Vector2 aiFieldZRange = new Vector2(4f, 13f);
    private readonly Vector2 playerFieldXRange = new Vector2(-6f, 6f);
    private readonly Vector2 playerFieldZRange = new Vector2(-13f, -4f);
    private readonly float[] playerHeights = new float[] { 0f, 2f, 4f };

    public bool top = false;
    public bool centre = false;
    public bool down= false;
    
    private void Start()
    {
        if (GetComponent<Rigidbody>() == null)
        {
            gameObject.AddComponent<Rigidbody>();
        }
        GetComponent<Rigidbody>().useGravity = false; // 我们将手动控制运动
    }
    // 从玩家方向发射到对方场地
    public void LaunchToOpponentCourt()
    {
        SetToPlayerBool(false, false, false);
        LaunchBall(transform.position, false);
    }

    // 从对方场地发射到玩家场地
    public void LaunchToPlayerCourt()
    {
        SetToPlayerBool(false, false, false);
        LaunchBall(transform.position, true);
    }

    public void ClearAllIndicator()
    {
        // 清除之前的指示器
        if (currentIndicator != null)
        {
            Destroy(currentIndicator);
        }
        if (currentVFXfallPos != null) Destroy(currentVFXfallPos);
    }

    public void LaunchBall(Vector3 hitPosition, bool toPlayerField)
    {
        ClearAllIndicator();

        startPos = hitPosition;
        targetPos = GenerateTargetPosition(toPlayerField);

        // 创建落点指示器
        if (targetIndicatorPrefab != null && toPlayerField)
        {
            currentIndicator = Instantiate(targetIndicatorPrefab, targetPos, Quaternion.identity);
        }
        currentVFXfallPos = Instantiate(VFXfallPosPrefab, new Vector3(targetPos.x, 0, targetPos.z), Quaternion.identity);
        // 计算初始速度
        CalculateTrajectory();

        // 重置移动状态
        moveTime = 0f;
        isMoving = true;
        
        // 设置球的位置到起始点
        transform.position = startPos;
        //如果有AI控制器，通知AI
        if (!toPlayerField && FindObjectOfType<AIBotController>() != null)
        {
            Debug.Log("打给ai的");
            AIBotController.Instance.SetTarget(transform.position, targetPos);
        }
    }

    private Vector3 GenerateTargetPosition(bool toPlayerField)
    {
        float x, z, y;
        if (toPlayerField)
        {
            // 随机决定左右方向
            GameManager.Instance.isLeft = Random.value > 0.5f;
            // 根据方向调整x的范围
            if (GameManager.Instance.isLeft)
            {
                GameManager.Instance.SetActiveImage(true,false);
                x = Random.Range(-6f, -2f);
            }
            else
            {
                GameManager.Instance.SetActiveImage(false, true);
                x = Random.Range(2f, 6f);
            }
            z = Random.Range(playerFieldZRange.x, playerFieldZRange.y);
            y = playerHeights[Random.Range(0, playerHeights.Length)];
            switch (y)
            {
                case 0: SetToPlayerBool(false, false, true); break;
                case 2: SetToPlayerBool(false, true, false); break;
                case 4: SetToPlayerBool(true, false, false); break;
            }
        }
        else
        {

            GameManager.Instance.SetActiveImage(false, false);
            // 生成AI场地的目标点
            x = Random.Range(aiFieldXRange.x, aiFieldXRange.y);
            z = Random.Range(aiFieldZRange.x, aiFieldZRange.y);
            y = 0.5f;
        }
    
        // 将游戏空间坐标转换为世界空间
        return CoordinateHelper.GameToWorldSpace(new Vector3(x, y, z));
    }

    private void CalculateTrajectory()
    {
        // 将位置转换到游戏空间进行计算
        Vector3 gameSpaceStart = CoordinateHelper.WorldToGameSpace(startPos);
        Vector3 gameSpaceTarget = CoordinateHelper.WorldToGameSpace(targetPos);
        
        // 根据起始高度动态计算网处的高度
        float heightAtNet = Mathf.Lerp(10f, 5f, gameSpaceStart.y / 4f);
        
        // 计算最高点
        float maxHeight = Mathf.Max(
            heightAtNet,
            gameSpaceStart.y + Vector3.Distance(gameSpaceStart, gameSpaceTarget) * 0.3f
        );
    
        // 设置控制点（在游戏空间中）
        Vector3[] controlPoints = new Vector3[3];
        controlPoints[0] = gameSpaceStart;
        controlPoints[1] = new Vector3(
            (gameSpaceStart.x + gameSpaceTarget.x) * 0.5f,
            maxHeight,
            0f
        );
        controlPoints[2] = gameSpaceTarget;
    
        // 计算初始速度（在游戏空间中）
        float time = totalTime / 2f;
        Vector3 midPoint = CalculateParabolaPoint(controlPoints[0], controlPoints[1], controlPoints[2], 0.5f);
        Vector3 gameSpaceVelocity = (midPoint - gameSpaceStart) / time;
        
        // 转换回世界空间
        initialVelocity = CoordinateHelper.GameToWorldSpace(gameSpaceVelocity);
    }

    private void Update()
    {
        if (!isMoving) return;

        moveTime += Time.deltaTime;
        float t = moveTime / totalTime;
        
        // 在游戏空间中计算抛物线
        Vector3 gameSpaceStart = CoordinateHelper.WorldToGameSpace(startPos);
        Vector3 gameSpaceTarget = CoordinateHelper.WorldToGameSpace(targetPos);
        float heightAtNet = Mathf.Lerp(10f, 8f, gameSpaceStart.y / 4f);
    
        Vector3 gameSpaceMid = new Vector3(
            (gameSpaceStart.x + gameSpaceTarget.x) * 0.5f,
            heightAtNet,
            0f
        );
    
        Vector3 gameSpaceNewPos = CalculateParabolaPoint(
            gameSpaceStart,
            gameSpaceMid,
            gameSpaceTarget,
            t
        );
        
        // 转换回世界空间
        transform.position = CoordinateHelper.GameToWorldSpace(gameSpaceNewPos);
    }
    private Vector3 CalculateParabolaPoint(Vector3 start, Vector3 mid, Vector3 end, float t)
    {
        float oneMinusT = 1f - t;
        return
        oneMinusT * oneMinusT * start +
        2f * oneMinusT * t * mid +
        t * t * end;
    }
    private void SetToPlayerBool(bool Top,bool Centre,bool Down)
    {
        top = Top;
        centre = Centre;
        down = Down;
    }

    public void SetServeMode(bool isPlayerServing)
    {
        isServing = isPlayerServing;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isMoving = false;
            ClearAllIndicator();
            gameObject.GetComponent<Collider>().enabled = false;

            // 判断得分
            Vector3 gameSpacePos = CoordinateHelper.WorldToGameSpace(transform.position);
            if (gameSpacePos.z > 0)
            {
                GameManager.Instance.AddPlayerScore();
            }
            else
            {
                GameManager.Instance.AddAIScore();
            }

            // 延迟一小段时间后销毁球并开始新回合
            StartCoroutine(DestroyBallAndStartNewRound());
        }
    }

    private IEnumerator DestroyBallAndStartNewRound()
    {
        yield return new WaitForSeconds(1f);
        GameManager.Instance.StartNewRound();
        Destroy(gameObject);
    }
}