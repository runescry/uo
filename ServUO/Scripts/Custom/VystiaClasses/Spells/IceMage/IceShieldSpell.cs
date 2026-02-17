using System;
using System.Collections.Generic;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;
using Server.Items;

using Server.Spells.VystiaSpells;
namespace Server.Spells.VystiaSpells.IceMage
{
    /// <summary>
    /// Ice Shield - Defensive buff with damage reflection
    /// Grants physical and cold resistance, reflects melee damage
    /// Circle: 2nd (6 mana)
    /// Reagents: Winterleaf, Glacier Crystal (Vystia reagents)
    /// </summary>
    public class IceShieldSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Ice Shield", "Sanct Glacius",
            236,
            9011,
            false
        );

        public override SpellCircle Circle => SpellCircle.Second;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Ice;

        public IceShieldSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            // Check for Vystia reagents
            if (!HasReagents(typeof(Winterleaf), 1) || !HasReagents(typeof(GlacierCrystal), 1))
            {
                Caster.SendMessage(0x22, "You do not have the required reagents (Winterleaf, Glacier Crystal).");
                return false;
            }

            return true;
        }

        public override bool ConsumeReagents()
        {
            return ConsumeReagent(typeof(Winterleaf), 1) && ConsumeReagent(typeof(GlacierCrystal), 1);
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

                // Duration: 60 seconds
                TimeSpan buffDuration = TimeSpan.FromSeconds(60.0);

                // Create resistance bonuses
                ResistanceMod physMod = new ResistanceMod(ResistanceType.Physical, 10);
                ResistanceMod coldMod = new ResistanceMod(ResistanceType.Cold, 15);

                target.AddResistanceMod(physMod);
                target.AddResistanceMod(coldMod);

                // Visual effect - orbiting ice shards
                target.FixedParticles(0x3779, 10, 15, 5038, 0x481, 0, EffectLayer.Waist);
                target.PlaySound(0x64F);

                target.SendMessage(0x481, "You are surrounded by orbiting ice shards!");
                target.SendMessage(0x481, "Physical +10, Cold +15 Resistance. 100% chance to reflect full physical damage. (TEST MODE)");

                // Create damage reflection effect
                new IceShieldReflectionEffect(target, physMod, coldMod, buffDuration).Start();
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly IceShieldSpell m_Owner;

            public InternalTarget(IceShieldSpell owner)
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

    /// <summary>
    /// Ice Shield Reflection Effect - Handles damage reflection
    /// Uses static dictionary to track active shields
    /// </summary>
    public class IceShieldReflectionEffect
    {
        private static Dictionary<Mobile, IceShieldReflectionEffect> m_ActiveShields = new Dictionary<Mobile, IceShieldReflectionEffect>();
        private static bool m_Reflecting = false; // Recursion guard to prevent infinite loops

        private Mobile m_Target;
        private ResistanceMod m_PhysMod;
        private ResistanceMod m_ColdMod;
        private Timer m_ExpireTimer;

        public IceShieldReflectionEffect(Mobile target, ResistanceMod physMod, ResistanceMod coldMod, TimeSpan duration)
        {
            m_Target = target;
            m_PhysMod = physMod;
            m_ColdMod = coldMod;
        }

        public void Start()
        {
            // Remove any existing shield
            if (m_ActiveShields.ContainsKey(m_Target))
            {
                m_ActiveShields[m_Target].Stop();
            }
            
            m_ActiveShields[m_Target] = this;
            
            // Set expiration timer
            m_ExpireTimer = Timer.DelayCall(TimeSpan.FromSeconds(60.0), Stop);
        }

        public static bool CheckReflection(Mobile target, Mobile attacker, int damage)
        {
            // Prevent infinite recursion - if we're already reflecting, don't reflect again
            if (m_Reflecting)
                return false;

            if (m_ActiveShields.ContainsKey(target))
            {
                IceShieldReflectionEffect effect = m_ActiveShields[target];

                // 100% chance to reflect full physical damage (TEST MODE - normally 15%)
                if (Utility.RandomDouble() < 1.0)
                {
                    // Set recursion guard before dealing damage
                    m_Reflecting = true;

                    try
                    {
                        // Reflect damage back to attacker as cold damage
                        AOS.Damage(attacker, target, damage, 0, 100, 0, 0, 0);

                        // Visual effect on attacker
                        attacker.FixedParticles(0x374A, 10, 15, 5038, 0x481, 0, EffectLayer.Waist);
                        attacker.PlaySound(0x64F);

                        // Clear messages for both parties
                        attacker.SendMessage(0x481, $"[ICE SHIELD DEFLECT] {damage} cold damage reflected back!");
                        target.SendMessage(0x3B2, $"[ICE SHIELD DEFLECT] Your shield reflects {damage} cold damage to {attacker.Name}!");

                        // Overhead message on target for visibility
                        target.PublicOverheadMessage(Network.MessageType.Regular, 0x481, false, $"*DEFLECT* {damage} dmg!");
                    }
                    finally
                    {
                        // Always clear the recursion guard
                        m_Reflecting = false;
                    }

                    return true;
                }
            }

            return false;
        }

        private void Stop()
        {
            if (m_Target != null && !m_Target.Deleted)
            {
                m_Target.RemoveResistanceMod(m_PhysMod);
                m_Target.RemoveResistanceMod(m_ColdMod);
                m_Target.SendMessage("Your ice shield shatters.");
                m_Target.FixedParticles(0x3735, 1, 30, 9966, 0x481, 0, EffectLayer.Waist);
            }

            if (m_ActiveShields.ContainsKey(m_Target))
            {
                m_ActiveShields.Remove(m_Target);
            }

            if (m_ExpireTimer != null)
            {
                m_ExpireTimer.Stop();
                m_ExpireTimer = null;
            }
        }
    }
}
