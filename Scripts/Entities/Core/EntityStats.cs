using UnityEngine;
using System.Collections;

public struct EntityStats
{
    public float speed;
    public float maxHP;

    public EntityStats(float speed, float maxHP)
    {
        this.speed = speed;
        this.maxHP = maxHP;
    }

    public static EntityStats operator +(EntityStats e1, EntityStats e2)
    {
        return new EntityStats(e1.speed + e2.speed, e1.maxHP + e2.maxHP);
    }

    public static EntityStats operator -(EntityStats e1, EntityStats e2)
    {
        return new EntityStats(e1.speed - e2.speed, e1.maxHP - e2.maxHP);
    }

    public EntityStats Difference(EntityStats other)
    {
        return new EntityStats(speed + (other.speed * -1), maxHP + (other.maxHP * -1));
    }

    public override string ToString()
    {
        return "(" + speed + " : " + maxHP +")";
    }

}