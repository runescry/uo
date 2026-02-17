using System;
using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Targeting;
using Server.Services.LLM;

namespace Server.Commands
{
    public class DiagnoseNPCCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("DiagnoseNPC", AccessLevel.GameMaster, new CommandEventHandler(DiagnoseNPC_OnCommand));
        }

        [Usage("DiagnoseNPC")]
        [Description("Diagnoses an NPC's LLM integration status")]
        public static void DiagnoseNPC_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendMessage("Target the NPC to diagnose...");
            e.Mobile.Target = new InternalTarget();
        }

        private class InternalTarget : Target
        {
            public InternalTarget() : base(15, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is Mobile npc)
                {
                    from.SendMessage(0x35, "=== NPC Diagnostic Report ===");
                    from.SendMessage($"Name: {npc.Name}");
                    from.SendMessage($"Type: {npc.GetType().FullName}");
                    from.SendMessage($"Karma: {npc.Karma}");

                    // Check if implements ILLMConversational
                    ILLMConversational llm = npc as ILLMConversational;
                    if (llm != null)
                    {
                        from.SendMessage(0x3F, "✓ Implements ILLMConversational");
                        from.SendMessage($"  - LLMConversationEnabled: {llm.LLMConversationEnabled}");
                        from.SendMessage($"  - PersonalityType: {llm.PersonalityType}");
                        from.SendMessage($"  - SpeechPattern: {llm.SpeechPattern}");
                        from.SendMessage($"  - HearingRange: {llm.HearingRange}");
                    }
                    else
                    {
                        from.SendMessage(0x21, "✗ Does NOT implement ILLMConversational");
                        from.SendMessage("This NPC was likely spawned with old code.");
                        from.SendMessage("Delete it and spawn a new one.");
                    }
                }
                else
                {
                    from.SendMessage(0x21, "That is not a mobile.");
                }
            }
        }
    }
}
