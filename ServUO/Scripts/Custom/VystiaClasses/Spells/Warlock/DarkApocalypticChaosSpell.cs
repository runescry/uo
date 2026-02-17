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
    /// Apocalyptic Chaos - Ultimate AoE devastation spell
    /// Circle: 7 (40 mana)
    /// Damage: 45-65 to all enemies in 6 tile radius
    /// Effects: Random element per target, 2s stun, all stat debuff
    /// </summary>
    public class DarkApocalypticChaosSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Apocalyptic Chaos", "Apocalypticum Chaosum",
            245,
            9042,
            false
        );

        public override SpellCircle Circle => SpellCircle.Seventh;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Dark;

        public DarkApocalypticChaosSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            // Vystia Reagent Check - expensive for Circle 7
            if (!Caster.Backpack.ConsumeTotal(typeof(DemonHeart), 2) || !Caster.Backpack.ConsumeTotal(typeof(ShadowEssence), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: DemonHeart (2), ShadowEssence (1)");
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

                // Massive visual effect - apocalyptic chaos explosion
                for (int i = 0; i < 5; i++)
                {
                    Point3D effectLoc = new Point3D(
                        loc.X + Utility.RandomMinMax(-2, 2),
                        loc.Y + Utility.RandomMinMax(-2, 2),
                        loc.Z);
                    Effects.SendLocationParticles(
                        EffectItem.Create(effectLoc, map, EffectItem.DefaultDuration),
                        0x3709, 10, 30, Utility.RandomList(0x481, 0x496, 0x21, 0x455, 0x1A), 0, 5052, 0);
                }
                Effects.PlaySound(loc, map, 0x307);
                Effects.PlaySound(loc, map, 0x29);

                Caster.SendMessage(0x3B2, "You unleash APOCALYPTIC CHAOS upon your enemies!");

                // Get all enemies in 6 tile radius
                List<Mobile> targets = new List<Mobile>();
                IPooledEnumerable eable = map.GetMobilesInRange(loc, 6);

                foreach (Mobile m in eable)
                {
                    if (m != Caster && m.Alive && Caster.CanBeHarmful(m, false))
                        targets.Add(m);
                }
                eable.Free();

                // Calculate debuff duration
                double duration = 8.0 + (Caster.Skills[GetSchoolSkill()].Value / 12.0);

                foreach (Mobile target in targets)
                {
                    Caster.DoHarmful(target);

                    // 45-65 damage with random element per target
                    double damage = Utility.RandomMinMax(45, 65);
                    damage += Caster.Skills[GetSchoolSkill()].Value * 0.2;

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

                    // Apply 2 second stun
                    target.Paralyzed = true;
                    Timer.DelayCall(TimeSpan.FromSeconds(2.0), () =>
                    {
                        if (target != null && !target.Deleted)
                            target.Paralyzed = false;
                    });

                    // Apply all-stat debuff
                    target.AddStatMod(new StatMod(StatType.Str, "ApocalypticChaos_Str", -15, TimeSpan.FromSeconds(duration)));
                    target.AddStatMod(new StatMod(StatType.Dex, "ApocalypticChaos_Dex", -15, TimeSpan.FromSeconds(duration)));
                    target.AddStatMod(new StatMod(StatType.Int, "ApocalypticChaos_Int", -15, TimeSpan.FromSeconds(duration)));

                    // Massive visual effect on target
                    target.FixedParticles(0x3709, 10, 30, 5052, hue, 0, EffectLayer.Waist);
                    target.FixedParticles(0x374A, 10, 15, 5013, hue, 0, EffectLayer.Head);
                    target.SendMessage(0x22, "APOCALYPTIC CHAOS tears your body and mind apart! (Stunned 2s, -15 all stats)");
                }

                if (targets.Count > 0)
                    Caster.SendMessage(0x3B2, $"Apocalyptic Chaos devastates {targets.Count} enemies!");
                else
                    Caster.SendMessage(0x22, "No enemies in range.");
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly DarkApocalypticChaosSpell m_Owner;

            public InternalTarget(DarkApocalypticChaosSpell owner)
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
