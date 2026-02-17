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
    /// Glacier Summon - Summons Ice Elemental
    /// Summons a powerful ice elemental to fight for the caster
    /// Circle: 7th (40 mana)
    /// Reagents: Arctic Pearl, Frozen Soul, Permafrost Essence (Vystia reagents)
    /// </summary>
    public class GlacierSummonSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Glacier Summon", "Kal Xen Frio",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Seventh;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Ice;

        public GlacierSummonSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            if (!HasReagents(typeof(ArcticPearl), 1) || !HasReagents(typeof(FrozenSoul), 1) || !HasReagents(typeof(PermafrostEssence), 1))
            {
                Caster.SendMessage(0x22, "You do not have the required reagents (Arctic Pearl, Frozen Soul, Permafrost Essence).");
                return false;
            }

            return true;
        }

        public override bool ConsumeReagents()
        {
            return ConsumeReagent(typeof(ArcticPearl), 1) &&
                   ConsumeReagent(typeof(FrozenSoul), 1) &&
                   ConsumeReagent(typeof(PermafrostEssence), 1);
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

        public void Target(IPoint3D p)
        {
            if (!Caster.CanSee(p))
            {
                Caster.SendLocalizedMessage(500237);
            }
            else if (CheckSequence())
            {
                SpellHelper.Turn(Caster, p);

                if (p is Item)
                    p = ((Item)p).GetWorldLocation();

                Point3D loc = new Point3D(p);

                // Visual effects
                Effects.SendLocationParticles(
                    EffectItem.Create(loc, Caster.Map, EffectItem.DefaultDuration),
                    0x3779, 10, 15, 0x481, 0, 5052, 0);
                Effects.PlaySound(loc, Caster.Map, 0x1F8);

                // Summon Ice Elemental
                BaseCreature summon = new IceElemental();
                summon.SetHits(700); // 700 HP
                summon.SetDamage(20, 30);

                // Set as summoned creature
                SpellHelper.Summon(summon, Caster, 0x217, TimeSpan.FromSeconds(180.0), false, false);
                summon.MoveToWorld(loc, Caster.Map);

                Caster.SendMessage(0x3B2, "You summon a mighty ice elemental!");
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private GlacierSummonSpell m_Owner;

            public InternalTarget(GlacierSummonSpell owner) : base(12, true, TargetFlags.None)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                IPoint3D p = o as IPoint3D;
                if (p != null)
                    m_Owner.Target(p);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}
