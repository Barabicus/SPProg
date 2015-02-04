using UnityEngine;
using System.Collections;

public class SpawnEntityAreaEvent : GameEvent
{
    public Entity[] entityPrefab;
    public float radius = 10f;
    public int spawnAmount = 10;
    public int spawnAttempts = 20;

    public override void TriggerEvent(Collider other)
    {
        base.TriggerEvent(other);
        int spawned = 0;
        for (int i = 0; i < spawnAttempts && spawned != spawnAmount; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(new Ray(transform.position + new Vector3(Random.Range(-radius / 2, radius /2), 500, Random.Range(-radius / 2, radius / 2)), -Vector3.up), out hit))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    int randIndex = Random.Range(0, entityPrefab.Length);
                    Instantiate(entityPrefab[randIndex], hit.point, Quaternion.identity);
                    spawned++;
                }
            }
        }
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, new Vector3(radius, radius, radius));
    }



}
