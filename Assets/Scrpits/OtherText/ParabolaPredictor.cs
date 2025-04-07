using UnityEngine;

public class ParabolaPredictor : MonoBehaviour
{
    public int linePoints = 50; // 线段的点数
    public float lineWidth = 0.1f; // 线的宽度
    //public Material lineMaterial; // 线的材质
    public LineRenderer lineRenderer;

    private void Start()
    {
        // 初始化LineRenderer
        //lineRenderer = gameObject.AddComponent<LineRenderer>();
        //lineRenderer.material = lineMaterial;
        lineRenderer.widthMultiplier = lineWidth;
        lineRenderer.positionCount = linePoints;

        // 随机设置起始位置
        transform.position = new Vector3(
            Random.Range(-6f, 6f),
            Random.Range(0f, 4f),
            Random.Range(-13f, -4f)
        );

        // 生成并绘制抛物线
        DrawParabola();
    }

    private void DrawParabola()
    {
        // 生成随机目标点
        Vector3 targetPos = new Vector3(
            Random.Range(-6f, 6f),
            0f,
            Random.Range(4f, 13f)
        );

        // 根据物体高度动态计算网处的高度
        float distanceToNet = Mathf.Abs(transform.position.z);
        float heightAtNet = Mathf.Lerp(10f, 5f, transform.position.y / 4f); // 在y=0时为10，y=4时为5
        
        // 计算抛物线的最高点
        float maxHeight = Mathf.Max(
            heightAtNet,
            transform.position.y + Vector3.Distance(transform.position, targetPos) * 0.3f
        );

        // 使用三个控制点来创建更自然的抛物线
        Vector3[] controlPoints = new Vector3[3];
        controlPoints[0] = transform.position;
        controlPoints[1] = new Vector3(
            (transform.position.x + targetPos.x) * 0.5f,
            maxHeight,
            0f // 在网的位置
        );
        controlPoints[2] = targetPos;

        // 设置点位置
        for (int i = 0; i < linePoints; i++)
        {
            float t = i / (float)(linePoints - 1);
            Vector3 point = CalculateParabolaPoint(
                controlPoints[0],
                controlPoints[1],
                controlPoints[2],
                t
            );
            lineRenderer.SetPosition(i, point);
        }
    }

    private Vector3 CalculateParabolaPoint(Vector3 start, Vector3 mid, Vector3 end, float t)
    {
        // 使用更平滑的贝塞尔曲线计算
        float oneMinusT = 1f - t;
        return
            oneMinusT * oneMinusT * start +
            2f * oneMinusT * t * mid +
            t * t * end;
    }

    // 可以通过调用这个方法重新生成抛物线
    public void RegenerateParabola()
    {
        DrawParabola();
    }

    private void OnDrawGizmos()
    {
        // 在Scene视图中显示起点和终点
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
        
        if (lineRenderer != null && lineRenderer.positionCount > 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(lineRenderer.GetPosition(lineRenderer.positionCount - 1), 0.3f);
        }
    }
}