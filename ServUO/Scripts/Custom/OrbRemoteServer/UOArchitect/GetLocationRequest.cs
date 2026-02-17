using System;
using OrbServerSDK;
using UOArchitectInterface;
using Server.Engines.OrbRemoteServer;

namespace Server.Engines.UOArchitect
{
	public class GetLocationRequest : BaseOrbToolRequest  
	{
		private static bool s_Initialized = false;
		
		public static void Initialize()
		{
			if (s_Initialized)
				return;
				
			OrbServer.Register("UOAR_GetLocation", typeof(GetLocationRequest), AccessLevel.GameMaster, true);
			s_Initialized = true;
		}

		public override void OnRequest(OrbClientInfo clientInfo, OrbRequestArgs args)
		{
			FindOnlineMobile(clientInfo);

			if(!this.IsOnline)
			{
				SendResponse(null);
				return;
			}

			UOAR_ObjectTarget target = new UOAR_ObjectTarget();
			target.OnTargetObject += new UOAR_ObjectTarget.TargetObjectEvent(OnTarget);
			target.OnCancelled += new UOAR_ObjectTarget.TargetCancelEvent(OnCancelTarget);
			
			this.Mobile.SendMessage("Target an object");
			this.Mobile.Target = target;
		}

		private void OnTarget(object targeted)
		{
			if(targeted != null)
			{
				IPoint3D location = (IPoint3D)targeted;
				SendResponse(new GetLocationResp(location.X, location.Y, location.Z));
			}
			else
			{
				SendResponse(null);
			}
		}

		private void OnCancelTarget()
		{
			SendResponse(null);
		}

	}
}
