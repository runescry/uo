using System;
using System.Collections.Generic;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;
using Server.Items;

using Server.Spells.VystiaSpells;
namespace Server.Spells.VystiaSpells.Divination
{
    /// <summary>
    /// Prismatic Storm - AoE prismatic energy storm with random elemental damage
    /// Circle: 6 (24 mana)
    /// Damage: 22-32 to all enemies in 5 tile radius, random element per target
    /// </summary>
    public class DivinationPrismaticStormSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Prismatic Storm", "Prismaticum Stormum",
            230,
            9022,
            false
        );

        public override SpellCircle Circle => SpellCircle.Sixth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Divination;

        public DivinationPrismaticStormSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(PropheticLeaf), 1) || !Caster.Backpack.ConsumeTotal(typeof(SeeingStone), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: PropheticLeaf (1), SeeingStone (1)");
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

                // Visual effect - prismatic explosion (multiple colors)
                for (int i = 0; i < 4; i++)
                {
                    Point3D effectLoc = new Point3D(
                        loc.X + Utility.RandomMinMax(-2, 2),
                        loc.Y + Utility.RandomMinMax(-2, 2),
                        loc.Z);
                    Effects.SendLocationParticles(
                        EffectItem.Create(effectLoc, map, EffectItem.DefaultDuration),
                        0x3709, 10, 30, Utility.RandomList(0x481, 0x496, 0x21, 0x455, 0x1A), 0, 5052, 0);
                }
                Effects.PlaySound(loc, map, 0x1F2);

                Caster.SendMessage(0x3B2, "You unleash a prismatic storm of energy!");

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

                    // 22-32 damage with random element per target
                    double damage = Utility.RandomMinMax(22, 32);
                    damage += Caster.Skills[GetSchoolSkill()].Value * 0.12;

                    // Random element for each target (prismatic = all colors)
                    int roll = Utility.Random(5);
                    int phys = 0, fire = 0, cold = 0, poison = 0, energy = 0;
                    int hue;
                    string element;
                    switch (roll)
                    {
                        case 0: phys = 100; hue = 0x455; element = "force"; break;
                        case 1: fire = 100; hue = 0x21; element = "fire"; break;
                        case 2: cold = 100; hue = 0x481; element = "cold"; break;
                        case 3: poison = 100; hue = 0x1A; element = "poison"; break;
                        default: energy = 100; hue = 0x496; element = "energy"; break;
                    }

                    AOS.Damage(target, Caster, (int)damage, phys, fire, cold, poison, energy);

                    target.FixedParticles(0x3709, 10, 30, 5052, hue, 0, EffectLayer.Waist);
                    target.SendMessage(0x22, $"Prismatic {element} tears through you!");
                }

                if (targets.Count > 0)
                    Caster.SendMessage(0x3B2, $"Prismatic Storm strikes {targets.Count} enemies!");
                else
                    Caster.SendMessage(0x22, "No enemies in range.");
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly DivinationPrismaticStormSpell m_Owner;

            public InternalTarget(DivinationPrismaticStormSpell owner)
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
