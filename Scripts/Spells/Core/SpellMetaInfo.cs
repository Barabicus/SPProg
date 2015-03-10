using UnityEngine;
using System.Collections;

public class SpellMetaInfo : MonoBehaviour
{
    public Spell spell;

    public void CreateMetaInfo(Spell spell)
    {
        this.spell = spell;
    }

}
