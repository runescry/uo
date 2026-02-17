using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Gumps;
using Server.Network;
using Server.Custom.VystiaClasses;
using Server.Custom.VystiaClasses.Gumps;

namespace Server.Mobiles
{
    /// <summary>
    /// Vystia Class Trainer NPCs
    /// One trainer per class (26 total)
    /// Trainers can:
    /// - Explain class mechanics and lore
    /// - Teach class-specific skills (with gold costs)
    /// - Open class selection gump for new players
    /// - Reset class for GM characters
    ///
    /// Training Costs (per 20 skill points):
    /// - 0-20: 500g (Novice)
    /// - 20-40: 2,000g (Apprentice)
    /// - 40-60: 5,000g (Journeyman)
    /// - 60-80: 10,000g (Expert)
    /// - 80-100: 25,000g (Master) - requires special quest completion
    /// </summary>

    #region Training Cost Structure

    public static class VystiaTrainingCosts
    {
        // Cost per 20 skill points at each tier
        public static readonly int[] TierCosts = { 500, 2000, 5000, 10000, 25000 };

        // Skill thresholds for each tier
        public static readonly double[] TierThresholds = { 0.0, 20.0, 40.0, 60.0, 80.0 };

        // Tier names
        public static readonly string[] TierNames = { "Novice", "Apprentice", "Journeyman", "Expert", "Master" };

        /// <summary>
        /// Get the tier index for a given skill level
        /// </summary>
        public static int GetTier(double skillLevel)
        {
            for (int i = TierThresholds.Length - 1; i >= 0; i--)
            {
                if (skillLevel >= TierThresholds[i])
                    return i;
            }
            return 0;
        }

        /// <summary>
        /// Calculate cost to train from current level to next tier
        /// </summary>
        public static int GetTrainingCost(double currentSkill, double targetSkill)
        {
            if (targetSkill <= currentSkill)
                return 0;

            int totalCost = 0;
            double remaining = targetSkill - currentSkill;
            double workingSkill = currentSkill;

            while (remaining > 0)
            {
                int tier = GetTier(workingSkill);
                double nextThreshold = tier < TierThresholds.Length - 1 ? TierThresholds[tier + 1] : 100.0;

                // How much can we train in this tier?
                double tierRemaining = Math.Min(nextThreshold - workingSkill, remaining);

                // Cost for this portion (proportional)
                int tierCost = TierCosts[tier];
                double portionCost = (tierRemaining / 20.0) * tierCost;
                totalCost += (int)Math.Ceiling(portionCost);

                workingSkill += tierRemaining;
                remaining -= tierRemaining;
            }

            return totalCost;
        }

        /// <summary>
        /// Get the next tier threshold above current skill
        /// </summary>
        public static double GetNextTierThreshold(double currentSkill)
        {
            for (int i = 0; i < TierThresholds.Length; i++)
            {
                if (TierThresholds[i] > currentSkill)
                    return TierThresholds[i];
            }
            return 100.0;
        }
    }

    #endregion

    #region Training Gump

    public class VystiaSkillTrainingGump : Gump
    {
        private VystiaClassTrainer m_Trainer;
        private PlayerMobile m_Player;
        private SkillName[] m_Skills;

        public VystiaSkillTrainingGump(VystiaClassTrainer trainer, PlayerMobile player) : base(50, 50)
        {
            m_Trainer = trainer;
            m_Player = player;
            m_Skills = trainer.TrainableSkills;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            BuildGump();
        }

        private void BuildGump()
        {
            AddPage(0);

            // Background
            AddBackground(0, 0, 450, 400, 9270);
            AddAlphaRegion(10, 10, 430, 380);

            // Title
            AddHtml(15, 15, 420, 25, String.Format("<CENTER><BASEFONT COLOR=#FFD700><BIG>{0} Training</BIG></BASEFONT></CENTER>", m_Trainer.TrainerClass), false, false);
            AddHtml(15, 40, 420, 20, String.Format("<CENTER><BASEFONT COLOR=#AAAAAA>Trainer: {0}</BASEFONT></CENTER>", m_Trainer.Name), false, false);

            // Current gold display
            int playerGold = Banker.GetBalance(m_Player) + GetBackpackGold(m_Player);
            AddHtml(15, 65, 420, 20, String.Format("<CENTER><BASEFONT COLOR=#FFFF00>Your Gold: {0:N0}</BASEFONT></CENTER>", playerGold), false, false);

            // Skill list header
            AddHtml(20, 95, 150, 20, "<BASEFONT COLOR=#FFFFFF>Skill</BASEFONT>", false, false);
            AddHtml(170, 95, 60, 20, "<BASEFONT COLOR=#FFFFFF>Current</BASEFONT>", false, false);
            AddHtml(230, 95, 60, 20, "<BASEFONT COLOR=#FFFFFF>Tier</BASEFONT>", false, false);
            AddHtml(310, 95, 80, 20, "<BASEFONT COLOR=#FFFFFF>Cost (20pt)</BASEFONT>", false, false);
            AddHtml(390, 95, 50, 20, "<BASEFONT COLOR=#FFFFFF>Train</BASEFONT>", false, false);

            // Horizontal line
            AddImageTiled(15, 115, 420, 2, 2624);

            int y = 125;
            for (int i = 0; i < m_Skills.Length && i < 10; i++)
            {
                SkillName skill = m_Skills[i];
                Skill playerSkill = m_Player.Skills[skill];
                double currentValue = playerSkill.Base;
                int tier = VystiaTrainingCosts.GetTier(currentValue);
                int tierCost = VystiaTrainingCosts.TierCosts[tier];
                string tierName = VystiaTrainingCosts.TierNames[tier];

                // Can they train more?
                bool canTrain = currentValue < 100.0 && currentValue < playerSkill.Cap;
                string skillColor = canTrain ? "#FFFFFF" : "#888888";
                string tierColor = GetTierColor(tier);

                AddHtml(20, y, 145, 20, String.Format("<BASEFONT COLOR={0}>{1}</BASEFONT>", skillColor, skill.ToString()), false, false);
                AddHtml(170, y, 55, 20, String.Format("<BASEFONT COLOR={0}>{1:F1}</BASEFONT>", skillColor, currentValue), false, false);
                AddHtml(230, y, 75, 20, String.Format("<BASEFONT COLOR={0}>{1}</BASEFONT>", tierColor, tierName), false, false);
                AddHtml(310, y, 70, 20, String.Format("<BASEFONT COLOR=#FFD700>{0:N0}g</BASEFONT>", tierCost), false, false);

                if (canTrain)
                {
                    AddButton(400, y, 4005, 4007, 100 + i, GumpButtonType.Reply, 0);
                }

                y += 25;
            }

            // Training cost legend
            y += 10;
            AddImageTiled(15, y, 420, 2, 2624);
            y += 10;

            AddHtml(15, y, 420, 20, "<BASEFONT COLOR=#FFD700>Training Cost Tiers (per 20 skill points):</BASEFONT>", false, false);
            y += 22;

            for (int i = 0; i < VystiaTrainingCosts.TierNames.Length; i++)
            {
                string color = GetTierColor(i);
                double start = VystiaTrainingCosts.TierThresholds[i];
                double end = i < VystiaTrainingCosts.TierThresholds.Length - 1 ? VystiaTrainingCosts.TierThresholds[i + 1] : 100.0;
                AddHtml(20, y, 420, 18, String.Format("<BASEFONT COLOR={0}>{1} ({2:F0}-{3:F0}): {4:N0} gold</BASEFONT>",
                    color, VystiaTrainingCosts.TierNames[i], start, end, VystiaTrainingCosts.TierCosts[i]), false, false);
                y += 18;
            }

            // Close button
            AddButton(380, 365, 4017, 4019, 0, GumpButtonType.Reply, 0);
            AddHtml(340, 368, 40, 20, "<BASEFONT COLOR=#FFFFFF>Close</BASEFONT>", false, false);
        }

        private string GetTierColor(int tier)
        {
            switch (tier)
            {
                case 0: return "#AAFFAA"; // Novice - light green
                case 1: return "#AAAAFF"; // Apprentice - light blue
                case 2: return "#FFFF88"; // Journeyman - yellow
                case 3: return "#FFAA44"; // Expert - orange
                case 4: return "#FF5555"; // Master - red
                default: return "#FFFFFF";
            }
        }

        private int GetBackpackGold(Mobile m)
        {
            if (m.Backpack == null)
                return 0;

            Item[] gold = m.Backpack.FindItemsByType(typeof(Gold));
            int total = 0;
            foreach (Item g in gold)
                total += g.Amount;
            return total;
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID >= 100 && info.ButtonID < 100 + m_Skills.Length)
            {
                int skillIndex = info.ButtonID - 100;
                SkillName skill = m_Skills[skillIndex];

                // Open the training confirmation gump
                m_Player.SendGump(new VystiaSkillTrainConfirmGump(m_Trainer, m_Player, skill));
            }
        }
    }

    public class VystiaSkillTrainConfirmGump : Gump
    {
        private VystiaClassTrainer m_Trainer;
        private PlayerMobile m_Player;
        private SkillName m_Skill;
        private double m_TrainAmount;

        public VystiaSkillTrainConfirmGump(VystiaClassTrainer trainer, PlayerMobile player, SkillName skill) : base(100, 100)
        {
            m_Trainer = trainer;
            m_Player = player;
            m_Skill = skill;

            Skill playerSkill = player.Skills[skill];
            double current = playerSkill.Base;
            double cap = playerSkill.Cap;

            // Default to training to next tier or cap, whichever is lower
            double nextTier = VystiaTrainingCosts.GetNextTierThreshold(current);
            m_TrainAmount = Math.Min(nextTier, cap) - current;
            if (m_TrainAmount > 20.0) m_TrainAmount = 20.0; // Max 20 per training session

            Closable = true;
            Disposable = true;
            Dragable = true;

            BuildGump();
        }

        private void BuildGump()
        {
            AddPage(0);

            Skill playerSkill = m_Player.Skills[m_Skill];
            double current = playerSkill.Base;
            double cap = playerSkill.Cap;
            double maxTrainable = Math.Min(cap - current, 20.0);

            int cost5 = VystiaTrainingCosts.GetTrainingCost(current, current + Math.Min(5.0, maxTrainable));
            int cost10 = VystiaTrainingCosts.GetTrainingCost(current, current + Math.Min(10.0, maxTrainable));
            int cost20 = VystiaTrainingCosts.GetTrainingCost(current, current + Math.Min(20.0, maxTrainable));

            int playerGold = Banker.GetBalance(m_Player) + GetBackpackGold(m_Player);

            AddBackground(0, 0, 350, 280, 9270);
            AddAlphaRegion(10, 10, 330, 260);

            AddHtml(15, 15, 320, 25, String.Format("<CENTER><BASEFONT COLOR=#FFD700><BIG>Train {0}</BIG></BASEFONT></CENTER>", m_Skill), false, false);

            AddHtml(20, 50, 310, 20, String.Format("<BASEFONT COLOR=#FFFFFF>Current Skill: {0:F1} / {1:F1}</BASEFONT>", current, cap), false, false);
            AddHtml(20, 70, 310, 20, String.Format("<BASEFONT COLOR=#FFFF00>Your Gold: {0:N0}</BASEFONT>", playerGold), false, false);

            int y = 100;

            AddHtml(20, y, 310, 20, "<BASEFONT COLOR=#AAAAAA>Select training amount:</BASEFONT>", false, false);
            y += 25;

            // Train +5 button
            if (maxTrainable >= 5.0)
            {
                bool canAfford = playerGold >= cost5;
                AddButton(20, y, canAfford ? 4005 : 4006, 4007, 1, GumpButtonType.Reply, 0);
                AddHtml(55, y, 280, 20, String.Format("<BASEFONT COLOR={0}>+5.0 skill points - {1:N0} gold</BASEFONT>",
                    canAfford ? "#FFFFFF" : "#888888", cost5), false, false);
                y += 25;
            }

            // Train +10 button
            if (maxTrainable >= 10.0)
            {
                bool canAfford = playerGold >= cost10;
                AddButton(20, y, canAfford ? 4005 : 4006, 4007, 2, GumpButtonType.Reply, 0);
                AddHtml(55, y, 280, 20, String.Format("<BASEFONT COLOR={0}>+10.0 skill points - {1:N0} gold</BASEFONT>",
                    canAfford ? "#FFFFFF" : "#888888", cost10), false, false);
                y += 25;
            }

            // Train +20 button (or max available)
            if (maxTrainable > 0)
            {
                bool canAfford = playerGold >= cost20;
                string label = maxTrainable >= 20.0 ? "+20.0 skill points" : String.Format("+{0:F1} skill points (max)", maxTrainable);
                AddButton(20, y, canAfford ? 4005 : 4006, 4007, 3, GumpButtonType.Reply, 0);
                AddHtml(55, y, 280, 20, String.Format("<BASEFONT COLOR={0}>{1} - {2:N0} gold</BASEFONT>",
                    canAfford ? "#FFFFFF" : "#888888", label, cost20), false, false);
                y += 25;
            }

            y += 10;

            // Back and Close buttons
            AddButton(20, 240, 4014, 4016, 4, GumpButtonType.Reply, 0);
            AddHtml(55, 243, 60, 20, "<BASEFONT COLOR=#FFFFFF>Back</BASEFONT>", false, false);

            AddButton(280, 240, 4017, 4019, 0, GumpButtonType.Reply, 0);
            AddHtml(240, 243, 40, 20, "<BASEFONT COLOR=#FFFFFF>Close</BASEFONT>", false, false);
        }

        private int GetBackpackGold(Mobile m)
        {
            if (m.Backpack == null)
                return 0;

            Item[] gold = m.Backpack.FindItemsByType(typeof(Gold));
            int total = 0;
            foreach (Item g in gold)
                total += g.Amount;
            return total;
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Skill playerSkill = m_Player.Skills[m_Skill];
            double current = playerSkill.Base;
            double cap = playerSkill.Cap;
            double maxTrainable = Math.Min(cap - current, 20.0);

            double trainAmount = 0;
            switch (info.ButtonID)
            {
                case 1: trainAmount = Math.Min(5.0, maxTrainable); break;
                case 2: trainAmount = Math.Min(10.0, maxTrainable); break;
                case 3: trainAmount = Math.Min(20.0, maxTrainable); break;
                case 4:
                    // Back to main training gump
                    m_Player.SendGump(new VystiaSkillTrainingGump(m_Trainer, m_Player));
                    return;
                default: return;
            }

            if (trainAmount <= 0)
            {
                m_Player.SendMessage(0x22, "You cannot train this skill any further.");
                return;
            }

            int cost = VystiaTrainingCosts.GetTrainingCost(current, current + trainAmount);
            int playerGold = Banker.GetBalance(m_Player) + GetBackpackGold(m_Player);

            if (playerGold < cost)
            {
                m_Player.SendMessage(0x22, "You do not have enough gold. You need {0:N0} gold.", cost);
                m_Player.SendGump(new VystiaSkillTrainConfirmGump(m_Trainer, m_Player, m_Skill));
                return;
            }

            // Deduct gold (try backpack first, then bank)
            if (!DeductGold(m_Player, cost))
            {
                m_Player.SendMessage(0x22, "Failed to deduct gold.");
                return;
            }

            // Apply training
            double newValue = current + trainAmount;
            playerSkill.Base = newValue;

            // Trainer says something encouraging
            m_Trainer.SayTo(m_Player, "Well done! Your {0} has improved to {1:F1}.", m_Skill.ToString(), newValue);
            m_Player.SendMessage(0x35, "You paid {0:N0} gold and trained {1} from {2:F1} to {3:F1}.", cost, m_Skill, current, newValue);

            // Effects
            m_Player.PlaySound(0x1F7); // Training sound
            m_Player.FixedParticles(0x376A, 9, 32, 5030, EffectLayer.Waist);

            // Reopen training gump
            m_Player.SendGump(new VystiaSkillTrainingGump(m_Trainer, m_Player));
        }

        private bool DeductGold(Mobile m, int amount)
        {
            // Try backpack first
            if (m.Backpack != null)
            {
                Item[] gold = m.Backpack.FindItemsByType(typeof(Gold));
                int backpackGold = 0;
                foreach (Item g in gold)
                    backpackGold += g.Amount;

                if (backpackGold >= amount)
                {
                    // Consume from backpack
                    m.Backpack.ConsumeTotal(typeof(Gold), amount);
                    return true;
                }
                else if (backpackGold > 0)
                {
                    // Consume what we can from backpack
                    m.Backpack.ConsumeTotal(typeof(Gold), backpackGold);
                    amount -= backpackGold;
                }
            }

            // Remainder from bank
            return Banker.Withdraw(m, amount);
        }
    }

    #endregion

    #region Base Trainer

    public abstract class VystiaClassTrainer : BaseVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        public abstract PlayerClassTypeV2 TrainerClass { get; }
        public abstract string TrainerGreeting { get; }
        public abstract string ClassDescription { get; }
        public abstract SkillName[] TrainableSkills { get; }

        public override bool IsActiveVendor { get { return false; } }
        public override bool CanTeach { get { return false; } } // Disable default free teaching

        public VystiaClassTrainer(string title) : base(title)
        {
        }

        public override void InitSBInfo()
        {
            // Trainers don't sell items - they teach skills
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            Mobile from = e.Mobile;

            if (!e.Handled && from.InRange(this.Location, 3))
            {
                string speech = e.Speech.ToLower();

                if (speech.Contains("join") || speech.Contains("class"))
                {
                    HandleClassInquiry(from);
                    e.Handled = true;
                }
                else if (speech.Contains("train") || speech.Contains("skill") || speech.Contains("learn"))
                {
                    HandleSkillInquiry(from);
                    e.Handled = true;
                }
                else if (speech.Contains("hello") || speech.Contains("greet"))
                {
                    SayTo(from, TrainerGreeting);
                    e.Handled = true;
                }
            }

            base.OnSpeech(e);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(this.Location, 4))
            {
                PlayerMobile pm = from as PlayerMobile;
                if (pm != null)
                {
                    if (VystiaClassManager.Instance.GetClassType(pm) == PlayerClassTypeV2.None)
                    {
                        SayTo(from, TrainerGreeting);
                        SayTo(from, "Say 'join' if you wish to learn the ways of my class.");
                    }
                    else if (VystiaClassManager.Instance.GetClassType(pm) == TrainerClass)
                    {
                        // Open training gump directly for matching class
                        SayTo(from, "Welcome back! Let us continue your training.");
                        pm.SendGump(new VystiaSkillTrainingGump(this, pm));
                    }
                    else
                    {
                        SayTo(from, "You have already chosen a different path. I cannot help you.");
                    }
                }
            }
            else
            {
                from.SendLocalizedMessage(500446); // That is too far away.
            }
        }

        private void HandleClassInquiry(Mobile from)
        {
            PlayerMobile pm = from as PlayerMobile;
            if (pm == null)
                return;

            if (VystiaClassManager.Instance.GetClassType(pm) == PlayerClassTypeV2.None)
            {
                SayTo(from, ClassDescription);
                SayTo(from, "If you wish to join our ranks, confirm your choice.");
                pm.SendGump(new VystiaClassConfirmationGump(pm, TrainerClass));
            }
            else if (VystiaClassManager.Instance.GetClassType(pm) == TrainerClass)
            {
                SayTo(from, "You are already one of us. Well met!");
            }
            else
            {
                SayTo(from, "You have already chosen a different path.");
            }
        }

        private void HandleSkillInquiry(Mobile from)
        {
            PlayerMobile pm = from as PlayerMobile;
            if (pm == null)
                return;

            if (VystiaClassManager.Instance.GetClassType(pm) != TrainerClass)
            {
                SayTo(from, "I only train those who have chosen my path.");
                return;
            }

            // Open the training gump
            SayTo(from, "Let me show you what training I can offer.");
            pm.SendGump(new VystiaSkillTrainingGump(this, pm));
        }

        public VystiaClassTrainer(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region Barbarian Trainer

    [CorpseNameAttribute("corpse of a barbarian battlemaster")]
    public class BarbarianTrainer : VystiaClassTrainer
    {
        public override PlayerClassTypeV2 TrainerClass { get { return PlayerClassTypeV2.Barbarian; } }
        public override string TrainerGreeting { get { return "The frozen north breeds the strongest warriors. Are you ready to embrace the rage?"; } }
        public override string ClassDescription { get { return "Savage warriors who channel primal rage into devastating melee attacks."; } }
        public override SkillName[] TrainableSkills { get { return new SkillName[] { SkillName.Swords, SkillName.Tactics, SkillName.Anatomy, SkillName.Healing, SkillName.Parry, SkillName.MagicResist }; } }

        [Constructable]
        public BarbarianTrainer() : base("Barbarian Battlemaster")
        {
            Name = NameList.RandomName(Female ? "female" : "male");
            Title = "the Barbarian Battlemaster";
            Hue = 1150;

            SetSkill(SkillName.Wrestling, 100.0);
            SetSkill(SkillName.Tactics, 100.0);
        }

        public BarbarianTrainer(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region Beastmaster Trainer

    [CorpseNameAttribute("corpse of a beastmaster elder")]
    public class BeastmasterTrainer : VystiaClassTrainer
    {
        public override PlayerClassTypeV2 TrainerClass { get { return PlayerClassTypeV2.Beastmaster; } }
        public override string TrainerGreeting { get { return "The beasts of Frosthold are loyal companions. Let me teach you their ways."; } }
        public override string ClassDescription { get { return "Masters of the wild who form bonds with fierce creatures of the north."; } }
        public override SkillName[] TrainableSkills { get { return new SkillName[] { SkillName.AnimalTaming, SkillName.AnimalLore, SkillName.Veterinary, SkillName.Archery, SkillName.Tracking, SkillName.Camping }; } }

        [Constructable]
        public BeastmasterTrainer() : base("Beastmaster Elder")
        {
            Name = NameList.RandomName(Female ? "female" : "male");
            Title = "the Beastmaster Elder";
            Hue = 1150;

            SetSkill(SkillName.Wrestling, 100.0);
            SetSkill(SkillName.Tactics, 100.0);
        }

        public BeastmasterTrainer(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region IceMage Trainer

    [CorpseNameAttribute("corpse of a ice mage archon")]
    public class IceMageTrainer : VystiaClassTrainer
    {
        public override PlayerClassTypeV2 TrainerClass { get { return PlayerClassTypeV2.IceMage; } }
        public override string TrainerGreeting { get { return "Ice is patient. Ice is relentless. Let me show you its secrets."; } }
        public override string ClassDescription { get { return "Wielders of frost magic who command the power of eternal winter."; } }
        public override SkillName[] TrainableSkills { get { return new SkillName[] { SkillName.Magery, SkillName.EvalInt, SkillName.Meditation, SkillName.MagicResist, SkillName.Inscribe, SkillName.Focus }; } }

        [Constructable]
        public IceMageTrainer() : base("Ice Mage Archon")
        {
            Name = NameList.RandomName(Female ? "female" : "male");
            Title = "the Ice Mage Archon";
            Hue = 1150;

            SetSkill(SkillName.Wrestling, 100.0);
            SetSkill(SkillName.Tactics, 100.0);
        }

        public IceMageTrainer(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region Sorcerer Trainer

    [CorpseNameAttribute("corpse of a sorcerer flamecaller")]
    public class SorcererTrainer : VystiaClassTrainer
    {
        public override PlayerClassTypeV2 TrainerClass { get { return PlayerClassTypeV2.Sorcerer; } }
        public override string TrainerGreeting { get { return "The elements rage within us all. I can teach you to command them."; } }
        public override string ClassDescription { get { return "Masters of elemental fury who command fire, water, earth, and air."; } }
        public override SkillName[] TrainableSkills { get { return new SkillName[] { SkillName.Magery, SkillName.EvalInt, SkillName.Meditation, SkillName.MagicResist, SkillName.Inscribe, SkillName.Focus }; } }

        [Constructable]
        public SorcererTrainer() : base("Sorcerer Flamecaller")
        {
            Name = NameList.RandomName(Female ? "female" : "male");
            Title = "the Sorcerer Flamecaller";
            Hue = 1358;

            SetSkill(SkillName.Wrestling, 100.0);
            SetSkill(SkillName.Tactics, 100.0);
        }

        public SorcererTrainer(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region Ranger Trainer

    [CorpseNameAttribute("corpse of a ranger sandstrider")]
    public class RangerTrainer : VystiaClassTrainer
    {
        public override PlayerClassTypeV2 TrainerClass { get { return PlayerClassTypeV2.Ranger; } }
        public override string TrainerGreeting { get { return "The desert reveals all secrets to those who know where to look."; } }
        public override string ClassDescription { get { return "Expert trackers and marksmen who thrive in the harsh desert."; } }
        public override SkillName[] TrainableSkills { get { return new SkillName[] { SkillName.Archery, SkillName.Tracking, SkillName.Hiding, SkillName.Stealth, SkillName.Tactics, SkillName.Anatomy }; } }

        [Constructable]
        public RangerTrainer() : base("Ranger Sandstrider")
        {
            Name = NameList.RandomName(Female ? "female" : "male");
            Title = "the Ranger Sandstrider";
            Hue = 1719;

            SetSkill(SkillName.Wrestling, 100.0);
            SetSkill(SkillName.Tactics, 100.0);
        }

        public RangerTrainer(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region Illusionist Trainer

    [CorpseNameAttribute("corpse of a illusionist mirageweaver")]
    public class IllusionistTrainer : VystiaClassTrainer
    {
        public override PlayerClassTypeV2 TrainerClass { get { return PlayerClassTypeV2.Illusionist; } }
        public override string TrainerGreeting { get { return "What you see is rarely what is. Let me show you the truth behind illusion."; } }
        public override string ClassDescription { get { return "Masters of deception who bend reality with their spells."; } }
        public override SkillName[] TrainableSkills { get { return new SkillName[] { SkillName.Magery, SkillName.EvalInt, SkillName.Meditation, SkillName.Hiding, SkillName.Stealth, SkillName.Inscribe }; } }

        [Constructable]
        public IllusionistTrainer() : base("Illusionist Mirageweaver")
        {
            Name = NameList.RandomName(Female ? "female" : "male");
            Title = "the Illusionist Mirageweaver";
            Hue = 1719;

            SetSkill(SkillName.Wrestling, 100.0);
            SetSkill(SkillName.Tactics, 100.0);
        }

        public IllusionistTrainer(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region Witch Trainer

    [CorpseNameAttribute("corpse of a witch hexmother")]
    public class WitchTrainer : VystiaClassTrainer
    {
        public override PlayerClassTypeV2 TrainerClass { get { return PlayerClassTypeV2.Witch; } }
        public override string TrainerGreeting { get { return "The swamp holds dark knowledge. Are you prepared to learn its secrets?"; } }
        public override string ClassDescription { get { return "Dark practitioners who specialize in curses, hexes, and poisons."; } }
        public override SkillName[] TrainableSkills { get { return new SkillName[] { SkillName.Magery, SkillName.EvalInt, SkillName.Poisoning, SkillName.Alchemy, SkillName.Meditation, SkillName.SpiritSpeak }; } }

        [Constructable]
        public WitchTrainer() : base("Witch Hexmother")
        {
            Name = NameList.RandomName(Female ? "female" : "male");
            Title = "the Witch Hexmother";
            Hue = 2073;

            SetSkill(SkillName.Wrestling, 100.0);
            SetSkill(SkillName.Tactics, 100.0);
        }

        public WitchTrainer(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region Warlock Trainer

    [CorpseNameAttribute("corpse of a warlock voidbinder")]
    public class WarlockTrainer : VystiaClassTrainer
    {
        public override PlayerClassTypeV2 TrainerClass { get { return PlayerClassTypeV2.Warlock; } }
        public override string TrainerGreeting { get { return "Power has a price. I can show you what lies beyond the veil."; } }
        public override string ClassDescription { get { return "Dark mages who forge pacts with demons for forbidden power."; } }
        public override SkillName[] TrainableSkills { get { return new SkillName[] { SkillName.Magery, SkillName.EvalInt, SkillName.SpiritSpeak, SkillName.Meditation, SkillName.MagicResist, SkillName.Necromancy }; } }

        [Constructable]
        public WarlockTrainer() : base("Warlock Voidbinder")
        {
            Name = NameList.RandomName(Female ? "female" : "male");
            Title = "the Warlock Voidbinder";
            Hue = 1109;

            SetSkill(SkillName.Wrestling, 100.0);
            SetSkill(SkillName.Tactics, 100.0);
        }

        public WarlockTrainer(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region Necromancer Trainer

    [CorpseNameAttribute("corpse of a necromancer deathspeaker")]
    public class NecromancerTrainer : VystiaClassTrainer
    {
        public override PlayerClassTypeV2 TrainerClass { get { return PlayerClassTypeV2.Necromancer; } }
        public override string TrainerGreeting { get { return "Death is not an ending, but a beginning. I can teach you to speak with the departed."; } }
        public override string ClassDescription { get { return "Masters of death magic who command the undead."; } }
        public override SkillName[] TrainableSkills { get { return new SkillName[] { SkillName.Magery, SkillName.EvalInt, SkillName.SpiritSpeak, SkillName.Meditation, SkillName.Necromancy, SkillName.MagicResist }; } }

        [Constructable]
        public NecromancerTrainer() : base("Necromancer Deathspeaker")
        {
            Name = NameList.RandomName(Female ? "female" : "male");
            Title = "the Necromancer Deathspeaker";
            Hue = 1109;

            SetSkill(SkillName.Wrestling, 100.0);
            SetSkill(SkillName.Tactics, 100.0);
        }

        public NecromancerTrainer(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region Druid Trainer

    [CorpseNameAttribute("corpse of a druid forestkeeper")]
    public class DruidTrainer : VystiaClassTrainer
    {
        public override PlayerClassTypeV2 TrainerClass { get { return PlayerClassTypeV2.Druid; } }
        public override string TrainerGreeting { get { return "The forest speaks to those who listen. Let me teach you its language."; } }
        public override string ClassDescription { get { return "Guardians of nature who shapeshift and command natural forces."; } }
        public override SkillName[] TrainableSkills { get { return new SkillName[] { SkillName.Magery, SkillName.AnimalLore, SkillName.Healing, SkillName.Meditation, SkillName.Veterinary, SkillName.Herding }; } }

        [Constructable]
        public DruidTrainer() : base("Druid Forestkeeper")
        {
            Name = NameList.RandomName(Female ? "female" : "male");
            Title = "the Druid Forestkeeper";
            Hue = 2010;

            SetSkill(SkillName.Wrestling, 100.0);
            SetSkill(SkillName.Tactics, 100.0);
        }

        public DruidTrainer(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region Alchemist Trainer

    [CorpseNameAttribute("corpse of a alchemist transmuter")]
    public class AlchemistTrainer : VystiaClassTrainer
    {
        public override PlayerClassTypeV2 TrainerClass { get { return PlayerClassTypeV2.Alchemist; } }
        public override string TrainerGreeting { get { return "All matter can be transformed. I will show you the art of transmutation."; } }
        public override string ClassDescription { get { return "Masters of potions and transmutation who unlock nature's secrets."; } }
        public override SkillName[] TrainableSkills { get { return new SkillName[] { SkillName.Alchemy, SkillName.Magery, SkillName.TasteID, SkillName.ItemID, SkillName.Cooking, SkillName.Healing }; } }

        [Constructable]
        public AlchemistTrainer() : base("Alchemist Transmuter")
        {
            Name = NameList.RandomName(Female ? "female" : "male");
            Title = "the Alchemist Transmuter";
            Hue = 2010;

            SetSkill(SkillName.Wrestling, 100.0);
            SetSkill(SkillName.Tactics, 100.0);
        }

        public AlchemistTrainer(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region Oracle Trainer

    [CorpseNameAttribute("corpse of a oracle seer")]
    public class OracleTrainer : VystiaClassTrainer
    {
        public override PlayerClassTypeV2 TrainerClass { get { return PlayerClassTypeV2.Oracle; } }
        public override string TrainerGreeting { get { return "Time flows like water. I can teach you to see its currents."; } }
        public override string ClassDescription { get { return "Seers who peer into the future and manipulate time itself."; } }
        public override SkillName[] TrainableSkills { get { return new SkillName[] { SkillName.Magery, SkillName.EvalInt, SkillName.Meditation, SkillName.SpiritSpeak, SkillName.Inscribe, SkillName.Focus }; } }

        [Constructable]
        public OracleTrainer() : base("Oracle Seer")
        {
            Name = NameList.RandomName(Female ? "female" : "male");
            Title = "the Oracle Seer";
            Hue = 1154;

            SetSkill(SkillName.Wrestling, 100.0);
            SetSkill(SkillName.Tactics, 100.0);
        }

        public OracleTrainer(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region Wizard Trainer

    [CorpseNameAttribute("corpse of a wizard archmage")]
    public class WizardTrainer : VystiaClassTrainer
    {
        public override PlayerClassTypeV2 TrainerClass { get { return PlayerClassTypeV2.Wizard; } }
        public override string TrainerGreeting { get { return "Knowledge is the greatest power. Let me share my wisdom with you."; } }
        public override string ClassDescription { get { return "Scholars of the arcane who master all schools of magic."; } }
        public override SkillName[] TrainableSkills { get { return new SkillName[] { SkillName.Magery, SkillName.EvalInt, SkillName.Meditation, SkillName.Inscribe, SkillName.MagicResist, SkillName.Focus }; } }

        [Constructable]
        public WizardTrainer() : base("Wizard Archmage")
        {
            Name = NameList.RandomName(Female ? "female" : "male");
            Title = "the Wizard Archmage";
            Hue = 1154;

            SetSkill(SkillName.Wrestling, 100.0);
            SetSkill(SkillName.Tactics, 100.0);
        }

        public WizardTrainer(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region Artificer Trainer

    [CorpseNameAttribute("corpse of a artificer gearmaster")]
    public class ArtificerTrainer : VystiaClassTrainer
    {
        public override PlayerClassTypeV2 TrainerClass { get { return PlayerClassTypeV2.Artificer; } }
        public override string TrainerGreeting { get { return "Steel and steam can accomplish anything. Let me show you the art of engineering."; } }
        public override string ClassDescription { get { return "Masters of clockwork who build mechanical companions and gadgets."; } }
        public override SkillName[] TrainableSkills { get { return new SkillName[] { SkillName.Tinkering, SkillName.Blacksmith, SkillName.Mining, SkillName.ArmsLore, SkillName.ItemID, SkillName.Carpentry }; } }

        [Constructable]
        public ArtificerTrainer() : base("Artificer Gearmaster")
        {
            Name = NameList.RandomName(Female ? "female" : "male");
            Title = "the Artificer Gearmaster";
            Hue = 2305;

            SetSkill(SkillName.Wrestling, 100.0);
            SetSkill(SkillName.Tactics, 100.0);
        }

        public ArtificerTrainer(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region Fighter Trainer

    [CorpseNameAttribute("corpse of a fighter champion")]
    public class FighterTrainer : VystiaClassTrainer
    {
        public override PlayerClassTypeV2 TrainerClass { get { return PlayerClassTypeV2.Fighter; } }
        public override string TrainerGreeting { get { return "A true warrior masters every weapon. Are you ready to train?"; } }
        public override string ClassDescription { get { return "Elite warriors who have mastered all forms of combat."; } }
        public override SkillName[] TrainableSkills { get { return new SkillName[] { SkillName.Swords, SkillName.Tactics, SkillName.Anatomy, SkillName.Parry, SkillName.Healing, SkillName.MagicResist }; } }

        [Constructable]
        public FighterTrainer() : base("Fighter Champion")
        {
            Name = NameList.RandomName(Female ? "female" : "male");
            Title = "the Fighter Champion";
            Hue = 2305;

            SetSkill(SkillName.Wrestling, 100.0);
            SetSkill(SkillName.Tactics, 100.0);
        }

        public FighterTrainer(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region Monk Trainer

    [CorpseNameAttribute("corpse of a monk grandmaster")]
    public class MonkTrainer : VystiaClassTrainer
    {
        public override PlayerClassTypeV2 TrainerClass { get { return PlayerClassTypeV2.Monk; } }
        public override string TrainerGreeting { get { return "The body is the ultimate weapon. Let me help you unlock its potential."; } }
        public override string ClassDescription { get { return "Masters of unarmed combat who channel inner strength."; } }
        public override SkillName[] TrainableSkills { get { return new SkillName[] { SkillName.Wrestling, SkillName.Tactics, SkillName.Anatomy, SkillName.Focus, SkillName.Meditation, SkillName.Healing }; } }

        [Constructable]
        public MonkTrainer() : base("Monk Grandmaster")
        {
            Name = NameList.RandomName(Female ? "female" : "male");
            Title = "the Monk Grandmaster";
            Hue = 2305;

            SetSkill(SkillName.Wrestling, 100.0);
            SetSkill(SkillName.Tactics, 100.0);
        }

        public MonkTrainer(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region Templar Trainer

    [CorpseNameAttribute("corpse of a templar crusader")]
    public class TemplarTrainer : VystiaClassTrainer
    {
        public override PlayerClassTypeV2 TrainerClass { get { return PlayerClassTypeV2.Templar; } }
        public override string TrainerGreeting { get { return "Faith and steel make us strong. I can teach you the ways of the Templar."; } }
        public override string ClassDescription { get { return "Holy warriors who serve the forges with unwavering devotion."; } }
        public override SkillName[] TrainableSkills { get { return new SkillName[] { SkillName.Swords, SkillName.Tactics, SkillName.Anatomy, SkillName.Chivalry, SkillName.MagicResist, SkillName.Parry }; } }

        [Constructable]
        public TemplarTrainer() : base("Templar Crusader")
        {
            Name = NameList.RandomName(Female ? "female" : "male");
            Title = "the Templar Crusader";
            Hue = 2305;

            SetSkill(SkillName.Wrestling, 100.0);
            SetSkill(SkillName.Tactics, 100.0);
        }

        public TemplarTrainer(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region Summoner Trainer

    [CorpseNameAttribute("corpse of a summoner planeswalker")]
    public class SummonerTrainer : VystiaClassTrainer
    {
        public override PlayerClassTypeV2 TrainerClass { get { return PlayerClassTypeV2.Summoner; } }
        public override string TrainerGreeting { get { return "Other realms teem with creatures waiting to serve. I will teach you to call them."; } }
        public override string ClassDescription { get { return "Masters of conjuration who summon creatures from other planes."; } }
        public override SkillName[] TrainableSkills { get { return new SkillName[] { SkillName.Magery, SkillName.EvalInt, SkillName.SpiritSpeak, SkillName.Meditation, SkillName.MagicResist, SkillName.Focus }; } }

        [Constructable]
        public SummonerTrainer() : base("Summoner Planeswalker")
        {
            Name = NameList.RandomName(Female ? "female" : "male");
            Title = "the Summoner Planeswalker";
            Hue = 1365;

            SetSkill(SkillName.Wrestling, 100.0);
            SetSkill(SkillName.Tactics, 100.0);
        }

        public SummonerTrainer(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region BountyHunter Trainer

    [CorpseNameAttribute("corpse of a bounty hunter tracker")]
    public class BountyHunterTrainer : VystiaClassTrainer
    {
        public override PlayerClassTypeV2 TrainerClass { get { return PlayerClassTypeV2.BountyHunter; } }
        public override string TrainerGreeting { get { return "Every target leaves a trail. I can teach you how to follow it."; } }
        public override string ClassDescription { get { return "Relentless trackers who hunt down targets for gold and justice."; } }
        public override SkillName[] TrainableSkills { get { return new SkillName[] { SkillName.Tracking, SkillName.Forensics, SkillName.DetectHidden, SkillName.Archery, SkillName.Hiding, SkillName.Stealth }; } }

        [Constructable]
        public BountyHunterTrainer() : base("Bounty Hunter Tracker")
        {
            Name = NameList.RandomName(Female ? "female" : "male");
            Title = "the Bounty Hunter Tracker";
            Hue = 1719;

            SetSkill(SkillName.Wrestling, 100.0);
            SetSkill(SkillName.Tactics, 100.0);
        }

        public BountyHunterTrainer(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region Knight Trainer

    [CorpseNameAttribute("corpse of a knight commander")]
    public class KnightTrainer : VystiaClassTrainer
    {
        public override PlayerClassTypeV2 TrainerClass { get { return PlayerClassTypeV2.Knight; } }
        public override string TrainerGreeting { get { return "Honor above all. Let me teach you the ways of true knighthood."; } }
        public override string ClassDescription { get { return "Noble warriors who uphold honor and protect the realm."; } }
        public override SkillName[] TrainableSkills { get { return new SkillName[] { SkillName.Swords, SkillName.Tactics, SkillName.Anatomy, SkillName.Chivalry, SkillName.Parry, SkillName.Healing }; } }

        [Constructable]
        public KnightTrainer() : base("Knight Commander")
        {
            Name = NameList.RandomName(Female ? "female" : "male");
            Title = "the Knight Commander";
            Hue = 1153;

            SetSkill(SkillName.Wrestling, 100.0);
            SetSkill(SkillName.Tactics, 100.0);
        }

        public KnightTrainer(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region Shaman Trainer

    [CorpseNameAttribute("corpse of a shaman spiritwalker")]
    public class ShamanTrainer : VystiaClassTrainer
    {
        public override PlayerClassTypeV2 TrainerClass { get { return PlayerClassTypeV2.Shaman; } }
        public override string TrainerGreeting { get { return "The spirits are always watching. I can teach you to hear their voices."; } }
        public override string ClassDescription { get { return "Spirit guides who commune with ancestors and elemental totems."; } }
        public override SkillName[] TrainableSkills { get { return new SkillName[] { SkillName.Magery, SkillName.SpiritSpeak, SkillName.Meditation, SkillName.Healing, SkillName.AnimalLore, SkillName.Herding }; } }

        [Constructable]
        public ShamanTrainer() : base("Shaman Spiritwalker")
        {
            Name = NameList.RandomName(Female ? "female" : "male");
            Title = "the Shaman Spiritwalker";
            Hue = 1281;

            SetSkill(SkillName.Wrestling, 100.0);
            SetSkill(SkillName.Tactics, 100.0);
        }

        public ShamanTrainer(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region Cleric Trainer

    [CorpseNameAttribute("corpse of a cleric high priest")]
    public class ClericTrainer : VystiaClassTrainer
    {
        public override PlayerClassTypeV2 TrainerClass { get { return PlayerClassTypeV2.Cleric; } }
        public override string TrainerGreeting { get { return "Divine light shines upon the faithful. Let me guide you in its ways."; } }
        public override string ClassDescription { get { return "Holy healers who channel divine power to aid allies."; } }
        public override SkillName[] TrainableSkills { get { return new SkillName[] { SkillName.Magery, SkillName.Healing, SkillName.Anatomy, SkillName.Meditation, SkillName.MagicResist, SkillName.Focus }; } }

        [Constructable]
        public ClericTrainer() : base("Cleric High Priest")
        {
            Name = NameList.RandomName(Female ? "female" : "male");
            Title = "the Cleric High Priest";
            Hue = 1153;

            SetSkill(SkillName.Wrestling, 100.0);
            SetSkill(SkillName.Tactics, 100.0);
        }

        public ClericTrainer(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region Paladin Trainer

    [CorpseNameAttribute("corpse of a paladin justicar")]
    public class PaladinTrainer : VystiaClassTrainer
    {
        public override PlayerClassTypeV2 TrainerClass { get { return PlayerClassTypeV2.Paladin; } }
        public override string TrainerGreeting { get { return "Righteousness guides our swords. Join me in the path of the Paladin."; } }
        public override string ClassDescription { get { return "Holy warriors who combine martial prowess with divine healing."; } }
        public override SkillName[] TrainableSkills { get { return new SkillName[] { SkillName.Swords, SkillName.Tactics, SkillName.Chivalry, SkillName.Healing, SkillName.Anatomy, SkillName.MagicResist }; } }

        [Constructable]
        public PaladinTrainer() : base("Paladin Justicar")
        {
            Name = NameList.RandomName(Female ? "female" : "male");
            Title = "the Paladin Justicar";
            Hue = 1153;

            SetSkill(SkillName.Wrestling, 100.0);
            SetSkill(SkillName.Tactics, 100.0);
        }

        public PaladinTrainer(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region Rogue Trainer

    [CorpseNameAttribute("corpse of a rogue shadowblade")]
    public class RogueTrainer : VystiaClassTrainer
    {
        public override PlayerClassTypeV2 TrainerClass { get { return PlayerClassTypeV2.Rogue; } }
        public override string TrainerGreeting { get { return "The shadows are honest. The steel is not. Let me show you how to vanish."; } }
        public override string ClassDescription { get { return "Stealth operatives who strike from the dark with poisons and precision."; } }
        public override SkillName[] TrainableSkills { get { return new SkillName[] { SkillName.Fencing, SkillName.Hiding, SkillName.Stealth, SkillName.Tactics, SkillName.Anatomy, SkillName.Poisoning }; } }

        [Constructable]
        public RogueTrainer() : base("Rogue Shadowblade")
        {
            Name = NameList.RandomName(Female ? "female" : "male");
            Title = "the Rogue Shadowblade";
            Hue = 1109;

            SetSkill(SkillName.Fencing, 100.0);
            SetSkill(SkillName.Tactics, 100.0);
        }

        public RogueTrainer(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region Bard Trainer

    [CorpseNameAttribute("corpse of a bard songmaster")]
    public class BardTrainer : VystiaClassTrainer
    {
        public override PlayerClassTypeV2 TrainerClass { get { return PlayerClassTypeV2.Bard; } }
        public override string TrainerGreeting { get { return "Music is the language of the soul. Let me teach you its power."; } }
        public override string ClassDescription { get { return "Masters of song who inspire allies and confound enemies."; } }
        public override SkillName[] TrainableSkills { get { return new SkillName[] { SkillName.Songweaving, SkillName.Musicianship, SkillName.Peacemaking, SkillName.Provocation, SkillName.Discordance, SkillName.Magery }; } }

        [Constructable]
        public BardTrainer() : base("Bard Songmaster")
        {
            Name = NameList.RandomName(Female ? "female" : "male");
            Title = "the Bard Songmaster";
            Hue = 2011;

            SetSkill(SkillName.Wrestling, 100.0);
            SetSkill(SkillName.Tactics, 100.0);
        }

        public BardTrainer(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region Enchanter Trainer

    [CorpseNameAttribute("corpse of a enchanter runesmith")]
    public class EnchanterTrainer : VystiaClassTrainer
    {
        public override PlayerClassTypeV2 TrainerClass { get { return PlayerClassTypeV2.Enchanter; } }
        public override string TrainerGreeting { get { return "Every object has potential. I can teach you to unlock it."; } }
        public override string ClassDescription { get { return "Arcane crafters who imbue items with magical properties."; } }
        public override SkillName[] TrainableSkills { get { return new SkillName[] { SkillName.Magery, SkillName.Inscribe, SkillName.EvalInt, SkillName.ItemID, SkillName.Meditation, SkillName.Focus }; } }

        [Constructable]
        public EnchanterTrainer() : base("Enchanter Runesmith")
        {
            Name = NameList.RandomName(Female ? "female" : "male");
            Title = "the Enchanter Runesmith";
            Hue = 1154;

            SetSkill(SkillName.Wrestling, 100.0);
            SetSkill(SkillName.Tactics, 100.0);
        }

        public EnchanterTrainer(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

}

