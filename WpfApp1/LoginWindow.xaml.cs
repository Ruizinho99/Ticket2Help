using BLL;
using System;
using System.Windows;
using DAL;
using UI.Views;

namespace UI
{
    /**
     * @class LoginWindow
     * @brief Janela para autenticação de utilizadores na aplicação.
     */
    public partial class LoginWindow : Window
    {
        /**
         * @brief Construtor da janela de login.
         * Inicializa os componentes e exibe exemplos de utilizadores.
         */
        public LoginWindow()
        {
            InitializeComponent();
            MostrarUtilizadorExemplo();
        }

        /**
         * @brief Exibe exemplos de utilizadores e suas credenciais para teste.
         * Busca exemplos na base de dados e mostra no controle de texto.
         */
        private void MostrarUtilizadorExemplo()
        {
            try
            {
                var exemplos = UtilizadorDAL.GetUtilizadoresExemplo();

                if (exemplos.itDesk != null || exemplos.funcionario != null)
                {
                    string texto = "Exemplos:\n";

                    if (exemplos.itDesk != null)
                        texto += $"[IT_DESK] Username: {exemplos.itDesk.Username}, Password: {exemplos.itDesk.Password}\n";

                    if (exemplos.funcionario != null)
                        texto += $"[FUNCIONARIO] Username: {exemplos.funcionario.Username}, Password: {exemplos.funcionario.Password}";

                    txtCredenciaisExemplo.Text = texto;
                }
                else
                {
                    txtCredenciaisExemplo.Text = "Sem utilizadores na base de dados.";
                }
            }
            catch (Exception ex)
            {
                txtCredenciaisExemplo.Text = $"Erro ao carregar utilizadores: {ex.Message}";
            }
        }

        /**
         * @brief Evento acionado ao clicar no botão "Entrar".
         * Realiza validação das credenciais e efetua login do utilizador.
         * 
         * @param sender Objeto que disparou o evento.
         * @param e Argumentos do evento.
         */
        private void BtnEntrar_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;

            // Verifica se campos foram preenchidos
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Por favor, preencha o Username e Password.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Tenta efetuar login usando o DAL
            Utilizador user = UtilizadorDAL.Login(username, password);

            if (user != null)
            {
                MessageBox.Show("Login bem sucedido!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                // Redireciona para a janela apropriada conforme o tipo de utilizador
                if (user.Tipo == "IT_DESK")
                {
                    ResponderTickets janela = new ResponderTickets(user);
                    janela.Show();
                }
                else
                {
                    UserTickets janela = new UserTickets(user);
                    janela.Show();
                }

                this.Close(); // Fecha janela de login
            }
            else
            {
                MessageBox.Show("Credenciais inválidas.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}




