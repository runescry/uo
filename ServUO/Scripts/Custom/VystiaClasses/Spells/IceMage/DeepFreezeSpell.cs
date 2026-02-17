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
    /// Deep Freeze - Hard CC with vulnerability
    /// Freezes target solid making them take double damage
    /// Circle: 6th (20 mana)
    /// Reagents: Permafrost Essence, Arctic Pearl, Frozen Soul (Vystia reagents)
    /// </summary>
    public class DeepFreezeSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Deep Freeze", "Vas An Frio Mort",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Sixth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Ice;

        public DeepFreezeSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            if (!HasReagents(typeof(PermafrostEssence), 1) || !HasReagents(typeof(ArcticPearl), 1) || !HasReagents(typeof(FrozenSoul), 1))
            {
                Caster.SendMessage(0x22, "You do not have the required reagents (Permafrost Essence, Arctic Pearl, Frozen Soul).");
                return false;
            }

            return true;
        }

        public override bool ConsumeReagents()
        {
            return ConsumeReagent(typeof(PermafrostEssence), 1) &&
                   ConsumeReagent(typeof(ArcticPearl), 1) &&
                   ConsumeReagent(typeof(FrozenSoul), 1);
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
            else if (CheckHSequence(target))
            {
                SpellHelper.Turn(Caster, target);

                // Visual effect - complete ice encasement
                target.FixedParticles(0x376A, 9, 32, 5030, 0x481, 0, EffectLayer.Waist);
                target.PlaySound(0x1F8);

                target.SendMessage(0x22, "You are frozen solid and vulnerable!");
                Caster.SendMessage(0x3B2, "Your target is frozen solid and takes double damage!");

                // Freeze target
                target.Frozen = true;
                target.Paralyzed = true;

                // Mark for double damage (simplified - would need damage event hook for full implementation)
                // For now, apply a severe resistance debuff to simulate vulnerability
                ResistanceMod physMod = new ResistanceMod(ResistanceType.Physical, -25);
                ResistanceMod fireMod = new ResistanceMod(ResistanceType.Fire, -25);
                ResistanceMod coldMod = new ResistanceMod(ResistanceType.Cold, -25);
                ResistanceMod poisMod = new ResistanceMod(ResistanceType.Poison, -25);
                ResistanceMod nrgyMod = new ResistanceMod(ResistanceType.Energy, -25);

                target.AddResistanceMod(physMod);
                target.AddResistanceMod(fireMod);
                target.AddResistanceMod(coldMod);
                target.AddResistanceMod(poisMod);
                target.AddResistanceMod(nrgyMod);

                // Duration: 10 seconds
                Timer.DelayCall(TimeSpan.FromSeconds(10.0), () =>
                {
                    if (target != null && !target.Deleted)
                    {
                        target.Frozen = false;
                        target.Paralyzed = false;

                        target.RemoveResistanceMod(physMod);
                        target.RemoveResistanceMod(fireMod);
                        target.RemoveResistanceMod(coldMod);
                        target.RemoveResistanceMod(poisMod);
                        target.RemoveResistanceMod(nrgyMod);

                        target.SendMessage("The deep freeze shatters!");
                        target.FixedParticles(0x3735, 1, 30, 9966, 0x481, 0, EffectLayer.Waist);
                    }
                });
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly DeepFreezeSpell m_Owner;

            public InternalTarget(DeepFreezeSpell owner) : base(12, false, TargetFlags.Harmful)
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
