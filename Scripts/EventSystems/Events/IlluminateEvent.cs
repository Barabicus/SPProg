using UnityEngine;
using System.Collections;

public class IlluminateEvent : GameEvent
{
    public Light light;
    public float fadeInTime = 1f;
    public float toIntensity = 1f;

    private float _currentTime;
    private float _startIntensity;

    public override void Start()
    {
        base.Start();
    }

    public override void TriggerEvent(Collider other)
    {
        base.TriggerEvent(other);
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        _startIntensity = light.intensity;
        for (; _currentTime < fadeInTime; _currentTime+= Time.deltaTime)
        {
            light.intensity = Mathf.Lerp(_startIntensity, toIntensity, _currentTime);
            yield return null;
        }
    }

}
