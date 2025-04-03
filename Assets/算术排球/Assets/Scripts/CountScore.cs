using System.Collections;
using TMPro;
using UnityEngine;

public class CountScore : MonoBehaviour
{
    private TextMeshProUGUI txt_score;
    private int score;
    void Awake()
    {
        txt_score = GetComponent<TextMeshProUGUI>();
        AddScore(0);
        EventHandle.EventAddScore += AddScore;
    }

    // Update is called once per frame
    void AddScore(int value)
    {
        score += value;
        txt_score.text = score.ToString();
    }

    public void Restart()
    {
        score = 0;
        AddScore(0);
    }

    private void Update()
    {
        UpdateScore();
    }

    private int showScore;
    private int showFrame;
    private void UpdateScore()
    {
        if (Time.frameCount - showFrame < 10)
            return;
        showFrame = Time.frameCount;
        if (showScore < score)
        {
            showScore += 1;
            txt_score.text = showScore.ToString();
        }
        else if (showScore > score)
        {
            showScore = score;
            txt_score.text = showScore.ToString();
        }
    }
}