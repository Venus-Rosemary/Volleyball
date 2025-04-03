using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestionInfo : MonoBehaviour
{
    public AudioSource source;
    public AudioClip[] clips;
    public TextMeshProUGUI txt_right;
    public TextMeshProUGUI txt_err;
    public TextMeshProUGUI txt_count;
    // Start is called before the first frame update
    void Start()
    {
        
        EventHandle.EventAnswerRight += FreshRightNum;
        EventHandle.EventFreshQuestNum += FreshQuestNum;
        EventHandle.EventAnswerErr += FreshErrortNum;
    }

    private void FreshRightNum()
    {
        txt_right.text = Data.Ins.rightNum.ToString();
        source.clip = clips[0];
        source.Play();
    }
    private void FreshErrortNum()
    {
        txt_err.text = Data.Ins.errNum.ToString();
        source.clip = clips[1];
        source.Play();
    }

    private void FreshQuestNum()
    {
        txt_count.text = string.Format("{0}/{1}",Data.Ins.createQuesNum,GameConst.QuesNum);
       
    }

    public void Restart()
    {
        FreshRightNum();
        FreshErrortNum();
        FreshQuestNum();
    }
   
}
