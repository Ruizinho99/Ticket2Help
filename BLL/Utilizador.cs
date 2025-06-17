using System;

namespace BLL
{
    /// <summary>
    /// Representa um utilizador do sistema.
    /// Contém informações de identificação, autenticação e perfil.
    /// </summary>
    public class Utilizador
    {
        /// <summary>
        /// Identificador único do utilizador.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome completo do utilizador.
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Nome de utilizador (username) usado para login.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Tipo de utilizador (ex: IT-DESK, funcionário).
        /// </summary>
        public string Tipo { get; set; }

        /// <summary>
        /// Palavra-passe associada ao utilizador para autenticação.
        /// </summary>
        public string Password { get; set; }
    }
}
