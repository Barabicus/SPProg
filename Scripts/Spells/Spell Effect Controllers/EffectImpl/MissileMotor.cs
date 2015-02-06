using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class MissileMotor : SpellEffect
{
    public float speed = 2f;
    public AnimationCurve xCurve;
    public AnimationCurve yCurve;
    public AnimationCurve zCurve;


    private float _currentAnimTime = 0f;
    private Vector3 direction;
    private bool shouldMove = true;

    private Vector3 Direction
    {
        get { return direction + transform.TransformDirection(new Vector3(xCurve.Evaluate(_currentAnimTime), yCurve.Evaluate(_currentAnimTime), zCurve.Evaluate(_currentAnimTime))); }
    }

    protected override void Start()
    {
        base.Start();
        direction = ((effectSetting.spell.SpellTargetPosition.Value + transform.forward) - effectSetting.transform.position).normalized;
        direction.y = 0;
        GetComponent<Rigidbody>().isKinematic = true;
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
            effectSetting.transform.position += Direction * speed * Time.deltaTime;
        UpdateAnim();
    }

    private void UpdateAnim()
    {
        _currentAnimTime += Time.deltaTime;
    }

    protected override void effectSetting_OnSpellDestroy(object sender, SpellEventargs e)
    {
        base.effectSetting_OnSpellDestroy(sender, e);
        shouldMove = false;
    }

}
