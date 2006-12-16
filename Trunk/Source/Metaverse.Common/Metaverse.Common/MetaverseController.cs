/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 12/13/2006
 * Time: 11:58 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections;

namespace Metaverse.Common
{
	/// <summary>
	/// Description of ServerController.
	/// </summary>
	public class MetaverseController
	{
		private static MetaverseController _singleton = null;
		private static ISim _simulator;
		private static IMetaverseServer _server;
		
		public static MetaverseController Singleton {
			get 
			{
				lock( _singleton ) {
					if( _singleton == null ) {
						_singleton = new MetaverseController();
					}
				}
				return _singleton;
			}
		}
		
		private MetaverseController() {
			
		}
		
		public void RegisterMetaverseServer( IMetaverseServer server ) {
			
			
			_server = server;
			
			return;
		}
		
		public IMetaverseServer GetSimServer( ISim simulator ) {
			
			return _server;
		}
		
		public void AttachSimulator( ISim simulator ) {
			_simulator = simulator;
		}
		
		public ArrayList GetSimulators() {
			return new ArrayList( new ISim[]{ _simulator } );
		}
	}
}
