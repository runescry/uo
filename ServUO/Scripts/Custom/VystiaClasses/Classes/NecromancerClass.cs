using System;
using Server;
using Server.Items;
using Server.Items.VystiaClassItems;

namespace Server.Custom.VystiaClasses
{
    /// <summary>
    /// Necromancer - Master of death magic who commands undead minions
    /// Region: ShadowVoid
    /// Role: Caster/Pet
    /// </summary>
    public class NecromancerClass : PlayerClass
    {
        public override PlayerClassType ClassType => PlayerClassType.Necromancer;
        public override string ClassName => "Necromancer";
        public override string ClassDescription => "Master of death magic who commands undead minions";

        // Stats (Total: 80) - Caster/Pet
        public override int StartStr => 18;  // Low strength
        public override int StartDex => 17;  // Low dexterity
        public override int StartInt => 45;  // High intelligence (death magic)

        // Stat Caps
        public override int StrCap => 100;
        public override int DexCap => 105;
        public override int IntCap => 145;

        // Primary Skills (7 skills)
        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.NecromancyArts, // Primary class skill - undead control
            SkillName.Necromancy, SkillName.SpiritSpeak, SkillName.Magery, SkillName.Meditation, SkillName.MagicResist, SkillName.EvalInt
        };

        public override double[] StartingSkillValues => new double[]
        {
            50.0, // NecromancyArts
            55.0, 50.0, 40.0, 40.0, 35.0, 20.0
        };

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null)
                return;

            // Cloth Armor
            m.AddItem(new Robe() { Name = "Necromancer's Robe", Hue = 1109 });
        m.AddItem(new Sandals() { Hue = 1109 });
        m.AddItem(new WizardsHat() { Hue = 1109 });

            // Weapon
            m.AddItem(new BoneHarvester() { Name = "Necromancer's BoneHarvester", Hue = 1109 });

            // Starting Resources
            m.Backpack.DropItem(new ObsidianOre(20));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            if (m == null || m.Backpack == null)
                return;

            // Special Class Item
                    // No special class item
        // Give class spellbook
        VystiaNecromancerSpellbook spellbook = new VystiaNecromancerSpellbook();
        m.Backpack.DropItem(spellbook);
        }
    }
}
