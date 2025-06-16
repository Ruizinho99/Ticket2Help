using System.Windows.Input;
using System;

using System;
using System.Windows.Input;

namespace UI.Helpers
{
    /// <summary>
    /// Implementação de um comando genérico que pode ser usado para associar ações a elementos de UI.
    /// </summary>
    public class RelayCommand : ICommand
    {
        /// <summary>
        /// Ação a ser executada quando o comando for invocado.
        /// </summary>
        private readonly Action<object> _executar;

        /// <summary>
        /// Função que determina se o comando pode ser executado.
        /// </summary>
        private readonly Predicate<object> _podeExecutar;

        /// <summary>
        /// Construtor do RelayCommand.
        /// </summary>
        /// <param name="executar">Ação a executar quando o comando for chamado.</param>
        /// <param name="podeExecutar">Função que avalia se o comando pode ser executado. Pode ser null.</param>
        /// <exception cref="ArgumentNullException">Lançado se a ação de execução for null.</exception>
        public RelayCommand(Action<object> executar, Predicate<object> podeExecutar = null)
        {
            _executar = executar ?? throw new ArgumentNullException(nameof(executar));
            _podeExecutar = podeExecutar;
        }

        /// <summary>
        /// Determina se o comando pode ser executado.
        /// </summary>
        /// <param name="parameter">Parâmetro passado para o comando.</param>
        /// <returns>True se o comando puder ser executado; caso contrário, false.</returns>
        public bool CanExecute(object parameter) => _podeExecutar == null || _podeExecutar(parameter);

        /// <summary>
        /// Executa a ação associada ao comando.
        /// </summary>
        /// <param name="parameter">Parâmetro passado para o comando.</param>
        public void Execute(object parameter) => _executar(parameter);

        /// <summary>
        /// Evento que é disparado quando o estado de execução do comando muda.
        /// Este evento está ligado ao CommandManager do WPF para atualização automática.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
