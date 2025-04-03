using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Question : MonoBehaviour
{
    private Outline outline;
    public TextMeshProUGUI txt_quest;
    public GameObject overPanel;
    private bool showErrEff;

    public GameObject btnStart;
    public GameObject btnRestart;
    public TextMeshProUGUI StateText;
    public Image imgErr;
    public Image imgRight;
    public bool Gamestart;
    public bool GameOver=false;
    public static Question instance;
    // Use this for initialization
    void Start()
    {
        btnStart.SetActive(true);
        btnRestart.SetActive(false);
        EventHandle.EventAnswerRight += AnswerRightHandler;
        EventHandle.EventAnswerErr += AnswerErrHandler;
        outline = GetComponent<Outline>();
        outline.enabled = false;
        instance = this;
        Gamestart = false;
    }

    private string addStr = "{0} + {1} = ?";
    private string subStr = "{0} - {1} = ?";

    private void AnswerErrHandler()
    {
        showErrEff = true;
        outline.enabled = true;
        imgErr.gameObject.SetActive(true);
        imgErr.transform.DOScale(4, 0.5f).From();
        imgErr.DOFade(0, 0.5f).From();
        Invoke("HideErrImag",0.5f);
        ShowQuestionAnswer();
        fadetoDark();
        Invoke("StopErrEff", GameConst.ErrShowTime);
    }

    private void HideErrImag()
    {
        imgErr.gameObject.SetActive(false);
    }
   

    private void StopErrEff()
    {
        showErrEff = false;
        outline.enabled = false;
        HidePreQuestion();
    }

    private void AnswerRightHandler()
    {
        imgRight.gameObject.SetActive(true);
        imgRight.transform.DOScale(4, 0.5f).From();
        imgRight.DOFade(0, 0.5f).From();
        Invoke("HidePreQuestion", 0.5f);
    }

    private void HidePreQuestion()
    {
        imgRight.gameObject.SetActive(false);
        if (Data.Ins.createQuesNum >= GameConst.QuesNum)
        {
            overPanel.SetActive(true);
            btnStart.SetActive(false);
            btnRestart.SetActive(true);
            GameOver = true;
            return;
        }
        txt_quest.transform.DOLocalMoveY(20, 1).onComplete += ShowQuestion;
    }

    private void ShowQuestion()
    {
        var question = Data.Ins.CreateQuestion();
        
        if (question[0] == 0)//减法
        {
            txt_quest.text = string.Format(subStr, question[1], question[2]);
        }
        else
        {
            txt_quest.text = string.Format(addStr, question[1], question[2]);
        }
        TweenShow();
        GameMain.instance.IsSelect = false;
    }

    private void ShowQuestionAnswer()
    {
        int answer = Data.Ins.GetAnswers()[0];
        txt_quest.text = txt_quest.text.Replace("?",answer.ToString());
        GameMain.instance.IsSelect = true;
    }

    private void TweenShow()
    {
        var pos = txt_quest.transform.localPosition;
        pos.y = -20;
        txt_quest.transform.localPosition = pos;
        txt_quest.transform.DOLocalMoveY(0, 1);
        Invoke("TellBallFly", 2);
    }


    private void TellBallFly()
    {
        EventHandle.CallEventBallFly();
    }
    private void fadetoLight()
    {
        if (!showErrEff) return;
        outline.DOFade(0.6f, 0.4f).onComplete += fadetoDark;
    }
    private void fadetoDark()
    {
        if (!showErrEff) return;
        outline.DOFade(0f, 0.4f).onComplete += fadetoLight;
    }

    public void Restart()
    {
        GameOver=false;
        StateText.gameObject.SetActive(false);
        overPanel.SetActive(false);
        Data.Ins.Restart();
        txt_quest.transform.DOLocalMoveY(20, 1).onComplete += ShowQuestion;
    }

    public void StartGame()
    {
        StateText.gameObject.SetActive(true);
        overPanel.SetActive(false);
        Data.Ins.Restart();
        StartCoroutine(waitForBegin(3,false));
        Gamestart = true;
    }

    IEnumerator waitForBegin(int time,bool isRestart)
    {
        StateText.text = time.ToString();
        Tween scale = StateText.transform.DOScale(5, 1).From();
        Tween fade = StateText.DOFade(0, 1).From();
        while (time > -1)
        {
            if (!fade.IsPlaying())
            {
                time--;
                if (time == -1)
                {
                    StateText.gameObject.SetActive(false);
                    ShowQuestion();
                }
                else if (time == 0)
                {
                    StateText.text = "Go!";
                }
                else
                {
                    StateText.text = time.ToString();
                }
                scale = StateText.transform.DOScale(5, 1).From();
                fade = StateText.DOFade(0, 1).From();
            }
            

            yield return new WaitForEndOfFrame();
            
        }
    }
}