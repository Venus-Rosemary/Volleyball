using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TargetParabola : MonoBehaviour
{
    public Transform Dotted_point;//���ߵ�ģ��
    public int Dotted_point_count = 20;//���ĵ�ģ������
    public List<Transform> DottedP = new List<Transform>();//���ߵ�
    void Start()
    {
        for (int fFor = 0; fFor < Dotted_point_count; fFor++)
        {
            DottedP.Add(Instantiate(Dotted_point));
            DottedP[DottedP.Count - 1].gameObject.SetActive(true);
        }
    }
    public Transform StartPos, EndPos, Pennis;//��ʼ�㡢�����㡢����
    public Vector3 StartPosVector3, EndPosVector3, SendDirection;//�յ�λ��, ���λ��, ���䷽������
    public float SendForce, Already_moved_time, Total_movement_time;//��������, �Ѿ��ƶ�ʱ��, ���ƶ�ʱ��
    void Update()
    {
        EndPosVector3 = EndPos.position;
        StartPosVector3 = StartPos.position;
        Vector3 �����ݴ� = EndPosVector3 - (StartPosVector3 + Physics.gravity * (0.5f * Total_movement_time * Total_movement_time));
        Vector3 ���䷽������� = �����ݴ� / Total_movement_time;
        Pennis.position = StartPosVector3 + ���䷽������� * Already_moved_time + Physics.gravity * (0.5f * Already_moved_time * Already_moved_time);
        for (int fFor = 0; fFor < DottedP.Count; fFor++)
        {
            float ����ʱ���ݴ� = (float)fFor * (Total_movement_time / (float)DottedP.Count);
            DottedP[fFor].position = StartPosVector3 + ���䷽������� * ����ʱ���ݴ� + Physics.gravity * (0.5f * ����ʱ���ݴ� * ����ʱ���ݴ�);
        }
    }
}
