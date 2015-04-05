using UnityEngine;
using System.Collections;

public class DestroyColliders : SpellEffect
{
    public bool destroyOnCollision;
    public float destroyDelay;

    private Collider[] _colliders;

    protected override void Start()
    {
        base.Start();
        _colliders = GetComponents<Collider>();
    }

    protected override void effectSetting_OnSpellDestroy(object sender, SpellEventargs e)
    {
        base.effectSetting_OnSpellDestroy(sender, e);
        Invoke("DestroyCollider", destroyDelay);
    }

    protected override void effectSetting_OnSpellCollision(ColliderEventArgs args, Collider obj)
    {
        base.effectSetting_OnSpellCollision(args, obj);
        Invoke("DestroyCollider", destroyDelay);        
    }

    private void DestroyCollider()
    {
        foreach (Collider c in _colliders)
        {
            Destroy(c);
        }
        enabled = false;
    }

}
