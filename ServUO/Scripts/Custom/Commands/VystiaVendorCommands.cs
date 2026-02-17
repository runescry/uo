using System;
using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Scripts.Commands
{
    /// <summary>
    /// GM commands for spawning Vystia vendors
    /// </summary>
    public class VystiaVendorCommands
    {
        public static void Initialize()
        {
            CommandSystem.Register("VystiaReagents", AccessLevel.GameMaster, new CommandEventHandler(SpawnReagentVendor_OnCommand));
            CommandSystem.Register("vreag", AccessLevel.GameMaster, new CommandEventHandler(SpawnReagentVendor_OnCommand));

            CommandSystem.Register("VystiaResources", AccessLevel.GameMaster, new CommandEventHandler(SpawnResourceVendor_OnCommand));
            CommandSystem.Register("vres", AccessLevel.GameMaster, new CommandEventHandler(SpawnResourceVendor_OnCommand));
        }

        [Usage("VystiaReagents | vreag")]
        [Description("Spawns a Vystia Reagent Vendor at cursor location. Sells all 96 magic reagents.")]
        private static void SpawnReagentVendor_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendMessage("Target the location to spawn a Vystia Reagent Vendor...");
            e.Mobile.Target = new InternalTarget(VendorType.Reagent);
        }

        [Usage("VystiaResources | vres")]
        [Description("Spawns a Vystia Resource Vendor at cursor location. Sells ores, ingots, and special materials.")]
        private static void SpawnResourceVendor_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendMessage("Target the location to spawn a Vystia Resource Vendor...");
            e.Mobile.Target = new InternalTarget(VendorType.Resource);
        }

        private enum VendorType
        {
            Reagent,
            Resource
        }

        private class InternalTarget : Target
        {
            private VendorType m_Type;

            public InternalTarget(VendorType type) : base(-1, true, TargetFlags.None)
            {
                m_Type = type;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is IPoint3D)
                {
                    IPoint3D p = (IPoint3D)targeted;
                    Point3D loc = new Point3D(p);

                    Mobile vendor = null;

                    switch (m_Type)
                    {
                        case VendorType.Reagent:
                            vendor = new VystiaReagentVendor();
                            from.SendMessage(0x35, "Vystia Reagent Vendor spawned! (Sells all 96 magic reagents)");
                            break;
                        case VendorType.Resource:
                            vendor = new VystiaResourceVendor();
                            from.SendMessage(0x35, "Vystia Resource Vendor spawned! (Sells ores, ingots, special materials)");
                            break;
                    }

                    if (vendor != null)
                    {
                        vendor.MoveToWorld(loc, from.Map);
                        if (vendor is BaseCreature bc)
                        {
                            bc.Home = loc;
                            bc.RangeHome = 5;
                        }
                    }
                }
                else
                {
                    from.SendMessage(0x22, "Invalid target location.");
                }
            }
        }
    }
}
