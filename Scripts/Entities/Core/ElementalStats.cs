using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct ElementalStats
{
    public float fire;
    public float water;
    public float air;
    public float earth;
    public float physical;

    public float this[Element e]
    {
        get
        {
            return GetElementalStat(e);
        }
        set
        {
            SetElementalStat(e, value);
        }
    }

    public ElementalStats(float fire, float water, float air, float earth, float kinetic)
    {
        this.fire = fire;
        this.water = water;
        this.physical = kinetic;
        this.air = air;
        this.earth = earth;
    }

    private float GetElementalStat(Element element)
    {
        switch (element)
        {
            case Element.Fire:
                return fire;
            case Element.Water:
                return water;
            case Element.Air:
                return air;
            case Element.Earth:
                return earth;
            case Element.Kinetic:
                return physical;
            default:
                return 0;
        }
    }

    private void SetElementalStat(Element element, float value)
    {
        switch (element)
        {
            case Element.Fire:
                this.fire = value;
                break;
            case Element.Water:
                this.water = value;
                break;
            case Element.Air:
                this.air = value;
                break;
            case Element.Earth:
                this.earth = value;
                break;
            case Element.Kinetic:
                this.physical = value;
                break;
        }
    }

    public static ElementalStats Zero
    {
        get
        {
            return new ElementalStats(0, 0, 0, 0, 0);
        }
    }

    public static ElementalStats One
    {
        get
        {
            return new ElementalStats(1, 1, 1, 1, 1);
        }
    }

    public static ElementalStats operator +(ElementalStats e1, ElementalStats e2)
    {
        return new ElementalStats(e1[Element.Fire] + e2[Element.Fire], e1[Element.Water] + e2[Element.Water], e1[Element.Air] + e2[Element.Air], e1[Element.Earth] + e2[Element.Earth], e1[Element.Kinetic] + e2[Element.Kinetic]);
    }

    public static ElementalStats operator -(ElementalStats e1, ElementalStats e2)
    {
        return new ElementalStats(e1[Element.Fire] - e2[Element.Fire], e1[Element.Water] - e2[Element.Water], e1[Element.Air] - e2[Element.Air], e1[Element.Earth] - e2[Element.Earth], e1[Element.Kinetic] - e2[Element.Kinetic]);
    }
    public static ElementalStats operator *(ElementalStats e1, ElementalStats e2)
    {
        return new ElementalStats(e1[Element.Fire] * e2[Element.Fire], e1[Element.Water] * e2[Element.Water], e1[Element.Air] * e2[Element.Air], e1[Element.Earth] * e2[Element.Earth], e1[Element.Kinetic] * e2[Element.Kinetic]);
    }

    public static ElementalStats operator /(ElementalStats e1, ElementalStats e2)
    {
        return new ElementalStats(e1[Element.Fire] / e2[Element.Fire], e1[Element.Water] / e2[Element.Water], e1[Element.Air] / e2[Element.Air], e1[Element.Earth] / e2[Element.Earth], e1[Element.Kinetic] / e2[Element.Kinetic]);
    }

    public static ElementalStats operator *(ElementalStats e1, float f)
    {
        return new ElementalStats(e1[Element.Fire] * f, e1[Element.Water] * f, e1[Element.Air] * f, e1[Element.Earth] * f, e1[Element.Kinetic] * f);
    }

    public override string ToString()
    {
        return "(" + this[Element.Fire] + " : " + this[Element.Water] + " : " + this[Element.Air] + " : " + this[Element.Earth] + " : " + this[Element.Kinetic] + ")";
    }
}

public enum Element
{
    Fire,
    Water,
    Kinetic,
    Air,
    Earth
}