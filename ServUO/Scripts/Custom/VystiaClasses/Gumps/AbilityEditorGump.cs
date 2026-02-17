/*
 * Vystia Class System v2.0
 * GM Ability Editor Gump
 *
 * In-game tool for GMs to browse, create, edit, and test abilities.
 * Part of Sprint 7: GM Editor Gump
 */

using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Commands;
using Server.Custom.VystiaClasses.Abilities;
using Server.Custom.VystiaClasses.Systems;

namespace Server.Custom.VystiaClasses.Gumps
{
    #region Main Editor Gump

    /// <summary>
    /// Main ability browser and editor gump for GMs
    /// </summary>
    public class AbilityEditorGump : Gump
    {
        private const int GumpWidth = 600;
        private const int GumpHeight = 500;

        private Mobile m_From;
        private AbilitySchool m_CurrentSchool;
        private int m_Page;
        private List<AbilityDefinition> m_Abilities;

        // Button IDs
        private const int ButtonPrevPage = 1;
        private const int ButtonNextPage = 2;
        private const int ButtonCreateNew = 3;
        private const int ButtonRefresh = 4;
        private const int ButtonSchoolBase = 100;
        private const int ButtonAbilityBase = 1000;
        private const int ButtonTestBase = 2000;
        private const int ButtonEditBase = 3000;

        public AbilityEditorGump(Mobile from, AbilitySchool school = AbilitySchool.Ice, int page = 0)
            : base(50, 50)
        {
            m_From = from;
            m_CurrentSchool = school;
            m_Page = page;

            // Get abilities for current school
            m_Abilities = AbilityRegistry.GetAbilitiesBySchool(school);
            if (m_Abilities == null)
                m_Abilities = new List<AbilityDefinition>();

            BuildGump();
        }

        private void BuildGump()
        {
            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            // Background
            AddPage(0);
            AddBackground(0, 0, GumpWidth, GumpHeight, 9270);

            // Title
            AddHtml(0, 15, GumpWidth, 20, Center(Color("Vystia Ability Editor", "#FFD700")), false, false);

            // School tabs
            int tabX = 20;
            int tabY = 45;
            int tabWidth = 80;
            int schoolIndex = 0;

            foreach (AbilitySchool school in GetDisplaySchools())
            {
                bool selected = (school == m_CurrentSchool);
                int buttonId = ButtonSchoolBase + schoolIndex;

                string schoolLabel = GetSchoolDisplayName(school);

                if (selected)
                {
                    AddButton(tabX, tabY, 4006, 4006, buttonId, GumpButtonType.Reply, 0);
                    AddHtml(tabX + 5, tabY + 3, tabWidth - 10, 20, Color(schoolLabel, "#FFFFFF"), false, false);
                }
                else
                {
                    AddButton(tabX, tabY, 4005, 4006, buttonId, GumpButtonType.Reply, 0);
                    AddHtml(tabX + 5, tabY + 3, tabWidth - 10, 20, Color(schoolLabel, "#AAAAAA"), false, false);
                }

                tabX += tabWidth + 5;
                if (tabX > GumpWidth - 100)
                {
                    tabX = 20;
                    tabY += 25;
                }
                schoolIndex++;
            }

            // Ability list area
            int listY = tabY + 35;
            int listHeight = GumpHeight - listY - 80;

            AddHtml(20, listY, 200, 20, Color($"Abilities ({m_Abilities.Count} total):", "#AAFFAA"), false, false);
            listY += 25;

            // Column headers
            AddHtml(20, listY, 50, 20, Color("ID", "#888888"), false, false);
            AddHtml(75, listY, 150, 20, Color("Name", "#888888"), false, false);
            AddHtml(230, listY, 50, 20, Color("Circle", "#888888"), false, false);
            AddHtml(285, listY, 60, 20, Color("Mana", "#888888"), false, false);
            AddHtml(350, listY, 80, 20, Color("Target", "#888888"), false, false);
            AddHtml(440, listY, 60, 20, Color("Test", "#888888"), false, false);
            AddHtml(510, listY, 60, 20, Color("Edit", "#888888"), false, false);
            listY += 22;

            // Ability entries
            int itemsPerPage = 12;
            int startIndex = m_Page * itemsPerPage;
            int endIndex = Math.Min(startIndex + itemsPerPage, m_Abilities.Count);

            for (int i = startIndex; i < endIndex; i++)
            {
                AbilityDefinition ability = m_Abilities[i];
                int buttonIndex = i - startIndex;

                // ID
                AddHtml(20, listY, 50, 20, ability.Id.ToString(), false, false);

                // Name
                AddHtml(75, listY, 150, 20, ability.Name ?? "Unnamed", false, false);

                // Circle
                AddHtml(230, listY, 50, 20, ability.Circle.ToString(), false, false);

                // Mana cost
                AddHtml(285, listY, 60, 20, ability.ManaCost.ToString(), false, false);

                // Target type
                AddHtml(350, listY, 80, 20, ability.TargetType.ToString(), false, false);

                // Test button
                AddButton(440, listY, 4011, 4012, ButtonTestBase + i, GumpButtonType.Reply, 0);

                // Edit button
                AddButton(510, listY, 4011, 4012, ButtonEditBase + i, GumpButtonType.Reply, 0);

                listY += 22;
            }

            // Pagination
            int pageY = GumpHeight - 70;
            int totalPages = (m_Abilities.Count + itemsPerPage - 1) / itemsPerPage;

            if (m_Page > 0)
                AddButton(20, pageY, 4014, 4015, ButtonPrevPage, GumpButtonType.Reply, 0);

            AddHtml(200, pageY, 200, 20, Center($"Page {m_Page + 1} of {Math.Max(1, totalPages)}"), false, false);

            if (m_Page < totalPages - 1)
                AddButton(GumpWidth - 60, pageY, 4005, 4006, ButtonNextPage, GumpButtonType.Reply, 0);

            // Bottom buttons
            int bottomY = GumpHeight - 40;

            // Create New
            AddButton(20, bottomY, 4029, 4030, ButtonCreateNew, GumpButtonType.Reply, 0);
            AddHtml(60, bottomY + 3, 100, 20, "Create New", false, false);

            // Refresh
            AddButton(170, bottomY, 4029, 4030, ButtonRefresh, GumpButtonType.Reply, 0);
            AddHtml(210, bottomY + 3, 100, 20, "Refresh", false, false);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;
            int buttonId = info.ButtonID;

            if (buttonId == 0)
                return;

            // School tabs
            if (buttonId >= ButtonSchoolBase && buttonId < ButtonSchoolBase + 100)
            {
                int schoolIndex = buttonId - ButtonSchoolBase;
                AbilitySchool[] schools = GetDisplaySchools();
                if (schoolIndex < schools.Length)
                {
                    from.SendGump(new AbilityEditorGump(from, schools[schoolIndex], 0));
                }
                return;
            }

            // Test ability
            if (buttonId >= ButtonTestBase && buttonId < ButtonEditBase)
            {
                int abilityIndex = buttonId - ButtonTestBase;
                if (abilityIndex < m_Abilities.Count)
                {
                    AbilityDefinition ability = m_Abilities[abilityIndex];
                    from.SendGump(new AbilityTestGump(from, ability));
                }
                return;
            }

            // Edit ability
            if (buttonId >= ButtonEditBase)
            {
                int abilityIndex = buttonId - ButtonEditBase;
                if (abilityIndex < m_Abilities.Count)
                {
                    AbilityDefinition ability = m_Abilities[abilityIndex];
                    from.SendGump(new AbilityDetailGump(from, ability, m_CurrentSchool));
                }
                return;
            }

            switch (buttonId)
            {
                case ButtonPrevPage:
                    from.SendGump(new AbilityEditorGump(from, m_CurrentSchool, Math.Max(0, m_Page - 1)));
                    break;

                case ButtonNextPage:
                    from.SendGump(new AbilityEditorGump(from, m_CurrentSchool, m_Page + 1));
                    break;

                case ButtonCreateNew:
                    from.SendGump(new AbilityCreateGump(from, m_CurrentSchool));
                    break;

                case ButtonRefresh:
                    from.SendGump(new AbilityEditorGump(from, m_CurrentSchool, m_Page));
                    break;
            }
        }


        private AbilitySchool[] GetDisplaySchools()
        {
            return new AbilitySchool[]
            {
                AbilitySchool.Ice,
                AbilitySchool.Nature,
                AbilitySchool.Dark,
                AbilitySchool.Elemental,
                AbilitySchool.Necromancy,
                AbilitySchool.Bardic,
                AbilitySchool.Rogue,
                AbilitySchool.Barbarian,
                AbilitySchool.Monk,
                AbilitySchool.Knight,
                AbilitySchool.Paladin,
                AbilitySchool.Ranger
            };
        }

        private string GetSchoolDisplayName(AbilitySchool school)
        {
            return school == AbilitySchool.Bardic ? "Songweaving" : school.ToString();
        }

        private string Center(string text)
        {
            return $"<CENTER>{text}</CENTER>";
        }

        private string Color(string text, string color)
        {
            return $"<BASEFONT COLOR={color}>{text}</BASEFONT>";
        }
    }

    #endregion

    #region Ability Detail Gump

    /// <summary>
    /// Detailed view of a single ability
    /// </summary>
    public class AbilityDetailGump : Gump
    {
        private Mobile m_From;
        private AbilityDefinition m_Ability;
        private AbilitySchool m_ReturnSchool;

        public AbilityDetailGump(Mobile from, AbilityDefinition ability, AbilitySchool returnSchool)
            : base(100, 100)
        {
            m_From = from;
            m_Ability = ability;
            m_ReturnSchool = returnSchool;

            BuildGump();
        }

        private void BuildGump()
        {
            Closable = true;
            Disposable = true;
            Dragable = true;

            AddPage(0);
            AddBackground(0, 0, 450, 500, 9270);

            // Title
            AddHtml(0, 15, 450, 20, Center(Color(m_Ability.Name ?? "Unnamed Ability", "#FFD700")), false, false);
            AddHtml(0, 35, 450, 20, Center(Color($"ID: {m_Ability.Id}", "#888888")), false, false);

            int y = 65;

            // Identity section
            AddHtml(20, y, 200, 20, Color("=== Identity ===", "#AAFFAA"), false, false);
            y += 22;
            string schoolLabel = m_Ability.School == AbilitySchool.Bardic ? "Songweaving" : m_Ability.School.ToString();
            AddHtml(20, y, 200, 20, $"School: {schoolLabel}", false, false);
            y += 20;
            AddHtml(20, y, 200, 20, $"Circle: {m_Ability.Circle}", false, false);
            y += 20;
            AddHtml(20, y, 410, 40, $"Description: {m_Ability.Description ?? "None"}", false, false);
            y += 45;

            // Costs section
            AddHtml(20, y, 200, 20, Color("=== Costs ===", "#AAFFAA"), false, false);
            y += 22;
            AddHtml(20, y, 150, 20, $"Mana: {m_Ability.ManaCost}", false, false);
            AddHtml(180, y, 150, 20, $"Stamina: {m_Ability.StaminaCost}", false, false);
            y += 20;
            AddHtml(20, y, 150, 20, $"Health: {m_Ability.HealthCost}", false, false);
            AddHtml(180, y, 150, 20, $"Reagent: {m_Ability.ReagentCost}", false, false);
            y += 25;

            // Targeting section
            AddHtml(20, y, 200, 20, Color("=== Targeting ===", "#AAFFAA"), false, false);
            y += 22;
            AddHtml(20, y, 150, 20, $"Type: {m_Ability.TargetType}", false, false);
            AddHtml(180, y, 150, 20, $"Range: {m_Ability.Range}", false, false);
            y += 20;
            AddHtml(20, y, 150, 20, $"AoE Radius: {m_Ability.AoERadius}", false, false);
            AddHtml(180, y, 150, 20, $"Max Targets: {m_Ability.MaxTargets}", false, false);
            y += 25;

            // Timing section
            AddHtml(20, y, 200, 20, Color("=== Timing ===", "#AAFFAA"), false, false);
            y += 22;
            AddHtml(20, y, 200, 20, $"Cast Time: {m_Ability.CastTime.TotalSeconds}s", false, false);
            AddHtml(230, y, 200, 20, $"Cooldown: {m_Ability.Cooldown.TotalSeconds}s", false, false);
            y += 25;

            // Flags section
            AddHtml(20, y, 200, 20, Color("=== Flags ===", "#AAFFAA"), false, false);
            y += 22;
            string flags = "";
            if (m_Ability.IsInstant) flags += "Instant ";
            if (m_Ability.IsChanneled) flags += "Channeled ";
            if (m_Ability.IsPassive) flags += "Passive ";
            if (m_Ability.IsFinisher) flags += "Finisher ";
            if (m_Ability.CanCrit) flags += "CanCrit ";
            if (m_Ability.BreaksStealth) flags += "BreaksStealth ";
            if (string.IsNullOrEmpty(flags)) flags = "None";
            AddHtml(20, y, 400, 20, flags, false, false);
            y += 25;

            // Effects section
            AddHtml(20, y, 200, 20, Color($"=== Effects ({m_Ability.Effects?.Count ?? 0}) ===", "#AAFFAA"), false, false);
            y += 22;

            if (m_Ability.Effects != null && m_Ability.Effects.Count > 0)
            {
                foreach (var effect in m_Ability.Effects)
                {
                    string effectStr = $"{effect.Type}";
                    if (effect.MinValue > 0 || effect.MaxValue > 0)
                        effectStr += $" ({effect.MinValue}-{effect.MaxValue})";
                    if (effect.DamageType != VystiaDamageType.Physical)
                        effectStr += $" [{effect.DamageType}]";
                    if (effect.Duration > TimeSpan.Zero)
                        effectStr += $" {effect.Duration.TotalSeconds}s";

                    AddHtml(30, y, 390, 20, effectStr, false, false);
                    y += 18;

                    if (y > 440) break;
                }
            }
            else
            {
                AddHtml(30, y, 200, 20, "No effects defined", false, false);
            }

            // Buttons
            AddButton(20, 460, 4014, 4015, 1, GumpButtonType.Reply, 0);
            AddHtml(60, 463, 80, 20, "Back", false, false);

            AddButton(150, 460, 4029, 4030, 2, GumpButtonType.Reply, 0);
            AddHtml(190, 463, 80, 20, "Test", false, false);

            AddButton(280, 460, 4029, 4030, 3, GumpButtonType.Reply, 0);
            AddHtml(320, 463, 100, 20, "Export Code", false, false);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            switch (info.ButtonID)
            {
                case 1: // Back
                    from.SendGump(new AbilityEditorGump(from, m_ReturnSchool));
                    break;

                case 2: // Test
                    from.SendGump(new AbilityTestGump(from, m_Ability));
                    break;

                case 3: // Export Code
                    ExportAbilityCode(from);
                    from.SendGump(new AbilityDetailGump(from, m_Ability, m_ReturnSchool));
                    break;
            }
        }

        private void ExportAbilityCode(Mobile from)
        {
            from.SendMessage(0x3B2, "=== Ability Code Export ===");
            from.SendMessage($"var {m_Ability.Name?.Replace(" ", "")} = new AbilityDefinition()");
            from.SendMessage($"    .WithId({m_Ability.Id})");
            from.SendMessage($"    .WithName(\"{m_Ability.Name}\")");
            from.SendMessage($"    .InSchool(AbilitySchool.{m_Ability.School})");
            from.SendMessage($"    .InCircle({m_Ability.Circle})");

            if (m_Ability.ManaCost > 0)
                from.SendMessage($"    .WithManaCost({m_Ability.ManaCost})");
            if (m_Ability.StaminaCost > 0)
                from.SendMessage($"    .WithStaminaCost({m_Ability.StaminaCost})");

            from.SendMessage($"    .Targeting(AbilityTargetType.{m_Ability.TargetType}, {m_Ability.Range})");

            if (m_Ability.Cooldown > TimeSpan.Zero)
                from.SendMessage($"    .WithCooldown({m_Ability.Cooldown.TotalSeconds})");

            from.SendMessage(";");
            from.SendMessage(0x3B2, "=== End Export ===");
        }

        private string Center(string text) => $"<CENTER>{text}</CENTER>";
        private string Color(string text, string color) => $"<BASEFONT COLOR={color}>{text}</BASEFONT>";
    }

    #endregion

    #region Ability Test Gump

    /// <summary>
    /// Test gump for executing abilities with diagnostic output
    /// </summary>
    public class AbilityTestGump : Gump
    {
        private Mobile m_From;
        private AbilityDefinition m_Ability;

        public AbilityTestGump(Mobile from, AbilityDefinition ability)
            : base(100, 100)
        {
            m_From = from;
            m_Ability = ability;

            BuildGump();
        }

        private void BuildGump()
        {
            Closable = true;
            Disposable = true;
            Dragable = true;

            AddPage(0);
            AddBackground(0, 0, 350, 300, 9270);

            AddHtml(0, 15, 350, 20, Center(Color("Test Ability", "#FFD700")), false, false);
            AddHtml(0, 35, 350, 20, Center(m_Ability.Name ?? "Unknown"), false, false);

            int y = 65;

            // Ability info
            AddHtml(20, y, 310, 20, $"ID: {m_Ability.Id} | School: {m_Ability.School}", false, false);
            y += 20;
            AddHtml(20, y, 310, 20, $"Mana: {m_Ability.ManaCost} | Range: {m_Ability.Range}", false, false);
            y += 20;
            AddHtml(20, y, 310, 20, $"Target Type: {m_Ability.TargetType}", false, false);
            y += 30;

            // Test options
            AddHtml(20, y, 200, 20, Color("Test Options:", "#AAFFAA"), false, false);
            y += 25;

            // Execute on target
            AddButton(20, y, 4029, 4030, 1, GumpButtonType.Reply, 0);
            AddHtml(60, y + 3, 250, 20, "Execute on current target", false, false);
            y += 30;

            // Execute on self
            AddButton(20, y, 4029, 4030, 2, GumpButtonType.Reply, 0);
            AddHtml(60, y + 3, 250, 20, "Execute on self", false, false);
            y += 30;

            // Execute at location
            AddButton(20, y, 4029, 4030, 3, GumpButtonType.Reply, 0);
            AddHtml(60, y + 3, 250, 20, "Execute at target location", false, false);
            y += 30;

            // Spawn dummy and test
            AddButton(20, y, 4029, 4030, 4, GumpButtonType.Reply, 0);
            AddHtml(60, y + 3, 250, 20, "Spawn dummy and execute", false, false);
            y += 40;

            // Back button
            AddButton(20, 260, 4014, 4015, 0, GumpButtonType.Reply, 0);
            AddHtml(60, 263, 80, 20, "Back", false, false);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            switch (info.ButtonID)
            {
                case 0: // Back/Close
                    from.SendGump(new AbilityEditorGump(from, m_Ability.School));
                    break;

                case 1: // Execute on target
                    ExecuteOnTarget(from);
                    break;

                case 2: // Execute on self
                    ExecuteOnSelf(from);
                    break;

                case 3: // Execute at location
                    ExecuteAtLocation(from);
                    break;

                case 4: // Spawn dummy and execute
                    SpawnAndExecute(from);
                    break;
            }
        }

        private void ExecuteOnTarget(Mobile from)
        {
            Mobile target = from.Combatant as Mobile;

            if (target == null)
            {
                from.SendMessage(0x22, "You need a target. Set a combatant first.");
                from.SendGump(new AbilityTestGump(from, m_Ability));
                return;
            }

            ExecuteAndReport(from, target);
        }

        private void ExecuteOnSelf(Mobile from)
        {
            ExecuteAndReport(from, from);
        }

        private void ExecuteAtLocation(Mobile from)
        {
            from.SendMessage("Target a location for the ability.");
            from.Target = new AbilityLocationTarget(m_Ability);
        }

        private void SpawnAndExecute(Mobile from)
        {
            PracticeDummy dummy = new PracticeDummy();
            Point3D loc = new Point3D(from.X + 2, from.Y, from.Z);
            dummy.MoveToWorld(loc, from.Map);

            from.SendMessage(0x3B2, "Practice Target spawned. Executing ability...");

            Timer.DelayCall(TimeSpan.FromMilliseconds(500), () =>
            {
                ExecuteAndReport(from, dummy);
            });
        }

        private void ExecuteAndReport(Mobile from, Mobile target)
        {
            from.SendMessage(0x3B2, $"=== Testing: {m_Ability.Name} ===");
            from.SendMessage($"Caster: {from.Name} | Target: {target.Name}");
            from.SendMessage($"Target HP before: {target.Hits}/{target.HitsMax}");

            AbilityExecutionResult result = AbilityExecutor.Execute(from, m_Ability, target);

            from.SendMessage($"Target HP after: {target.Hits}/{target.HitsMax}");
            from.SendMessage(0x3B2, "--- Result ---");
            from.SendMessage($"Success: {result.Success}");

            if (!result.Success)
            {
                from.SendMessage(0x22, $"Failure: {result.FailureReason}");
            }
            else
            {
                from.SendMessage($"Damage: {result.TotalDamage}");
                from.SendMessage($"Healing: {result.TotalHealing}");
                from.SendMessage($"Targets Hit: {result.TargetsHit}");
                from.SendMessage($"Was Crit: {result.WasCrit}");
            }

            from.SendMessage(0x3B2, "=== Test Complete ===");
            from.SendGump(new AbilityTestGump(from, m_Ability));
        }

        private string Center(string text) => $"<CENTER>{text}</CENTER>";
        private string Color(string text, string color) => $"<BASEFONT COLOR={color}>{text}</BASEFONT>";
    }

    /// <summary>
    /// Target for location-based ability testing
    /// </summary>
    public class AbilityLocationTarget : Server.Targeting.Target
    {
        private AbilityDefinition m_Ability;

        public AbilityLocationTarget(AbilityDefinition ability) : base(12, true, Server.Targeting.TargetFlags.None)
        {
            m_Ability = ability;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            IPoint3D p = targeted as IPoint3D;
            if (p == null)
            {
                from.SendMessage(0x22, "Invalid target location.");
                return;
            }

            Point3D loc = new Point3D(p);

            from.SendMessage(0x3B2, $"=== Testing: {m_Ability.Name} at {loc} ===");

            AbilityExecutionResult result = AbilityExecutor.Execute(from, m_Ability, null, loc);

            from.SendMessage(0x3B2, "--- Result ---");
            from.SendMessage($"Success: {result.Success}");

            if (!result.Success)
            {
                from.SendMessage(0x22, $"Failure: {result.FailureReason}");
            }
            else
            {
                from.SendMessage($"Targets Hit: {result.TargetsHit}");
                from.SendMessage($"Total Damage: {result.TotalDamage}");
            }

            from.SendMessage(0x3B2, "=== Test Complete ===");
            from.SendGump(new AbilityTestGump(from, m_Ability));
        }
    }

    #endregion

    #region Ability Create Gump

    /// <summary>
    /// Gump for creating new abilities
    /// </summary>
    public class AbilityCreateGump : Gump
    {
        private Mobile m_From;
        private AbilitySchool m_School;

        // Text entry IDs
        private const int EntryId = 1;
        private const int EntryName = 2;
        private const int EntryCircle = 3;
        private const int EntryManaCost = 4;
        private const int EntryRange = 5;
        private const int EntryMinDamage = 6;
        private const int EntryMaxDamage = 7;
        private const int EntryCooldown = 8;

        public AbilityCreateGump(Mobile from, AbilitySchool school)
            : base(100, 100)
        {
            m_From = from;
            m_School = school;

            BuildGump();
        }

        private void BuildGump()
        {
            Closable = true;
            Disposable = true;
            Dragable = true;

            AddPage(0);
            AddBackground(0, 0, 400, 450, 9270);

            AddHtml(0, 15, 400, 20, Center(Color("Create New Ability", "#FFD700")), false, false);
            AddHtml(0, 35, 400, 20, Center($"School: {m_School}"), false, false);

            int y = 65;
            int labelWidth = 100;
            int entryWidth = 150;

            // ID
            AddHtml(20, y, labelWidth, 20, "ID:", false, false);
            AddBackground(130, y - 2, entryWidth, 24, 9350);
            AddTextEntry(135, y, entryWidth - 10, 20, 0, EntryId, GetNextAbilityId().ToString());
            y += 30;

            // Name
            AddHtml(20, y, labelWidth, 20, "Name:", false, false);
            AddBackground(130, y - 2, entryWidth, 24, 9350);
            AddTextEntry(135, y, entryWidth - 10, 20, 0, EntryName, "New Ability");
            y += 30;

            // Circle
            AddHtml(20, y, labelWidth, 20, "Circle (1-8):", false, false);
            AddBackground(130, y - 2, 60, 24, 9350);
            AddTextEntry(135, y, 50, 20, 0, EntryCircle, "1");
            y += 30;

            // Mana Cost
            AddHtml(20, y, labelWidth, 20, "Mana Cost:", false, false);
            AddBackground(130, y - 2, 60, 24, 9350);
            AddTextEntry(135, y, 50, 20, 0, EntryManaCost, "4");
            y += 30;

            // Range
            AddHtml(20, y, labelWidth, 20, "Range:", false, false);
            AddBackground(130, y - 2, 60, 24, 9350);
            AddTextEntry(135, y, 50, 20, 0, EntryRange, "12");
            y += 30;

            // Min Damage
            AddHtml(20, y, labelWidth, 20, "Min Damage:", false, false);
            AddBackground(130, y - 2, 60, 24, 9350);
            AddTextEntry(135, y, 50, 20, 0, EntryMinDamage, "10");
            y += 30;

            // Max Damage
            AddHtml(20, y, labelWidth, 20, "Max Damage:", false, false);
            AddBackground(130, y - 2, 60, 24, 9350);
            AddTextEntry(135, y, 50, 20, 0, EntryMaxDamage, "20");
            y += 30;

            // Cooldown
            AddHtml(20, y, labelWidth, 20, "Cooldown (s):", false, false);
            AddBackground(130, y - 2, 60, 24, 9350);
            AddTextEntry(135, y, 50, 20, 0, EntryCooldown, "0");
            y += 40;

            // Preset buttons
            AddHtml(20, y, 200, 20, Color("Quick Presets:", "#AAFFAA"), false, false);
            y += 25;

            AddButton(20, y, 4029, 4030, 10, GumpButtonType.Reply, 0);
            AddHtml(60, y + 3, 150, 20, "Damage Spell", false, false);

            AddButton(200, y, 4029, 4030, 11, GumpButtonType.Reply, 0);
            AddHtml(240, y + 3, 150, 20, "Heal Spell", false, false);
            y += 30;

            AddButton(20, y, 4029, 4030, 12, GumpButtonType.Reply, 0);
            AddHtml(60, y + 3, 150, 20, "DoT Spell", false, false);

            AddButton(200, y, 4029, 4030, 13, GumpButtonType.Reply, 0);
            AddHtml(240, y + 3, 150, 20, "Buff Spell", false, false);
            y += 30;

            AddButton(20, y, 4029, 4030, 14, GumpButtonType.Reply, 0);
            AddHtml(60, y + 3, 150, 20, "Melee Strike", false, false);

            AddButton(200, y, 4029, 4030, 15, GumpButtonType.Reply, 0);
            AddHtml(240, y + 3, 150, 20, "Finisher", false, false);

            // Bottom buttons
            AddButton(20, 410, 4014, 4015, 0, GumpButtonType.Reply, 0);
            AddHtml(60, 413, 80, 20, "Cancel", false, false);

            AddButton(280, 410, 4029, 4030, 1, GumpButtonType.Reply, 0);
            AddHtml(320, 413, 80, 20, "Create", false, false);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (info.ButtonID == 0)
            {
                from.SendGump(new AbilityEditorGump(from, m_School));
                return;
            }

            if (info.ButtonID == 1) // Create
            {
                CreateAbility(from, info);
                return;
            }

            // Presets
            AbilityDefinition preset = null;
            int nextId = GetNextAbilityId();

            switch (info.ButtonID)
            {
                case 10: // Damage Spell
                    preset = AbilityDefinition.CreateDamageSpell(nextId, "New Damage Spell", m_School, 1, 10, 20, VystiaDamageType.Physical, 5);
                    break;

                case 11: // Heal Spell
                    preset = AbilityDefinition.CreateHealSpell(nextId, "New Heal Spell", m_School, 1, 20, 40, 8);
                    break;

                case 12: // DoT Spell
                    preset = AbilityDefinition.CreateDoTSpell(nextId, "New DoT Spell", m_School, 2, 5, 15.0, VystiaDamageType.Physical, 10);
                    break;

                case 13: // Buff Spell
                    preset = AbilityDefinition.CreateBuffSpell(nextId, "New Buff Spell", m_School, 1, VystiaBuffType.StrengthBuff, 10, 60.0, 6);
                    break;

                case 14: // Melee Strike
                    preset = AbilityDefinition.CreateMartialStrike(nextId, "New Strike", m_School, 15, 25, 10);
                    break;

                case 15: // Finisher
                    preset = AbilityDefinition.CreateFinisher(nextId, "New Finisher", m_School, 20, 10, 25);
                    break;
            }

            if (preset != null)
            {
                AbilityRegistry.RegisterAbility(preset);
                from.SendMessage(0x3B2, $"Created preset ability: {preset.Name} (ID: {preset.Id})");
            }

            from.SendGump(new AbilityEditorGump(from, m_School));
        }

        private void CreateAbility(Mobile from, RelayInfo info)
        {
            try
            {
                int id = int.Parse(GetTextEntry(info, EntryId, "0"));
                string name = GetTextEntry(info, EntryName, "Unnamed");
                int circle = int.Parse(GetTextEntry(info, EntryCircle, "1"));
                int manaCost = int.Parse(GetTextEntry(info, EntryManaCost, "0"));
                int range = int.Parse(GetTextEntry(info, EntryRange, "12"));
                int minDmg = int.Parse(GetTextEntry(info, EntryMinDamage, "0"));
                int maxDmg = int.Parse(GetTextEntry(info, EntryMaxDamage, "0"));
                double cooldown = double.Parse(GetTextEntry(info, EntryCooldown, "0"));

                AbilityDefinition ability = new AbilityDefinition()
                    .WithId(id)
                    .WithName(name)
                    .InSchool(m_School)
                    .InCircle(circle)
                    .WithManaCost(manaCost)
                    .Targeting(AbilityTargetType.SingleTarget, range)
                    .WithCooldown(cooldown);

                if (minDmg > 0 || maxDmg > 0)
                {
                    ability.WithDamage(minDmg, maxDmg, VystiaDamageType.Physical);
                }

                AbilityRegistry.RegisterAbility(ability);
                from.SendMessage(0x3B2, $"Created ability: {name} (ID: {id})");
            }
            catch (Exception ex)
            {
                from.SendMessage(0x22, $"Error creating ability: {ex.Message}");
            }

            from.SendGump(new AbilityEditorGump(from, m_School));
        }

        private string GetTextEntry(RelayInfo info, int id, string defaultValue)
        {
            TextRelay entry = info.GetTextEntry(id);
            if (entry != null && !string.IsNullOrEmpty(entry.Text))
                return entry.Text;
            return defaultValue;
        }

        private int GetNextAbilityId()
        {
            // Find highest ID and add 1
            int maxId = 10000;
            foreach (var ability in AbilityRegistry.AllAbilities)
            {
                if (ability.Id >= maxId)
                    maxId = ability.Id + 1;
            }
            return maxId;
        }

        private string Center(string text) => $"<CENTER>{text}</CENTER>";
        private string Color(string text, string color) => $"<BASEFONT COLOR={color}>{text}</BASEFONT>";
    }

    #endregion

    #region Test Dummy

    /// <summary>
    /// Training dummy for ability testing
    /// </summary>
    #endregion

    #region GM Commands

    /// <summary>
    /// GM commands for the ability editor system
    /// </summary>
    public class AbilityEditorCommands
    {
        public static void Initialize()
        {
            CommandSystem.Register("AbilityEditor", AccessLevel.GameMaster, new CommandEventHandler(AbilityEditor_OnCommand));
            CommandSystem.Register("AE", AccessLevel.GameMaster, new CommandEventHandler(AbilityEditor_OnCommand));
        }

        [Usage("AbilityEditor")]
        [Description("Opens the GM Ability Editor gump.")]
        private static void AbilityEditor_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new AbilityEditorGump(e.Mobile));
        }
    }

    #endregion
}
