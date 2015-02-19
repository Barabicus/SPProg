using UnityEngine;
using System.Collections;

public struct EntityStats
{
    public float speed;
    public float health;

    public EntityStats(float speed, float health)
    {
        this.speed = speed;
        this.health = health;
    }

    public static EntityStats operator +(EntityStats e1, EntityStats e2)
    {
        return new EntityStats(e1.speed + e2.speed, e1.health + e2.health);
    }

    public static EntityStats operator -(EntityStats e1, EntityStats e2)
    {
        return new EntityStats(e1.speed - e2.speed, e1.health - e2.health);
    }

    public EntityStats Difference(EntityStats other)
    {
        return new EntityStats(speed + (other.speed * -1), health + (other.health * -1));
    }

    public override string ToString()
    {
        return "(" + speed + " : " + health +")";
    }

}