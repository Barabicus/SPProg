using UnityEngine;
using System.Collections;

public abstract class TimedUpdateableEffect : SpellEffect
{
    public bool singleShot = false;
    public float updateDelay = 2f;
    private Timer timer;

    protected override void OnSpellStart()
    {
        base.OnSpellStart();
        timer = new Timer(updateDelay);
        enabled = true;
        Debug.Log(gameObject);
    }

    protected override void Update()
    {
        if (OnlyUpdateOnSpellEnabled)
            return;

        if (timer.CanTickAndReset())
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
