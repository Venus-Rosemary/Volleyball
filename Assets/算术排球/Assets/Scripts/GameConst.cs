using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class GameConst
{

    public const int AnswerTime = 5;//答题时间,题显示后2s后不能答题
    public const int ErrShowTime = 3;//答错显示时间
    public const int scoreAdd = 10;//答对加分
    public const int scoreReduce = -2;//答错扣分

    public const int QuesNum = 5;//题目数

    public const int leftAnserIndex = 1;
    public const int rightAnserIndex = 2;
}

