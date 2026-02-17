using System;
using Server;
using Server.Spells;
using Server.Items;

using Server.Spells.VystiaSpells;
namespace Server.Spells.VystiaSpells.IceMage
{
    /// <summary>
    /// Glacial Fortress - Ultimate defense buff
    /// Creates ice fortress with massive resistances and damage reflection
    /// Circle: 6th (20 mana)
    /// Reagents: Permafrost Essence, Arctic Pearl, Frozen Soul (Vystia reagents)
    /// </summary>
    public class GlacialFortressSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Glacial Fortress", "Vas An Frio Sanct",
            236,
            9011,
            false
        );

        public override SpellCircle Circle => SpellCircle.Sixth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Ice;

        public GlacialFortressSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            if (!HasReagents(typeof(PermafrostEssence), 1) || !HasReagents(typeof(ArcticPearl), 1) || !HasReagents(typeof(FrozenSoul), 1))
            {
                Caster.SendMessage(0x22, "You do not have the required reagents (Permafrost Essence, Arctic Pearl, Frozen Soul).");
                return false;
            }

            return true;
        }

        public override bool ConsumeReagents()
        {
            return ConsumeReagent(typeof(PermafrostEssence), 1) &&
                   ConsumeReagent(typeof(ArcticPearl), 1) &&
                   ConsumeReagent(typeof(FrozenSoul), 1);
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
                // Visual effect - ice dome
                Caster.FixedParticles(0x3779, 10, 15, 5038, 0x481, 0, EffectLayer.Waist);
                Caster.PlaySound(0x1F8);

                Caster.SendMessage(0x3B2, "You are surrounded by a glacial fortress!");

                // Duration: 60 seconds
                TimeSpan duration = TimeSpan.FromSeconds(60.0);

                // Create resistance bonuses (+25 all)
                ResistanceMod physMod = new ResistanceMod(ResistanceType.Physical, 25);
                ResistanceMod fireMod = new ResistanceMod(ResistanceType.Fire, 25);
                ResistanceMod coldMod = new ResistanceMod(ResistanceType.Cold, 25);
                ResistanceMod poisMod = new ResistanceMod(ResistanceType.Poison, 25);
                ResistanceMod nrgyMod = new ResistanceMod(ResistanceType.Energy, 25);

                Caster.AddResistanceMod(physMod);
                Caster.AddResistanceMod(fireMod);
                Caster.AddResistanceMod(coldMod);
                Caster.AddResistanceMod(poisMod);
                Caster.AddResistanceMod(nrgyMod);

                // TODO: Implement damage reflection (complex, requires damage event hooks)
                // TODO: Implement projectile blocking (complex, requires projectile system hooks)

                // Remove buffs after duration
                Timer.DelayCall(duration, () =>
                {
                    if (Caster != null && !Caster.Deleted)
                    {
                        Caster.RemoveResistanceMod(physMod);
                        Caster.RemoveResistanceMod(fireMod);
                        Caster.RemoveResistanceMod(coldMod);
                        Caster.RemoveResistanceMod(poisMod);
                        Caster.RemoveResistanceMod(nrgyMod);
                        Caster.SendMessage("Your glacial fortress crumbles.");
                        Caster.FixedParticles(0x3735, 1, 30, 9966, 0x481, 0, EffectLayer.Waist);
                    }
                });
            }

            FinishSequence();
        }
    }
}
