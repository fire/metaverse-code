/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 12/28/2006
 * Time: 11:44 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace Metaverse.Common.Controller
{
	/// <summary>
	/// Description of IServerController.
	/// </summary>
	public interface IServerControllers
	{
		IServerController Server { get; }
		IPluginController Plugin { get; }
	}
}
