using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Commands;
using Server.Custom.VystiaClasses.Religion;
using Server.Custom.VystiaClasses.Factions;
using Server.Targeting;

namespace Server.Custom.VystiaClasses.Testing
{
    public enum CombatDummyMode
    {
        Passive,      // Takes damage, doesn't fight back
        Melee,        // Fights back with melee only
        Caster,       // Casts spells only
        Hybrid        // Both melee and spells
    }

    /// <summary>
    /// Regional magic schools for combat dummy spell casting
    /// </summary>
    public enum RegionalMagicSchool
    {
        Fire,         // FlameLegion / SuryasSandscript
        Ice,          // Frostguard / FrosthelmFaith
        Nature,       // Greenward / LunarasCovenant
        Dark,         // Voidborn / OceanasCovenant
        Divination,   // ArcaneConclave / CelestisArcanum
        Enchanting,   // Technoguild / CogsmithCreed
        Illusion,     // Sandwalkers
        Necromancy,   // Used by some Voidborn
        Shamanic,     // Nature variant
        Bardic        // Support variant
    }

    /// <summary>
    /// Combat test dummy that fights back and can be assigned faction/religion
    /// Used to test aggro, damage, and faction/religion interactions
    /// </summary>
    public class VystiaCombatDummy : BaseCreature
    {
        private VystiaFaction m_Faction;
        private VystiaReligion m_Religion;
        private CombatDummyMode m_CombatMode;
        private PlayerClassTypeV2 m_ClassType;
        private int m_TotalDamageTaken;
        private int m_HitCount;
        private DateTime m_LastDamageTime;
        private DateTime m_LastSpellTime;
        private Mobile m_DamageSource;

        [CommandProperty(AccessLevel.GameMaster)]
        public VystiaFaction Faction
        {
            get { return m_Faction; }
            set { m_Faction = value; UpdateName(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public VystiaReligion Religion
        {
            get { return m_Religion; }
            set { m_Religion = value; UpdateName(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public CombatDummyMode CombatMode
        {
            get { return m_CombatMode; }
            set { m_CombatMode = value; UpdateName(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public PlayerClassTypeV2 ClassType
        {
            get { return m_ClassType; }
            set { m_ClassType = value; UpdateName(); SetSkillsForClass(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TotalDamageTaken
        {
            get { return m_TotalDamageTaken; }
            set { m_TotalDamageTaken = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int HitCount
        {
            get { return m_HitCount; }
            set { m_HitCount = value; }
        }

        [Constructable]
        public VystiaCombatDummy() : this(VystiaFaction.None, VystiaReligion.None, CombatDummyMode.Hybrid, PlayerClassTypeV2.None)
        {
        }

        [Constructable]
        public VystiaCombatDummy(VystiaFaction faction) : this(faction, VystiaReligion.None, CombatDummyMode.Hybrid, PlayerClassTypeV2.None)
        {
        }

        [Constructable]
        public VystiaCombatDummy(VystiaFaction faction, VystiaReligion religion, CombatDummyMode mode)
            : this(faction, religion, mode, PlayerClassTypeV2.None)
        {
        }

        [Constructable]
        public VystiaCombatDummy(VystiaFaction faction, VystiaReligion religion, CombatDummyMode mode, PlayerClassTypeV2 classType)
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            m_Faction = faction;
            m_Religion = religion;
            m_CombatMode = mode;
            m_ClassType = classType;

            Body = 400; // Human male
            Hue = Utility.RandomSkinHue();

            SetStr(200, 250);
            SetDex(100, 125);
            SetInt(150, 200);

            SetHits(500, 750);
            SetMana(300, 400);
            SetStam(100, 125);

            SetDamage(15, 25);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 30, 40);

            Fame = 0;
            Karma = 0;

            VirtualArmor = 30;

            // Set maxed skills based on class
            SetSkillsForClass();

            // Give equipment based on mode
            EquipForMode(mode);

            UpdateName();
        }

        /// <summary>
        /// Sets skills to 100.0 based on the assigned class type
        /// </summary>
        private void SetSkillsForClass()
        {
            // Base combat skills for all dummies
            SetSkill(SkillName.MagicResist, 100.0);
            SetSkill(SkillName.Wrestling, 100.0);
            SetSkill(SkillName.Tactics, 100.0);

            switch (m_ClassType)
            {
                // Magic Classes - max magic skills
                case PlayerClassTypeV2.IceMage:
                case PlayerClassTypeV2.Sorcerer:
                case PlayerClassTypeV2.Warlock:
                case PlayerClassTypeV2.Wizard:
                case PlayerClassTypeV2.Oracle:
                    SetSkill(SkillName.Magery, 100.0);
                    SetSkill(SkillName.EvalInt, 100.0);
                    SetSkill(SkillName.Meditation, 100.0);
                    SetSkill(SkillName.Inscribe, 100.0);
                    break;

                case PlayerClassTypeV2.Necromancer:
                    SetSkill(SkillName.Necromancy, 100.0);
                    SetSkill(SkillName.SpiritSpeak, 100.0);
                    SetSkill(SkillName.Magery, 100.0);
                    SetSkill(SkillName.EvalInt, 100.0);
                    SetSkill(SkillName.Meditation, 100.0);
                    break;

                case PlayerClassTypeV2.Druid:
                case PlayerClassTypeV2.Shaman:
                    SetSkill(SkillName.Magery, 100.0);
                    SetSkill(SkillName.EvalInt, 100.0);
                    SetSkill(SkillName.Meditation, 100.0);
                    SetSkill(SkillName.AnimalLore, 100.0);
                    SetSkill(SkillName.AnimalTaming, 100.0);
                    SetSkill(SkillName.Veterinary, 100.0);
                    break;

                case PlayerClassTypeV2.Summoner:
                    SetSkill(SkillName.Magery, 100.0);
                    SetSkill(SkillName.EvalInt, 100.0);
                    SetSkill(SkillName.Meditation, 100.0);
                    SetSkill(SkillName.AnimalLore, 100.0);
                    SetSkill(SkillName.AnimalTaming, 100.0);
                    break;

                case PlayerClassTypeV2.Bard:
                    SetSkill(SkillName.Musicianship, 100.0);
                    SetSkill(SkillName.Provocation, 100.0);
                    SetSkill(SkillName.Discordance, 100.0);
                    SetSkill(SkillName.Peacemaking, 100.0);
                    SetSkill(SkillName.Magery, 100.0);
                    SetSkill(SkillName.EvalInt, 100.0);
                    break;

                case PlayerClassTypeV2.Witch:
                case PlayerClassTypeV2.Illusionist:
                case PlayerClassTypeV2.Enchanter:
                    SetSkill(SkillName.Magery, 100.0);
                    SetSkill(SkillName.EvalInt, 100.0);
                    SetSkill(SkillName.Meditation, 100.0);
                    SetSkill(SkillName.Alchemy, 100.0);
                    break;

                // Martial Classes - max melee skills
                case PlayerClassTypeV2.Barbarian:
                case PlayerClassTypeV2.Fighter:
                case PlayerClassTypeV2.Knight:
                    SetSkill(SkillName.Swords, 100.0);
                    SetSkill(SkillName.Anatomy, 100.0);
                    SetSkill(SkillName.Parry, 100.0);
                    SetSkill(SkillName.Healing, 100.0);
                    SetSkill(SkillName.Macing, 100.0);
                    SetSkill(SkillName.Fencing, 100.0);
                    break;

                case PlayerClassTypeV2.Rogue:
                case PlayerClassTypeV2.BountyHunter:
                    SetSkill(SkillName.Fencing, 100.0);
                    SetSkill(SkillName.Snooping, 100.0);
                    SetSkill(SkillName.Stealing, 100.0);
                    SetSkill(SkillName.Stealth, 100.0);
                    SetSkill(SkillName.Hiding, 100.0);
                    SetSkill(SkillName.Poisoning, 100.0);
                    SetSkill(SkillName.Anatomy, 100.0);
                    break;

                case PlayerClassTypeV2.Monk:
                    SetSkill(SkillName.Wrestling, 100.0);
                    SetSkill(SkillName.Focus, 100.0);
                    SetSkill(SkillName.Anatomy, 100.0);
                    SetSkill(SkillName.Healing, 100.0);
                    SetSkill(SkillName.Meditation, 100.0);
                    break;

                case PlayerClassTypeV2.Paladin:
                case PlayerClassTypeV2.Templar:
                    SetSkill(SkillName.Chivalry, 100.0);
                    SetSkill(SkillName.Swords, 100.0);
                    SetSkill(SkillName.Anatomy, 100.0);
                    SetSkill(SkillName.Parry, 100.0);
                    SetSkill(SkillName.Focus, 100.0);
                    SetSkill(SkillName.Healing, 100.0);
                    break;

                case PlayerClassTypeV2.Ranger:
                    SetSkill(SkillName.Archery, 100.0);
                    SetSkill(SkillName.Tracking, 100.0);
                    SetSkill(SkillName.Camping, 100.0);
                    SetSkill(SkillName.AnimalLore, 100.0);
                    SetSkill(SkillName.AnimalTaming, 100.0);
                    SetSkill(SkillName.Anatomy, 100.0);
                    SetSkill(SkillName.Healing, 100.0);
                    break;

                case PlayerClassTypeV2.Beastmaster:
                    SetSkill(SkillName.AnimalLore, 100.0);
                    SetSkill(SkillName.AnimalTaming, 100.0);
                    SetSkill(SkillName.Veterinary, 100.0);
                    SetSkill(SkillName.Swords, 100.0);
                    SetSkill(SkillName.Anatomy, 100.0);
                    break;

                case PlayerClassTypeV2.Artificer:
                    SetSkill(SkillName.Tinkering, 100.0);
                    SetSkill(SkillName.Magery, 100.0);
                    SetSkill(SkillName.EvalInt, 100.0);
                    SetSkill(SkillName.Mining, 100.0);
                    SetSkill(SkillName.Blacksmith, 100.0);
                    break;

                case PlayerClassTypeV2.Alchemist:
                    SetSkill(SkillName.Alchemy, 100.0);
                    SetSkill(SkillName.TasteID, 100.0);
                    SetSkill(SkillName.Magery, 100.0);
                    SetSkill(SkillName.Poisoning, 100.0);
                    SetSkill(SkillName.ItemID, 100.0);
                    break;

                case PlayerClassTypeV2.Cleric:
                    SetSkill(SkillName.Healing, 100.0);
                    SetSkill(SkillName.Anatomy, 100.0);
                    SetSkill(SkillName.SpiritSpeak, 100.0);
                    SetSkill(SkillName.Magery, 100.0);
                    SetSkill(SkillName.Meditation, 100.0);
                    break;

                // Default - set all combat skills to max
                case PlayerClassTypeV2.None:
                default:
                    SetSkill(SkillName.Swords, 100.0);
                    SetSkill(SkillName.Anatomy, 100.0);
                    SetSkill(SkillName.Parry, 100.0);
                    SetSkill(SkillName.Healing, 100.0);
                    SetSkill(SkillName.Magery, 100.0);
                    SetSkill(SkillName.EvalInt, 100.0);
                    SetSkill(SkillName.Meditation, 100.0);
                    SetSkill(SkillName.Archery, 100.0);
                    SetSkill(SkillName.Fencing, 100.0);
                    SetSkill(SkillName.Macing, 100.0);
                    break;
            }
        }

        private void EquipForMode(CombatDummyMode mode)
        {
            // Basic armor
            AddItem(new PlateChest());
            AddItem(new PlateLegs());
            AddItem(new PlateArms());
            AddItem(new PlateGloves());
            AddItem(new PlateHelm());

            switch (mode)
            {
                case CombatDummyMode.Melee:
                case CombatDummyMode.Hybrid:
                    AddItem(new Longsword());
                    AddItem(new MetalKiteShield());
                    break;
                case CombatDummyMode.Caster:
                    AddItem(new Spellbook(ulong.MaxValue)); // All spells
                    AddItem(new Robe(Utility.RandomDyedHue()));
                    break;
            }
        }

        private void UpdateName()
        {
            string factionName = m_Faction != VystiaFaction.None
                ? FactionData.GetInfo(m_Faction)?.Name ?? m_Faction.ToString()
                : "Unaffiliated";

            string religionName = m_Religion != VystiaReligion.None
                ? ReligionData.GetInfo(m_Religion)?.Name ?? m_Religion.ToString()
                : "Secular";

            string className = m_ClassType != PlayerClassTypeV2.None
                ? m_ClassType.ToString()
                : "Generic";

            Name = $"Combat Dummy [{m_CombatMode}] - {className}";
            Title = $"({factionName} / {religionName})";

            // Set hue based on faction
            if (m_Faction != VystiaFaction.None)
            {
                var info = FactionData.GetInfo(m_Faction);
                if (info != null)
                    Hue = info.Hue;
            }
            else if (m_Religion != VystiaReligion.None)
            {
                var info = ReligionData.GetInfo(m_Religion);
                if (info != null)
                    Hue = info.Hue;
            }
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            base.OnDamage(amount, from, willKill);

            m_TotalDamageTaken += amount;
            m_HitCount++;
            m_LastDamageTime = DateTime.UtcNow;
            m_DamageSource = from;

            // Display detailed damage overhead
            string damageInfo = $"-{amount} (Total: {m_TotalDamageTaken}, Hits: {m_HitCount})";
            PublicOverheadMessage(Server.Network.MessageType.Regular, 0x22, false, damageInfo);

            // Send detailed system message to attacker
            if (from != null)
            {
                string sourceName = from.Name ?? "Unknown";
                double healthPct = (double)Hits / HitsMax * 100;

                from.SendMessage(0x35, $"[COMBAT] You hit {Name} for {amount} damage. Target HP: {Hits}/{HitsMax} ({healthPct:F1}%)");

                // Check faction/religion relationship for bonus info
                if (IsOppositeFaction(from))
                {
                    VystiaFaction opposite = GetOppositeFaction(m_Faction);
                    from.SendMessage(0x22, $"[FACTION] Enemy faction detected! {m_Faction} vs {opposite}");
                }
                if (IsOppositeReligion(from))
                {
                    VystiaReligion opposite = GetOppositeReligion(m_Religion);
                    from.SendMessage(0x22, $"[RELIGION] Enemy religion detected! {m_Religion} vs {opposite}");
                }

                // Critical health warning
                if (healthPct < 25 && healthPct > 0)
                    from.SendMessage(0x21, $"[COMBAT] {Name} is critically wounded!");
                else if (healthPct < 50)
                    from.SendMessage(0x44, $"[COMBAT] {Name} is badly hurt.");
            }

            // If passive, don't fight back
            if (m_CombatMode == CombatDummyMode.Passive)
            {
                Combatant = null;
                return;
            }

            // Engage the attacker
            if (from != null && Combatant == null)
            {
                Combatant = from;
            }
        }

        /// <summary>
        /// Handle being poisoned - show detailed messages
        /// </summary>
        public override void OnPoisoned(Mobile from, Poison poison, Poison oldPoison)
        {
            base.OnPoisoned(from, poison, oldPoison);

            string poisonLevel = poison != null ? poison.Name : "Unknown";
            string sourceName = from != null ? from.Name : "Unknown source";

            PublicOverheadMessage(Server.Network.MessageType.Regular, 0x44, false, $"*POISONED* ({poisonLevel})");

            if (from != null)
            {
                from.SendMessage(0x44, $"[EFFECT] You poisoned {Name} with {poisonLevel} poison!");
            }

            // Broadcast to nearby players
            foreach (Mobile m in GetMobilesInRange(12))
            {
                if (m is PlayerMobile pm && pm != from)
                    pm.SendMessage(0x44, $"[EFFECT] {Name} has been poisoned by {sourceName}! ({poisonLevel})");
            }
        }

        /// <summary>
        /// Handle poison curing - show messages
        /// </summary>
        public override void OnCured(Mobile from, Poison oldPoison)
        {
            base.OnCured(from, oldPoison);

            PublicOverheadMessage(Server.Network.MessageType.Regular, 0x59, false, "*Poison Cured*");

            if (from != null)
            {
                from.SendMessage(0x59, $"[EFFECT] {Name}'s poison has been cured.");
            }
        }

        /// <summary>
        /// Handle being healed - show messages
        /// </summary>
        public override void OnHeal(ref int amount, Mobile from)
        {
            base.OnHeal(ref amount, from);

            if (amount > 0)
            {
                PublicOverheadMessage(Server.Network.MessageType.Regular, 0x59, false, $"+{amount} HP");

                if (from != null)
                {
                    double healthPct = (double)Hits / HitsMax * 100;
                    from.SendMessage(0x59, $"[HEAL] {Name} healed for {amount}. HP: {Hits}/{HitsMax} ({healthPct:F1}%)");
                }
            }
        }

        /// <summary>
        /// Handle paralysis - show messages (using new since base is not virtual)
        /// </summary>
        public new void Paralyze(TimeSpan duration)
        {
            base.Paralyze(duration);

            double seconds = duration.TotalSeconds;
            PublicOverheadMessage(Server.Network.MessageType.Regular, 0x21, false, $"*PARALYZED* ({seconds:F1}s)");

            // Notify nearby players
            foreach (Mobile m in GetMobilesInRange(12))
            {
                if (m is PlayerMobile pm)
                    pm.SendMessage(0x21, $"[CC] {Name} has been paralyzed for {seconds:F1} seconds!");
            }
        }

        /// <summary>
        /// Handle freeze (rooted) - show messages (using new since base is not virtual)
        /// </summary>
        public new void Freeze(TimeSpan duration)
        {
            base.Freeze(duration);

            double seconds = duration.TotalSeconds;
            PublicOverheadMessage(Server.Network.MessageType.Regular, 0x5A, false, $"*FROZEN* ({seconds:F1}s)");

            // Notify nearby players
            foreach (Mobile m in GetMobilesInRange(12))
            {
                if (m is PlayerMobile pm)
                    pm.SendMessage(0x5A, $"[CC] {Name} has been frozen for {seconds:F1} seconds!");
            }
        }

        /// <summary>
        /// When dummy deals damage to a target, show messages
        /// </summary>
        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (defender != null && defender is PlayerMobile pm)
            {
                // Get approximate damage from last hit
                pm.SendMessage(0x22, $"[COMBAT] {Name} ({Title}) hit you with a melee attack!");

                // Show faction/religion conflict info
                if (IsOppositeFaction(pm))
                    pm.SendMessage(0x22, $"[FACTION] You are being attacked by enemy faction {m_Faction}!");
                if (IsOppositeReligion(pm))
                    pm.SendMessage(0x22, $"[RELIGION] You are being attacked by enemy religion {m_Religion}!");
            }
        }

        /// <summary>
        /// When dummy takes melee attack, show additional info
        /// </summary>
        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            if (attacker != null)
            {
                // Check for special weapon effects
                if (attacker.Weapon is BaseWeapon weapon)
                {
                    string weaponName = weapon.Name ?? weapon.GetType().Name;
                    attacker.SendMessage(0x35, $"[COMBAT] You struck {Name} with {weaponName}");
                }
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            if (m_CombatMode == CombatDummyMode.Passive)
            {
                Combatant = null;
                return;
            }

            // Actively seek enemies if we don't have a target
            if (Combatant == null || Combatant.Deleted || !Combatant.Alive)
            {
                Mobile enemy = FindNearbyEnemy();
                if (enemy != null)
                {
                    Combatant = enemy;

                    // Announce engagement
                    string reason = "";
                    if (IsOppositeFaction(enemy))
                        reason = $"Faction enemy ({GetOppositeFaction(m_Faction)})";
                    else if (IsOppositeReligion(enemy))
                        reason = $"Religious enemy ({GetOppositeReligion(m_Religion)})";
                    else
                        reason = "Hostile target";

                    Say($"*Engaging {enemy.Name}* ({reason})");

                    // Alert the target
                    if (enemy is PlayerMobile pm)
                    {
                        pm.SendMessage(0x22, $"[AGGRO] {Name} ({Title}) is attacking you! Reason: {reason}");
                    }
                }
            }

            if (Combatant != null && !Combatant.Deleted && Combatant.Alive && InRange(Combatant, 12))
            {
                // Casting logic for caster/hybrid modes
                if ((m_CombatMode == CombatDummyMode.Caster || m_CombatMode == CombatDummyMode.Hybrid)
                    && DateTime.UtcNow > m_LastSpellTime.AddSeconds(3))
                {
                    if (Mana > 20 && Utility.RandomDouble() < 0.4)
                    {
                        CastOffensiveSpell();
                        m_LastSpellTime = DateTime.UtcNow;
                    }
                    else if (Hits < HitsMax / 2 && Mana > 10 && Utility.RandomDouble() < 0.3)
                    {
                        CastHealingSpell();
                        m_LastSpellTime = DateTime.UtcNow;
                    }
                }
            }

            // Reset damage stats if no damage for 30 seconds
            if (m_TotalDamageTaken > 0 && DateTime.UtcNow > m_LastDamageTime.AddSeconds(30))
            {
                Say($"Combat ended. Total damage: {m_TotalDamageTaken}, Hits: {m_HitCount}, Avg: {(m_HitCount > 0 ? m_TotalDamageTaken / m_HitCount : 0)}");
                m_TotalDamageTaken = 0;
                m_HitCount = 0;
            }
        }

        /// <summary>
        /// Find nearest enemy faction/religion target in range
        /// </summary>
        private Mobile FindNearbyEnemy()
        {
            Mobile closest = null;
            double closestDist = double.MaxValue;

            foreach (Mobile m in GetMobilesInRange(10))
            {
                if (m == null || m == this || m.Deleted || !m.Alive)
                    continue;

                // Check if this is an enemy
                bool isEnemy = false;

                if (m is VystiaCombatDummy otherDummy)
                {
                    // Attack opposite faction/religion dummies
                    if (m_Faction != VystiaFaction.None && otherDummy.Faction == GetOppositeFaction(m_Faction))
                        isEnemy = true;
                    else if (m_Religion != VystiaReligion.None && otherDummy.Religion == GetOppositeReligion(m_Religion))
                        isEnemy = true;
                }
                else if (m is PlayerMobile)
                {
                    // Attack players from opposite faction/religion
                    if (IsOppositeFaction(m) || IsOppositeReligion(m))
                        isEnemy = true;
                }

                if (isEnemy)
                {
                    double dist = GetDistanceToSqrt(m);
                    if (dist < closestDist)
                    {
                        closest = m;
                        closestDist = dist;
                    }
                }
            }

            return closest;
        }

        private void CastOffensiveSpell()
        {
            if (Combatant == null)
                return;

            Mobile target = Combatant as Mobile;
            if (target == null)
                return;

            // Get magic school based on faction/religion
            var school = GetMagicSchool();
            double dist = GetDistanceToSqrt(target);

            // Cast regional spell based on school
            CastRegionalSpell(target, school, dist);
        }

        /// <summary>
        /// Get magic school based on faction/religion affiliation
        /// </summary>
        private RegionalMagicSchool GetMagicSchool()
        {
            // Faction-based magic schools
            switch (m_Faction)
            {
                case VystiaFaction.Frostguard: return RegionalMagicSchool.Ice;
                case VystiaFaction.FlameLegion: return RegionalMagicSchool.Fire;
                case VystiaFaction.Greenward: return RegionalMagicSchool.Nature;
                case VystiaFaction.ArcaneConclave: return RegionalMagicSchool.Divination;
                case VystiaFaction.Technoguild: return RegionalMagicSchool.Enchanting;
                case VystiaFaction.Sandwalkers: return RegionalMagicSchool.Illusion;
                case VystiaFaction.Voidborn: return RegionalMagicSchool.Dark;
            }

            // Religion-based magic schools (fallback)
            switch (m_Religion)
            {
                case VystiaReligion.FrosthelmFaith: return RegionalMagicSchool.Ice;
                case VystiaReligion.SuryasSandscript: return RegionalMagicSchool.Fire;
                case VystiaReligion.LunarasCovenant: return RegionalMagicSchool.Nature;
                case VystiaReligion.CelestisArcanum: return RegionalMagicSchool.Divination;
                case VystiaReligion.OceanasCovenant: return RegionalMagicSchool.Dark;
                case VystiaReligion.CogsmithCreed: return RegionalMagicSchool.Enchanting;
            }

            // Default to fire if no affiliation
            return RegionalMagicSchool.Fire;
        }

        /// <summary>
        /// Cast a regional spell with appropriate visual effects and damage type
        /// </summary>
        private void CastRegionalSpell(Mobile target, RegionalMagicSchool school, double dist)
        {
            if (target == null || !target.Alive)
                return;

            int damage = Utility.RandomMinMax(20, 35);
            int manaCost = 15;

            if (Mana < manaCost)
                return;

            Mana -= manaCost;

            // Get damage type and effects based on school
            int effectHue;
            int effectId;
            int soundId;
            string spellName;
            int physDmg = 0, fireDmg = 0, coldDmg = 0, poisDmg = 0, energyDmg = 0;

            switch (school)
            {
                case RegionalMagicSchool.Ice:
                    effectHue = 0x480; // Ice blue
                    effectId = 0x36BD; // Ice effect
                    soundId = 0x1E3; // Ice sound
                    spellName = "Ice Bolt";
                    coldDmg = 100;
                    break;

                case RegionalMagicSchool.Fire:
                    effectHue = 0x489; // Fire orange
                    effectId = 0x36D4; // Fire effect
                    soundId = 0x208; // Fire sound
                    spellName = "Flame Bolt";
                    fireDmg = 100;
                    break;

                case RegionalMagicSchool.Nature:
                    effectHue = 0x59B; // Nature green
                    effectId = 0x373A; // Nature effect
                    soundId = 0x1E6; // Nature sound
                    spellName = "Thorn Dart";
                    physDmg = 50;
                    poisDmg = 50;
                    break;

                case RegionalMagicSchool.Dark:
                    effectHue = 0x455; // Shadow purple
                    effectId = 0x374A; // Shadow effect
                    soundId = 0x1FB; // Shadow sound
                    spellName = "Shadow Bolt";
                    energyDmg = 50;
                    coldDmg = 50;
                    break;

                case RegionalMagicSchool.Divination:
                    effectHue = 0x47E; // Crystal white
                    effectId = 0x375A; // Energy effect
                    soundId = 0x211; // Energy sound
                    spellName = "Crystal Dart";
                    energyDmg = 100;
                    break;

                case RegionalMagicSchool.Enchanting:
                    effectHue = 0x47F; // Arcane silver
                    effectId = 0x3728; // Arcane effect
                    soundId = 0x1F5; // Arcane sound
                    spellName = "Arcane Bolt";
                    energyDmg = 60;
                    physDmg = 40;
                    break;

                case RegionalMagicSchool.Illusion:
                    effectHue = 0x48D; // Illusion pink
                    effectId = 0x3779; // Illusion effect
                    soundId = 0x1ED; // Illusion sound
                    spellName = "Mind Spike";
                    energyDmg = 70;
                    coldDmg = 30;
                    break;

                case RegionalMagicSchool.Necromancy:
                    effectHue = 0x763; // Death grey
                    effectId = 0x37C4; // Death effect
                    soundId = 0x1F1; // Death sound
                    spellName = "Death Bolt";
                    coldDmg = 50;
                    poisDmg = 50;
                    break;

                default:
                    effectHue = 0;
                    effectId = 0x36D4;
                    soundId = 0x208;
                    spellName = "Magic Bolt";
                    energyDmg = 100;
                    break;
            }

            // Visual effects
            Effects.SendMovingEffect(this, target, effectId, 10, 0, false, false, effectHue, 0);
            this.PlaySound(soundId);

            // Delay damage to match visual
            Timer.DelayCall(TimeSpan.FromMilliseconds(500), () =>
            {
                if (target == null || target.Deleted || !target.Alive)
                    return;

                target.PlaySound(0x1F1);

                // Deal damage with appropriate type distribution
                AOS.Damage(target, this, damage, physDmg, fireDmg, coldDmg, poisDmg, energyDmg);

                // Send combat message
                Say($"*casts {spellName}*");

                if (target is PlayerMobile pm)
                {
                    pm.SendMessage(0x22, $"[SPELL] {Name} cast {spellName} for {damage} damage!");
                }
            });
        }

        private void CastHealingSpell()
        {
            int manaCost = 10;
            if (Mana < manaCost)
                return;

            Mana -= manaCost;

            // Get heal amount and effect based on school
            var school = GetMagicSchool();
            int healAmount = Utility.RandomMinMax(30, 50);
            string healName;
            int effectHue;

            switch (school)
            {
                case RegionalMagicSchool.Ice:
                    healName = "Glacial Mend";
                    effectHue = 0x480;
                    break;
                case RegionalMagicSchool.Fire:
                    healName = "Flame Renewal";
                    effectHue = 0x489;
                    break;
                case RegionalMagicSchool.Nature:
                    healName = "Nature's Touch";
                    effectHue = 0x59B;
                    break;
                case RegionalMagicSchool.Dark:
                    healName = "Life Tap";
                    effectHue = 0x455;
                    break;
                default:
                    healName = "Arcane Healing";
                    effectHue = 0x47E;
                    break;
            }

            // Visual effect
            FixedParticles(0x376A, 9, 32, 5030, effectHue, 0, EffectLayer.Waist);
            PlaySound(0x202);

            // Apply heal
            Heal(healAmount);

            Say($"*casts {healName}*");

            // Notify nearby players
            foreach (Mobile m in GetMobilesInRange(12))
            {
                if (m is PlayerMobile pm)
                    pm.SendMessage(0x59, $"[HEAL] {Name} cast {healName} for {healAmount} healing!");
            }
        }

        public override bool OnBeforeDeath()
        {
            if (m_DamageSource != null)
            {
                m_DamageSource.SendMessage(0x35,
                    $"Combat Dummy defeated! Total damage dealt: {m_TotalDamageTaken}, Hits: {m_HitCount}, Avg per hit: {(m_HitCount > 0 ? m_TotalDamageTaken / m_HitCount : 0)}");
            }

            return base.OnBeforeDeath();
        }

        // Faction/Religion checks for aggro testing
        public bool IsSameFaction(Mobile m)
        {
            if (m_Faction == VystiaFaction.None)
                return false;

            if (m is PlayerMobile pm)
            {
                // Check player's faction reputation
                int rep = VystiaReputation.GetFactionReputation(pm, m_Faction);
                return rep >= 3000; // Friendly or higher
            }

            if (m is VystiaCombatDummy dummy)
            {
                return dummy.Faction == m_Faction;
            }

            return false;
        }

        public bool IsSameReligion(Mobile m)
        {
            if (m_Religion == VystiaReligion.None)
                return false;

            if (m is PlayerMobile pm)
            {
                var piety = VystiaPiety.GetPiety(pm);
                return piety != null && piety.Religion == m_Religion;
            }

            if (m is VystiaCombatDummy dummy)
            {
                return dummy.Religion == m_Religion;
            }

            return false;
        }

        /// <summary>
        /// Get the opposing faction for this dummy's faction
        /// </summary>
        public static VystiaFaction GetOppositeFaction(VystiaFaction faction)
        {
            switch (faction)
            {
                case VystiaFaction.Frostguard: return VystiaFaction.FlameLegion;
                case VystiaFaction.FlameLegion: return VystiaFaction.Frostguard;
                case VystiaFaction.Greenward: return VystiaFaction.Voidborn;
                case VystiaFaction.Voidborn: return VystiaFaction.Greenward;
                case VystiaFaction.ArcaneConclave: return VystiaFaction.Technoguild;
                case VystiaFaction.Technoguild: return VystiaFaction.ArcaneConclave;
                case VystiaFaction.Sandwalkers: return VystiaFaction.Frostguard; // Desert vs Ice
                default: return VystiaFaction.None;
            }
        }

        /// <summary>
        /// Get the opposing religion for this dummy's religion
        /// </summary>
        public static VystiaReligion GetOppositeReligion(VystiaReligion religion)
        {
            switch (religion)
            {
                case VystiaReligion.FrosthelmFaith: return VystiaReligion.SuryasSandscript;
                case VystiaReligion.SuryasSandscript: return VystiaReligion.FrosthelmFaith;
                case VystiaReligion.LunarasCovenant: return VystiaReligion.OceanasCovenant;
                case VystiaReligion.OceanasCovenant: return VystiaReligion.LunarasCovenant;
                case VystiaReligion.CelestisArcanum: return VystiaReligion.CogsmithCreed;
                case VystiaReligion.CogsmithCreed: return VystiaReligion.CelestisArcanum;
                default: return VystiaReligion.None;
            }
        }

        /// <summary>
        /// Check if target is from the opposite faction
        /// </summary>
        public bool IsOppositeFaction(Mobile m)
        {
            if (m_Faction == VystiaFaction.None)
                return false;

            VystiaFaction oppositeFaction = GetOppositeFaction(m_Faction);
            if (oppositeFaction == VystiaFaction.None)
                return false;

            if (m is PlayerMobile pm)
            {
                // Check if player is hostile to us OR friendly to our enemy
                int ourRep = VystiaReputation.GetFactionReputation(pm, m_Faction);
                int enemyRep = VystiaReputation.GetFactionReputation(pm, oppositeFaction);
                return ourRep < -1000 || enemyRep >= 3000; // Hostile to us OR friendly to our enemy
            }

            if (m is VystiaCombatDummy dummy)
            {
                return dummy.Faction == oppositeFaction;
            }

            return false;
        }

        /// <summary>
        /// Check if target is from the opposite religion
        /// </summary>
        public bool IsOppositeReligion(Mobile m)
        {
            if (m_Religion == VystiaReligion.None)
                return false;

            VystiaReligion oppositeReligion = GetOppositeReligion(m_Religion);
            if (oppositeReligion == VystiaReligion.None)
                return false;

            if (m is PlayerMobile pm)
            {
                var piety = VystiaPiety.GetPiety(pm);
                return piety != null && piety.Religion == oppositeReligion;
            }

            if (m is VystiaCombatDummy dummy)
            {
                return dummy.Religion == oppositeReligion;
            }

            return false;
        }

        public override bool IsEnemy(Mobile m)
        {
            // Don't attack same faction/religion members
            if (IsSameFaction(m) || IsSameReligion(m))
                return false;

            // Attack opposite faction/religion on sight!
            if (IsOppositeFaction(m) || IsOppositeReligion(m))
                return true;

            return base.IsEnemy(m);
        }

        public VystiaCombatDummy(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version - incremented for class support

            writer.Write((int)m_Faction);
            writer.Write((int)m_Religion);
            writer.Write((int)m_CombatMode);
            writer.Write((int)m_ClassType);
            writer.Write(m_TotalDamageTaken);
            writer.Write(m_HitCount);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Faction = (VystiaFaction)reader.ReadInt();
            m_Religion = (VystiaReligion)reader.ReadInt();
            m_CombatMode = (CombatDummyMode)reader.ReadInt();

            if (version >= 1)
                m_ClassType = (PlayerClassTypeV2)reader.ReadInt();
            else
                m_ClassType = PlayerClassTypeV2.None;

            m_TotalDamageTaken = reader.ReadInt();
            m_HitCount = reader.ReadInt();
        }
    }

    /// <summary>
    /// Stone item to spawn configurable combat dummies
    /// </summary>
    public class VystiaCombatDummyStone : Item
    {
        private VystiaFaction m_Faction;
        private VystiaReligion m_Religion;
        private CombatDummyMode m_Mode;

        [CommandProperty(AccessLevel.GameMaster)]
        public VystiaFaction Faction
        {
            get { return m_Faction; }
            set { m_Faction = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public VystiaReligion Religion
        {
            get { return m_Religion; }
            set { m_Religion = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public CombatDummyMode Mode
        {
            get { return m_Mode; }
            set { m_Mode = value; InvalidateProperties(); }
        }

        [Constructable]
        public VystiaCombatDummyStone() : this(VystiaFaction.None, VystiaReligion.None, CombatDummyMode.Hybrid)
        {
        }

        [Constructable]
        public VystiaCombatDummyStone(VystiaFaction faction, VystiaReligion religion, CombatDummyMode mode)
            : base(0x1870) // Statue graphic
        {
            m_Faction = faction;
            m_Religion = religion;
            m_Mode = mode;

            Name = "Combat Dummy Stone";
            Hue = 0x455;
            Weight = 1.0;
            Movable = true;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from == null || !(from is PlayerMobile pm))
                return;

            if (pm.AccessLevel < AccessLevel.GameMaster)
            {
                pm.SendMessage(0x22, "Only Game Masters can spawn combat dummies.");
                return;
            }

            VystiaCombatDummy dummy = new VystiaCombatDummy(m_Faction, m_Religion, m_Mode);
            dummy.MoveToWorld(pm.Location, pm.Map);

            pm.SendMessage(0x35, "Spawned Combat Dummy [{0}] - {1} / {2}",
                m_Mode,
                m_Faction != VystiaFaction.None ? m_Faction.ToString() : "No Faction",
                m_Religion != VystiaReligion.None ? m_Religion.ToString() : "No Religion");
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Mode\t{0}", m_Mode);
            if (m_Faction != VystiaFaction.None)
                list.Add(1060659, "Faction\t{0}", m_Faction);
            if (m_Religion != VystiaReligion.None)
                list.Add(1060660, "Religion\t{0}", m_Religion);
        }

        public VystiaCombatDummyStone(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write((int)m_Faction);
            writer.Write((int)m_Religion);
            writer.Write((int)m_Mode);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_Faction = (VystiaFaction)reader.ReadInt();
            m_Religion = (VystiaReligion)reader.ReadInt();
            m_Mode = (CombatDummyMode)reader.ReadInt();
        }
    }

    /// <summary>
    /// Commands for spawning combat dummies
    /// </summary>
    public class CombatDummyCommands
    {
        public static void Initialize()
        {
            CommandSystem.Register("CombatDummy", AccessLevel.GameMaster, CombatDummy_OnCommand);
            CommandSystem.Register("CD", AccessLevel.GameMaster, CombatDummy_OnCommand);
            CommandSystem.Register("CombatDummyStones", AccessLevel.GameMaster, CombatDummyStones_OnCommand);
            CommandSystem.Register("CDS", AccessLevel.GameMaster, CombatDummyStones_OnCommand);
        }

        [Usage("CombatDummy [mode] [faction] [religion]")]
        [Description("Spawns a combat dummy. Modes: Passive, Melee, Caster, Hybrid (default)")]
        private static void CombatDummy_OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;
            if (pm == null)
                return;

            CombatDummyMode mode = CombatDummyMode.Hybrid;
            VystiaFaction faction = VystiaFaction.None;
            VystiaReligion religion = VystiaReligion.None;

            // Parse arguments
            if (e.Arguments.Length >= 1)
            {
                if (!Enum.TryParse(e.Arguments[0], true, out mode))
                {
                    pm.SendMessage(0x22, "Invalid mode. Use: Passive, Melee, Caster, or Hybrid");
                    return;
                }
            }

            if (e.Arguments.Length >= 2)
            {
                if (!Enum.TryParse(e.Arguments[1], true, out faction))
                {
                    pm.SendMessage(0x22, "Invalid faction.");
                    return;
                }
            }

            if (e.Arguments.Length >= 3)
            {
                if (!Enum.TryParse(e.Arguments[2], true, out religion))
                {
                    pm.SendMessage(0x22, "Invalid religion.");
                    return;
                }
            }

            VystiaCombatDummy dummy = new VystiaCombatDummy(faction, religion, mode);
            dummy.MoveToWorld(pm.Location, pm.Map);

            pm.SendMessage(0x35, "Spawned Combat Dummy [{0}] at your location.", mode);
        }

        [Usage("CombatDummyStones")]
        [Description("Adds a set of combat dummy stones to your backpack")]
        private static void CombatDummyStones_OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;
            if (pm == null || pm.Backpack == null)
                return;

            // Create stones for each mode
            foreach (CombatDummyMode mode in Enum.GetValues(typeof(CombatDummyMode)))
            {
                pm.Backpack.DropItem(new VystiaCombatDummyStone(VystiaFaction.None, VystiaReligion.None, mode));
            }

            // Create faction-affiliated dummies
            pm.Backpack.DropItem(new VystiaCombatDummyStone(VystiaFaction.Frostguard, VystiaReligion.None, CombatDummyMode.Hybrid));
            pm.Backpack.DropItem(new VystiaCombatDummyStone(VystiaFaction.FlameLegion, VystiaReligion.None, CombatDummyMode.Hybrid));
            pm.Backpack.DropItem(new VystiaCombatDummyStone(VystiaFaction.Voidborn, VystiaReligion.None, CombatDummyMode.Caster));

            // Create religion-affiliated dummies
            pm.Backpack.DropItem(new VystiaCombatDummyStone(VystiaFaction.None, VystiaReligion.FrosthelmFaith, CombatDummyMode.Hybrid));
            pm.Backpack.DropItem(new VystiaCombatDummyStone(VystiaFaction.None, VystiaReligion.SuryasSandscript, CombatDummyMode.Melee));
            pm.Backpack.DropItem(new VystiaCombatDummyStone(VystiaFaction.None, VystiaReligion.OceanasCovenant, CombatDummyMode.Caster));

            pm.SendMessage(0x35, "Combat dummy stones added to your backpack!");
        }
    }
}

