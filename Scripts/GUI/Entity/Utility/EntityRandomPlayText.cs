using UnityEngine;
using System.Collections;

public class EntityRandomPlayText : EntityComponent
{

    public float randomMinTime = 1f;
    public float randomMaxTime = 2f;
    public string[] playTexts;

    private Timer _playTimer;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        SetupTimer();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (_playTimer.CanTickAndReset())
        {
            if (playTexts.Length > 0)
            {
                Entity.SpeechBubble.QueueText(playTexts[Random.Range(0, playTexts.Length)]);
                SetupTimer();
            }
        }
    }

    private void SetupTimer()
    {
        _playTimer = new Timer(Random.Range(randomMinTime, randomMaxTime));
    }
}
