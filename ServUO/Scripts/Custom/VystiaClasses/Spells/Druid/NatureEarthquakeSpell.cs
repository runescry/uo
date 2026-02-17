using System;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;
using Server.Items;

using Server.Spells.VystiaSpells;
namespace Server.Spells.VystiaSpells.Nature
{
    /// <summary>
    /// Earthquake - Deals 20-35 physical damage, knocks down enemies (stun 2s), destroys ice walls/barriers
    /// Circle: 5 (14 mana)
    /// </summary>
    public class NatureEarthquakeSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Earthquake", "Earthquakeus",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Fifth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Nature;

        public NatureEarthquakeSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(ElderwoodSeed), 1) || !Caster.Backpack.ConsumeTotal(typeof(PrimalVine), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: ElderwoodSeed (1), PrimalVine (1)");
                return false;
            }



            return true;
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
                Map map = Caster.Map;

                if (map == null)
                {
                    FinishSequence();
                    return;
                }

                // Visual effect - ground shaking
                Effects.SendLocationParticles(
                    EffectItem.Create(loc, map, EffectItem.DefaultDuration),
                    0x36B0, 10, 30, 0x21, 0, 5044, 0);
                Effects.PlaySound(loc, map, 0x307);

                Caster.SendMessage(0x3B2, "The earth trembles beneath your enemies!");

                // Get all mobiles in 5-tile radius
                System.Collections.Generic.List<Mobile> targets = new System.Collections.Generic.List<Mobile>();
                IPooledEnumerable eable = map.GetMobilesInRange(loc, 5);

                foreach (Mobile m in eable)
                {
                    if (m != Caster && m.Alive && Caster.CanBeHarmful(m, false))
                        targets.Add(m);
                }
                eable.Free();

                foreach (Mobile m in targets)
                {
                    Caster.DoHarmful(m);

                    // 20-35 physical damage
                    double damage = Utility.RandomMinMax(20, 35);
                    damage += Caster.Skills.Magery.Value / 10.0;

                    SpellHelper.Damage(this, m, damage, 100, 0, 0, 0, 0);

                    m.FixedParticles(0x36B0, 10, 20, 5044, 0x21, 0, EffectLayer.Waist);

                    // 2 second stun (knockdown) - use Paralyzed for proper stun
                    m.Paralyzed = true;
                    Timer.DelayCall(TimeSpan.FromSeconds(2.0), () =>
                    {
                        if (m != null && !m.Deleted)
                            m.Paralyzed = false;
                    });
                    m.SendMessage(0x22, "The earthquake knocks you down! (Stunned 2s)");
                }

                // Destroy ice walls and barriers in the area
                int wallsDestroyed = 0;
                IPooledEnumerable items = map.GetItemsInRange(loc, 5);
                System.Collections.Generic.List<Item> toDestroy = new System.Collections.Generic.List<Item>();

                foreach (Item item in items)
                {
                    // Check for ice-related field/wall items by type name
                    string name = item.GetType().Name.ToLower();
                    if (name.Contains("icewall") || name.Contains("ice") && name.Contains("field") ||
                        name.Contains("wallofice") || name.Contains("icebarrier"))
                    {
                        toDestroy.Add(item);
                    }
                }
                items.Free();

                foreach (Item wall in toDestroy)
                {
                    // Visual destruction effect
                    Effects.SendLocationParticles(
                        EffectItem.Create(wall.Location, map, EffectItem.DefaultDuration),
                        0x36B0, 10, 20, 0x481, 0, 5044, 0);
                    wall.Delete();
                    wallsDestroyed++;
                }

                if (targets.Count > 0 || wallsDestroyed > 0)
                {
                    string msg = $"Your earthquake hits {targets.Count} targets";
                    if (wallsDestroyed > 0)
                        msg += $" and destroys {wallsDestroyed} ice walls";
                    Caster.SendMessage(0x3B2, msg + "!");
                }
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly NatureEarthquakeSpell m_Owner;

            public InternalTarget(NatureEarthquakeSpell owner)
                : base(10, true, TargetFlags.None)
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
