using System;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;
using Server.Items;

using Server.Spells.VystiaSpells;
namespace Server.Spells.VystiaSpells.IceMage
{
    /// <summary>
    /// Frost Ward - Basic cold protection buff
    /// Grants +5 Cold Resistance for 30 seconds
    /// Circle: 1st (4 mana)
    /// Reagents: Winterleaf, Frostbloom (Vystia reagents)
    /// </summary>
    public class FrostWardSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Frost Ward", "Sanct Frio",
            236,
            9011,
            false
        );

        public override SpellCircle Circle => SpellCircle.First;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Ice;

        public FrostWardSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            // Check for Vystia reagents
            if (!HasReagents(typeof(Winterleaf), 1) || !HasReagents(typeof(Frostbloom), 1))
            {
                Caster.SendMessage(0x22, "You do not have the required reagents (Winterleaf, Frostbloom).");
                return false;
            }

            return true;
        }

        public override bool ConsumeReagents()
        {
            return ConsumeReagent(typeof(Winterleaf), 1) && ConsumeReagent(typeof(Frostbloom), 1);
        }

        private bool HasReagents(Type type, int amount)
        {
            return (Caster.Backpack != null && Caster.Backpack.GetAmount(type) >= amount);
        }

        private bool ConsumeReagent(Type type, int amount)
        {
            if (Caster.Backpack == null)
                return false;

            return Caster.Backpack.ConsumeTotal(type, amount);
        }

        public override void OnCast()
        {
// Check fizzle and trigger skill gain

            Caster.Target = new InternalTarget(this);
        }

        public void Target(Mobile target)
        {
            if (!Caster.CanSee(target))
            {
                Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (CheckBSequence(target))
            {
                SpellHelper.Turn(Caster, target);

                // Duration: 30 seconds
                TimeSpan buffDuration = TimeSpan.FromSeconds(30.0);

                // Create cold resistance bonus
                ResistanceMod coldMod = new ResistanceMod(ResistanceType.Cold, 5);

                target.AddResistanceMod(coldMod);

                // Visual effect - faint blue shimmer
                target.FixedParticles(0x375A, 9, 20, 5027, 0x481, 0, EffectLayer.Waist);
                target.PlaySound(0x1E9);

                target.SendMessage(0x481, "You are surrounded by a protective frost ward!");

                // Remove buff after duration
                Timer.DelayCall(buffDuration, () =>
                {
                    if (target != null && !target.Deleted)
                    {
                        target.RemoveResistanceMod(coldMod);
                        target.SendMessage("Your frost ward dissipates.");
                        target.FixedParticles(0x3735, 1, 30, 9966, 0x481, 0, EffectLayer.Waist);
                    }
                });
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly FrostWardSpell m_Owner;

            public InternalTarget(FrostWardSpell owner)
                : base(12, false, TargetFlags.Beneficial)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is Mobile)
                    m_Owner.Target((Mobile)o);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}
