//using System.Collections;
//using UnityEngine;
//using DG.Tweening;
//using TMPro;

//public class Ball : MonoBehaviour
//{
//    //左边是1右边是2
//    public int indexType;
//    public TextMeshPro txt_answer;
//    private float startTime = 10;
//    private bool flying = false;
//    private Vector3 startPos;
//    private Vector3[] topPos = { new Vector3(67.88f, 4.92f, -188.1f), new Vector3(66f, 4.92f, -191.1f) };
//    private Vector3[] endPos = { new Vector3(60.5f, 3.51f, -182.5f), new Vector3(57.10f, 3.51f, -186.32f) };
//    private Vector3[] endPos1 = { new Vector3(54.2f, 1f, -178f), new Vector3(50.4f, 1f, -183f) };
//    private Vector3[] endPos2 = { new Vector3(86.81f, 10.9f, -182.6f), new Vector3(65.96f, 10.9f, -203.7f) };
//    //56.53,180
//    private void Awake()
//    {
//        EventHandle.EventShowAnswer += ShowAnswerHandler;
//        EventHandle.EventBallFly += Fly;
//        startPos = transform.position;
//        gameObject.SetActive(false);
//    }

//    private void ShowAnswerHandler(int index,int value)
//    {
//        if(index == indexType)
//        {
//            txt_answer.text = value.ToString();
//        }
//    }

//    private Tweener flyTween;
//    private Sequence seqTween;
//    public void Fly()
//    {
//        gameObject.SetActive(true);
//        flying = true;
//        transform.position = startPos;
//        var startTime = Time.time;
//       seqTween  = DOTween.Sequence();
//        flyTween = DOTween.To(setter: value =>
//        {
//            transform.position = Util.Parabola(startPos, endPos[indexType - 1], topPos[indexType -1].y - startPos.y, value);
//            //Debug.Log(transform.position + "=======" + (Time.time - startTime));
//        }, startValue: 0, endValue: 1, duration: GameConst.AnswerTime).SetEase(Ease.Linear);
//        seqTween.Append(flyTween);
//        var flytween1 = DOTween.To(setter: value =>
//        {
//            transform.position = Util.Parabola(endPos[indexType - 1], endPos1[indexType - 1], 0, value);
//            //Debug.Log(transform.position + "=======" + (Time.time - startTime));
//        }, startValue: 0, endValue: 1, duration: GameConst.AnswerTime/4).SetEase(Ease.Linear);
//        seqTween.Append(flytween1);
//        seqTween.Play();
//    }


//    public void StopFly()
//    {
//        seqTween.Kill();
//        transform.DOMove(endPos2[indexType - 1], 0.2f);
//    }


//}



using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class Ball : MonoBehaviour
{
    //左边是1右边是2
    public int indexType;
    public TextMeshPro txt_answer;
    private float startTime = 10;
    private bool flying = false;
    private Vector3 startPos;
    private Vector3[] topPos = { new Vector3(67.88f, 4.92f, -188.1f), new Vector3(66f, 4.92f, -191.1f) };
    private Vector3[] endPos = { new Vector3(60.5f, 3.51f, -182.5f), new Vector3(57.10f, 3.51f, -186.32f) };
    private Vector3[] endPos1 = { new Vector3(54.2f, 0f, -178f), new Vector3(50.4f, 0f, -183f) };
    private Vector3[] endPos2 = { new Vector3(86.81f, 10.9f, -182.6f), new Vector3(65.96f, 10.9f, -203.7f) };
    //56.53,180
    private void Awake()
    {
        EventHandle.EventShowAnswer += ShowAnswerHandler;
        EventHandle.EventBallFly += Fly;
        startPos = transform.position;
        gameObject.SetActive(false);
    }

    private void ShowAnswerHandler(int index, int value)
    {
        if (index == indexType)
        {
            txt_answer.text = value.ToString();
        }
    }

    private Tweener flyTween;
    private Sequence seqTween;
    public void Fly()
    {
        gameObject.SetActive(true);
        flying = true;
        transform.position = startPos;
        var startTime = Time.time;
        seqTween = DOTween.Sequence();
        flyTween = DOTween.To(setter: value =>
        {
            transform.position = Util.Parabola(startPos, endPos[indexType - 1], topPos[indexType - 1].y - startPos.y, value);
            //Debug.Log(transform.position + "=======" + (Time.time - startTime));
        }, startValue: 0, endValue: 1, duration: GameConst.AnswerTime).SetEase(Ease.Linear);
        seqTween.Append(flyTween);
        var flytween1 = DOTween.To(setter: value =>
        {
            transform.position = Util.Parabola(endPos[indexType - 1], endPos1[indexType - 1], 0, value);
            //transform.position = Util.Parabola(endPos[indexType - 1], startPos, 0, value);
            //Debug.Log(transform.position + "=======" + (Time.time - startTime));
        }, startValue: 0, endValue: 1, duration: GameConst.AnswerTime / 4).SetEase(Ease.Linear);
        seqTween.Append(flytween1);
        seqTween.Play();
    }


    public void StopFly()
    {
        //seqTween.Kill();
        //transform.DOMove(startPos, 0.3f);
        //transform.DOMove(endPos1[indexType - 1], 2f);
        //transform.DOMove(endPos2[indexType - 1], 0.2f);
        //Debug.Log("the pos is");
        //Debug.Log(indexType);
    }


}