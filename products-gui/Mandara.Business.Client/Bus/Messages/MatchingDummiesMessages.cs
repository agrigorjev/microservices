using Mandara.Business.Bus.Messages.Base;
using Mandara.Entities.MatchingDummies;

namespace Mandara.Business.Bus.Messages
{

    public class MatchingDummiesMessageArgs : MessageBase
    {
         
    }

    /// <summary>
    /// For Buse: Wrong match args.
    /// </summary>
    public class MatchingDummiesWrongMessageArgs: MessageBase
    {
        public MatchingDummiesObject MatchingDummiesObject { get; set; }

        public bool SupressAudit { get; set; }
    }

    /// <summary>
    /// For Buse: Get match snapshot.
    /// </summary>
    public class MatchingDummiesSnapshotMessage: MessageBase
    {
        public MatchingDummiesObjectCollection MatchingDummiesObjectCollection { get; set; }
    }

    /// <summary>
    /// For Buse: Wrong match massage.
    /// </summary>
    public class MatchingToWrongMessage : MessageBase
    {
        public MatchingToWrongMessage()
        {}
        public MatchingToWrongMessage(bool answer):this()
        {
            Answer = answer;
        }
        public bool Answer { get; set; }
    }

}