using System;
using System.Windows.Input;

namespace UI.Helpers
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _executar;
        private readonly Predicate<object> _podeExecutar;

        public RelayCommand(Action<object> executar, Predicate<object> podeExecutar = null)
        {
            _executar = executar ?? throw new ArgumentNullException(nameof(executar));
            _podeExecutar = podeExecutar;
        }

        public bool CanExecute(object parameter) => _podeExecutar == null || _podeExecutar(parameter);

        public void Execute(object parameter) => _executar(parameter);

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
