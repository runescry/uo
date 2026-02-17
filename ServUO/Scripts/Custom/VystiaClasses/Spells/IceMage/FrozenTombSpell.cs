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
    /// Frozen Tomb - Ice prison that protects target
    /// Encases target in ice making them immune to damage but unable to act
    /// Circle: 5th (14 mana)
    /// Reagents: Permafrost Essence, Arctic Pearl (Vystia reagents)
    /// </summary>
    public class FrozenTombSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Frozen Tomb", "An Frio Sanct",
            236,
            9011,
            false
        );

        public override SpellCircle Circle => SpellCircle.Fifth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Ice;

        public FrozenTombSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            if (!HasReagents(typeof(PermafrostEssence), 1) || !HasReagents(typeof(ArcticPearl), 1))
            {
                Caster.SendMessage(0x22, "You do not have the required reagents (Permafrost Essence, Arctic Pearl).");
                return false;
            }

            return true;
        }

        public override bool ConsumeReagents()
        {
            return ConsumeReagent(typeof(PermafrostEssence), 1) && ConsumeReagent(typeof(ArcticPearl), 1);
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
                Caster.SendLocalizedMessage(500237);
            }
            else if (CheckBSequence(target))
            {
                SpellHelper.Turn(Caster, target);

                // Visual effect - ice sphere
                target.FixedParticles(0x3779, 10, 15, 5038, 0x481, 0, EffectLayer.Waist);
                target.PlaySound(0x1F8);

                target.SendMessage(0x481, "You are encased in a frozen tomb!");

                // Freeze and paralyze target
                target.Frozen = true;
                target.Paralyzed = true;

                // Store original blessed state and set blessed
                bool wasBless = target.Blessed;
                target.Blessed = true;

                // Release after 6 seconds
                Timer.DelayCall(TimeSpan.FromSeconds(6.0), () =>
                {
                    if (target != null && !target.Deleted)
                    {
                        target.Frozen = false;
                        target.Paralyzed = false;
                        target.Blessed = wasBless;
                        target.SendMessage("The frozen tomb shatters!");
                        target.FixedParticles(0x3735, 1, 30, 9966, 0x481, 0, EffectLayer.Waist);
                    }
                });
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly FrozenTombSpell m_Owner;

            public InternalTarget(FrozenTombSpell owner) : base(10, false, TargetFlags.Beneficial)
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
