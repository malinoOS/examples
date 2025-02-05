using libmalino;
using System;
using System.Threading;

namespace doomOScs;

public class Program {
	public static void Main(string[] args) {
		string stage = "starting";
		try {
			malinoIO.ClearScreen();
			Console.WriteLine($"doomOScs v{MalinoAutoGenerated.OSVersion} - malino example");
			
			// mount /proc
			stage = "mounting /proc";
			malino.MountProcFS();

			// mount /dev
			stage = "mounting /dev";
			malino.MountDevFS();

			// start fbdoom
			stage = "running DOOM";
			malino.SpawnProcess("/bin/fbdoom", "/", [], true, ["-iwad", "DOOM.WAD"]);
		} catch (Exception e) {
			Console.WriteLine("\n--- doomOScs \x1b[91mPANIC!\x1b[39m ---");
			Console.WriteLine(e.Message);
			Console.WriteLine("This happened while " + stage);
			Console.WriteLine("\nThe system is halted.");
			while (true)
				Thread.Sleep(3600000);
		}
	}
}