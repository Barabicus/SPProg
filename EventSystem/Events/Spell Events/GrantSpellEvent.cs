using UnityEngine;
using System.Collections;

public class GrantSpellEvent : GameEvent
{

    public Spell spell;

    public override void EnterEvent(Collider other)
    {
        base.EnterEvent(other);
        PlayerController player = GameplayGUI.instance.player;

        for (int i = 0; i < player.spellList.Length; i++)
        {
            if (player.spellList[i] == null)
            {
                GameplayGUI.instance.SetPlayerSpellAtIndex(spell, i);
                return;
            }
        }
    }

}
