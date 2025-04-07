using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovemont : MonoBehaviour
{
    public float maxBounceHeight = 2f; // 最大反弹高度
    public float bounceFactor = 0.5f; // 弹跳系数
    public float maxBounces = 3; // 最大弹跳次数

    public GameObject targetIndicatorPrefab; // 目标落点指示器预制体
    private GameObject targetIndicator;
    private GameObject heightIndicator; // 地面指示器
    private int bounceCount = 0; // 弹跳计数

    // 记录最后一次击球的时间
    private float lastHitTime = 0f;

    // 可选的Y轴高度
    private float[] possibleHeights = new float[] { 0f, 3f, 6f };

    // 添加网高参数
    public float netHeight = 5f; // 网的高度
    public float additionalHeightFactor = 1.2f; // 额外高度系数，确保球能越过网

    Vector3 predictedLandingPos;

    private void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
    }

    // 从玩家方向发射到对方场地
    public void LaunchToOpponentCourt()
    {
        OnHit(transform.position, true);
    }

    // 从对方场地发射到玩家场地
    public void LaunchToPlayerCourt()
    {
        OnHit(transform.position, false);
    }

    public void OnHit(Vector3 hitPosition, bool isPlayerHit = true)
    {
        // 记录击球时间
        lastHitTime = Time.time;

        // 重置弹跳计数
        bounceCount = 0;

        float randomX = Random.Range(-6f, 6f); // 对方场地X范围
        float randomZ;

        if (isPlayerHit)
        {
            randomZ = Random.Range(4f, 13f);
        }
        else
        {
            randomZ = Random.Range(-13f, -4f);
        }

        float targetY = 0.3f;
        if (!isPlayerHit)
        {
            targetY = possibleHeights[Random.Range(0, possibleHeights.Length)];
        }
        else
        {
            targetY = 0.3f;
        }

        Vector3 targetPosition;
        targetPosition = new Vector3(randomX, isPlayerHit ? 0.3f : targetY, randomZ);

        // 计算抛物线轨迹
        LaunchBall(hitPosition, targetPosition, isPlayerHit);

        //如果有AI控制器，通知AI
        if (isPlayerHit && FindObjectOfType<AIBotController>() != null)
        {
            Debug.Log("打给ai的");
            AIBotController.Instance.SetTarget(hitPosition, predictedLandingPos);
        }
    }



    private void LaunchBall(Vector3 startPos, Vector3 targetPos, bool isPlayerHit)
    {
        // 清理之前的指示器
        ClearIndicators();

        // 获取水平距离和方向
        Vector3 displacement = targetPos - startPos;
        float horizontalDistance = new Vector2(displacement.x, displacement.z).magnitude;
        Vector3 horizontalDirection = new Vector3(displacement.x, 0, displacement.z).normalized;

        // 计算网的位置（假设网在z=0处）
        float netPosition = 0f;

        // 计算需要的最小抛物线高度，确保能越过网
        float requiredHeight = netHeight * additionalHeightFactor;

        // 计算从起点到网，以及从网到终点的距离比例
        float startToNetDistance = Mathf.Abs(startPos.z - netPosition);
        float netToTargetDistance = Mathf.Abs(targetPos.z - netPosition);
        float totalDistance = startToNetDistance + netToTargetDistance;

        // 计算网在整个路径中的位置比例
        float netPositionRatio = startToNetDistance / totalDistance;

        // 使用物理公式计算初速度
        float actualGravity = Physics.gravity.magnitude;

        // 计算水平和垂直位移
        float verticalDisplacement = targetPos.y - startPos.y;

        // 使用更精确的时间估算
        float estimatedTime = horizontalDistance / 10f;

        // 从目标点反向计算所需的初始垂直速度
        float v0 = (verticalDisplacement + 0.5f * actualGravity * estimatedTime * estimatedTime) / estimatedTime;

        // 检查在网处的高度是否满足条件
        float timeAtNet = estimatedTime * netPositionRatio;
        float heightAtNet = startPos.y + v0 * timeAtNet - 0.5f * actualGravity * timeAtNet * timeAtNet;

        // 如果高度不够，调整初始速度
        if (heightAtNet < requiredHeight)
        {
            // 计算需要的初始速度以达到网处的所需高度
            float requiredV0 = (requiredHeight - startPos.y + 0.5f * actualGravity * timeAtNet * timeAtNet) / timeAtNet;

            // 使用更高的初始速度
            v0 = Mathf.Max(v0, requiredV0);

            // 重新计算到达目标所需的时间
            float a = 0.5f * actualGravity;
            float b = -v0;
            float c = startPos.y - targetPos.y;

            // 使用求根公式，确保使用正确的解
            float discriminant = b * b - 4 * a * c;
            if (discriminant >= 0)
            {
                // 计算两个解
                float t1 = (-b + Mathf.Sqrt(discriminant)) / (2 * a);
                float t2 = (-b - Mathf.Sqrt(discriminant)) / (2 * a);

                // 选择正值且较大的解（完整抛物线时间）
                if (t1 > 0 && t2 > 0)
                    estimatedTime = Mathf.Max(t1, t2);
                else if (t1 > 0)
                    estimatedTime = t1;
                else if (t2 > 0)
                    estimatedTime = t2;
            }
        }

        // 计算水平速度，确保精确到达目标
        float horizontalSpeed = horizontalDistance / estimatedTime;

        // 设置最终速度向量
        Vector3 velocity = horizontalDirection * horizontalSpeed;
        velocity.y = v0;

        // 应用速度
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = velocity;

        // 计算预测的实际落点
        predictedLandingPos = PredictLandingPosition(transform.position, velocity);

        // 创建目标落点指示器
        if (targetIndicatorPrefab != null)
        {
            if (!isPlayerHit)
            {
                // 创建目标落点指示器
                targetIndicator = Instantiate(targetIndicatorPrefab);
                targetIndicator.transform.position = targetPos;

                // 创建地面指示器（确保在正确的地面高度）
                heightIndicator = Instantiate(targetIndicatorPrefab);
                // 使用目标点的x和z坐标，但y坐标设为0.3f（地面高度）
                heightIndicator.transform.position = new Vector3(targetPos.x, 0.3f, targetPos.z);

                // 为高度指示器设置不同的颜色
                Renderer renderer = heightIndicator.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = Color.yellow;
                }
            }

        }

        // 记录详细的调试信息
        Debug.Log($"目标位置: {targetPos}, 初始速度: {velocity}, 预计时间: {estimatedTime}秒, 网处高度: {heightAtNet}");
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            // 检查是否在击球后的0.5秒内
            if (Time.time - lastHitTime < 0.5f)
            {
                Debug.Log("球在击打后0.5秒内触地，忽略弹跳");
                return;
            }

            // 第一次弹跳时销毁目标指示器
            if (bounceCount == 0)
            {
                ClearIndicators();
            }

            bounceCount++;

            // 如果超过最大弹跳次数，处理得分逻辑
            if (bounceCount >= maxBounces)
            {
                // 根据球的位置判断得分
                if (transform.position.z > 0)
                {
                    // 球在对方场地，玩家得分
                    Debug.Log("玩家得分");
                    // 这里可以调用得分系统
                }
                else if (transform.position.z < 0)
                {
                    // 球在玩家场地，对方得分
                    Debug.Log("对方得分");
                    // 这里可以调用得分系统
                }
                return;
            }

            // 计算反弹速度
            Rigidbody rb = GetComponent<Rigidbody>();
            Vector3 newVelocity = rb.velocity;

            // 设置反弹垂直速度
            newVelocity.y = Mathf.Sqrt(2 * Physics.gravity.magnitude * maxBounceHeight * bounceFactor);

            // 每次弹跳后水平速度略微减小
            newVelocity.x *= 0.95f;
            newVelocity.z *= 0.95f;

            rb.velocity = newVelocity;
        }
    }

    // 供球拍调用的方法
    public void HitByRacket(Vector3 hitPosition, Vector3 hitDirection, float hitForce)
    {
        // 根据击球方向判断是玩家还是对手击球
        bool isPlayerHit = hitDirection.z > 0;

        // 调用OnHit方法处理击球
        OnHit(hitPosition, isPlayerHit);
    }

    // 清理所有指示器
    private void ClearIndicators()
    {
        if (targetIndicator != null)
        {
            Destroy(targetIndicator);
            targetIndicator = null;
        }

        if (heightIndicator != null)
        {
            Destroy(heightIndicator);
            heightIndicator = null;
        }
    }

    // 添加一个方法来预测球的落点，用于调试
    private Vector3 PredictLandingPosition(Vector3 startPos, Vector3 velocity)
    {
        // 计算球落地时的水平位移
        // 使用公式：t = (v0y + sqrt(v0y^2 + 2*g*h0))/g，其中h0是初始高度
        float timeToLand = (velocity.y + Mathf.Sqrt(velocity.y * velocity.y + 2 * Physics.gravity.magnitude * startPos.y)) / Physics.gravity.magnitude;

        // 计算水平位移
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);
        Vector3 landingPos = startPos + horizontalVelocity * timeToLand;
        landingPos.y = 0.3f; // 设置为地面高度

        return landingPos;
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                // 绘制预测轨迹
                Vector3 predictedLanding = PredictLandingPosition(transform.position, rb.velocity);
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, predictedLanding);
                Gizmos.DrawSphere(predictedLanding, 0.2f);
            }
        }
    }
}