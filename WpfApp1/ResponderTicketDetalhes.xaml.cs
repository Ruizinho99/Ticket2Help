using BLL;
using DAL;
using System;
using System.Windows;
using System.Windows.Controls;

namespace UI.Views
{
    /**
     * @class ResponderTicketDetalhes
     * @brief Janela para exibir detalhes de um ticket e permitir responder e alterar seu estado.
     */
    public partial class ResponderTicketDetalhes : Window
    {
        private Ticket _ticket;           /**< Ticket que será visualizado e editado */
        private Utilizador _tecnicoLogado; /**< Técnico que está respondendo o ticket */

        /**
         * @brief Construtor da janela ResponderTicketDetalhes.
         * Inicializa a interface e configura o ticket e técnico logado.
         * Seleciona o estado atual do ticket no ComboBox.
         * 
         * @param ticket Ticket a ser exibido e modificado.
         * @param tecnicoLogado Técnico que está respondendo ao ticket.
         */
        public ResponderTicketDetalhes(Ticket ticket, Utilizador tecnicoLogado)
        {
            InitializeComponent();
            _ticket = ticket;
            _tecnicoLogado = tecnicoLogado;

            // Seleciona o estado atual no ComboBox
            foreach (ComboBoxItem item in cmbEstadoTicket.Items)
            {
                if ((string)item.Content == _ticket.EstadoTicket)
                {
                    cmbEstadoTicket.SelectedItem = item;
                    break;
                }
            }
        }

        /**
         * @brief Evento disparado ao clicar no botão "Guardar".
         * Valida a resposta se o estado for "atendido" e atualiza o ticket no banco.
         * Define datas de atendimento/conclusão conforme o estado selecionado.
         * Fecha a janela após salvar.
         * 
         * @param sender Objeto que disparou o evento.
         * @param e Argumentos do evento.
         */
        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            string estadoSelecionado = (cmbEstadoTicket.SelectedItem as ComboBoxItem)?.Content.ToString();
            string resposta = txtResposta.Text.Trim();

            // Se o estado for "atendido", a resposta é obrigatória
            if (estadoSelecionado == "atendido" && string.IsNullOrWhiteSpace(resposta))
            {
                MessageBox.Show("Por favor, insira uma resposta antes de guardar.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _ticket.EstadoTicket = estadoSelecionado;
            _ticket.DetalhesTecnico = resposta;

            if (estadoSelecionado == "porAtender")
            {
                // Não altera datas
            }
            else if (estadoSelecionado == "emAtendimento")
            {
                _ticket.DataAtendimento = DateTime.Now;
                _ticket.DataConclusao = null;  // reseta caso tenha tido valor
                _ticket.RespondidoPor = _tecnicoLogado.Id;
            }
            else if (estadoSelecionado == "atendido")
            {
                if (!_ticket.DataAtendimento.HasValue)
                {
                    // Caso não tenha sido setada ainda a data de atendimento, setar agora
                    _ticket.DataAtendimento = DateTime.Now;
                }
                _ticket.DataConclusao = DateTime.Now;
                _ticket.RespondidoPor = _tecnicoLogado.Id;
            }

            TicketDAL.ResponderTicket(_ticket);

            MessageBox.Show("Resposta enviada com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        /**
         * @brief Evento disparado ao clicar no botão "Cancelar".
         * Fecha a janela sem salvar alterações.
         * 
         * @param sender Objeto que disparou o evento.
         * @param e Argumentos do evento.
         */
        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
