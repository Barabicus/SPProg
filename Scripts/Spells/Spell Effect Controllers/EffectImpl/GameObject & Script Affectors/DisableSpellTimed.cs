using UnityEngine;
using System.Collections;

public class DisableSpellTimed : SpellEffect
{
    [Tooltip("The countdown time until the spell should be disabled")]
    public float disableTime = 1f;

    private Timer countdownTimer;

    protected override void Start()
    {
        base.Start();
        countdownTimer = new Timer(disableTime);
    }

    protected override void UpdateSpell()
    {
        base.UpdateSpell();
        if (countdownTimer.CanTick)
        {
            effectSetting.spell.enabled = false;
            enabled = false;
        }
    }
}
