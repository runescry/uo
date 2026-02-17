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
    /// Hex Storm - AoE zone pulses random hexes: curses, poisons, slows, silences
    /// Circle: 6 (20 mana)
    /// Damage: 20-30 to all enemies in 5 tile radius, applies random debuff
    /// </summary>
    public class HexHexStormSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Hex Storm", "Hexum Stormum",
            230,
            9022,
            false
        );

        public override SpellCircle Circle => SpellCircle.Sixth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Hex;

        public HexHexStormSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(HagsHair), 1) || !Caster.Backpack.ConsumeTotal(typeof(CursedPearl), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: HagsHair (1), CursedPearl (1)");
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

                // Visual effect - hex storm swirl
                Effects.SendLocationParticles(
                    EffectItem.Create(loc, map, EffectItem.DefaultDuration),
                    0x3709, 10, 30, 0x496, 0, 5052, 0);
                Effects.PlaySound(loc, map, 0x1FC);

                Caster.SendMessage(0x3B2, "You unleash a storm of hexes!");

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

                    // 20-30 poison damage
                    double damage = Utility.RandomMinMax(20, 30);
                    damage += Caster.Skills[GetSchoolSkill()].Value * 0.12;

                    AOS.Damage(target, Caster, (int)damage, 0, 0, 0, 100, 0);

                    // Apply random hex effect
                    int hexRoll = Utility.Random(4);
                    double duration = 6.0 + (Caster.Skills[GetSchoolSkill()].Value / 20.0);
                    string hexName;

                    switch (hexRoll)
                    {
                        case 0: // STR curse
                            target.AddStatMod(new StatMod(StatType.Str, "HexStorm_Str", -15, TimeSpan.FromSeconds(duration)));
                            hexName = "Weakness";
                            break;
                        case 1: // DEX curse (slow)
                            target.AddStatMod(new StatMod(StatType.Dex, "HexStorm_Dex", -20, TimeSpan.FromSeconds(duration)));
                            hexName = "Lethargy";
                            break;
                        case 2: // INT curse (silence-like)
                            target.AddStatMod(new StatMod(StatType.Int, "HexStorm_Int", -20, TimeSpan.FromSeconds(duration)));
                            hexName = "Confusion";
                            break;
                        default: // Poison
                            target.ApplyPoison(Caster, Poison.Greater);
                            hexName = "Venom";
                            break;
                    }

                    target.FixedParticles(0x374A, 10, 15, 5013, 0x496, 0, EffectLayer.Waist);
                    target.SendMessage(0x22, $"The Hex Storm afflicts you with {hexName}!");
                }

                if (targets.Count > 0)
                    Caster.SendMessage(0x3B2, $"Hex Storm afflicts {targets.Count} enemies!");
                else
                    Caster.SendMessage(0x22, "No enemies in range.");
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly HexHexStormSpell m_Owner;

            public InternalTarget(HexHexStormSpell owner)
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
