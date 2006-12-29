/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 12/28/2006
 * Time: 10:03 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using OSMP;
using Metaverse.Common.Controller;

namespace Metaverse.Controller
{
	/// <summary>
	/// Description of MetaverseController.
	/// </summary>
	public class ClientController : IClientController
	{
		private static IClientController _instance = null;
		
		public static IClientController Instance {
			get {
				if( _instance == null ) {
					_instance = new ClientController();
				}
				return _instance;
			}
		}
		
		private ClientController() { }
		
		public void Initialize( string[] args ) {
			MetaverseClient.GetInstance().Init(args, ClientControllers.Instance);
		}
		
		public void InitializeWithServer( string[] args ) {
			
			
			MetaverseServer.GetInstance().Init(args, ServerControllers.Instance);
            MetaverseClient.GetInstance().Tick += new MetaverseClient.TickHandler(EntryPoint_Tick);
            MetaverseClient.GetInstance().Init(args, ClientControllers.Instance);
		}
		
		private void EntryPoint_Tick()
        {
            MetaverseServer.GetInstance().OnTick();
        }
	}
}
