/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 12/28/2006
 * Time: 2:54 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace OSMP
{
	/// <summary>
	/// Description of IDirtyObjectQueue.
	/// </summary>
	public interface IDirtyObjectQueue
	{
		void MarkDeleted(int reference, string typename);
		void MarkAllDirty();
        void MarkDirty(IHasReference targetentity, Type[] dirtytypes);
        void Tick();
	}
}
