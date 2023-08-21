using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mandara.Entities.FeedImport;

namespace Mandara.Entities.Extensions
{
    public static class FeedSourceTypeExtensions
    {
        public static bool IsClearPort (this FeedSourceType instanse)
        {
           return  instanse == FeedSourceType.ClearPort;
        }

        public static bool IsIce(this FeedSourceType instanse)
        {
            return instanse == FeedSourceType.ICE;
        }
    }
}
