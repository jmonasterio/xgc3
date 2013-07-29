using System;
using System.Collections.Generic;
using System.Text;

using xgc3.Core;
using xgc3.Resources;

namespace xgc3.RuntimeEnv
{
    public class GameManager : BaseRuntimeEnvInstance
    {
        public XnaGame Game;
        public GameInfo GameInformation;
        public RunningObjectTable RunningObjectTable;
        public FocusManager FocusManager;
        public SymbolTable SymbolTable;

        public GameManager()
        {
            this.SymbolTable = new SymbolTable();
            this.FocusManager = new FocusManager();
            this.RunningObjectTable = new RunningObjectTable();
        }

        /// <summary>
        /// Load all classes then construct the first room.
        /// </summary>
        /// <param name="compiler"></param>
        public void Run(Loader ldr)
        {

        }
    }
}
