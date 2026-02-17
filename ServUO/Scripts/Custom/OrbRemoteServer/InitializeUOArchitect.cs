// This file initializes all UO Architect components
// Place this in your ServUO/Scripts/Custom/OrbRemoteServer/ directory

using System;
using Server;

namespace Server.Engines.OrbRemoteServer
{
	public class InitializeUOArchitect
	{
		public static void Initialize()
		{
			// The OrbServer static constructor will auto-initialize when the class is first accessed
			// Just accessing it here ensures it's loaded
			_ = OrbServer.IsRunning;

			// Initialize all UO Architect request handlers
			Server.Engines.UOArchitect.BuildDesignRequest.Initialize();
			Server.Engines.UOArchitect.ExtractItemsRequest.Initialize();
			Server.Engines.UOArchitect.SelectItemsRequest.Initialize();
			Server.Engines.UOArchitect.GetLocationRequest.Initialize();

			// Initialize all UO Architect command handlers
			Server.Scripts.UOArchitect.DeleteItems.Initialize();
			Server.Scripts.UOArchitect.MoveItemsCommand.Initialize();

			// Initialize client commands (NudgeSelfUp, NudgeSelfDown, MRemove)
			// These are optional utility commands for UO Architect.
			try
			{
				Server.Engines.UOArchitect.Commands.ClientCommands.Initialize();
				Server.Engines.UOArchitect.Commands.UOArchitectMultiDeleteCommand.Initialize();
			}
			catch
			{
				Console.WriteLine("Warning: Could not initialize UO Architect client commands. They are optional.");
			}

			Console.WriteLine("UO Architect Server initialized and listening on port 2594");
		}
	}
}

