using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DAL;

namespace UI
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            MostrarUtilizadorExemplo();
        }

        private void MostrarUtilizadorExemplo()
        {
            try
            {
                Utilizador user = UtilizadorDAL.GetPrimeiroUtilizador();

                if (user != null)
                {
                    txtCredenciaisExemplo.Text = $"Exemplo:\nUsername: {user.Username}\nPassword: {user.Id * 1000} (exemplo)";
                }
                else
                {
                    txtCredenciaisExemplo.Text = "Sem utilizadores na base de dados.";
                }
            }
            catch (Exception ex)
            {
                txtCredenciaisExemplo.Text = $"Erro ao carregar utilizador: {ex.Message}";
            }
        }


        private void BtnEntrar_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;

            // Aqui você pode chamar seu método de login, por exemplo:
            // var user = UtilizadorDAL.Login(username, password);
            // if (user != null) { sucesso } else { erro }

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Por favor, preencha o Username e Password.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Apenas exemplo:
            if (username == "admin" && password == "1234")
            {
                MessageBox.Show("Login bem sucedido!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                // Abrir a janela principal, fechar o login etc.
                this.Close();
            }
            else
            {
                MessageBox.Show("Credenciais inválidas.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
