using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using xgc3.RuntimeEnv;


namespace xgcc
{
    public class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            string xgcFile;
            if (args.Length < 1)
            {
                throw new Exception(@"Command line argument should be xgc file like: C:\src\xgc3\xgc3\GameEditor.xgc");
            }
            else
            {
                xgcFile = args[0];
            }

            GameManager gm = new GameManager();

            Compiler compiler = new Compiler();
            compiler.OpenXml(new FileInfo(xgcFile));
            
            // IN MEMORY...compiler.CompileClasses(gm );
            // TO DLL
            try
            {
                compiler.CompileClassesToDll(gm, xgcFile + ".dll", true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(1);
                return;
            }

            Console.WriteLine("Compilation successful.");
            Environment.Exit(0);
            return;
        }
    }
}

