using UnityEngine;
using System.Collections;

public class DummyEntity : Entity
{

    public float randomRadius = 20f;

    protected override void Start()
    {
        base.Start();
    }

    //Living MoveToPosition()
    //{
    //    while (true)
    //    {
    //        Vector3 randPosition = new Vector3(Random.Range(-randomRadius, randomRadius), 0f, Random.Range(-randomRadius, randomRadius));
    //        RaycastHit hit;
    //        if (Physics.Raycast(new Ray(transform.position + randPosition, -Vector3.up), out hit, 1000f, 1 << LayerMask.NameToLayer("Ground")))
    //        {
    //            navMeshAgent.SetDestination(hit.point);
    //        }
    //        yield return new WaitForSeconds(Random.Range(1f, 3f));
    //    }
    //}

    protected override void LivingUpdate()
    {
        base.LivingUpdate();
    }

    protected override bool KeepBeamAlive()
    {
        return false;
    }

}
