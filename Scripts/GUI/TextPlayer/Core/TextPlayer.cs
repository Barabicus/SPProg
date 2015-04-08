using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class TextPlayer : MonoBehaviour
{

    public Text talkText;
    public Image panelImage;
    public float playTextTime = 2f;
    public event Action TextFinished;

    private Queue<string> textQueue = new Queue<string>();
    private string currentText;
    private Timer timer;
    private Color panelStartColor;

    public event Action FinishedPlaying;

    protected virtual void Awake() { }

    protected virtual void Start()
    {
        timer = new Timer(playTextTime);
        panelStartColor = panelImage.color;
        panelImage.color = Color.clear;
    }

    public void QueueText(string text)
    {
        textQueue.Enqueue(text);
    }

    public void PlayNextText()
    {
        if (textQueue.Count == 0 && currentText != null && TextFinished != null)
            TextFinished();

        if (textQueue.Count > 0)
        {
            currentText = textQueue.Dequeue();
        }
        else
        {
            currentText = null;
            if (FinishedPlaying != null)
                FinishedPlaying();
        }
        talkText.text = currentText;
        timer.Reset();

    }

    protected virtual void Update()
    {
        if (currentText == null)
        {
            PlayNextText();
        }
        else if (timer.CanTick)
        {
            PlayNextText();
        }

        FadeBanner();
    }


    private void FadeBanner()
    {
        if (currentText != null)
            panelImage.color = Color.Lerp(panelImage.color, panelStartColor, Time.deltaTime * 2f);
        else
            panelImage.color = Color.Lerp(panelImage.color, Color.clear, Time.deltaTime * 3f);
    }

}
