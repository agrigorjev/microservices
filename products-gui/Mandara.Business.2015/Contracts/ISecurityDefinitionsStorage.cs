using Mandara.Entities;
using Mandara.Extensions.Option;
using System;
using System.Collections.Generic;

namespace Mandara.Business.Contracts
{
    public interface ISecurityDefinitionsStorage
    {
        int MaxId { get; }
        void Update();

        /// <summary>
        /// Clears all cache for storage and updates all security definitions and precalc details from DB
        /// </summary>
        void Reset();

        void Clean();

        /// <summary>
        /// Search for a SecurityDefinition matching the given ID.  If none is found in memory the database will be
        /// checked.  If found in the database the SecurityDefinition may be added to memory, but this is not required.
        /// </summary>
        /// <param name="secDefId"></param>
        /// <returns></returns>
        TryGetResult<SecurityDefinition> TryGetSecurityDefinition(int secDefId);

        /// <summary>
        /// Search for a SecurityDefinition matching the given symbol.  If none is found in memory the database will be
        /// checked.  If found in the database the SecurityDefinition may be added to memory, but this is not required.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        TryGetResult<SecurityDefinition> TryGetSecurityDefinitionBySymbol(string symbol);

        /// <summary>
        /// This method accepts a predicate that tests a SecurityDefinition using a set of unknown criteria - the value
        /// or values of fields being the most likely.  It is only expected to check the SecurityDefinitions in memory
        /// because the predicate cannot be used as a filter in a SQL query.
        /// </summary>
        /// <param name="secDefFilter"></param>
        /// <returns></returns>
        TryGetResult<SecurityDefinition> TryGetSecurityDefinitionByFields(Predicate<SecurityDefinition> secDefFilter);
        bool TryAdd(SecurityDefinition secDef);
        IEnumerable<SecurityDefinition> GetSecurityDefinitions();
        IEnumerable<SecurityDefinition> Where(Func<SecurityDefinition, bool> filter);
        void MarkSecurityDefinitionForReread(int secDefId);
        void CleanupNegativeIdSecurityDefinitions();
    }
}