using UnityEngine;
using System.Collections;

namespace Spells.SpellList {
    public class FireBeam : BeamSpell
    {
        public override void Update()
        {
            base.Update();
        }

        public override float SpellLiveTime
        {
            get { return 60f; }
        }

        public override ElementalStats ElementalPower
        {
            get { return new ElementalStats(12f, 0f, 0f); }
        }


        public override float SpellCastDelay
        {
            get { return 0.25f; }
        }

    }
}
