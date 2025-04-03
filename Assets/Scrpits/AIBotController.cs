using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBotController : Singleton<AIBotController>
{
    public float moveSpeed = 3f;
    public float smoothTime = 0.3f;

    private Vector3 velocity = Vector3.zero;
    public Vector3 targetPosition;

    public void SetTarget(Vector3 startPos, Vector3 position)
    {
        // 将世界坐标转换为游戏空间进行计算
        Vector3 gameSpaceStart = CoordinateHelper.WorldToGameSpace(startPos);
        Vector3 gameSpacePos = CoordinateHelper.WorldToGameSpace(position);
        
        Vector3 displacement = gameSpacePos - gameSpaceStart;
        Vector3 horizontalDirection = new Vector3(displacement.x, 0, displacement.z).normalized;
        
        // 计算目标位置并转回世界空间
        Vector3 gameSpaceTarget = new Vector3(gameSpacePos.x, gameObject.transform.position.y, gameSpacePos.z);
        targetPosition = CoordinateHelper.GameToWorldSpace(gameSpaceTarget);
    }

    void Update()
    {
        if (targetPosition != Vector3.zero)
        {
            Vector3 direction = targetPosition - transform.position;
            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
        }
    }

    public float hitCooldown = 0.5f;
    private float lastHitTime = 0f;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            // ����Ƿ���Ի�����ȴʱ�䣩
            if (Time.time - lastHitTime >= hitCooldown)
            {
                lastHitTime = Time.time;

                // ��ȡ���������������ҳ���
                NewBallMovement ball = other.GetComponent<NewBallMovement>();
                if (ball != null)
                {
                    ball.LaunchToPlayerCourt();

                    // �������ӻ�����Ч����Ч
                    // PlayHitSound();
                }
            }
        }
    }
}
