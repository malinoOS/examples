using libmalino;
using System;
using System.IO;

namespace mouse;

public class Program {
	public static void Main(string[] args) {
		Console.WriteLine("malino (project mouse v"+MalinoAutoGenerated.OSVersion+") booted successfully. Type a line of text to get it echoed back.");
		try {
			malino.MountDevFS();
			Console.WriteLine("Mounted /dev!");
			malino.MountProcFS();
			Console.WriteLine("Mounted /proc!");

			malino.LoadAllKernelModules();

			PrintMouseCoordinates();
			
			malino.ShutdownComputer();
		} catch(Exception ex) {
			Console.WriteLine("Exception: " + ex.Message);
			MsbBindings.Reboot((uint)LINUX_REBOOT.CMD_HALT);
		}
	}

	static void PrintMouseCoordinates()
    {
        const string mouseDevice = "/dev/input/mice";

        try
        {
            using (FileStream fs = new FileStream(mouseDevice, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[3];
                int x = 0, y = 0;

                while (true)
                {
                    int bytesRead = fs.Read(buffer, 0, buffer.Length);

                    if (bytesRead == buffer.Length)
                    {
                        int leftButton = (buffer[0] & 0x1) > 0 ? 1 : 0;
                        int rightButton = (buffer[0] & 0x2) > 0 ? 1 : 0;
                        int middleButton = (buffer[0] & 0x4) > 0 ? 1 : 0;
                        
                        int xMovement = (buffer[1] > 127 ? buffer[1] - 256 : buffer[1]);
                        int yMovement = (buffer[2] > 127 ? buffer[2] - 256 : buffer[2]);

                        // Update the coordinates
                        x = Math.Max(0, x + xMovement);
                        y = Math.Max(0, y - yMovement);


			if (x > 160) x=160;
			if (y > 50) y=50;

			malinoIO.ClearScreen();

			Console.Write($"\x1b[{y};{x}H");
			Console.Write("\x1b[107m \x1b[40m\x1b[H");

                        Console.WriteLine($"X: {x}, Y: {y}, Left: {leftButton}, Right: {rightButton}, Middle: {middleButton}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}
