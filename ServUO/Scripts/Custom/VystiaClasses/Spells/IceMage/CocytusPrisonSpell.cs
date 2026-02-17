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
    /// Cocytus Prison - Ultimate lockdown
    /// Imprisons target in unbreakable ice requiring channel
    /// Circle: 8th (50 mana)
    /// Reagents: Frozen Soul, EternalIce (3), HeartOfWinter (Vystia reagents)
    /// </summary>
    public class CocytusPrisonSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Cocytus Prison", "Vas An Corp Frio Sanct",
            236,
            9011,
            false
        );

        public override SpellCircle Circle => SpellCircle.Eighth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Ice;

        public CocytusPrisonSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            if (!HasReagents(typeof(FrozenSoul), 1) || !HasReagents(typeof(FrostEssence), 3) || !HasReagents(typeof(Server.Items.HeartOfWinter), 1))
            {
                Caster.SendMessage(0x22, "You do not have the required reagents (Frozen Soul, 3 Eternal Ice, Heart of Winter).");
                return false;
            }

            return true;
        }

        public override bool ConsumeReagents()
        {
            return ConsumeReagent(typeof(FrozenSoul), 1) &&
                   ConsumeReagent(typeof(FrostEssence), 3) &&
                   ConsumeReagent(typeof(Server.Items.HeartOfWinter), 1);
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

                // Visual effect - massive ice prison
                target.FixedParticles(0x3779, 10, 15, 5038, 0x481, 0, EffectLayer.Waist);
                target.PlaySound(0x1F8);

                target.SendMessage(0x22, "You are imprisoned in Cocytus!");
                Caster.SendMessage(0x3B2, "You imprison your target in Cocytus! You must channel to maintain it.");

                // Completely disable target
                target.Frozen = true;
                target.Paralyzed = true;
                target.Blessed = true; // Immune to damage

                // Freeze caster too (channeling)
                Caster.Frozen = true;
                Caster.SendMessage(0x22, "You are channeling Cocytus Prison and cannot move or cast!");

                // Create prison effect with visual marker
                CocytusPrisonItem prison = new CocytusPrisonItem(Caster, target);
                prison.MoveToWorld(target.Location, target.Map);

                // Auto-release after 60 seconds max
                Timer.DelayCall(TimeSpan.FromSeconds(60.0), () =>
                {
                    if (!prison.Deleted)
                        prison.Release();
                });
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly CocytusPrisonSpell m_Owner;

            public InternalTarget(CocytusPrisonSpell owner) : base(12, false, TargetFlags.Harmful)
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

    /// <summary>
    /// Cocytus Prison visual item
    /// </summary>
    public class CocytusPrisonItem : Item
    {
        private Mobile m_Caster;
        private Mobile m_Target;
        private Timer m_EffectTimer;
        private bool m_wasCasterBless;
        private bool m_wasTargetBless;

        public CocytusPrisonItem(Mobile caster, Mobile target) : base(0x3779)
        {
            m_Caster = caster;
            m_Target = target;

            Movable = false;
            Hue = 0x481;
            Name = "Cocytus Prison";

            // Store blessed states
            m_wasCasterBless = caster.Blessed;
            m_wasTargetBless = target.Blessed;

            // Visual effects timer
            m_EffectTimer = Timer.DelayCall(TimeSpan.FromSeconds(2.0), TimeSpan.FromSeconds(2.0), () =>
            {
                if (m_Target != null && !m_Target.Deleted)
                    m_Target.FixedParticles(0x3779, 10, 15, 5038, 0x481, 0, EffectLayer.Waist);
            });
        }

        public CocytusPrisonItem(Serial serial) : base(serial)
        {
        }

        public void Release()
        {
            if (m_EffectTimer != null)
            {
                m_EffectTimer.Stop();
                m_EffectTimer = null;
            }

            // Release target - ALWAYS restore Blessed state
            if (m_Target != null && !m_Target.Deleted)
            {
                m_Target.Frozen = false;
                m_Target.Paralyzed = false;
                m_Target.Blessed = m_wasTargetBless; // Restore original Blessed state
                m_Target.SendMessage("The Cocytus prison shatters!");
                m_Target.FixedParticles(0x3735, 1, 30, 9966, 0x481, 0, EffectLayer.Waist);
            }

            // Release caster
            if (m_Caster != null && !m_Caster.Deleted)
            {
                m_Caster.Frozen = false;
                m_Caster.SendMessage("You release the Cocytus prison.");
            }

            Delete();
        }

        public override void OnAfterDelete()
        {
            // Ensure Release is called even if item is deleted unexpectedly
            // This prevents the target from staying invulnerable forever
            if (m_Target != null && !m_Target.Deleted)
            {
                m_Target.Frozen = false;
                m_Target.Paralyzed = false;
                m_Target.Blessed = m_wasTargetBless; // Always restore Blessed state
            }
            if (m_Caster != null && !m_Caster.Deleted)
            {
                m_Caster.Frozen = false;
            }
            
            base.OnAfterDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version 1 - added mobile refs

            writer.Write(m_Caster);
            writer.Write(m_Target);
            writer.Write(m_wasTargetBless);
            writer.Write(m_wasCasterBless);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version >= 1)
            {
                m_Caster = reader.ReadMobile();
                m_Target = reader.ReadMobile();
                m_wasTargetBless = reader.ReadBool();
                m_wasCasterBless = reader.ReadBool();
            }

            // Release the prison on server restart - now we have the mobile refs
            Timer.DelayCall(TimeSpan.Zero, Release);
        }
    }
}
