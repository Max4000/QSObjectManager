using System;

namespace UtilClasses.ProgramOptionsClasses
{
    public class ProgramOptionsEventArgs : EventArgs
    {

        public readonly ProgramOptions ProgramOptions;

        //Конструкторы
        public ProgramOptionsEventArgs(ProgramOptions record)
        {
            ProgramOptions = record;
        }
    }

    public interface IProgramOptionsEvent
    {
        event ProgramOptionsHandler NewProgramOptionsSend;
    }

    public delegate void ProgramOptionsHandler(object sender, ProgramOptionsEventArgs e);
}