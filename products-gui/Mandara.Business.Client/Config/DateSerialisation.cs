using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mandara.Business.Config
{
    public enum DateTimeSerialisation
    {
        Default,
        Unspecified,
        Utc,
        Local
    }

    public static class DateSerialisation
    {
        private static readonly Dictionary<string, DateTimeSerialisation> _dateTimeSerialisations =
            BuildSerialisationTypes();

        private static Dictionary<string, DateTimeSerialisation> BuildSerialisationTypes()
        {
            return KindDefinitions(Enum.GetValues(typeof(DateTimeSerialisation)).GetEnumerator())
                .ToDictionary(kindDef => kindDef.name, kindDef => kindDef.kind);

            IEnumerable<(string name, DateTimeSerialisation kind)> KindDefinitions(IEnumerator kinds)
            {
                while (kinds.MoveNext())
                {
                    yield return SerialisationDefinition((DateTimeSerialisation)kinds.Current);
                }
            }

            (string name, DateTimeSerialisation kind) SerialisationDefinition(DateTimeSerialisation kind)
            {
                return (kind.ToString().ToLowerInvariant(), kind);
            }
        }

        public static DateTimeSerialisation To(string targetKind) =>
            _dateTimeSerialisations.TryGetValue(targetKind.ToLowerInvariant(), out DateTimeSerialisation kind)
                ? kind
                : DateTimeSerialisation.Default;
    }
}