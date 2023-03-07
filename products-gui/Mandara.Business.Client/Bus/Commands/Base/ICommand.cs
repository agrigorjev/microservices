namespace Mandara.Business.Bus.Commands.Base
{
    public interface ICommand
    {
        CommandManager CommandManager { get; set; }

        void Execute();
    }
}