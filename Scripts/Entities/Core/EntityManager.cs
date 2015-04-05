using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class EntityManager : MonoBehaviour
{

    public static EntityManager Instance { get; set; }

    private List<Entity> _loadedEntities;

    public event Action<Entity> EntityKilled;

    private void Awake()
    {
        Instance = this;
    }

    public void TriggerEntityKilled(Entity entity)
    {
        if (EntityKilled != null)
            EntityKilled(entity);
    }



}
