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
    /// Frost Touch - Melee stun spell
    /// Paralyzes target for 2 seconds
    /// Circle: 1st (4 mana)
    /// Reagents: Frostbloom (Vystia reagent)
    /// </summary>
    public class FrostTouchSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Frost Touch", "Frio Tactus",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.First;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Ice;

        public FrostTouchSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            // Check for Vystia reagents
            if (!HasReagents(typeof(Frostbloom), 1))
            {
                Caster.SendMessage(0x22, "You do not have the required reagent (Frostbloom).");
                return false;
            }

            return true;
        }

        public override bool ConsumeReagents()
        {
            return ConsumeReagent(typeof(Frostbloom), 1);
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
            if (CheckSequence())
            {
            Caster.Target = new InternalTarget(this);
            }
            else
            {
                FinishSequence();
            }
        }

        public void Target(IDamageable target)
        {
            if (!Caster.CanSee(target))
            {
                Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (CheckHSequence(target))
            {
                IDamageable source = Caster;
                IDamageable dest = target;

                SpellHelper.Turn(Caster, target);

                // Visual effect - frost on hands
                Caster.FixedParticles(0x373A, 10, 15, 5038, 0x481, 0, EffectLayer.Waist);
                Caster.PlaySound(0x1FB);

                // Apply 2 second stun
                if (dest is Mobile m)
                {
                    m.Paralyze(TimeSpan.FromSeconds(2.0));
                    m.FixedEffect(0x376A, 10, 16, 0x481, 0); // Paralyze field effect recolored to ice blue
                    m.PlaySound(0x204); // Paralyze sound
                    m.SendMessage(0x3B2, "You are frozen solid!");
                    Caster.SendMessage(0x3B2, "You freeze your target solid!");
                }
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly FrostTouchSpell m_Owner;

            public InternalTarget(FrostTouchSpell owner)
                : base(1, false, TargetFlags.Harmful)  // 1 tile range (melee)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is IDamageable)
                    m_Owner.Target((IDamageable)o);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}
