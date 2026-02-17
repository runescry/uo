using System;
using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Targeting;
using Server.Services.AISidekicks;

namespace Server.Scripts.Commands
{
    public class ApplyArchetype
    {
        public static void Initialize()
        {
            CommandSystem.Register("ApplyArchetype", AccessLevel.GameMaster, new CommandEventHandler(ApplyArchetype_OnCommand));
            CommandSystem.Register("aa", AccessLevel.GameMaster, new CommandEventHandler(ApplyArchetype_OnCommand));
        }

        [Usage("ApplyArchetype <archetype>")]
        [Description("Applies an archetype's skills and stats to a targeted mobile. Available: Warrior, Mage, Archer, Healer, Paladin, Ranger, Thief, Necromancer, Battlemage, Cleric, Druid, Tamer")]
        private static void ApplyArchetype_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (e.Length == 0)
            {
                from.SendMessage("Usage: [ApplyArchetype <archetype>");
                from.SendMessage("Available archetypes: Warrior, Mage, Archer, Healer, Paladin, Ranger, Thief, Necromancer, Battlemage, Cleric, Druid, Tamer");
                return;
            }

            string archetypeName = e.GetString(0);
            SidekickArchetype archetype = SidekickArchetype.GetArchetypeByName(archetypeName);

            if (archetype == null)
            {
                from.SendMessage("Unknown archetype: {0}", archetypeName);
                from.SendMessage("Available archetypes: Warrior, Mage, Archer, Healer, Paladin, Ranger, Thief, Necromancer, Battlemage, Cleric, Druid, Tamer");
                return;
            }

            from.SendMessage("Target a mobile to apply the {0} archetype...", archetype.Name);
            from.Target = new ApplyArchetypeTarget(archetype);
        }

        private class ApplyArchetypeTarget : Target
        {
            private SidekickArchetype m_Archetype;

            public ApplyArchetypeTarget(SidekickArchetype archetype) : base(-1, false, TargetFlags.None)
            {
                m_Archetype = archetype;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is Mobile target)
                {
                    // Apply stats
                    target.RawStr = m_Archetype.StartingStr;
                    target.RawDex = m_Archetype.StartingDex;
                    target.RawInt = m_Archetype.StartingInt;

                    // Apply stat locks
                    target.StrLock = m_Archetype.StatLocks.StrLock;
                    target.DexLock = m_Archetype.StatLocks.DexLock;
                    target.IntLock = m_Archetype.StatLocks.IntLock;

                    // Reset all skills to 0 first
                    for (int i = 0; i < target.Skills.Length; i++)
                    {
                        target.Skills[i].Base = 0.0;
                    }

                    // Apply archetype skills
                    foreach (StartingSkill skill in m_Archetype.StartingSkills)
                    {
                        target.Skills[skill.SkillName].Base = skill.Value;
                    }

                    // Refresh stats
                    target.Hits = target.HitsMax;
                    target.Mana = target.ManaMax;
                    target.Stam = target.StamMax;

                    from.SendMessage("Applied {0} archetype to {1}:", m_Archetype.Name, target.Name);
                    from.SendMessage("  Stats: Str {0}, Dex {1}, Int {2}", m_Archetype.StartingStr, m_Archetype.StartingDex, m_Archetype.StartingInt);
                    from.SendMessage("  Skills: {0} skills set", m_Archetype.StartingSkills.Count);

                    foreach (StartingSkill skill in m_Archetype.StartingSkills)
                    {
                        from.SendMessage("    - {0}: {1:F1}", skill.SkillName, skill.Value);
                    }

                    if (target is PlayerMobile)
                    {
                        target.SendMessage("A GM has applied the {0} archetype to you.", m_Archetype.Name);
                    }
                }
                else
                {
                    from.SendMessage("That is not a mobile.");
                }
            }
        }
    }
}
