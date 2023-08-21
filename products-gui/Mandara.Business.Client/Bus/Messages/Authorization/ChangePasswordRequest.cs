namespace Mandara.Business.Bus.Messages.Authorization
{
    using Mandara.Business.Bus.Messages.Base;

    public class ChangePasswordRequest : MessageBase
    {
        public string PasswordHash { get; set; }
        public string NewPasswordHash { get; set; }
    }
}
