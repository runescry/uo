using System;
using System.Collections.Generic;
using Server;
using Server.Spells;
using Server.Items;

using Server.Spells.VystiaSpells;
namespace Server.Spells.VystiaSpells.IceMage
{
    /// <summary>
    /// Fimbulwinter's Wrath - Transformation ultimate
    /// Caster becomes avatar of winter with massive power boost
    /// Circle: 7th (40 mana)
    /// Reagents: Arctic Pearl, Frozen Soul, EternalIce, HeartOfWinter (Vystia reagents)
    /// </summary>
    public class FimbulwintersWrathSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Fimbulwinter's Wrath", "Kal Vas Corp Frio",
            236,
            9011,
            false
        );

        public override SpellCircle Circle => SpellCircle.Seventh;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Ice;

        public FimbulwintersWrathSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            if (!HasReagents(typeof(ArcticPearl), 1) || !HasReagents(typeof(FrozenSoul), 1) ||
                !HasReagents(typeof(FrostEssence), 1) || !HasReagents(typeof(HeartOfWinter), 1))
            {
                Caster.SendMessage(0x22, "You do not have the required reagents (Arctic Pearl, Frozen Soul, Eternal Ice, Heart of Winter).");
                return false;
            }

            return true;
        }

        public override bool ConsumeReagents()
        {
            return ConsumeReagent(typeof(ArcticPearl), 1) &&
                   ConsumeReagent(typeof(FrozenSoul), 1) &&
                   ConsumeReagent(typeof(FrostEssence), 1) &&
                   ConsumeReagent(typeof(HeartOfWinter), 1);
        }

        private bool HasReagents(Type type, int amount)
        {
            return (Caster.Backpack != null && Caster.Backpack.GetAmount(type) >= amount);
        }

        private bool ConsumeReagent(Type type, int amount)
        {
            if (Caster.Backpack == null)
                return false;

            return Caster.Backpack.ConsumeTotal(type, amount);
        }

        public override void OnCast()
        {
// Check fizzle and trigger skill gain

            if (CheckSequence())
            {
                // Fimbulwinter's Wrath - MASSIVE NUKE (Circle 7 ultimate)
                // Visual effects - dramatic ice explosion
                Caster.FixedParticles(0x375A, 10, 15, 5038, 0x481, 0, EffectLayer.Waist);
                Caster.FixedParticles(0x3779, 10, 15, 5038, 0x481, 0, EffectLayer.Head);
                Caster.PlaySound(0x307);

                Caster.SendMessage(0x3B2, "You unleash the wrath of Fimbulwinter!");

                // Get all enemies in a 10 tile radius
                int radius = 10;
                IPooledEnumerable eable = Caster.Map.GetMobilesInRange(Caster.Location, radius);
                List<Mobile> targets = new List<Mobile>();

                foreach (Mobile m in eable)
                {
                    if (m != Caster && m.Alive && Caster.CanBeHarmful(m, false))
                    {
                        targets.Add(m);
                    }
                }
                eable.Free();

                // Calculate massive nuke damage: 80-120 base + magery scaling
                // Circle 7 should be devastating
                double baseDamage = 80.0 + (Caster.Skills[GetSchoolSkill()].Value * 0.40);

                foreach (Mobile target in targets)
                {
                    Caster.DoHarmful(target);

                    // Visual effect on each target - ice explosion
                    target.FixedParticles(0x375A, 10, 30, 5038, 0x481, 0, EffectLayer.Waist);
                    Effects.SendMovingParticles(Caster, target, 0x36D4, 7, 0, false, true, 0x481, 0, 9502, 1, 0, (EffectLayer)255, 0x100);

                    // Deal massive cold damage
                    int damage = (int)(baseDamage + Utility.RandomMinMax(-10, 20));
                    AOS.Damage(target, Caster, damage, 0, 0, 100, 0, 0);

                    // Chance to freeze (stun) for 3 seconds
                    if (Utility.RandomDouble() < 0.50) // 50% freeze chance
                    {
                        target.Frozen = true;
                        target.SendMessage(0x481, "You are frozen solid by the wrath of Fimbulwinter!");

                        Timer.DelayCall(TimeSpan.FromSeconds(3.0), () =>
                        {
                            if (target != null && !target.Deleted)
                            {
                                target.Frozen = false;
                                target.SendMessage("The ice shatters and you can move again.");
                            }
                        });
                    }

                    target.SendMessage(0x481, $"Fimbulwinter's Wrath deals {damage} cold damage!");
                }

                if (targets.Count == 0)
                {
                    Caster.SendMessage(0x22, "No enemies in range to feel winter's wrath.");
                }
                else
                {
                    Caster.SendMessage(0x3B2, $"Fimbulwinter's Wrath struck {targets.Count} enemies!");
                }
            }

            FinishSequence();
        }
    }
}
