using System;
using System.IO.Ports;
using System.Runtime.InteropServices;

class Program
{

    [DllImport("user32.dll", SetLastError = true)]
    static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
    static int counter = 0;

    private const int KEYEVENTF_KEYDOWN = 0x0000;
    private const int KEYEVENTF_KEYUP = 0x0002;

    // Virtuelle Key-Codes (VK_*)
    private const byte VK_A = 0x41;
    private const byte VK_S = 0x53;
    private const byte VK_D = 0x44;

    static void PressKey(byte keyCode)
    {
        keybd_event(keyCode, 0, KEYEVENTF_KEYDOWN, 0);
        keybd_event(keyCode, 0, KEYEVENTF_KEYUP, 0);
    }

    static void Main()
    {
        Console.WriteLine("Gib hier den COM Port vom Arduino ein z.B. COM3");
        string arduPort = Console.ReadLine();

        SerialPort serialPort = new SerialPort("COM3", 9600);
        serialPort.ReadTimeout = 1000;
        serialPort.Open();

        Console.WriteLine("Starte... warte auf Daten von Arduino:");

        while (true)
        {
            try
            {
                string line = serialPort.ReadLine().Trim();

                if (line == "A")
                {
                    PressKey(VK_A);
                    counter++;
                    //Console.WriteLine($"A empfangen! Zähler = {counter}");
                }
                else if (line == "S")
                {
                    PressKey(VK_S);
                    counter++;
                    //Console.WriteLine($"S empfangen! Zähler = {counter}");
                }
                else if (line == "D")
                {
                    PressKey(VK_D);
                    counter++;
                    //Console.WriteLine($"D empfangen! Zähler = {counter}");
                }
            }
            catch (TimeoutException) { }
        }
    }
}
