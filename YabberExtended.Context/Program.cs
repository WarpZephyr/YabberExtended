using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;

namespace YabberExtended.Context
{
    class Program
    {
        static void Main(string[] args)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Console.Write(
                $"{assembly.GetName().Name} {assembly.GetName().Version}\n\n" +
                "This program will register YabberExtended.exe and YabberExtended.DCX.exe\n" +
                "so that they can be run by right-clicking on a file or folder.\n" +
                "Enter R to register, U to unregister, or anything else to exit.\n" +
                "> ");
            string choice = Console.ReadLine().ToUpper();
            Console.WriteLine();

            if (choice == "R" || choice == "U")
            {
                try
                {
                    RegistryKey classes = Registry.CurrentUser.OpenSubKey("Software\\Classes", true);
                    if (choice == "R")
                    {
                        string yabberPath = Path.GetFullPath("YabberExtended.exe");
                        RegistryKey yabberFileKey = classes.CreateSubKey("*\\shell\\yabberextended");
                        RegistryKey yabberFileCommand = yabberFileKey.CreateSubKey("command");
                        yabberFileKey.SetValue(null, "YabberExtended");
                        yabberFileCommand.SetValue(null, $"\"{yabberPath}\" \"%1\"");
                        RegistryKey yabberDirKey = classes.CreateSubKey("directory\\shell\\yabberextended");
                        RegistryKey yabberDirCommand = yabberDirKey.CreateSubKey("command");
                        yabberDirKey.SetValue(null, "YabberExtended");
                        yabberDirCommand.SetValue(null, $"\"{yabberPath}\" \"%1\"");

                        string dcxPath = Path.GetFullPath("YabberExtended.DCX.exe");
                        RegistryKey dcxFileKey = classes.CreateSubKey("*\\shell\\yabberextendeddcx");
                        RegistryKey dcxFileCommand = dcxFileKey.CreateSubKey("command");
                        dcxFileKey.SetValue(null, "YabberExtended.DCX");
                        dcxFileCommand.SetValue(null, $"\"{dcxPath}\" \"%1\"");

                        Console.WriteLine("Programs registered!");
                    }
                    else if (choice == "U")
                    {
                        classes.DeleteSubKeyTree("*\\shell\\yabberextended", false);
                        classes.DeleteSubKeyTree("directory\\shell\\yabberextended", false);
                        classes.DeleteSubKeyTree("*\\shell\\yabberextendeddcx", false);
                        Console.WriteLine("Programs unregistered.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Operation failed; try running As Administrator. Reason:\n{ex}");
                }

                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
        }
    }
}
