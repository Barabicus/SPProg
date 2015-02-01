using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct ElementalStats
{

    private Dictionary<Element, float> elementalStats;

    public float this[Element e]
    {
        get { return GetElementalStat(e); }
        set { elementalStats[e] = value; }
    }

    public ElementalStats(float fire, float water, float kinetic)
    {
        elementalStats = new Dictionary<Element, float>();

        elementalStats.Add(Element.Fire, fire);
        elementalStats.Add(Element.Water, water);
        elementalStats.Add(Element.Kinetic, kinetic);
    }

    public float GetElementalStat(Element element)
    {
        return elementalStats[element];
    }

    public static ElementalStats Zero
    {
        get
        {
            return new ElementalStats(0, 0, 0);
        }
    }

    public static ElementalStats operator +(ElementalStats e1, ElementalStats e2)
    {
        return new ElementalStats(e1[Element.Fire] + e2[Element.Fire], e1[Element.Water] + e2[Element.Water], e1[Element.Kinetic] + e2[Element.Kinetic]);
    }

    public static ElementalStats operator -(ElementalStats e1, ElementalStats e2)
    {
        return new ElementalStats(e1[Element.Fire] - e2[Element.Fire], e1[Element.Water] - e2[Element.Water], e1[Element.Kinetic] - e2[Element.Kinetic]);
    }

    public override string ToString()
    {
        return "(" + this[Element.Fire] + " : " + this[Element.Water] + " : " + this[Element.Kinetic] + ")";
    }
}

public enum Element
{
    Fire,
    Water,
    Kinetic
}
