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

    public GameObject HitBallTrigger;

    void Start()
    {
        if (HitBallTrigger != null)
        {
            HitBallTrigger.SetActive(false);
        }
    }

    void Update()
    {
        Player_Move();
        CheckHitTrigger();
    }

    private void CheckHitTrigger()
    {
        GameObject ball = GameObject.FindGameObjectWithTag("Ball");
        if (ball != null)
        {
            NewBallMovement ballMovement = ball.GetComponent<NewBallMovement>();
            if (ballMovement != null && HitBallTrigger != null)
            {
                bool shouldActivate = false;
                
                if (ballMovement.isServing)
                {
                    // 发球模式，只需要按F键
                    if (Input.GetKey(KeyCode.F))
                    {
                        shouldActivate = true;
                        ballMovement.isServing=false; 
                        HitBallTrigger.transform.localPosition = new Vector3(0, 0, 1);
                    }
                }
                else
                {
                    // 正常击球模式
                    bool correctDirectionKey = (GameManager.Instance.isLeft && Input.GetKey(KeyCode.Q)) || 
                                             (!GameManager.Instance.isLeft && Input.GetKey(KeyCode.E));
                    
                    if (ballMovement.down && Input.GetKey(KeyCode.O) && correctDirectionKey)
                    {
                        shouldActivate = true;
                        HitBallTrigger.transform.localPosition = new Vector3(0, 0, 1);
                    }
                    else if (ballMovement.centre && Input.GetKey(KeyCode.F) && correctDirectionKey)
                    {
                        shouldActivate = true;
                        HitBallTrigger.transform.localPosition = new Vector3(0, 0, 1);
                    }
                    else if (ballMovement.top && Input.GetKey(KeyCode.P) && correctDirectionKey)
                    {
                        shouldActivate = true;
                        HitBallTrigger.transform.localPosition = new Vector3(0, 1.5f, 1);
                    }
                }

                HitBallTrigger.SetActive(shouldActivate);
            }
        }
    }
    #region 移动
    private void Player_Move()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        // 将输入方向转换为世界空间
        Vector3 movement = CoordinateHelper.GameToWorldSpace(new Vector3(moveHorizontal, 0.0f, moveVertical)).normalized;
        transform.position += movement * Time.deltaTime * moveSpeed;

        // 将世界坐标转换回游戏空间进行边界检查
        Vector3 gameSpacePos = CoordinateHelper.WorldToGameSpace(transform.position);
        float clampedX = Mathf.Clamp(gameSpacePos.x, -boundaryX, boundaryX);
        float clampedZ = Mathf.Clamp(gameSpacePos.z, -boundaryZ, -4f);
        
        // 将限制后的坐标转回世界空间
        transform.position = CoordinateHelper.GameToWorldSpace(new Vector3(clampedX, transform.position.y, clampedZ));
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
                NewBallMovement ball = other.GetComponent<NewBallMovement>();
                if (ball != null)
                {
                    ball.transform.SetParent(null);
                    ball.LaunchToOpponentCourt();
                    
                    // 可以添加击球音效或特效
                    // PlayHitSound();
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        DrawLine_Boundary(Color.blue, new Vector3(0,0.3f,0), new Vector3(boundaryX,0, boundaryZ)); // 绘制移动边界线
        DrawLine_Boundary(Color.green, new Vector3(0, 0.3f, 0), new Vector3(6, 0, 12));// 绘制落点边界线
    }

    //绘制矩形边界
    private void DrawLine_Boundary(Color color, Vector3 center, Vector3 Radius)
    {
        Gizmos.color = color;

        // 转换所有顶点到世界空间
        Vector3[] corners = new Vector3[]
        {
            CoordinateHelper.GameToWorldSpace(center + new Vector3(-Radius.x, 0, -Radius.z)),
            CoordinateHelper.GameToWorldSpace(center + new Vector3(Radius.x, 0, -Radius.z)),
            CoordinateHelper.GameToWorldSpace(center + new Vector3(-Radius.x, 0, Radius.z)),
            CoordinateHelper.GameToWorldSpace(center + new Vector3(Radius.x, 0, Radius.z))
        };

        // 绘制边界线
        Gizmos.DrawLine(corners[0], corners[1]); // 前边
        Gizmos.DrawLine(corners[2], corners[3]); // 后边
        Gizmos.DrawLine(corners[0], corners[2]); // 左边
        Gizmos.DrawLine(corners[1], corners[3]); // 右边
    }
}
