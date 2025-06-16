using BLL;
using DAL;
using System.Windows;
using System.Windows.Controls;
using UI.Views;

namespace UI
{
    /**
     * @class DetalhesTicket
     * @brief Janela que exibe os detalhes de um ticket específico e permite alterar o estado do atendimento.
     */
    public partial class DetalhesTicket : Window
    {
        /** Referência ao ticket que será exibido e editado */
        private Ticket _ticket;

        /**
         * @brief Construtor que inicializa a janela com o ticket informado.
         * 
         * @param ticket Ticket a ser exibido e editado.
         */
        public DetalhesTicket(Ticket ticket)
        {
            InitializeComponent(); ///< Inicializa componentes da interface

            _ticket = ticket; ///< Armazena o ticket recebido
            DataContext = _ticket; ///< Define contexto de dados para data binding no XAML

            // Habilita/desabilita o ComboBox de estado de atendimento dependendo do estado do ticket
            cmbEstadoAtendimento.IsEnabled = _ticket.EstadoTicket == "atendido";

            // Inicializa o ComboBox com o estado atual do atendimento do ticket
            if (!string.IsNullOrEmpty(_ticket.EstadoAtendimento))
            {
                if (_ticket.EstadoAtendimento == "resolvido")
                    cmbEstadoAtendimento.SelectedIndex = 0; ///< Seleciona "Sim"
                else if (_ticket.EstadoAtendimento == "naoResolvido")
                    cmbEstadoAtendimento.SelectedIndex = 1; ///< Seleciona "Não"
                else
                    cmbEstadoAtendimento.SelectedIndex = -1; ///< Nenhuma seleção
            }
            else
            {
                cmbEstadoAtendimento.SelectedIndex = -1; ///< Nenhuma seleção se vazio ou nulo
            }
        }

        /**
         * @brief Evento acionado ao clicar no botão "Salvar", atualiza o estado do ticket.
         * 
         * @param sender Objeto que disparou o evento.
         * @param e Argumentos do evento.
         */
        private void Salvar_Click(object sender, RoutedEventArgs e)
        {
            // Verifica o item selecionado no ComboBox e atualiza o estado do atendimento no ticket
            if (cmbEstadoAtendimento.SelectedItem is ComboBoxItem item)
            {
                string escolha = item.Content.ToString();

                if (escolha == "Sim")
                    _ticket.EstadoAtendimento = "resolvido";
                else if (escolha == "Não")
                    _ticket.EstadoAtendimento = "naoResolvido";
            }

            // Atualiza o ticket no banco de dados
            TicketDAL.AtualizarTicket(_ticket);

            // Mostra mensagem de confirmação
            MessageBox.Show("Ticket atualizado com sucesso.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

            // Fecha a janela
            this.Close();
        }
    }
}
