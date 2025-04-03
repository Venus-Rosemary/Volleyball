using System.Collections;
using UnityEngine;

public class Data 
{
    private static Data _ins;
    public static Data Ins
    {
        get
        {
            if (_ins == null)
                _ins = new Data();
            return _ins;
        }
    }


    private int[] question = new int[3];
    private int[] answer = new int[2];
    private int rightIndex;
    public int createQuesNum;
    
    /// <summary>
    /// 创建问题
    /// </summary>
    /// <returns></returns>
    public int[] CreateQuestion()
    {
        GameMain.instance.setBirdFalse();
        createQuesNum += 1;
        EventHandle.CallEventFreshQuestNum();
        if (Random.Range(0, 10) >= 5)//加法
        {
            question[0] = 1;
            question[1] = Random.Range(0, 10);
            question[2] = Random.Range(0, 10);
            answer[0] = question[1] + question[2];
            answer[1] = GetErrAnswer();
        }
        else//减法
        {
            question[0] = 0;
            question[1] = Random.Range(5, 10);
            question[2] = Random.Range(1, 4);
            answer[0] = question[1] - question[2];
            answer[1] = GetErrAnswer();
        }
        if (Random.Range(0, 20) < 10)
        {
            EventHandle.CallEventShowAnswer(GameConst.leftAnserIndex,answer[0]);//左侧
            EventHandle.CallEventShowAnswer(GameConst.rightAnserIndex, answer[1]);//右侧
            rightIndex = 1;
        }
        else
        {
            EventHandle.CallEventShowAnswer(GameConst.leftAnserIndex, answer[1]);//左侧
            EventHandle.CallEventShowAnswer(GameConst.rightAnserIndex, answer[0]);//右侧
            rightIndex = 2;
        }
        return question;
    }


    //校验答案
    public bool CheckAnswerIsRight(int index)
    {
        return index == rightIndex;
    }

    public int[] GetAnswers()
    {
        return answer;
    }

    private int GetErrAnswer()
    {
        int value = Random.Range(0, 10);
        if (value == answer[0])
        {
            return GetErrAnswer();
        }
        return value;
    }
    public int rightNum = 0;
    public int errNum = 0;
    public void AnswerRight()
    {
        rightNum += 1;
        GameMain.instance.setBirdyes();
        EventHandle.CallEventAddScore(GameConst.scoreAdd);
    }

    public void AnswerErr()
    {
        errNum += 1;
        GameMain.instance.setBirdno();
        EventHandle.CallEventAddScore(GameConst.scoreReduce);
    }

    public void Restart()
    {
        rightNum = 0;
        errNum = 0;
        createQuesNum = 0;
    }
}