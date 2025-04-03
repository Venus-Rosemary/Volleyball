using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using System.Runtime.InteropServices;
using System.Threading;
public class GameMain : MonoBehaviour
{
    //[DllImport("MediaHolisticTrackingApi", CallingConvention = CallingConvention.Cdecl)]
    //static extern int MediapipeHolisticTracking_Init(int Camer_id = 1, bool isReOpen = false);

    //[DllImport("MediaHolisticTrackingApi", CallingConvention = CallingConvention.Cdecl)]
    //static extern int MediapipeHolisticTrackingDetect_FrameDirect(float[] detect_result, bool show_result_image = false);

    //[DllImport("MediaHolisticTrackingApi", CallingConvention = CallingConvention.Cdecl)]
    //static extern int MediapipeHolisticTracking_Release();

    public Animator animators_1;
    public Animator animators_2;
    public GameObject bird;

    public AudioSource audioSource;
    public AudioSource bgaudio;
    public AudioClip[] Clips;
    public GameObject player;
    public Ball leftBall;
    public Ball rightBall;
    public Animator playerAni;

    private int selectIndex;
    private Ball selectBall;
    private Vector3 aimPos;
    private bool playerCanMove;
    private Vector3 centerPos;
    private Vector2 centerScreenPos;
    private float ballFlyStartTime;

    [SerializeField] public float[] detect_result;
    public Thread detectThread = null;
    public ThreadStart detect = null;
    public int camIndex;
    public int isDetect;
    public float detect_long = 0.3f;
    public float now_position = 0.5f;
    private float CanStartTimer;//是否开始游戏的计时器
    public bool canStart;//判断是否可以开始游戏了
    public static GameMain instance;
    public bool IsSelect;
    void Start()
    {
        InitPos();
        EventHandle.EventBallFly += PlayerCanMoveHandler;
        animators_2.SetInteger("animation", 1);
        animators_1.SetInteger("animation", 0);
        camIndex = 0;
        detect_result = new float[99];
        //MediapipeHolisticTracking_Init(camIndex, false);
        //MediapipeHolisticTrackingDetect_FrameDirect(detect_result, false);

        //detect = new ThreadStart(StartDetec);
        detectThread = new Thread(detect);
        detectThread.Start();
        instance = this;
        setBirdFalse();

        audioSource.PlayOneShot(Clips[0]);
        bgaudio.Pause();
    }


    //public void StartDetec()
    //{
    //    float[] detect_result_Temp;

    //    while (true)
    //    {
    //        //print("detect");
    //        detect_result_Temp = detect_result;
    //        isDetect = MediapipeHolisticTrackingDetect_FrameDirect(detect_result, false);
    //        Thread.Sleep(0);
    //    }
    //}
    private void PlayerCanMoveHandler()
    {
        selectIndex = -1;
        selectBall = null;
        animationState = 0;
        playerCanMove = true;
        ballFlyStartTime = Time.time;
        Invoke("AnswerOutTime",GameConst.AnswerTime);

    }
    /// <summary>
    /// 答题时间超了
    /// </summary>
    private void AnswerOutTime()
    {
        if (selectBall == null)//超时还没有选择
        {
            Data.Ins.AnswerErr();
            EventHandle.CallEventAnswerErr();
        }
    }
    private int animationState = 0;//0 -- stand,1-- left_up,2--right_up

    public void setBirdFalse()
    {
        bird.SetActive(false);
    }
    public void setBirdyes()
    {
        bird.SetActive(true);
        animators_1.SetInteger("animation", 11);
    }
    public void setBirdno()
    {
        bird.SetActive(true);
        animators_1.SetInteger("animation", 12);
    }

    void Update()
    {
        Debug.Log(Data.Ins.createQuesNum);
        //if ((Data.Ins.createQuesNum > GameConst.QuesNum)||(!Question.instance.Gamestart))
        if ((Question.instance.GameOver) || (!Question.instance.Gamestart))
        {
            doStart();
            Debug.Log(canStart);
            if(canStart)
            {
                if(!Question.instance.Gamestart)
                {
                    audioSource.Stop();
                    bgaudio.UnPause();
                    Question.instance.StartGame();
                    now_position= (detect_result[23 * 3] + detect_result[24 * 3]) / 2;
                    detect_long= Mathf.Abs(detect_result[12 * 3 + 1] - detect_result[24 * 3 + 1]);
                }
                else
                {
                    Question.instance.Restart();
                    now_position = (detect_result[23 * 3] + detect_result[24 * 3]) / 2;
                    detect_long = Mathf.Abs(detect_result[12 * 3 + 1] - detect_result[24 * 3 + 1]);
                }
                IsSelect = false;
            }
            return;
        }
            MouseMove();
        var selected = selectIndex;
        if (!IsSelect && selectBall != null && selectIndex != -1 && animationState == 0)
        {
            var value = GameConst.AnswerTime - (Time.time - ballFlyStartTime);
            if (value > 1)
                return;
            if(selectIndex == GameConst.leftAnserIndex)
            {
                playerAni.SetTrigger("left");
                animationState = 1;
            }
            else
            {
                playerAni.SetTrigger("right");
                animationState = 2;
            }
            IsSelect=true;
            player.transform.DOMove(aimPos, value).onComplete += () =>
            {
                selectBall.StopFly();
                SelectOver(selected);
                playerAni.SetTrigger("playOver");
                player.transform.DOMove(centerPos, 0.3f).onComplete += () =>
                {
                    
                };
            };
            selectIndex = -1;
        }
    }

    private void InitPos()
    {
        centerPos = player.transform.position;
        centerScreenPos = Camera.main.WorldToScreenPoint(centerPos);
    }

    private void MouseMove()
    {
        if (!playerCanMove) return;
        float coor = (detect_result[23 * 3] + detect_result[24 * 3]) / 2;
        Debug.Log("coor");
        Debug.Log(coor);
        //float coor_left = 0.5f - detect_long / 2;
        //float coor_right = 0.5f + detect_long / 2;
        float coor_left = now_position - detect_long / 1.5f;
        float coor_right = now_position + detect_long / 1.5f;
        var value = GameConst.AnswerTime - (Time.time - ballFlyStartTime);
        if (value < 0.2f)
            return;
        if (coor<coor_left || coor>coor_right)
        {
            audioSource.Play();
            //玩家移动到目标点
            if (coor > coor_right)//选择right
            {
                Debug.Log("===========================1===");
                selectIndex = GameConst.rightAnserIndex;
                selectBall = rightBall;
                aimPos = new Vector3(55.46f, 0.73f, -185.38f);
                //player.SetTrigger("right");
            }
            else
            {
                Debug.Log("===========================2===");
                selectIndex = GameConst.leftAnserIndex;
                selectBall = leftBall;
                aimPos = new Vector3(58.14f, 0.8f, -181.9f);
                //player.SetTrigger("left");
            }
            playerCanMove = false;
        }
        //if (Input.GetMouseButtonDown(0))
        //{
        //    audioSource.Play();
        //    //玩家移动到目标点
        //    if (Input.mousePosition.x > centerScreenPos.x)//选择right
        //    {
        //        Debug.Log("===========================1===");
        //        selectIndex = GameConst.rightAnserIndex;
        //        selectBall = rightBall;
        //        aimPos = new Vector3(55.46f, 0.73f,-185.38f);
        //        //player.SetTrigger("right");
        //    }
        //    else
        //    {
        //        Debug.Log("===========================2===");
        //        selectIndex = GameConst.leftAnserIndex;
        //        selectBall = leftBall;
        //        aimPos = new Vector3(58.14f, 0.8f, -181.9f);
        //        //player.SetTrigger("left");
        //    }
        //    playerCanMove = false;
        //}
    }

    private void SelectOver(int selected)
    {
        if(Data.Ins.CheckAnswerIsRight(selected))
        {
            Data.Ins.AnswerRight();
            EventHandle.CallEventAnswerRight();
        }
        else
        {
            Data.Ins.AnswerErr();
            EventHandle.CallEventAnswerErr();
        }
    }


    public void doStart()//判断是否满足游戏的开始条件
    {
        float isHigherR;
        float isHignerL;

        isHigherR = -((detect_result[18 * 3 + 1] + detect_result[16 * 3 + 1] + detect_result[20 * 3 + 1] + detect_result[22 * 3 + 1]) / 4 -
            (detect_result[6 * 3 + 1] + detect_result[5 * 3 + 1] + detect_result[4 * 3 + 1] + detect_result[1 * 3 + 1] + detect_result[2 * 3 + 1] + detect_result[3 * 3 + 1]) / 6);
        isHignerL = -((detect_result[15 * 3 + 1] + detect_result[17 * 3 + 1] + detect_result[19 * 3 + 1] + detect_result[21 * 3 + 1]) / 4 -
            (detect_result[6 * 3 + 1] + detect_result[5 * 3 + 1] + detect_result[4 * 3 + 1] + detect_result[1 * 3 + 1] + detect_result[2 * 3 + 1] + detect_result[3 * 3 + 1]) / 6);
        if (CanStartTimer > 0f)
        {
            canStart = true;
            CanStartTimer = 0;
        }
        else if (isHigherR > 0 && isHignerL > 0)
        {
            CanStartTimer += Time.unscaledDeltaTime;
        }
        else
        {
            CanStartTimer = 0;
            canStart = false;
        }
    }

    public void OnExitGame()//定义一个退出游戏的方法
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//如果是在unity编译器中
#else
        Application.Quit();//否则在打包文件中
#endif
    }

}
