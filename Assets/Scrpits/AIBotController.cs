using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBotController : Singleton<AIBotController>
{
    public float moveSpeed = 3f;
    public float smoothTime = 0.3f;

    private Vector3 velocity = Vector3.zero;
    public Vector3 targetPosition;

    void Update()
    {
        if (targetPosition != Vector3.zero)
        {
            // ƽ���ƶ���Ŀ���
            //Vector3 direction = (targetPosition - transform.position).normalized;
            //transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

            Vector3 direction = targetPosition - transform.position;

            float step = moveSpeed * Time.deltaTime;

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
        }
    }

    public void SetTarget(Vector3 startPos, Vector3 position)
    {
        Vector3 displacement = position - startPos;
        Vector3 horizontalDirection = new Vector3(displacement.x, 0, displacement.z).normalized;
        //targetPosition = new Vector3(position.x,gameObject.transform.position.y, gameObject.transform.position.z);
        targetPosition = new Vector3(position.x, gameObject.transform.position.y, position.z); 
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
                Ball ball = other.GetComponent<Ball>();
                if (ball != null)
                {
                    ball.LaunchToPlayerCourt();

                    // ������ӻ�����Ч����Ч
                    // PlayHitSound();
                }
            }
        }
    }
}
