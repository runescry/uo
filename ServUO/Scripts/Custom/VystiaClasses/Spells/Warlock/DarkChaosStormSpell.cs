using System;
using System.Collections.Generic;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;
using Server.Items;

using Server.Spells.VystiaSpells;
namespace Server.Spells.VystiaSpells.Dark
{
    /// <summary>
    /// Chaos Storm - Massive AoE chaos damage with random elements
    /// Circle: 6 (20 mana)
    /// Damage: 28-38 to all enemies in 5 tile radius, random element per target
    /// </summary>
    public class DarkChaosStormSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Chaos Storm", "Chaosum Stormum",
            230,
            9022,
            false
        );

        public override SpellCircle Circle => SpellCircle.Sixth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Dark;

        public DarkChaosStormSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(VoidSilk), 1) || !Caster.Backpack.ConsumeTotal(typeof(DemonHeart), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: VoidSilk (1), DemonHeart (1)");
                return false;
            }

            return true;
        }

        public override void OnCast()
        {
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
                Map map = Caster.Map;

                if (map == null)
                {
                    FinishSequence();
                    return;
                }

                // Visual effect - chaotic storm
                Effects.SendLocationParticles(
                    EffectItem.Create(loc, map, EffectItem.DefaultDuration),
                    0x3709, 10, 30, Utility.RandomList(0x481, 0x496, 0x21, 0x455), 0, 5052, 0);
                Effects.PlaySound(loc, map, 0x29);

                Caster.SendMessage(0x3B2, "You unleash a storm of pure chaos!");

                // Get all enemies in 5 tile radius
                List<Mobile> targets = new List<Mobile>();
                IPooledEnumerable eable = map.GetMobilesInRange(loc, 5);

                foreach (Mobile m in eable)
                {
                    if (m != Caster && m.Alive && Caster.CanBeHarmful(m, false))
                        targets.Add(m);
                }
                eable.Free();

                foreach (Mobile target in targets)
                {
                    Caster.DoHarmful(target);

                    // 28-38 damage with random element per target
                    double damage = Utility.RandomMinMax(28, 38);
                    damage += Caster.Skills[GetSchoolSkill()].Value * 0.15;

                    // Random element for each target
                    int roll = Utility.Random(5);
                    int phys = 0, fire = 0, cold = 0, poison = 0, energy = 0;
                    int hue;
                    switch (roll)
                    {
                        case 0: phys = 100; hue = 0x455; break;
                        case 1: fire = 100; hue = 0x21; break;
                        case 2: cold = 100; hue = 0x481; break;
                        case 3: poison = 100; hue = 0x1A; break;
                        default: energy = 100; hue = 0x496; break;
                    }

                    AOS.Damage(target, Caster, (int)damage, phys, fire, cold, poison, energy);

                    target.FixedParticles(0x3709, 10, 30, 5052, hue, 0, EffectLayer.Waist);
                    target.SendMessage(0x22, "The chaos storm tears through you!");
                }

                if (targets.Count > 0)
                    Caster.SendMessage(0x3B2, $"Chaos Storm strikes {targets.Count} enemies!");
                else
                    Caster.SendMessage(0x22, "No enemies in range.");
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly DarkChaosStormSpell m_Owner;

            public InternalTarget(DarkChaosStormSpell owner)
                : base(12, true, TargetFlags.Harmful)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is IPoint3D)
                    m_Owner.Target((IPoint3D)o);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}
