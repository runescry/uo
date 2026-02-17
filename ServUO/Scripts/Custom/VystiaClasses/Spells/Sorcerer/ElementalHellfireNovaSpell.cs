using System;
using System.Collections.Generic;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;
using Server.Items;

using Server.Spells.VystiaSpells;
namespace Server.Spells.VystiaSpells.Elemental
{
    /// <summary>
    /// Hellfire Nova - AoE fire explosion centered on caster
    /// Circle: 6 (24 mana)
    /// Damage: 25-35 fire damage to all enemies in 5 tile radius
    /// </summary>
    public class ElementalHellfireNovaSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Hellfire Nova", "Hellfireus Novaum",
            230,
            9022,
            false
        );

        public override SpellCircle Circle => SpellCircle.Sixth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Elemental;

        public ElementalHellfireNovaSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(DragonHeart), 1) || !Caster.Backpack.ConsumeTotal(typeof(PrimordialEmber), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: DragonHeart (1), PrimordialEmber (1)");
                return false;
            }

            return true;
        }

        public override void OnCast()
        {
            if (CheckSequence())
            {
                // Visual effect - massive fire explosion from caster
                Effects.SendLocationParticles(
                    EffectItem.Create(Caster.Location, Caster.Map, EffectItem.DefaultDuration),
                    0x3709, 10, 30, 0x21, 0, 5052, 0);
                Effects.SendLocationParticles(
                    EffectItem.Create(Caster.Location, Caster.Map, EffectItem.DefaultDuration),
                    0x36BD, 20, 10, 0x21, 0, 5044, 0);

                // Sound effect - fire explosion
                Caster.PlaySound(0x208);
                Caster.PlaySound(0x307);

                Caster.SendMessage(0x3B2, "Hellfire explodes outward from your body!");

                // Get all enemies in 5 tile radius
                Map map = Caster.Map;
                if (map == null)
                {
                    FinishSequence();
                    return;
                }

                List<Mobile> targets = new List<Mobile>();
                IPooledEnumerable eable = map.GetMobilesInRange(Caster.Location, 5);

                foreach (Mobile m in eable)
                {
                    if (m != Caster && m.Alive && Caster.CanBeHarmful(m, false))
                        targets.Add(m);
                }
                eable.Free();

                foreach (Mobile target in targets)
                {
                    Caster.DoHarmful(target);

                    // 25-35 fire damage
                    double damage = Utility.RandomMinMax(25, 35);
                    damage += Caster.Skills[GetSchoolSkill()].Value * 0.15;

                    AOS.Damage(target, Caster, (int)damage, 0, 100, 0, 0, 0);

                    // Fire visual on target
                    target.FixedParticles(0x3709, 10, 30, 5052, 0x21, 0, EffectLayer.Waist);
                    target.SendMessage(0x22, "Hellfire burns through you!");
                }

                if (targets.Count > 0)
                    Caster.SendMessage(0x3B2, $"Hellfire Nova scorches {targets.Count} enemies!");
                else
                    Caster.SendMessage(0x22, "No enemies in range.");
            }

            FinishSequence();
        }
    }
}
