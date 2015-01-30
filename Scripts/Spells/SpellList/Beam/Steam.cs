using UnityEngine;
using System.Collections;

namespace Spells.SpellList
{
    public class Steam : BeamSpell
    {
        public override float SpellLiveTime
        {
            get { return 60.5f; }
        }

        public override SpellID SpellID
        {
            get { return global::SpellID.Steam; }
        }

        public override ElementalStats ElementalPower
        {
            get { return new ElementalStats(2f, 2f, 0f); }
        }

        public override float SpellCastDelay
        {
            get { return 0.05f; }
        }
    }
}