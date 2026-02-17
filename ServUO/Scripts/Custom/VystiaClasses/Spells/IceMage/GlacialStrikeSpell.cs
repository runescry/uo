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
    /// Glacial Strike - High damage with freeze chance
    /// Deals heavy cold damage with 50% chance to stun
    /// Circle: 5th (14 mana)
    /// Reagents: Permafrost Essence, Arctic Pearl (Vystia reagents)
    /// </summary>
    public class GlacialStrikeSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Glacial Strike", "Vas Ort Frio",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Fifth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Ice;

        public GlacialStrikeSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
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

        public void Target(IDamageable target)
        {
            if (!Caster.CanSee(target))
            {
                Caster.SendLocalizedMessage(500237);
            }
            else if (CheckHSequence(target))
            {
                IDamageable source = Caster;
                IDamageable dest = target;

                SpellHelper.Turn(Caster, target);

                // Spell reflection
                if (SpellHelper.CheckReflect((int)Circle, ref source, ref dest))
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(0.5), () =>
                    {
                        source.MovingParticles(dest, 0x36E4, 10, 0, false, true, 0x481, 0, 0x1FB);
                    });
                }

                // Visual effect - massive ice bolt
                Caster.MovingParticles(dest, 0x36E4, 10, 0, false, true, 0x481, 0, 0x1FB);
                Caster.PlaySound(0x1FB);

                // Calculate damage (35-55)
                double damage = Utility.RandomMinMax(35, 55);
                SpellHelper.Damage(this, dest, damage, 0, 0, 100, 0, 0);

                // 50% chance to freeze (stun for 2s)
                if (target is Mobile && Utility.RandomDouble() < 0.50)
                {
                    Mobile m = (Mobile)target;
                    m.Frozen = true;
                    m.SendMessage(0x481, "You are frozen solid!");
                    m.FixedParticles(0x376A, 9, 32, 5030, 0x481, 0, EffectLayer.Waist);

                    Timer.DelayCall(TimeSpan.FromSeconds(2.0), () =>
                    {
                        if (m != null && !m.Deleted)
                        {
                            m.Frozen = false;
                            m.SendMessage("You break free from the ice!");
                        }
                    });
                }
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly GlacialStrikeSpell m_Owner;

            public InternalTarget(GlacialStrikeSpell owner) : base(14, false, TargetFlags.Harmful)
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
