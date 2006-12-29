/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 12/28/2006
 * Time: 10:12 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using OSMP;
using System.Threading;
using Metaverse.Common.Controller;

namespace Metaverse.Controller
{
	/// <summary>
	/// Description of MetaverseServerController.
	/// </summary>
	public class ServerController : IServerController
	{
		private static IServerController _instance = null;
		
		public static IServerController Instance {
			get {
				if( _instance == null ) {
					_instance = new ServerController();
				}
				return _instance;
			}
		}
		
		private ServerController() { }
		
		public void Initialize( string[] args ) {
			MetaverseServer.GetInstance().Init(args, ServerControllers.Instance);
            
			while (true)
            {
                MetaverseServer.GetInstance().OnTick();
                Thread.Sleep(50);
            }
		}
		
	}
}
