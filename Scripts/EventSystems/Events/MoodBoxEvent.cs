using UnityEngine;
using System.Collections;

public class MoodBoxEvent : GameEvent {

    public Texture2D changeTo;
    public AmplifyColorEffect colorEffect;
    public float changeTime = 1f;

    private float _currentTime = 0f;
    public override void TriggerEvent(Collider other)
    {
        base.TriggerEvent(other);
        colorEffect.BlendTo(changeTo, changeTime, () => { });
    }

}
