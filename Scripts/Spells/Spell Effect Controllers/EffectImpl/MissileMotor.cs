using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class MissileMotor : SpellEffect
{
    public float speed = 2f;
    public float randomXPos = 0;
    public float randomYPos = 0;
    public float randomZPos = 0;
    Vector3 direction;

    private bool shouldMove = true;

    protected override void Start()
    {
        base.Start();
        direction = ((effectSetting.spell.SpellTargetPosition.Value + (Vector3.Scale(transform.forward,new Vector3(Random.Range(-randomXPos, randomXPos), Random.Range(-randomYPos, randomYPos), Random.Range(-randomZPos, randomZPos))))) - effectSetting.transform.position).normalized;
        direction.y = 0;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<SphereCollider>().isTrigger = true;
        transform.parent.forward = direction;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != effectSetting.spell.CastingEntity.gameObject && other.gameObject.layer != LayerMask.NameToLayer("Spell") && other.gameObject.layer != LayerMask.NameToLayer("Ground") && other.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))
        {
            effectSetting.TriggerCollision(new ColliderEventArgs(), other);
        }
    }



    protected override void UpdateSpell()
    {
        base.UpdateSpell();
        if (shouldMove)
            effectSetting.transform.position += direction * speed * Time.deltaTime;
    }

    protected override void effectSetting_OnSpellDestroy(object sender, SpellEventargs e)
    {
        base.effectSetting_OnSpellDestroy(sender, e);
        shouldMove = false;
    }

}
