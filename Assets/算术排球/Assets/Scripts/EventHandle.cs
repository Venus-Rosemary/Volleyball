using System;
using UnityEngine;

public class EventHandle
{

	public static event Action EventAnswerRight;
	public static void CallEventAnswerRight()
	{
		Action events = EventHandle.EventAnswerRight;
		if (events == null)
		{
			return;
		}
		events();
	}

	public static event Action EventBallFly;
	public static void CallEventBallFly()
	{
		Action events = EventHandle.EventBallFly;
		if (events == null)
		{
			return;
		}
		events();
	}

	public static event Action<int,int> EventShowAnswer;
	public static void CallEventShowAnswer(int index,int value)
	{
		Action<int,int> events = EventHandle.EventShowAnswer;
		if (events == null)
		{
			return;
		}
		events(index,value);
	}

	public static event Action EventAnswerErr;
	public static void CallEventAnswerErr()
	{
		Action events = EventHandle.EventAnswerErr;
		if (events == null)
		{
			return;
		}
		events();
	}


	public static event Action<int> EventAddScore;
	public static void CallEventAddScore(int score)
	{
		Action<int> events = EventHandle.EventAddScore;
		if (events == null)
		{
			return;
		}
		events(score);
	}

	public static event Action EventFreshQuestNum;
	public static void CallEventFreshQuestNum()
	{
		Action events = EventHandle.EventFreshQuestNum;
		if (events == null)
		{
			return;
		}
		events();
	}
}
