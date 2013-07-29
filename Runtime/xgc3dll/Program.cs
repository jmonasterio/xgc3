using System;
using System.IO;

using xgc3.RuntimeEnv;
using xgc3.GameObjects;

namespace xgc3
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            string xgcFile;
            if (args.Length < 1)
            {
                xgcFile = "GameEditor.xgc";
                //throw new Exception( @"Command line argument should be compiled dll game file like: C:\src\xgc3\xgc3\GameEditor.xgc.dll");
            }
            else
            {
                xgcFile = args[0];
            }

            GameManager gm = new GameManager();

            using (XnaGame game = new XnaGame(gm))
            {
                gm.Game = game;

                // Create the symbol table. IGNORE the cscript generation... because the script was already
                //  compiled to a DLL by the XGCC compiler.
                Compiler compiler = new Compiler();
                compiler.OpenXml(new FileInfo(xgcFile));
                compiler.BuildSymbolTable(gm);

                // Read in XML and flatten out any includes.
                Loader loader = new Loader();
                loader.OpenXml(new FileInfo(xgcFile));

                System.Diagnostics.Debug.WriteLine("About to call LoadInstances");

                loader.LoadInstances(gm);

                // This will create first room and all instances it needs.
                //compiler.Run(compiler);
                game.GotoRoom( gm.RunningObjectTable.GetInstance("Editor") as Room, gm); // TODO: First room?

                game.Run();

            }
        }
    }
}

