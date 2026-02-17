using System;
using System.Collections.Generic;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;
using Server.Items;

using Server.Spells.VystiaSpells;
namespace Server.Spells.VystiaSpells.Hex
{
    /// <summary>
    /// Hex of Agony - Target cannot be healed and takes damage over time
    /// Any healing attempted on the target deals damage instead
    /// Circle: 4 (11 mana)
    /// Duration: 8-12 seconds based on Magery skill
    /// </summary>
    public class HexHexofAgonySpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Hex of Agony", "Hexum Ofum Agonyum",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Fourth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Hex;

        // Static tracking for hexed targets
        private static readonly Dictionary<Mobile, HexOfAgonyEffect> m_HexedTargets = new Dictionary<Mobile, HexOfAgonyEffect>();

        public static bool IsHexed(Mobile m)
        {
            return m_HexedTargets.ContainsKey(m);
        }

        public static void BeginHex(Mobile caster, Mobile target, TimeSpan duration)
        {
            if (m_HexedTargets.ContainsKey(target))
            {
                EndHex(target);
            }

            var effect = new HexOfAgonyEffect(caster, target, duration);
            m_HexedTargets[target] = effect;
            effect.Start();

            target.YellowHealthbar = true;
            target.SendMessage(0x22, "You have been cursed with the Hex of Agony! Healing will harm you!");
        }

        public static void EndHex(Mobile m)
        {
            if (!IsHexed(m))
                return;

            var effect = m_HexedTargets[m];
            effect.Stop();
            m_HexedTargets.Remove(m);

            m.YellowHealthbar = false;
            m.SendMessage(0x3B2, "The Hex of Agony fades away.");
        }

        /// <summary>
        /// Called when a hexed target would be healed - converts healing to damage
        /// </summary>
        public static int ProcessHealing(Mobile target, int healAmount)
        {
            if (!IsHexed(target) || healAmount <= 0)
                return healAmount;

            var effect = m_HexedTargets[target];

            // Convert healing to damage
            target.FixedParticles(0x374A, 10, 15, 5013, 0x21, 0, EffectLayer.Waist);
            target.PlaySound(0x1F1);

            AOS.Damage(target, effect.Caster, healAmount, 0, 0, 0, 100, 0); // Poison damage
            target.SendMessage(0x22, $"The Hex of Agony converts {healAmount} healing into damage!");

            if (effect.Caster != null && !effect.Caster.Deleted)
                effect.Caster.SendMessage(0x3B2, $"Your hex converts {healAmount} healing into agony!");

            return 0; // No healing occurs
        }

        public HexHexofAgonySpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(ToadsEye), 1) || !Caster.Backpack.ConsumeTotal(typeof(SwampLotus), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: ToadsEye (1), SwampLotus (1)");
                return false;
            }

            return true;
        }

        public override void OnCast()
        {
            Caster.Target = new InternalTarget(this);
        }

        public void Target(Mobile target)
        {
            if (!Caster.CanSee(target))
            {
                Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (CheckHSequence(target))
            {
                SpellHelper.Turn(Caster, target);

                // Visual effect - dark purple curse
                target.FixedParticles(0x374A, 10, 30, 5013, 0x496, 0, EffectLayer.Waist);
                Effects.SendTargetParticles(target, 0x37C4, 1, 12, 0x496, 0, 9502, EffectLayer.Waist, 0);

                // Sound effect
                Caster.PlaySound(0x1FC);
                target.PlaySound(0x56D);

                // Calculate duration based on Magery skill (8-12 seconds)
                double duration = 8.0 + (Caster.Skills[GetSchoolSkill()].Value / 25.0);
                if (duration > 12.0)
                    duration = 12.0;

                // Apply the hex
                BeginHex(Caster, target, TimeSpan.FromSeconds(duration));

                Caster.SendMessage(0x3B2, $"You curse your target with the Hex of Agony for {duration:F1} seconds!");
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly HexHexofAgonySpell m_Owner;

            public InternalTarget(HexHexofAgonySpell owner)
                : base(10, false, TargetFlags.Harmful)
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
    /// Handles the Hex of Agony effect - prevents healing and deals DoT
    /// </summary>
    public class HexOfAgonyEffect
    {
        public Mobile Caster { get; private set; }
        public Mobile Target { get; private set; }
        private Timer m_Timer;
        private Timer m_DotTimer;
        private int m_TicksRemaining;

        public HexOfAgonyEffect(Mobile caster, Mobile target, TimeSpan duration)
        {
            Caster = caster;
            Target = target;
            m_TicksRemaining = (int)duration.TotalSeconds;
        }

        public void Start()
        {
            // End timer
            m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(m_TicksRemaining), () =>
            {
                HexHexofAgonySpell.EndHex(Target);
            });

            // DoT timer - deals 3-5 damage per second while hexed
            m_DotTimer = Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), OnDotTick);
        }

        private void OnDotTick()
        {
            if (Target == null || Target.Deleted || !Target.Alive || m_TicksRemaining <= 0)
            {
                HexHexofAgonySpell.EndHex(Target);
                return;
            }

            m_TicksRemaining--;

            // Deal 3-5 poison damage per tick
            int damage = Utility.RandomMinMax(3, 5);

            if (Caster != null && !Caster.Deleted)
                Caster.DoHarmful(Target);

            AOS.Damage(Target, Caster, damage, 0, 0, 0, 100, 0);

            // Visual effect every 2 ticks
            if (m_TicksRemaining % 2 == 0)
            {
                Target.FixedParticles(0x374A, 5, 10, 5013, 0x496, 0, EffectLayer.Waist);
            }

            Target.SendMessage(0x22, $"[Hex of Agony] {damage} damage! ({m_TicksRemaining}s remaining)");

            if (m_TicksRemaining <= 0)
            {
                HexHexofAgonySpell.EndHex(Target);
            }
        }

        public void Stop()
        {
            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }

            if (m_DotTimer != null)
            {
                m_DotTimer.Stop();
                m_DotTimer = null;
            }
        }
    }
}
