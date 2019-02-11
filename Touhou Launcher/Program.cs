using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Touhou_Launcher
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            Application.Run(new MainForm());
        }

        static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.Contains("Newtonsoft.Json"))
            {
                using (var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Touhou_Launcher.jsonnet.Newtonsoft.Json.dll"))
                {
                    try
                    {
                        var assemblyData = new Byte[stream.Length];
                        stream.Read(assemblyData, 0, assemblyData.Length);
                        return System.Reflection.Assembly.Load(assemblyData);
                    }
                    catch (System.IO.IOException)
                    {
                        return null;
                    }
                    catch (BadImageFormatException)
                    {
                        return null;
                    }
                }
            }
            else
                return null;
        }
    }
}
