using System.Runtime.Serialization;

namespace Mandara.Entities.FeedImport
{
     [DataContract(IsReference = true)]
    public class InvalidCredentialsException: FeedImportException
    {
        public InvalidCredentialsException(){}

        public InvalidCredentialsException(FeedSourceType feedSourceType) : base(GetMessage(feedSourceType)) { }

        private static string GetMessage(FeedSourceType feedSourceType)
        {
            return string.Format("Invalid user name or password for the source:'{0}'.", feedSourceType);
        }
    }
}
