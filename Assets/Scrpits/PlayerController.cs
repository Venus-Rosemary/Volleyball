using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float hitForce = 10f;

    public float boundaryX = 6f; // 左右边界限制
    public float boundaryZ = 16f;
    
    // 添加击球冷却时间
    public float hitCooldown = 0.5f;
    private float lastHitTime = 0f;

    void Update()
    {
        Player_Move();
    }
    #region 移动
    private void Player_Move()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical).normalized;
        transform.position += movement * Time.deltaTime * moveSpeed;

        float clampedX = Mathf.Clamp(transform.position.x, -boundaryX, boundaryX);
        float clampedZ = Mathf.Clamp(transform.position.z, -boundaryZ, -4f);
        transform.position = new Vector3(clampedX, transform.position.y, clampedZ);
    }
    #endregion

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            // 检查是否可以击球（冷却时间）
            if (Time.time - lastHitTime >= hitCooldown)
            {
                lastHitTime = Time.time;
                
                // 获取球的组件并发射
                Ball ball = other.GetComponent<Ball>();
                if (ball != null)
                {
                    ball.LaunchToOpponentCourt();
                    
                    // 可以添加击球音效或特效
                    // PlayHitSound();
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        DrawLine_Boundary(Color.blue, Vector3.zero, new Vector3(boundaryX,0, boundaryZ)); // 绘制移动边界线
        DrawLine_Boundary(Color.green, Vector3.zero, new Vector3(6, 0, 12));// 绘制落点边界线
    }

    //绘制矩形边界
    private void DrawLine_Boundary(Color color, Vector3 center, Vector3 Radius)
    {
        Gizmos.color = color;

        // 绘制矩形边界线
        // 前边
        Gizmos.DrawLine(center + new Vector3(-Radius.x, 0, -Radius.z), center + new Vector3(Radius.x, 0, -Radius.z));
        // 后边
        Gizmos.DrawLine(center + new Vector3(-Radius.x, 0, Radius.z), center + new Vector3(Radius.x, 0, Radius.z));
        // 左边
        Gizmos.DrawLine(center + new Vector3(-Radius.x, 0, -Radius.z), center + new Vector3(-Radius.x, 0, Radius.z));
        // 右边
        Gizmos.DrawLine(center + new Vector3(Radius.x, 0, -Radius.z), center + new Vector3(Radius.x, 0, Radius.z));
    }
}
