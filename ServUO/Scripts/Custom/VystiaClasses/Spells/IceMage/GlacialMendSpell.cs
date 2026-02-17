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
    /// Glacial Mend - Ice-themed healing spell
    /// Heals target and grants cold resistance buff
    /// Circle: 2nd (6 mana)
    /// Reagents: Winterleaf, Frostbloom (Vystia reagents)
    /// </summary>
    public class GlacialMendSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Glacial Mend", "In Mani Frio",
            236,
            9011,
            false
        );

        public override SpellCircle Circle => SpellCircle.Second;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Ice;

        public GlacialMendSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
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

                // Calculate healing amount (15-25 HP)
                int healAmount = Utility.RandomMinMax(15, 25);

                // Apply healing
                target.Heal(healAmount);

                // Visual effect - blue healing sparkles
                target.FixedParticles(0x376A, 9, 32, 5030, 0x481, 0, EffectLayer.Waist);
                target.PlaySound(0x1F2);

                target.SendMessage(0x481, String.Format("Glacial energy heals you for {0} health!", healAmount));

                // Grant +5 Cold Resistance buff for 30 seconds
                TimeSpan buffDuration = TimeSpan.FromSeconds(30.0);
                ResistanceMod coldMod = new ResistanceMod(ResistanceType.Cold, 5);

                target.AddResistanceMod(coldMod);
                target.SendMessage(0x481, "You feel protected from the cold!");

                // Remove buff after duration
                Timer.DelayCall(buffDuration, () =>
                {
                    if (target != null && !target.Deleted)
                    {
                        target.RemoveResistanceMod(coldMod);
                        target.SendMessage("The cold protection from Glacial Mend fades.");
                    }
                });
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly GlacialMendSpell m_Owner;

            public InternalTarget(GlacialMendSpell owner)
                : base(8, false, TargetFlags.Beneficial)
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
