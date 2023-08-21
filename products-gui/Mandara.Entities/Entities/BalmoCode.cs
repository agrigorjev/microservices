using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities
{
    public abstract class BalmoCode
    {
        [NotMapped]
        protected string prefixChar;

        [NotMapped]
        protected string startChar;

        [NotMapped]
        protected string endChar;

        public bool IsValid()
        {
            return prefixChar != null
                   && startChar != null
                   && endChar != null
                   && prefixChar.Length >= 1
                   && prefixChar.Length <= 10
                   && startChar.Length == 1
                   && endChar.Length == 1;
        }
    }
}
