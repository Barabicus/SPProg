using UnityEngine;
using System.Collections;

public class SpawnEntityAreaEvent : GameEvent
{
    public Entity[] spawnEntities;

    public int spawnAmount = 1;
    public int spawnAttempts = 5;

    public int xWidth = 5;
    public int zWidth = 5;
    public int yWidth = 50;

    public override void TriggerEnterEvent(Collider other)
    {
        base.TriggerEnterEvent(other);
        int spawned = 0;
        for (int i = 0; i < spawnAttempts && spawned != spawnAmount; i++)
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position + new Vector3(Random.Range(-xWidth / 2, xWidth / 2), 0, Random.Range(-zWidth / 2, zWidth / 2)), -Vector3.up);
            if (Physics.Raycast(ray, out hit, yWidth, 1 << LayerMask.NameToLayer("Ground")))
            {
                Instantiate(spawnEntities[Random.Range(0, spawnEntities.Length)], hit.point, Quaternion.identity);
                spawned++;
            }
        }
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position - new Vector3(0, yWidth / 2, 0), new Vector3(xWidth, yWidth, zWidth));
    }





}
