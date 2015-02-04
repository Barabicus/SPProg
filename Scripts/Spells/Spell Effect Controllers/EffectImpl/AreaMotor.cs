using UnityEngine;
using System.Collections;

public class AreaMotor : SpellEffect
{
    public bool singleFire = true;
    public float checkDelay = 1f;
    public float radius = 5f;

    private float _lastCheckTime;

    protected override void Start()
    {
        base.Start();
    }

    protected override void UpdateSpell()
    {
        base.UpdateSpell();
        if (singleFire)
        {
            CheckCast();
            enabled = false;
        }
        if (singleFire == false && Time.time - _lastCheckTime >= checkDelay)
        {
            _lastCheckTime = Time.time;
            CheckCast();
        }
    }

    private void CheckCast()
    {
        Collider[] colls = Physics.OverlapSphere(transform.parent.position, radius, 1 << 9);
        foreach (Collider c in colls)
        {
            if (c.gameObject != effectSetting.spell.CastingEntity.gameObject)
                effectSetting.TriggerCollision(new ColliderEventArgs(), c);
        }
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.parent.position, radius);
    }



}
