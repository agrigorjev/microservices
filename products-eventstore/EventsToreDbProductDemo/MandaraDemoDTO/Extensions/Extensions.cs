using MandaraDemoDTO.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandaraDemoDTO.Extensions
{
    public static class Extensions
    {

        public static bool EqualIgnoreCase(this string inString, string compareWith)
        {

            return inString.Trim().Equals(compareWith.Trim(),StringComparison.InvariantCultureIgnoreCase);

        }
        public static KnownEvents toKnownEvent(this string? value)
        {
            if (value != null)
            {
                if (value.EqualIgnoreCase("create") || value.EqualIgnoreCase("new"))
                {
                    return KnownEvents.Create;
                }
                if (value.EqualIgnoreCase("update"))
                {
                    return KnownEvents.Update;
                }
                if (value.EqualIgnoreCase("delete"))
                {
                    return KnownEvents.Delete;
                }
            }
            return KnownEvents.UNKNOWN;

        }

        public static string ToString(this KnownEvents knownEvents)
        {
            switch (knownEvents)
            {
                case KnownEvents.Create:
                    return "Create";
                case KnownEvents.Update:
                    return "Update";
                case KnownEvents.Delete:
                    return "Delete";
                default:
                    return "UNKNOWN";
            }
        }

        public static State toStatus(this string? value)
        {
            if (value != null)
            {
                if (value.EqualIgnoreCase("removed"))
                {
                    return State.REMOVED;
                }
               
            }
            return State.UNSPECIFIED;

        }
    }
}
