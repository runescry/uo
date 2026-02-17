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
    /// Shadow Nova - AoE shadow explosion centered on caster
    /// Circle: 3 (9 mana)
    /// Damage: 12-18 physical to all enemies in 4 tile radius
    /// </summary>
    public class DarkShadowNovaSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Shadow Nova", "Umbra Novaum",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Third;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Dark;

        public DarkShadowNovaSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(VoidWeed), 1) || !Caster.Backpack.ConsumeTotal(typeof(ShadowPetal), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: VoidWeed (1), ShadowPetal (1)");
                return false;
            }

            return true;
        }

        public override void OnCast()
        {
            if (CheckSequence())
            {
                // Visual effect - shadow explosion from caster
                Effects.SendLocationParticles(
                    EffectItem.Create(Caster.Location, Caster.Map, EffectItem.DefaultDuration),
                    0x3709, 10, 30, 0x455, 0, 5052, 0);

                // Sound effect
                Caster.PlaySound(0x307);

                Caster.SendMessage(0x3B2, "Shadow energy explodes outward!");

                // Get all enemies in 4 tile radius
                Map map = Caster.Map;
                if (map == null)
                {
                    FinishSequence();
                    return;
                }

                List<Mobile> targets = new List<Mobile>();
                IPooledEnumerable eable = map.GetMobilesInRange(Caster.Location, 4);

                foreach (Mobile m in eable)
                {
                    if (m != Caster && m.Alive && Caster.CanBeHarmful(m, false))
                        targets.Add(m);
                }
                eable.Free();

                foreach (Mobile target in targets)
                {
                    Caster.DoHarmful(target);

                    // 12-18 physical damage (shadow/physical)
                    double damage = Utility.RandomMinMax(12, 18);
                    damage += Caster.Skills[GetSchoolSkill()].Value * 0.08;

                    AOS.Damage(target, Caster, (int)damage, 100, 0, 0, 0, 0);

                    target.FixedParticles(0x374A, 10, 15, 5038, 0x455, 0, EffectLayer.Waist);
                    target.SendMessage(0x22, "You are struck by the shadow nova!");
                }

                if (targets.Count > 0)
                    Caster.SendMessage(0x3B2, $"Shadow Nova hits {targets.Count} enemies!");
                else
                    Caster.SendMessage(0x22, "No enemies in range.");
            }

            FinishSequence();
        }
    }
}
