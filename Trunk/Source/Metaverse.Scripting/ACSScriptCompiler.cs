using System;

namespace Metaverse.Scripting 
{
	public abstract class ACSScriptCompiler : IScriptCompiler 
	{
		public abstract IScriptGenerator Compile( IScriptPackage package );
	}
}
