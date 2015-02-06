using UnityEngine;
using System.Collections;
using System;

public abstract class TimedUpdateableEvent : GameEvent
{
    public float updateTime = 1f;

    private float _currentTime = 0f;
    private bool _eventTriggered = false;
    /// <summary>
    /// The update percent amount from 0 - 1
    /// </summary>
    public float UpdatePercent
    {
        get { return _currentTime / updateTime; }
    }

    public event Action eventFinished;

    private Timer timer;


    public override void TriggerEvent(Collider other)
    {
        base.TriggerEvent(other);
        _eventTriggered = true;
    }

    public override void Update()
    {
        base.Update();
        if (UpdatePercent == 1f)
        {
            enabled = false;
            return;
        }
        if (_eventTriggered)
            EventTriggeredUpdate();
    }

    protected virtual void EventTriggeredUpdate()
    {
        _currentTime = Mathf.Min(_currentTime + Time.deltaTime, updateTime);
    }



}
