﻿using UnityEngine;
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

    //public static ElementalStats operator +(ElementalStats e1, ElementalStats e2)
    //{
    //    return new ElementalStats(e1.GetElementalStat(Element.Fire) + e2.GetElementalStat(Element.Fire), e1.GetElementalStat(Element.Water) + e2.GetElementalStat(Element.Water));
    //}

    //public ElementalStats Inverse
    //{
    //    get { return new ElementalStats(-Fire, -Water, -Air, -Earth); }
    //}
}

public enum Element
{
    Fire,
    Water,
    Kinetic
}
