using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct ElementalStats
{

    private Dictionary<Element, float> elementalStats;

    // Editor fields, these fields can be assigned in the editor and will be applied
    // if the default constructor is used
    public float fire;
    public float water;
    public float air;
    public float kinetic;

    public float this[Element e]
    {
        get
        {
            // Ensure the elemental stats are created if the default constructor is used
            if (elementalStats == null)
                BuildStats(fire, water, air, kinetic);
            return GetElementalStat(e);
        }
        set
        {
            // Ensure the elemental stats are created if the default constructor is used
            if (elementalStats == null)
                BuildStats(fire, water, air, kinetic);
            elementalStats[e] = value;
        }
    }

    public ElementalStats(float fire, float water, float air, float kinetic)
    {
        this.fire = fire;
        this.water = water;
        this.kinetic = kinetic;
        this.air = air;
        elementalStats = null;
    }

    private void BuildStats(float fire, float water, float air, float kinetic)
    {
        elementalStats = new Dictionary<Element, float>();
        elementalStats.Add(Element.Fire, fire);
        elementalStats.Add(Element.Water, water);
        elementalStats.Add(Element.Kinetic, kinetic);
        elementalStats.Add(Element.Air, air);
    }

    private float GetElementalStat(Element element)
    {
        return elementalStats[element];
    }

    public static ElementalStats Zero
    {
        get
        {
            return new ElementalStats(0, 0, 0, 0);
        }
    }

    public static ElementalStats operator +(ElementalStats e1, ElementalStats e2)
    {
        return new ElementalStats(e1[Element.Fire] + e2[Element.Fire], e1[Element.Water] + e2[Element.Water], e1[Element.Air] + e2[Element.Air], e1[Element.Kinetic] + e2[Element.Kinetic]);
    }

    public static ElementalStats operator -(ElementalStats e1, ElementalStats e2)
    {
        return new ElementalStats(e1[Element.Fire] - e2[Element.Fire], e1[Element.Water] - e2[Element.Water], e1[Element.Air] - e2[Element.Air], e1[Element.Kinetic] - e2[Element.Kinetic]);
    }

    public static ElementalStats operator *(ElementalStats e1, float f)
    {
        return new ElementalStats(e1[Element.Fire] * f, e1[Element.Water] * f, e1[Element.Air] * f, e1[Element.Kinetic] * f);
    }

    public override string ToString()
    {
        return "(" + this[Element.Fire] + " : " + this[Element.Water] + " : " + this[Element.Air] + " : "+ this[Element.Kinetic] + ")";
    }
}

public enum Element
{
    Fire,
    Water,
    Kinetic,
    Air
}