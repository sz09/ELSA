namespace ELSA.WebAPI.Models.Authentication
{
    /// <summary>
    /// Login information
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// The user name or email
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// The password
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Use for refresh token
        /// </summary>
        public bool KeepMeLogIn { get; set; }
    }
}

