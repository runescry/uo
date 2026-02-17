using System;
using System.Collections.Generic;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;
using Server.Items;

using Server.Spells.VystiaSpells;
namespace Server.Spells.VystiaSpells.IceMage
{
    /// <summary>
    /// Frost Armor - Defensive buff that grants physical and cold resistance
    /// Circle: 4th (11 mana)
    /// Duration: 120-240 seconds (scales with Magery)
    /// Reagents: Winterleaf, Glacier Crystal, Permafrost Essence (Vystia reagents)
    /// </summary>
    public class FrostArmorSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Frost Armor", "Sanct Ort Frio",
            236,
            9011,
            false
        );

        public override SpellCircle Circle => SpellCircle.Fourth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Ice;

        public FrostArmorSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            // Check for Vystia reagents (Circle 4: Winterleaf, Glacier Crystal, Permafrost Essence)
            if (!HasReagents(typeof(Winterleaf), 1) || !HasReagents(typeof(GlacierCrystal), 1) || !HasReagents(typeof(PermafrostEssence), 1))
            {
                Caster.SendMessage(0x22, "You do not have the required reagents (Winterleaf, Glacier Crystal, Permafrost Essence).");
                return false;
            }

            return true;
        }

        public override bool ConsumeReagents()
        {
            return ConsumeReagent(typeof(Winterleaf), 1) &&
                   ConsumeReagent(typeof(GlacierCrystal), 1) &&
                   ConsumeReagent(typeof(PermafrostEssence), 1);
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

            Caster.Target = new InternalTarget(this);
        }

        public void Target(Mobile target)
        {
            if (!Caster.CanSee(target))
            {
                Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (CheckBSequence(target))
            {
                SpellHelper.Turn(Caster, target);

                // Calculate duration based on Magery skill (2-4 minutes)
                double duration = 120.0 + (Caster.Skills[GetSchoolSkill()].Value * 1.2);
                TimeSpan buffDuration = TimeSpan.FromSeconds(Math.Min(duration, 240.0));

                // Create resistance bonuses
                ResistanceMod physMod = new ResistanceMod(ResistanceType.Physical, 10);
                ResistanceMod coldMod = new ResistanceMod(ResistanceType.Cold, 20);

                target.AddResistanceMod(physMod);
                target.AddResistanceMod(coldMod);

                // Equip ice-colored plate armor
                List<Item> armorPieces = new List<Item>();
                int iceHue = 0x481; // Ice blue hue

                // Create and equip plate pieces
                PlateChest chest = new PlateChest { Hue = iceHue, Movable = false, LootType = LootType.Blessed };
                PlateLegs legs = new PlateLegs { Hue = iceHue, Movable = false, LootType = LootType.Blessed };
                PlateArms arms = new PlateArms { Hue = iceHue, Movable = false, LootType = LootType.Blessed };
                PlateGloves gloves = new PlateGloves { Hue = iceHue, Movable = false, LootType = LootType.Blessed };
                PlateHelm helm = new PlateHelm { Hue = iceHue, Movable = false, LootType = LootType.Blessed };
                PlateGorget gorget = new PlateGorget { Hue = iceHue, Movable = false, LootType = LootType.Blessed };

                // Store original items if any
                List<Item> originalItems = new List<Item>();
                if (target.FindItemOnLayer(Layer.InnerTorso) != null)
                    originalItems.Add(target.FindItemOnLayer(Layer.InnerTorso));
                if (target.FindItemOnLayer(Layer.Pants) != null)
                    originalItems.Add(target.FindItemOnLayer(Layer.Pants));
                if (target.FindItemOnLayer(Layer.Arms) != null)
                    originalItems.Add(target.FindItemOnLayer(Layer.Arms));
                if (target.FindItemOnLayer(Layer.Gloves) != null)
                    originalItems.Add(target.FindItemOnLayer(Layer.Gloves));
                if (target.FindItemOnLayer(Layer.Helm) != null)
                    originalItems.Add(target.FindItemOnLayer(Layer.Helm));
                if (target.FindItemOnLayer(Layer.Neck) != null)
                    originalItems.Add(target.FindItemOnLayer(Layer.Neck));

                // Equip the ice plate armor
                target.EquipItem(chest);
                target.EquipItem(legs);
                target.EquipItem(arms);
                target.EquipItem(gloves);
                target.EquipItem(helm);
                target.EquipItem(gorget);

                armorPieces.AddRange(new Item[] { chest, legs, arms, gloves, helm, gorget });

                // Visual effect - frost aura
                target.FixedParticles(0x375A, 9, 20, 5027, 0x481, 0, EffectLayer.Waist);
                target.PlaySound(0x1E9);

                target.SendMessage(0x481, "You are encased in protective frost armor!");

                // Remove buffs and armor after duration
                Timer.DelayCall(buffDuration, () =>
                {
                    if (target != null && !target.Deleted)
                    {
                        // Remove resistance mods
                        target.RemoveResistanceMod(physMod);
                        target.RemoveResistanceMod(coldMod);

                        // Remove ice armor pieces
                        foreach (Item piece in armorPieces)
                        {
                            if (piece != null && !piece.Deleted)
                            {
                                piece.Delete();
                            }
                        }

                        // Re-equip original items if they still exist
                        foreach (Item orig in originalItems)
                        {
                            if (orig != null && !orig.Deleted && target.Backpack != null)
                            {
                                target.EquipItem(orig);
                            }
                        }

                        target.SendMessage("Your frost armor dissipates.");
                        target.FixedParticles(0x3735, 1, 30, 9966, 0x481, 0, EffectLayer.Waist);
                    }
                });
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly FrostArmorSpell m_Owner;

            public InternalTarget(FrostArmorSpell owner)
                : base(12, false, TargetFlags.Beneficial)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is Mobile)
                    m_Owner.Target((Mobile)o);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}
