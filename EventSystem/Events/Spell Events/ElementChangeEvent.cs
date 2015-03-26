using UnityEngine;
using System.Collections;
using System;

public class ElementChangeEvent : GameEvent
{
    public ElementalStats statChange = new ElementalStats(-1, -1, -1, -1, -1);
    private Player player;

    public override void Start()
    {
        base.Start();
        player = GameplayGUI.instance.player;
    }

    public override void EnterEvent(Collider other)
    {
        base.EnterEvent(other);
        ElementalStats stats = player.MaxElementalCharge;
        foreach (Element e in Enum.GetValues(typeof(Element)))
        {
            if (statChange[e] != -1)
                stats[e] = statChange[e];
        }
        player.MaxElementalCharge = stats;

    }


}
