using UnityEngine;
using System.Collections;
using System;

[Serializable]
public struct EntityStats
{
    [SerializeField]
    private float _speed;

    public float Speed { get { return _speed; } set { _speed = value; } }

    public EntityStats(float speed)
    {
        this._speed = speed;
    }

    public static EntityStats operator +(EntityStats e1, EntityStats e2)
    {
        return new EntityStats(e1._speed + e2._speed);
    }

    public static EntityStats operator -(EntityStats e1, EntityStats e2)
    {
        return new EntityStats(e1._speed - e2._speed);
    }

    public EntityStats Difference(EntityStats other)
    {
        return new EntityStats(_speed + (other._speed * -1));
    }

    public override string ToString()
    {
        return "(" + _speed + ")";
    }

}