using UnityEngine;
using System.Collections;

public abstract class TimedUpdateableEffect : SpellEffect
{
    public bool singleShot = false;
    public float updateDelay = 2f;
    private Timer timer;

    protected override void Start()
    {
        base.Start();
        timer = new Timer(updateDelay);
    }

    protected override void Update()
    {
        if (OnlyUpdateOnSpellEnabled)
            return;

        if (timer.CanTick)
        {
            if (singleShot)
            {
                UpdateSpell();
                enabled = false;
            }
            else
                UpdateSpell();
        }

    }

}
