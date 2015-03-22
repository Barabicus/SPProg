using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class FPSCounter : MonoBehaviour
{

    public float waitForSecondsOnStart = 1f;
    private Text text;
    private Timer timer;
    private int frameCount;
    private int lastFrameCount;
    private int? lowestFrameCount = null;

    public string Output
    {
        get { return "Current FPS: " + lastFrameCount + " Lowest FPS: " + lowestFrameCount.Value; }
    }

    void Start()
    {
        text = GetComponent<Text>();
        timer = new Timer(1f);
        StartCoroutine(UpdateFrames());
    }

    IEnumerator UpdateFrames()
    {
        {
            yield return new WaitForSeconds(waitForSecondsOnStart);
            timer.Reset();
            while (true)
            {
                frameCount++;
                if (timer.CanTickAndReset())
                {
                    lastFrameCount = frameCount;
                    if (lowestFrameCount == null || frameCount < lowestFrameCount.Value)
                        lowestFrameCount = frameCount;

                    frameCount = 0;
                    text.text = Output;
                }
                yield return null;
            }
        }
    }
}
