using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public abstract class MissileSpell : ElementalSpell
{
    public float missileSpeed = 5f;
    private Vector3 direction;

    Rigidbody rigidbody;

    public override void Start()
    {
        base.Start();
        // Ensure the components are properly setup
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;
        GetComponent<SphereCollider>().isTrigger = true;
        transform.position = CastingEntity.transform.position;

        direction = (SpellTargetPosition.Value - CastingEntity.transform.position).normalized;
        direction.y = 0;

    }

    public override abstract SpellID SpellID
    {
        get;
    }

    public override SpellType SpellType
    {
        get { return SpellType.Missile; }
    }

    public override void Update()
    {
        transform.position += direction * missileSpeed * Time.deltaTime;
    }

    public override float SpellLiveTime
    {
        get { return 2f; }
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.layer + " : " + other.gameObject.name);
        if (other.gameObject != CastingEntity.gameObject && other.gameObject.layer == LayerMask.NameToLayer("Entity"))
        {
            DestinationReached();
            ApplySpell(other.GetComponent<Entity>());
        }
    }

    public override abstract ElementalStats ElementalPower
    {
        get;
    }

    public virtual void DestinationReached()
    {
        Invoke("DestroySpell", 0f);
    }
}
