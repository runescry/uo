using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Items;

namespace Server.Mobiles
{
    /// <summary>
    /// Practice Target - Frozen red NPC with 100 million HP for spell testing
    /// Has 0% resistances so spell effects show clearly (like a player)
    /// Reports ALL effects: damage, heals, buffs, debuffs, DoTs, CC, transforms, etc.
    /// Spawnable via [PracticeTarget or [PT command, or [spawnvystia gump (Misc category)
    /// </summary>
    [CorpseName("a practice target corpse")]
    public class PracticeDummy : BaseCreature
    {
        private Mobile m_LastAttacker;
        private int m_OriginalBody;
        private int m_OriginalHue;

        [Constructable]
        public PracticeDummy() : base(AIType.AI_Melee, FightMode.None, 10, 1, 0.2, 0.4)
        {
            Name = "Practice Target";
            Title = "[Invulnerable Test NPC]";
            Body = 0x190; // Human male body
            Hue = 33; // Bright red hue
            m_OriginalBody = Body;
            m_OriginalHue = Hue;

            // Give it some equipment so it looks like an actual NPC
            AddItem(new ChainChest() { Hue = 33, Movable = false });
            AddItem(new ChainLegs() { Hue = 33, Movable = false });
            AddItem(new PlateArms() { Hue = 33, Movable = false });
            AddItem(new PlateGloves() { Hue = 33, Movable = false });
            AddItem(new PlateHelm() { Hue = 33, Movable = false });

            // Absurdly high HP for testing
            SetHits(100000000, 100000000); // 100 million HP

            // Make it unable to move or fight
            Frozen = true;
            CantWalk = true;
            Blessed = false; // Can be targeted

            // Stats (minimal, it's a dummy)
            SetStr(100);
            SetDex(100);
            SetInt(100);

            // No damage
            SetDamage(0, 0);

            // Zero resistances so spell effects show clearly (like a player)
            // HP is high enough (100M) that it won't die
            SetResistance(ResistanceType.Physical, 0, 0);
            SetResistance(ResistanceType.Fire, 0, 0);
            SetResistance(ResistanceType.Cold, 0, 0);
            SetResistance(ResistanceType.Poison, 0, 0);
            SetResistance(ResistanceType.Energy, 0, 0);

            // No skills needed
            Fame = 0;
            Karma = 0;

            VirtualArmor = 100;
        }

        public override bool DeleteCorpseOnDeath => true;
        public override bool AlwaysMurderer => true; // Make it appear red (hostile) instead of blue (innocent)
        public override bool IsInvulnerable => false; // Allow damage for testing
        public override bool CanBeParagon => false;

        private void BroadcastEffect(int hue, string message)
        {
            // Send to last attacker and all nearby players
            if (m_LastAttacker != null && m_LastAttacker.Player)
                m_LastAttacker.SendMessage(hue, $"[Dummy] {message}");

            // Also overhead message for visibility
            PublicOverheadMessage(Network.MessageType.Regular, hue, false, message);
        }

        #region Damage Tracking

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            m_LastAttacker = from;

            // Display damage dealt for testing purposes
            if (from != null && from.Player)
            {
                from.SendMessage(38, $"[Dummy] DAMAGE: {amount} (HP: {Hits:N0}/{HitsMax:N0})");
            }

            base.OnDamage(amount, from, willKill);
        }

        #endregion

        #region Heal Tracking

        public new int Heal(int amount)
        {
            BroadcastEffect(68, $"HEAL: +{amount} HP");
            return base.Heal(amount);
        }

        public new int Heal(int amount, Mobile from, bool message)
        {
            m_LastAttacker = from;
            BroadcastEffect(68, $"HEAL: +{amount} HP (from {from?.Name ?? "unknown"})");
            return base.Heal(amount, from, message);
        }

        #endregion

        #region Stat Mod Tracking

        public new void AddStatMod(StatMod mod)
        {
            string effect = mod.Offset >= 0 ? "BUFF" : "DEBUFF";
            int hue = mod.Offset >= 0 ? 68 : 38; // Green for buff, red for debuff
            string sign = mod.Offset >= 0 ? "+" : "";

            BroadcastEffect(hue, $"{effect}: {sign}{mod.Offset} {mod.Type} ({mod.Name})");

            base.AddStatMod(mod);
        }

        public new void RemoveStatMod(string name)
        {
            StatMod existing = GetStatMod(name);
            if (existing != null)
            {
                BroadcastEffect(53, $"EXPIRED: {existing.Type} mod ({name}) removed");
            }

            base.RemoveStatMod(name);
        }

        #endregion

        #region Resistance Mod Tracking

        public override void AddResistanceMod(ResistanceMod toAdd)
        {
            string effect = toAdd.Offset >= 0 ? "BUFF" : "DEBUFF";
            int hue = toAdd.Offset >= 0 ? 68 : 38;
            string sign = toAdd.Offset >= 0 ? "+" : "";

            BroadcastEffect(hue, $"{effect}: {sign}{toAdd.Offset}% {toAdd.Type} Resist");

            base.AddResistanceMod(toAdd);
        }

        public override void RemoveResistanceMod(ResistanceMod toRemove)
        {
            BroadcastEffect(53, $"EXPIRED: {toRemove.Type} Resist mod removed");
            base.RemoveResistanceMod(toRemove);
        }

        #endregion

        #region Poison Tracking

        public override ApplyPoisonResult ApplyPoison(Mobile from, Poison poison)
        {
            m_LastAttacker = from;

            if (poison != null)
            {
                string level = poison.Name ?? $"Level {poison.Level}";
                BroadcastEffect(63, $"POISON: {level} applied!");
            }

            return base.ApplyPoison(from, poison);
        }

        public override void OnPoisonImmunity(Mobile from, Poison poison)
        {
            BroadcastEffect(53, $"POISON IMMUNE: Resisted {poison?.Name ?? "poison"}");
            base.OnPoisonImmunity(from, poison);
        }

        #endregion

        #region State Tracking via OnThink

        private int m_LastDex = -1;
        private int m_LastStr = -1;
        private int m_LastInt = -1;
        private int m_LastBodyMod = 0;
        private int m_LastHueMod = -1;
        private bool m_LastParalyzed = false;
        private bool m_LastHidden = false;
        private bool m_LastPoisoned = false;

        public override void OnThink()
        {
            base.OnThink();

            // Initialize tracking values
            if (m_LastDex == -1)
            {
                m_LastDex = Dex;
                m_LastStr = Str;
                m_LastInt = Int;
                m_LastBodyMod = BodyMod;
                m_LastHueMod = HueMod;
                m_LastParalyzed = Paralyzed;
                m_LastHidden = Hidden;
                m_LastPoisoned = Poisoned;
                return;
            }

            // Check for stat changes
            if (Str != m_LastStr)
            {
                int change = Str - m_LastStr;
                string effect = change > 0 ? "BUFF" : "DEBUFF";
                int hue = change > 0 ? 68 : 38;
                BroadcastEffect(hue, $"{effect}: STR now {Str} ({(change > 0 ? "+" : "")}{change})");
                m_LastStr = Str;
            }

            if (Dex != m_LastDex)
            {
                int change = Dex - m_LastDex;
                string effect = change > 0 ? "SPEED BUFF" : "SLOW";
                int hue = change > 0 ? 68 : 38;
                BroadcastEffect(hue, $"{effect}: DEX now {Dex} ({(change > 0 ? "+" : "")}{change})");
                m_LastDex = Dex;
            }

            if (Int != m_LastInt)
            {
                int change = Int - m_LastInt;
                string effect = change > 0 ? "BUFF" : "DEBUFF";
                int hue = change > 0 ? 68 : 38;
                BroadcastEffect(hue, $"{effect}: INT now {Int} ({(change > 0 ? "+" : "")}{change})");
                m_LastInt = Int;
            }

            // Check for body/hue changes (transforms)
            if (BodyMod != m_LastBodyMod)
            {
                if (BodyMod != 0)
                    BroadcastEffect(1153, $"TRANSFORM: Body changed to {BodyMod}");
                else
                    BroadcastEffect(53, $"TRANSFORM: Returned to normal body");
                m_LastBodyMod = BodyMod;
            }

            if (HueMod != m_LastHueMod && HueMod != -1)
            {
                BroadcastEffect(1153, $"HUE CHANGE: Changed to hue {HueMod}");
                m_LastHueMod = HueMod;
            }

            // Check for CC effects
            if (Paralyzed != m_LastParalyzed)
            {
                if (Paralyzed)
                    BroadcastEffect(38, "CC: PARALYZED!");
                else
                    BroadcastEffect(68, "CC: Paralysis ended");
                m_LastParalyzed = Paralyzed;
            }

            if (Hidden != m_LastHidden)
            {
                if (Hidden)
                    BroadcastEffect(1153, "STEALTH: Now hidden");
                else
                    BroadcastEffect(53, "REVEALED: No longer hidden");
                m_LastHidden = Hidden;
            }

            // Check for poison cure
            if (m_LastPoisoned && !Poisoned)
            {
                BroadcastEffect(68, "CURED: Poison removed");
            }
            m_LastPoisoned = Poisoned;
        }

        #endregion

        #region Status Summary Command

        public void ShowStatus(Mobile to)
        {
            to.SendMessage(53, "=== Practice Dummy Status ===");
            to.SendMessage(53, $"HP: {Hits:N0}/{HitsMax:N0}");
            to.SendMessage(53, $"Stats: STR {Str} / DEX {Dex} / INT {Int}");
            to.SendMessage(53, $"Resists: Phys {PhysicalResistance}% Fire {FireResistance}% Cold {ColdResistance}% Pois {PoisonResistance}% Ener {EnergyResistance}%");

            if (Poisoned)
                to.SendMessage(63, $"POISONED: {Poison?.Name ?? "unknown"}");

            if (Paralyzed)
                to.SendMessage(38, "PARALYZED");

            if (BodyMod != 0)
                to.SendMessage(1153, $"TRANSFORMED: Body {BodyMod}");

            // List active stat mods
            if (StatMods != null && StatMods.Count > 0)
            {
                to.SendMessage(53, $"Active Stat Mods ({StatMods.Count}):");
                foreach (StatMod mod in StatMods)
                {
                    string sign = mod.Offset >= 0 ? "+" : "";
                    to.SendMessage(53, $"  - {mod.Name}: {sign}{mod.Offset} {mod.Type}");
                }
            }

            // List active resistance mods
            if (ResistanceMods != null && ResistanceMods.Count > 0)
            {
                to.SendMessage(53, $"Active Resist Mods ({ResistanceMods.Count}):");
                foreach (ResistanceMod mod in ResistanceMods)
                {
                    string sign = mod.Offset >= 0 ? "+" : "";
                    to.SendMessage(53, $"  - {sign}{mod.Offset}% {mod.Type}");
                }
            }

            to.SendMessage(53, "=============================");
        }

        public override bool HandlesOnSpeech(Mobile from) => true;

        public override void OnSpeech(SpeechEventArgs e)
        {
            if (e.Mobile != null && e.Mobile.Player && e.Mobile.InRange(this, 5))
            {
                string speech = e.Speech.ToLower();
                if (speech.Contains("status") || speech.Contains("stats") || speech.Contains("info"))
                {
                    ShowStatus(e.Mobile);
                }
                else if (speech.Contains("reset") || speech.Contains("clear"))
                {
                    ResetEffects(e.Mobile);
                }
            }

            base.OnSpeech(e);
        }

        public void ResetEffects(Mobile from)
        {
            // Clear all stat mods
            List<string> modNames = new List<string>();
            foreach (StatMod mod in StatMods)
                modNames.Add(mod.Name);
            foreach (string name in modNames)
                RemoveStatMod(name);

            // Clear all resistance mods
            List<ResistanceMod> resMods = new List<ResistanceMod>(ResistanceMods);
            foreach (ResistanceMod mod in resMods)
                RemoveResistanceMod(mod);

            // Cure poison
            if (Poisoned)
                CurePoison(from);

            // Remove paralysis
            Paralyzed = false;

            // Reset body/hue
            BodyMod = 0;
            HueMod = -1;

            // Restore HP
            Hits = HitsMax;

            from.SendMessage(68, "[Dummy] All effects cleared and HP restored!");
        }

        #endregion

        public override void OnDeath(Container c)
        {
            // Don't drop any loot
            base.OnDeath(c);
        }

        public PracticeDummy(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_OriginalBody = Body;
            m_OriginalHue = Hue;
        }
    }
}
