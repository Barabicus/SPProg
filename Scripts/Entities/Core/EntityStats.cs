using UnityEngine;
using System.Collections;

public struct EntityStats
{
    public float speed;

    public EntityStats(float speed)
    {
        this.speed = speed;
    }

    public static EntityStats operator +(EntityStats e1, EntityStats e2)
    {
        return new EntityStats(e1.speed + e2.speed);
    }

    public static EntityStats operator -(EntityStats e1, EntityStats e2)
    {
        return new EntityStats(e1.speed - e2.speed);
    }

    public EntityStats Difference(EntityStats other)
    {
        return new EntityStats(speed + (other.speed * -1));
    }

    public override string ToString()
    {
        return "(" + speed + ")";
    }

}