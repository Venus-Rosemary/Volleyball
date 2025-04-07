using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TargetParabola : MonoBehaviour
{
    public Transform Dotted_point;//虚线点模板
    public int Dotted_point_count = 20;//虚心点模板数量
    public List<Transform> DottedP = new List<Transform>();//虚线点
    void Start()
    {
        for (int fFor = 0; fFor < Dotted_point_count; fFor++)
        {
            DottedP.Add(Instantiate(Dotted_point));
            DottedP[DottedP.Count - 1].gameObject.SetActive(true);
        }
    }
    public Transform StartPos, EndPos, Pennis;//起始点、结束点、网球
    public Vector3 StartPosVector3, EndPosVector3, SendDirection;//终点位置, 起点位置, 发射方向向量
    public float SendForce, Already_moved_time, Total_movement_time;//发射力度, 已经移动时间, 总移动时间
    void Update()
    {
        EndPosVector3 = EndPos.position;
        StartPosVector3 = StartPos.position;
        Vector3 向量暂存 = EndPosVector3 - (StartPosVector3 + Physics.gravity * (0.5f * Total_movement_time * Total_movement_time));
        Vector3 发射方向和力度 = 向量暂存 / Total_movement_time;
        Pennis.position = StartPosVector3 + 发射方向和力度 * Already_moved_time + Physics.gravity * (0.5f * Already_moved_time * Already_moved_time);
        for (int fFor = 0; fFor < DottedP.Count; fFor++)
        {
            float 虚线时间暂存 = (float)fFor * (Total_movement_time / (float)DottedP.Count);
            DottedP[fFor].position = StartPosVector3 + 发射方向和力度 * 虚线时间暂存 + Physics.gravity * (0.5f * 虚线时间暂存 * 虚线时间暂存);
        }
    }
}
