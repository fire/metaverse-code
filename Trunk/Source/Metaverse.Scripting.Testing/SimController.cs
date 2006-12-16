/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 12/14/2006
 * Time: 12:29 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections;

namespace Metaverse.Common
{
	/// <summary>
	/// Description of SimController.
	/// </summary>
	public class SimController
	{
		private static SimController _singleton = null;
		
		public static SimController Singleton {
			get 
			{
				lock( _singleton ) {
					if( _singleton == null ) {
						_singleton = new SimController();
					}
				}
				return _singleton;
			}
		}
		
		public ISim GetScriptGenericSimInterface( IScript script ) {
			
			ArrayList simulators = MetaverseController.Singleton.GetSimulators();
			
			ISim sim = (ISim)simulators[0];
			
			if( sim == null )  {
				throw new Exception( "Simulator is not attached" );
			}
						
			return sim;
		}
		
		
		public ArrayList GetSimulatorScripts( ISim simulator ) 
		{
			return MetaverseController.Singleton.GetSimServer( simulator ).GetScripts();
		}
		
		public void InsertScript( ISim simulator, IScript script )
		{
			MetaverseController.Singleton.GetSimServer( simulator ).InsertScript( script );
		}
		
		public WorldModel GetSimulatorWorldModel( ISim simulator ) {
			return MetaverseController.Singleton.GetSimServer( simulator ).GetWorldModel( simulator );
		}
	}
}
