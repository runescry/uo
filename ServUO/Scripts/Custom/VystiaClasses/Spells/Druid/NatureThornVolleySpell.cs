using System;
using System.Collections.Generic;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;
using Server.Items;

using Server.Spells.VystiaSpells;
namespace Server.Spells.VystiaSpells.Nature
{
    /// <summary>
    /// Thorn Volley - Deals 12-22 damage to all enemies in area (50% poison, 50% physical)
    /// Circle: 3 (9 mana)
    /// </summary>
    public class NatureThornVolleySpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Thorn Volley", "Thornum Volleyum",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Third;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Nature;

        public NatureThornVolleySpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(DruidBark), 1) || !Caster.Backpack.ConsumeTotal(typeof(TreantSap), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: DruidBark (1), TreantSap (1)");
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
                Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (CheckSequence())
            {
                SpellHelper.Turn(Caster, p);

                Point3D loc = new Point3D(p);
                Map map = Caster.Map;

                // Visual effect - thorns flying outward
                Effects.SendLocationParticles(
                    EffectItem.Create(loc, map, EffectItem.DefaultDuration),
                    0x36BD, 10, 30, 0x21, 0, 5029, 0);
                Caster.PlaySound(0x5D3);

                Caster.SendMessage(0x3B2, "You unleash a volley of thorns!");

                // AoE damage to all enemies in 5 tile radius
                const int RADIUS = 5;
                IPooledEnumerable eable = map.GetMobilesInRange(loc, RADIUS);
                List<Mobile> targets = new List<Mobile>();

                foreach (Mobile m in eable)
                {
                    if (m != Caster && m.Alive && Caster.CanBeHarmful(m, false))
                        targets.Add(m);
                }
                eable.Free();

                foreach (Mobile target in targets)
                {
                    Caster.DoHarmful(target);

                    // Thorn projectile from center to target
                    Effects.SendMovingParticles(
                        new Entity(Serial.Zero, loc, map),
                        target,
                        0x10D3, 7, 0, false, false, 0x21, 0, 9502, 1, 0, (EffectLayer)255, 0x100);

                    // 12-22 damage, 50% physical / 50% poison
                    int damage = Utility.RandomMinMax(12, 22);
                    AOS.Damage(target, Caster, damage, 50, 0, 0, 50, 0);

                    target.FixedParticles(0x374A, 10, 15, 5021, 0x21, 0, EffectLayer.Waist);
                    target.SendMessage(0x22, $"You are struck by thorns for {damage} damage!");
                }

                if (targets.Count > 0)
                    Caster.SendMessage(0x3B2, $"Thorn Volley struck {targets.Count} enemies!");
                else
                    Caster.SendMessage(0x22, "No enemies in range.");
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly NatureThornVolleySpell m_Owner;

            public InternalTarget(NatureThornVolleySpell owner)
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
