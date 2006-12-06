// Copyright Hugh Perkins 2006
// hughperkins@gmail.com http://manageddreams.com
//
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General Public License version 2 as published by the
// Free Software Foundation;
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
//  more details.
//
// You should have received a copy of the GNU General Public License along
// with this program in the file licence.txt; if not, write to the
// Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-
// 1307 USA
// You can find the licence also on the web at:
// http://www.opensource.org/licenses/gpl-license.php
//

using System;
using System.Collections;
using System.Xml;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("csscript")]
[assembly: AssemblyDescription("C# Script")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Hugh Perkins, hughperkins@gmail.com")]
[assembly: AssemblyProduct("cscript")]
[assembly: AssemblyCopyright("Copyright Hugh Perkins 2006, source-code available under GPL licence")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyVersion("0.11.*")]

class Info
{
    public const string Version = "0.11";
}

class Arguments
{
	public StringHashtable Named;
	public StringArrayList Unnamed;
	
	public Arguments( string[] args )
	{
		Named = new StringHashtable();
		Unnamed = new StringArrayList();
		
		int i = 0; 
		while( i <= args.GetUpperBound(0) )
		{
			if( args[i][0] == '-' )
			{
				if( args[i][1] == '-' )
				{
					Named[ args[i].Substring( 2 ) ] = args[i].Substring( 2 );
					i += 0;
				}
				else
				{
					Named[ args[i].Substring( 1 ) ] = args[ i + 1 ];
					i += 1;
				}
			}
			else
			{
				Unnamed.Add( args[i] );
			}
			i++;
		}
	}
}

class StringArrayList : ArrayList{
	public new virtual string this[ int index ]{get{return (string)base[ index ];}set{base[ index ] = value;}}
	public class StringEnumerator{
		IEnumerator enumerator;
		public StringEnumerator( IEnumerator enumerator ){this.enumerator = enumerator;}
		public string Current{get{return (string)enumerator.Current;}}
		public void MoveNext(){enumerator.MoveNext();}
		public void Reset(){enumerator.Reset();}
	}
	public new StringEnumerator GetEnumerator()
	{
		return new StringEnumerator( base.GetEnumerator() );
	}
}

class StringHashtable : Hashtable{
	public virtual string this[ string index ]{get{return (string)base[ index ];}set{base[ index ] = value;}}
}

class Help
{
	public void DisplayUsage()
	{
		DisplayUsage2();
		System.Environment.Exit(0);
	}
	public void DisplayUsage2()
	{
		Console.WriteLine( "Quick and Easy");
		Console.WriteLine( "==============");
		Console.WriteLine("" );
		Console.WriteLine( "Usage 1: lescript [--nologo] <C# sourcefilepath>");
		Console.WriteLine("" );
		Console.WriteLine("Easiest usage is with no options and a single .cs file." );
		Console.WriteLine("This will run the .cs file, after compiling it to Temp.  The temporary" );
		Console.WriteLine("executable file is cleaned up after use." );
		Console.WriteLine("" );
		Console.WriteLine("Intermediate" );
		Console.WriteLine( "============");
		Console.WriteLine("" );
		Console.WriteLine( "Usage 2: lescript [--nologo] [--buildonly|--runoptimal] <XML config file>");
		Console.WriteLine("" );
		Console.WriteLine("More powerful is to provide an xml config file. This lets you build multiple" );
		Console.WriteLine(".cs files together. In the xml config file, you need to specify at least one" );
		Console.WriteLine("file, with a name.  The package directory is prepended to the filenames." );
		Console.WriteLine("" );
		Console.WriteLine("Minimal config file:" );
		Console.WriteLine("" );
			Console.WriteLine("<root>");
Console.WriteLine("  <projects>");
Console.WriteLine("    <project>");
Console.WriteLine("      <packages>");
Console.WriteLine("        <package directory=\"g:\\dev\">");
Console.WriteLine("          <files>");
Console.WriteLine("            <file name=\"SourceFile1.cs\"/>");
Console.WriteLine("            ... as many files as you like...");
Console.WriteLine("          </files>");
Console.WriteLine("        </package>");
Console.WriteLine("        ... as many packages as you like...");
Console.WriteLine("      </packages>");
Console.WriteLine("    </project>");
Console.WriteLine("  </projects>");
Console.WriteLine("</root>");
		Console.WriteLine("" );
		Console.WriteLine("By default, this will build to temp, and clean-up after use." );
		Console.WriteLine("" );
		Console.WriteLine("If you specify the option --runoptimal, the executable will be created with the" );
		Console.WriteLine("filename specified by the attribute target in the project xml node:" );
		Console.WriteLine("" );
		Console.WriteLine("       <project target=\"g:\\dev\\myexe.exe\">");
		Console.WriteLine("" );
		Console.WriteLine("Specifying the option --buildonly will create the target executable, without" );
		Console.WriteLine("running it." );
		Console.WriteLine("" );
		Console.WriteLine("Advanced" );
		Console.WriteLine("========" );
		Console.WriteLine("" );
		Console.WriteLine( "Usage 3: lescript [--nologo] --generatemergefile <XML config file>");
		Console.WriteLine("" );
		Console.WriteLine("Lastly (advanced usage), you can use LeScript to merge many .cs files into a" );
		Console.WriteLine("single .cs file.  You'll want to specify a mergefilename for each package, and");
		Console.WriteLine("you probably want to add at least the following using and sed nodes:" );
		Console.WriteLine("" );
		Console.WriteLine("  <packages>");
		Console.WriteLine("    <package directory=\"g:\\dev\" mergefilename=\"mergefile.cs\">");
		Console.WriteLine("      <usings>");
		Console.WriteLine("        <using target=\"System\" />");
		Console.WriteLine("        <using target=\"System.Collections\" />");
		Console.WriteLine("      </usings>");
		Console.WriteLine("      <sed>");
		Console.WriteLine("        <remove startswith=\"using\" />");
		Console.WriteLine("      </sed>");
		Console.WriteLine("" );
		Console.WriteLine("The mergefilename attribute is the name of the merged file that will be " );
		Console.WriteLine("generated. The using nodes specify using commands that are appended to the head" );
		Console.WriteLine("of the merged file. The sed section allows you to comment out lines from the" );
		Console.WriteLine("source files starting with using.  This eliminates warnings where the same" );
		Console.WriteLine("using statements are present multiple times" );
	}
	public void DisplayUsage1()
	{
			Console.WriteLine( "usage: buildsubmission <configfilename>");
			Console.WriteLine("");
			Console.WriteLine("config file format:");
			Console.WriteLine("");
			Console.WriteLine("<root>");
Console.WriteLine("<projects>");
Console.WriteLine("");
Console.WriteLine("<project directory=\"c:\\mydirectory\" outfilename=\"_outfilegen1.cs\">");
Console.WriteLine("<usings>");
Console.WriteLine("<using target=\"System\" />");
Console.WriteLine("<using target=\"System.Collections\" />");
Console.WriteLine("<using target=\"System.Collections.Specialized\" />");
Console.WriteLine("<using target=\"System.Text.RegularExpressions\" />");
Console.WriteLine("</usings>");
Console.WriteLine("<files>");
Console.WriteLine("<file name=\"Module3.cs\"/>");
Console.WriteLine("<file name=\"Module4.cs\"/>");
Console.WriteLine("</files>");
Console.WriteLine("</project>");
Console.WriteLine("");
Console.WriteLine("<project directory=\"c:\\mydirectory\" outfilename=\"_outfilegen2.cs\">");
Console.WriteLine("<usings>");
Console.WriteLine("<using target=\"System\" />");
Console.WriteLine("<using target=\"System.Collections\" />");
Console.WriteLine("<using target=\"System.Collections.Specialized\" />");
Console.WriteLine("<using target=\"System.Text.RegularExpressions\" />");
Console.WriteLine("</usings>");
Console.WriteLine("<files>");
Console.WriteLine("<file name=\"Module1.cs\"/>");
Console.WriteLine("<file name=\"Module2.cs\"/>");
Console.WriteLine("</files>");
Console.WriteLine("</project>");
Console.WriteLine("");
Console.WriteLine("</projects>");
Console.WriteLine("</root>");
			Test.Exit();
	}
}

class Test
{
	public static void Exit()
	{
		System.Environment.Exit(0);
	}
}

public class XmlHelper
{
	public static XmlDocument CreateDOM()
	{
		XmlDocument newdoc = new XmlDocument();
		newdoc.PreserveWhitespace = true;
		return newdoc;
	}
	public static XmlDocument OpenDOM( string sfilepath )
	{
		XmlDocument newdoc = CreateDOM();
		newdoc.Load( sfilepath );
		return newdoc;
	}
}

public delegate bool CtrlHandlerCallback( int fdwCtrlType );

class SubmissionBuilder
{
	string ReadFile( string sfilename )
	{
		string contents;
		FileStream infilestream = File.OpenRead( sfilename );
		StreamReader instreamreader = new StreamReader( infilestream );
		contents = instreamreader.ReadToEnd();
		instreamreader.Close();
		infilestream.Close();
		return contents;
	}
	
	public void RunScript( string sConfigFilename, string arguments )
	{
		RunXML( true, sConfigFilename, arguments );
	}
	
	public void RunOptimal( string sConfigFilename, string arguments )
	{
		RunXML( false, sConfigFilename, arguments );
	}
	
	[DllImport("Kernel32.dll")]public static extern bool SetConsoleCtrlHandler(CtrlHandlerCallback HandlerRoutine, bool add );
	public static bool CtrlHandler( int fdwCtrlType ) 
	{
		//Console.WriteLine("Cleaning...");
		if( processtokill != null )
		{
			try{
				processtokill.Kill();
				Thread.Sleep(500);
				if( bDeleteFiles )
				{
					File.Delete( sfiletodelete );
					File.Delete( sfiletodelete.Substring( 0, sfiletodelete.Length - 4 ) + ".PDB" );	
				}
			}
			catch( Exception e )
			{
				Console.WriteLine( e );
			}
			//Console.WriteLine("Cleaned up running process");
		}
		return false;
	}
	
	static Process processtokill;
	static bool bDeleteFiles;
	static string sfiletodelete;
	
	public void RunXML( bool bTempOutfile, string sConfigFilename, string arguments )
	{
		string stargetfilename = Build( bTempOutfile, sConfigFilename );
		if( stargetfilename != "" && File.Exists( stargetfilename ) )
		{
            Console.WriteLine("Running...");
            
			ProcessStartInfo processstartinfo = new ProcessStartInfo();
			processstartinfo.FileName = stargetfilename;
			processstartinfo.UseShellExecute = false;
			processstartinfo.CreateNoWindow = true;
			processstartinfo.RedirectStandardOutput = true;
			processstartinfo.Arguments = arguments;
		
			CtrlHandlerCallback mycallback = new CtrlHandlerCallback( SubmissionBuilder.CtrlHandler );
			SetConsoleCtrlHandler( mycallback, true );

			if( bTempOutfile )
			{
				bDeleteFiles = true;
				sfiletodelete = stargetfilename;
			}
			processtokill = Process.Start(processstartinfo);
			string soutput = processtokill.StandardOutput.ReadLine();
			while( soutput != null )
			{
				Console.WriteLine( soutput );
				soutput = processtokill.StandardOutput.ReadLine();
			}
			processtokill.WaitForExit();
			Thread.Sleep(500);
			if( bTempOutfile )
			{
				File.Delete( stargetfilename );
				File.Delete( stargetfilename.Substring( 0, stargetfilename.Length - 4 ) + ".PDB" );				
			}
			processtokill = null;
		}
	}
	
	public string Build( bool bTempOutfile, string sConfigFilename )
	{
		if( !File.Exists( sConfigFilename ) )
		{
			Console.WriteLine( "file " + sConfigFilename + " does not exist.");
			Test.Exit();
		}
		
		XmlDocument configfile = XmlHelper.OpenDOM( sConfigFilename );
		StringArrayList files = new StringArrayList();		
		XmlElement projectselement = (XmlElement)configfile.SelectSingleNode("/root/projects/project");
		
		string stargetfilename;
		if( bTempOutfile )
		{
			string stempfullpath = Path.GetTempFileName();
            stargetfilename = Path.GetDirectoryName( stempfullpath ) + "\\" + Path.GetFileNameWithoutExtension( sConfigFilename ) +
                Path.GetFileName( stempfullpath );
		}
		else
		{
			stargetfilename = projectselement.GetAttribute("target");
		}
		
		StringArrayList assemblies = new StringArrayList();
		foreach( XmlElement assemblyelement in projectselement.SelectNodes("assemblies/assembly") )
		{
			assemblies.Add( assemblyelement.GetAttribute("path" ) );
		}

		foreach( XmlElement package in projectselement.SelectNodes("packages/package") )
		{
			Console.WriteLine( package.GetAttribute("outfilename"));
			string sDirectory = package.GetAttribute("directory");
			string soutfilename = package.GetAttribute("outfilename");			
			foreach( XmlElement xmlelement in package.SelectNodes( "files/file") )
			{
				string sfilename = xmlelement.GetAttribute("name");	
				if( File.Exists( sDirectory + "\\" + sfilename ) )
				{
					files.Add( sDirectory + "\\" + sfilename );
					Console.WriteLine( sfilename );
				}
				else
				{
					Console.WriteLine( "Error: file " + sDirectory + "\\" + sfilename + " does not exist.");
					Test.Exit();
				}
			}
			Console.WriteLine();
			//outfilestream.Close();
		}
		string allfilesstring = "";
		foreach( string filestring in files )
		{
			allfilesstring += "\"" + filestring + "\" ";
		}
		
		string assembliesstring = "";
		foreach( string assemblystring in assemblies )
		{
			assembliesstring += "/r:" + assemblystring + " ";
		}
		
		string scscpath = Environment.GetFolderPath( Environment.SpecialFolder.System )  + "\\..\\Microsoft.Net\\Framework\\v1.1.4322\\csc.exe";
		// Console.WriteLine( scscpath );
		
		ProcessStartInfo processstartinfo = new ProcessStartInfo();
		processstartinfo.FileName = scscpath;
		//processstartinfo.FileName = "csc.exe";
		//processstartinfo.FileName = Environment.SystemDirectory + "\\notepad.exe";
		processstartinfo.UseShellExecute = false;
		processstartinfo.CreateNoWindow = true;
		processstartinfo.RedirectStandardOutput = true;
		if( bTempOutfile )
		{
			processstartinfo.Arguments = "/nologo /debug /out:" + stargetfilename + " /target:exe " + assembliesstring + " " + allfilesstring + "";
		}
		else
		{
			processstartinfo.Arguments = "/nologo /debug /incremental+ /out:" + stargetfilename + " /target:exe " + assembliesstring + " " + allfilesstring + "";
		}
		Console.WriteLine( processstartinfo.Arguments );
		//processstartinfo.Arguments = "somefile.txt asdf";
		
		Process process;
		//process = Process.Start(scscpath,"/debug /out:" + soutputfilename + " /target:exe \"" + sFilename + "\"" );
		process = Process.Start( processstartinfo );
		string soutput = process.StandardOutput.ReadToEnd();
		bool bErrors = false;
		if( soutput != "" )
		{
			Console.WriteLine(soutput);
			if( soutput.IndexOf("error") >= 0 )
			{
				bErrors = true;
			}
		}
		process.WaitForExit();
		if( bErrors )
		{
			if( bTempOutfile )
			{
				File.Delete( stargetfilename );
			}
			return "";
		}
        
        if( bTempOutfile )
        {
            string stempdirectory = Path.GetDirectoryName( stargetfilename );
            foreach( string assemblystring in assemblies )
            {
                string stempassemblyfilename = stempdirectory + "\\" + Path.GetFileName( assemblystring );
                if( !File.Exists( stempassemblyfilename ) || File.GetLastWriteTime( stempassemblyfilename ) != File.GetLastWriteTime( stargetfilename ) )
                {
                    File.Copy( assemblystring, stempassemblyfilename, true );
                }
            }
        }
		return stargetfilename;
	}
	
	public void RunScript( string sConfigFilename )
	{
	}
	
	public void Run( string sFilename, string sargs )
	{
		if( !File.Exists( sFilename ) )
		{
			Console.WriteLine( "File " + sFilename + " not found." );
			Console.WriteLine("");
			new Help().DisplayUsage();
		}
		
		string soutputfilename = Path.GetTempFileName();
		
		string scscpath = Environment.GetFolderPath( Environment.SpecialFolder.System )  + "\\..\\Microsoft.Net\\Framework\\v1.1.4322\\csc.exe";
		// Console.WriteLine( scscpath );
		
		ProcessStartInfo processstartinfo = new ProcessStartInfo();
		processstartinfo.FileName = scscpath;
		//processstartinfo.FileName = "csc.exe";
		//processstartinfo.FileName = Environment.SystemDirectory + "\\notepad.exe";
		processstartinfo.UseShellExecute = false;
		processstartinfo.CreateNoWindow = true;
		processstartinfo.RedirectStandardOutput = true;
		processstartinfo.Arguments = sFilename;
		processstartinfo.Arguments = "/nologo /debug /out:" + soutputfilename + " /target:exe \"" + sFilename + "\"";
		//processstartinfo.Arguments = "somefile.txt asdf";
		
		Process process;
		//process = Process.Start(scscpath,"/debug /out:" + soutputfilename + " /target:exe \"" + sFilename + "\"" );
		process = Process.Start( processstartinfo );
		string soutput = process.StandardOutput.ReadToEnd();
		if( soutput != "" )
		{
			Console.WriteLine(soutput);
		}
		process.WaitForExit();
		
		if( File.Exists( soutputfilename ) )
		{
            Console.WriteLine("Running...");
            
			processstartinfo.FileName = soutputfilename;
			processstartinfo.UseShellExecute = false;
			processstartinfo.CreateNoWindow = true;
			processstartinfo.RedirectStandardOutput = true;
			processstartinfo.Arguments = sargs;
			
			CtrlHandlerCallback mycallback = new CtrlHandlerCallback( SubmissionBuilder.CtrlHandler );
			SetConsoleCtrlHandler( mycallback, true );

			bDeleteFiles = true;
			sfiletodelete = soutputfilename;

			processtokill = Process.Start(processstartinfo);
			soutput = processtokill.StandardOutput.ReadLine();
			while( soutput != null )
			{
				Console.WriteLine( soutput );
				soutput = processtokill.StandardOutput.ReadLine();
			}
			processtokill.WaitForExit();
			Thread.Sleep(500);
			File.Delete( soutputfilename );
			File.Delete( soutputfilename.Substring( 0, soutputfilename.Length - 4 ) + ".PDB" );
			processtokill = null;
		}
		else
		{
			// Console.WriteLine("Compilation error: no outputfile created at " + soutputfilename );
		}
	}
	
	//const string sConfigFile = "routepacketsconfig.xml";
	public void GenerateSingleFile( string sConfigFilename )
	{
		if( !File.Exists( sConfigFilename ) )
		{
			Console.WriteLine( "file " + sConfigFilename + " does not exist.");
			Test.Exit();
		}
		
		XmlDocument configfile = XmlHelper.OpenDOM( sConfigFilename );
		
		XmlElement projectselement = (XmlElement)configfile.SelectSingleNode("/root/projects/project");
		//string sprojectsdirectory = projectselement.GetAttribute("directory");
		//string sMakefilefilepath = sprojectsdirectory + "\\" + projectselement.GetAttribute("makefile");
		//string maketarget = projectselement.GetAttribute("target");
		
		//StreamWriter makefilesw = File.CreateText( sMakefilefilepath );
		
		ArrayList files = new ArrayList();
		
		foreach( XmlElement package in projectselement.SelectNodes("packages/package") )
		{
			Console.WriteLine( package.GetAttribute("mergefilename"));
			string sDirectory = package.GetAttribute("directory");
			string soutfilename = package.GetAttribute("mergefilename");
			
			//FileStream outfilestream = File.OpenWrite( sDirectory + "\\" + soutfilename );
			//StreamWriter streamwriter = new StreamWriter( outfilestream );
			
            Console.WriteLine( "Trying to write to file " + sDirectory + "\\" + soutfilename + " ..." );
			StreamWriter streamwriter = File.CreateText( sDirectory + "\\" + soutfilename );
	
			foreach( XmlElement usingelement in package.SelectNodes("usings/using") )
			{
				streamwriter.Write( "using " + usingelement.GetAttribute("target") + ";\n" );
			//streamwriter.Write( "using System.Collections;\n" );
			//streamwriter.Write( "using System.Threading;\n" );
			}
			streamwriter.Write( "\n" );
	
			foreach( XmlElement xmlelement in package.SelectNodes( "files/file") )
			{
				string sfilename = xmlelement.GetAttribute("name");	
				if( File.Exists( sDirectory + "\\" + sfilename ) )
				{
		//			files.Add( sDirectory + "\\" + sfilename );
					Console.WriteLine( sfilename );
					FileStream infilestream = File.OpenRead( sDirectory + "\\" + sfilename );
					StreamReader instreamreader = new StreamReader( infilestream );
					
					streamwriter.Write( "////////////////////////////////////////////////////////////////////////////////////////////////////////\n" );
					streamwriter.Write( "//\n" );
					streamwriter.Write( "// " + sfilename + "\n" );
					streamwriter.Write( "//\n" );
					streamwriter.Write( "////////////////////////////////////////////////////////////////////////////////////////////////////////\n" );
					streamwriter.Write( "\n" );
					streamwriter.Write(  "#line 1 \"" + sfilename + "\"\n" );
					string sline = instreamreader.ReadLine();
					while( sline != null )
					{
						if( sline.StartsWith( "using" ) )
						{
							streamwriter.Write(  "// " + sline + "\n" );
						}
						else
						{
							streamwriter.Write(  sline + "\n" );
						}
						sline = instreamreader.ReadLine();
					}					
					
					instreamreader.Close();
					infilestream.Close();
				}
				else
				{
					Console.WriteLine( "Error: file " + sDirectory + "\\" + sfilename + " does not exist.");
					Test.Exit();
				}
			}
			
			streamwriter.Close();
			Console.WriteLine();
			//outfilestream.Close();
		}
	}
}

class SourceCode
{
	public void DumpSource( string filename )
	{
		if( filename != "" )
		{
			StreamWriter streamwriter = File.CreateText( filename );
			streamwriter.Write( GetSource() );
			streamwriter.Close();
		}
		else
		{
			Console.WriteLine( GetSource() );
		}
	}
	string GetSource()
	{
		string source = "";
		source += "<dummy source>";
		return source;
	}
}

class EntryPoint
{
	public int Go( string[] args )
	{
		if( args.GetUpperBound(0) + 1 == 0 )
		{
			new Help().DisplayUsage();
		}
		
		Arguments arguments = new Arguments( args );
		if( arguments.Unnamed.Count < 1 )
		{
			new Help().DisplayUsage();
		}
		string sfilename = arguments.Unnamed[0];
		string sargs = "";
		for( int i = 1; i < arguments.Unnamed.Count; i++ )
		{
			sargs += arguments.Unnamed[i] + " ";
		}
		
		if( sfilename.ToLower().Substring( sfilename.Length - 3 ) == ".cs" )
		{
			new SubmissionBuilder().Run( sfilename, sargs );
		}
		else if( sfilename.ToLower().Substring( sfilename.Length - 4 ) == ".xml" )
		{
			string argumentsstring = "";
			foreach( string arg in args )
			{
				argumentsstring += arg + " ";
			}
			if( arguments.Named["generatemergefile"] != null )
			{
				new SubmissionBuilder().GenerateSingleFile( sfilename );
			}
			else if( arguments.Named["buildonly"] != null )
			{
				new SubmissionBuilder().Build( false, sfilename );
			}
			else if( arguments.Named["runoptimal"] != null )
			{
				new SubmissionBuilder().RunOptimal( sfilename, argumentsstring );
			}
			else
			{
				new SubmissionBuilder().RunScript( sfilename, argumentsstring );
			}
		}
		
		return 0;
	}
}

class _EntryPoint
{
	public static int Main( string[] args )
	{
		Arguments arguments = new Arguments( args );
		
		StringArrayList OurKeys = new StringArrayList();
		OurKeys.Add("nologo");
		OurKeys.Add("runoptimal");
		OurKeys.Add("config");
		
		if( arguments.Named["nologo"] == null )
		{
			Console.WriteLine( "LeScript C# scripting host v" + Info.Version );
			Console.WriteLine( "Hugh Perkins 2006, hughperkins@gmail.com" );
			Console.WriteLine( "" );
		}

		if( arguments.Named[ "dumpsource" ] != null )
		{
			new SourceCode().DumpSource( arguments.Named["dumpsource"] );
		}
		else if( arguments.Named[ "help" ] != null )
		{
			new Help().DisplayUsage();
		}
		else
		{
			return new EntryPoint().Go( args );
		}
		return 0;
	}	
}

		