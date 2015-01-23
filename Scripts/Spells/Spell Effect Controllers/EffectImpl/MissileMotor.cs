using UnityEngine;
using System.Collections;

public class MissileMotor : SpellEffect
{
    public float speed = 2f;
    public bool triggerStopMovement = true;
    Vector3 direction;

    private bool shouldMove = true;

    protected override void Start()
    {
        base.Start();
        direction = (effectSetting.spell.SpellTargetPosition.Value - effectSetting.transform.position).normalized;
        direction.y = 0;
    }

    protected override void Update()
    {
        base.Update();
        if (shouldMove)
            effectSetting.transform.position += direction * speed * Time.deltaTime;
    }

    protected override void OnSpellDestroy()
    {
        base.OnSpellDestroy();
        shouldMove = false;
    }

}
