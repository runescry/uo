using System;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;
using Server.Items;

using Server.Spells.VystiaSpells;
namespace Server.Spells.VystiaSpells.Hex
{
    /// <summary>
    /// Doomcurse - After 12 seconds, target takes massive damage (100-150) unless dispelled. Cannot be removed by normal means.
    /// Circle: 6 (20 mana)
    /// </summary>
    public class HexDoomcurseSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Doomcurse", "Doomcurseus",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Sixth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Hex;

        public HexDoomcurseSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
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

                Point3D loc = new Point3D(p);
                Map map = Caster.Map;

                // Visual effect at location
                Effects.SendLocationParticles(
                    EffectItem.Create(loc, map, EffectItem.DefaultDuration),
                    0x36BD, 20, 30, 0x21, 0, 5044, 0);
                Effects.PlaySound(loc, map, 0x307);

                Caster.SendMessage(0x3B2, "Your spell erupts!");

                // Doomcurse - After 12 seconds, target takes massive damage (100-150) unless dispelled. Cannot be removed by normal means. Circle: 6 (20 mana)
                System.Collections.Generic.List<Mobile> targets = new System.Collections.Generic.List<Mobile>();
                IPooledEnumerable eable = map.GetMobilesInRange(loc, 4);

                foreach (Mobile m in eable)
                {
                    if (m != Caster && m.Alive && Caster.CanBeHarmful(m, false))
                        targets.Add(m);
                }
                eable.Free();

                foreach (Mobile m in targets)
                {
                    Caster.DoHarmful(m);

                    double damage = Utility.RandomMinMax(100, 150);
                    damage += Caster.Skills.Magery.Value / 10.0;

                    SpellHelper.Damage(this, m, damage, 100, 0, 0, 0, 0);

                    m.FixedParticles(0x36BD, 10, 20, 5044, 0x21, 0, EffectLayer.Waist);
                }

                if (targets.Count > 0)
                    Caster.SendMessage(0x3B2, $"You strike {targets.Count} enemies!");
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly HexDoomcurseSpell m_Owner;

            public InternalTarget(HexDoomcurseSpell owner)
                : base(12, true, TargetFlags.None)
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
