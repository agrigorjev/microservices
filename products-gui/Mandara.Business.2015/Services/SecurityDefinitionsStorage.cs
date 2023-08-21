using Mandara.Business.Contracts;
using Mandara.Entities;
using Mandara.Extensions.Option;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Mandara.Extensions.Collections;

namespace Mandara.Business.Services
{
    public class SecurityDefinitionsStorage : ReplaceableDbContext, ISecurityDefinitionsStorage
    {
        private ConcurrentDictionary<int, SecurityDefinition> SecurityDefinitions;
        public int MaxId { get; private set; }
        private int _maxPrecalcDetailId;
        private readonly object updateLock = new object();
        private readonly ILogger _log;
        private HashSet<int> _securityDefinitionsMissingPrecalcDetails = new HashSet<int>();
        private readonly object _hashSetLock = new object();

        public SecurityDefinitionsStorage(ILogger log)
        {
            SecurityDefinitions = new ConcurrentDictionary<int, SecurityDefinition>();
            MaxId = 0;
            _log = log;
        }

        public void Reset()
        {
            Clean();
            Update();
        }

        public void Clean()
        {
            lock (updateLock)
            {
                SecurityDefinitions = new ConcurrentDictionary<int, SecurityDefinition>();
                MaxId = 0;
                _maxPrecalcDetailId = 0;
            }

            lock (_hashSetLock)
            {
                _securityDefinitionsMissingPrecalcDetails = new HashSet<int>();
            }
        }

        public void Update()
        {
            const int takeNum = 10000;
            int startMaxId = MaxId;

            List<SecurityDefinition> securityDefinitions;
            List<PrecalcSdDetail> secDefPrecalcDetails;

            // Don't simply exit if an update is already in progress - the update must complete with any updated data.
            lock (updateLock)
            {
                using (var cxt = _dbContextCreator())
                {
                    cxt.Database.CommandTimeout = 0;

                    securityDefinitions = LoadSecurityDefinitionsPack(
                        cxt,
                        takeNum);
                    secDefPrecalcDetails = LoadPrecalcDetailsPack(cxt, takeNum);
                    MergeSecurityDefinitionsWithPrecalcDetails(secDefPrecalcDetails, securityDefinitions);
                    LogUpdateResult(startMaxId, securityDefinitions.Count);
                }
            }

            securityDefinitions.ForEach(sd => SecurityDefinitions.TryAdd(sd.SecurityDefinitionId, sd));
        }

        private List<SecurityDefinition> LoadSecurityDefinitionsPack(
            MandaraEntities cxt,
            int takeNum)
        {
            _log.Trace("Loading SecurityDefinitions");

            List<SecurityDefinition> packOfSecurityDefinitions;
            List<SecurityDefinition> securityDefinitions = new List<SecurityDefinition>();

            do
            {
                int id = MaxId;
                _log.Trace("Loading max({0}) security from id {1}",takeNum,id);
                packOfSecurityDefinitions =
                    cxt.SecurityDefinitions.AsNoTracking()
                       .Where(tc => tc.SecurityDefinitionId > id)
                       .Take(takeNum)
                       .OrderBy(tc => tc.SecurityDefinitionId)
                       .ToList();
                _log.Trace("Loaded {0} security from id {1}", packOfSecurityDefinitions.Count, id);
                if (packOfSecurityDefinitions.Count > 0)
                {
                    securityDefinitions.AddRange(packOfSecurityDefinitions);
                    MaxId = packOfSecurityDefinitions[packOfSecurityDefinitions.Count - 1].SecurityDefinitionId;
                }
            }
            while (packOfSecurityDefinitions.Count == takeNum);

            return securityDefinitions;
        }

        /// <summary>
        /// Load SecurityDefinition Precalc Details (previously these were loaded via Include, but that // eventually 
        /// created a query that never completed.
        /// </summary>
        /// <param name="cxt"></param>
        /// <param name="takeNum"></param>
        /// <param name="secDefPrecalcDetails"></param>
        private List<PrecalcSdDetail> LoadPrecalcDetailsPack(
            MandaraEntities cxt,
            int takeNum)
        {
            _log.Trace("Loading SecurityDefinition Precalc Details");

            List<PrecalcSdDetail> precalcDetailPack;
            List<PrecalcSdDetail> secDefPrecalcDetails = new List<PrecalcSdDetail>();

            do
            {
                int id = _maxPrecalcDetailId;
                _log.Trace("Loading max({0}) PrecalcSdDetail from id {1}", takeNum, id);
                precalcDetailPack =
                    cxt.PrecalcSdDetails.AsNoTracking().Where(precalc => precalc.PrecalcDetailId > id).Take(takeNum)
                       //.OrderBy(precalc => precalc.SecurityDefinitionId)
                       .ToList();

                if (precalcDetailPack.Count > 0)
                {
                    secDefPrecalcDetails.AddRange(precalcDetailPack);
                    _maxPrecalcDetailId = precalcDetailPack[precalcDetailPack.Count - 1].PrecalcDetailId;
                }
            }
            while (precalcDetailPack.Count > 0);

            return secDefPrecalcDetails;
        }

        private void MergeSecurityDefinitionsWithPrecalcDetails(
            List<PrecalcSdDetail> secDefPrecalcDetails,
            List<SecurityDefinition> securityDefinitions)
        {
            _log.Trace("Merging SecurityDefinitions with their Precalc Details");
            Dictionary<int,SecurityDefinition> secDefMap = 
                securityDefinitions.ToDictionary(x => x.SecurityDefinitionId, y => y);
            secDefPrecalcDetails.GroupBy(precalc => precalc.SecurityDefinitionId).ForEach(
                precalcGrp =>
                {
                    int key = precalcGrp.Key;
                    TryGetResult<SecurityDefinition> securityDef = 
                            secDefMap.ContainsKey(key) ?
                            new TryGetRef<SecurityDefinition>(secDefMap[key]) :
                            TryGetSecurityDefinition(key);
                    if (!securityDef.HasValue)
                    {
                        _log.Trace("Thrown away PrecalcSdDetail no related SecurityDefinition found with id={0}",precalcGrp.Key);
                    }
                    else
                    {
                        securityDef.Value.PrecalcDetails = precalcGrp.ToList();
                    }
                });
        }

        public TryGetResult<SecurityDefinition> TryGetSecurityDefinition(int secDefId)
        {
            return TryGetSecurityDefinition(
                secDefId,
                TryGetSecurityDefinitionById,
                TryLoadSecurityDefinitionById);
        }

        private TryGetResult<SecurityDefinition> TryGetSecurityDefinitionById(int secDefId)
        {
            SecurityDefinition secDef;

            SecurityDefinitions.TryGetValue(secDefId, out secDef);

            return new TryGetRef<SecurityDefinition>() { Value = secDef };
        }

        private TryGetResult<SecurityDefinition> TryLoadSecurityDefinitionById(int secDefId)
        {
            SecurityDefinition secDef;

            using (MandaraEntities mandaraContext = _dbContextCreator())
            {
                secDef =
                    mandaraContext.SecurityDefinitions.Include(x => x.PrecalcDetails)
                                  .AsNoTracking()
                                  .SingleOrDefault(x => x.SecurityDefinitionId == secDefId);
            }

            return new TryGetRef<SecurityDefinition>() { Value = secDef };
        }

        public TryGetResult<SecurityDefinition> TryGetSecurityDefinitionBySymbol(string symbol)
        {
            return TryGetSecurityDefinition(
                symbol,
                TryGetSecurityDefinitionFromMemoryBySymbol,
                TryLoadSecurityDefinitionBySymbol);
        }

        private TryGetResult<SecurityDefinition> TryGetSecurityDefinitionFromMemoryBySymbol(string symbol)
        {
            SecurityDefinition secDef = SecurityDefinitions.Values.FirstOrDefault(x => x.UnderlyingSymbol == symbol);

            return new TryGetRef<SecurityDefinition>() { Value = secDef };
        }

        private TryGetResult<SecurityDefinition> TryLoadSecurityDefinitionBySymbol(string symbol)
        {
            SecurityDefinition secDef;

            using (MandaraEntities mandaraContext = _dbContextCreator())
            {
                secDef =
                    mandaraContext.SecurityDefinitions.Include(x => x.PrecalcDetails)
                                  .AsNoTracking()
                                  .SingleOrDefault(x => x.UnderlyingSymbol == symbol);
            }

            return new TryGetRef<SecurityDefinition>() { Value = secDef };
        }

        private TryGetResult<SecurityDefinition> TryGetSecurityDefinition<T>(
            T identifier,
            Func<T, TryGetResult<SecurityDefinition>> dictionaryFunc,
            Func<T, TryGetResult<SecurityDefinition>> dbFunc)
        {
            TryGetResult<SecurityDefinition> secDef = dictionaryFunc(identifier);

            if (!secDef.HasValue)
            {
                secDef = dbFunc(identifier);
            }

            if (secDef.HasValue)
            {
                lock (_hashSetLock)
                {
                    if (AreMissingPrecalcDetailsFound(secDef.Value.SecurityDefinitionId, secDef.Value))
                    {
                        SecurityDefinitions.TryAdd(secDef.Value.SecurityDefinitionId, secDef.Value);
                        _securityDefinitionsMissingPrecalcDetails.Remove(secDef.Value.SecurityDefinitionId);
                    }
                }
            }

            return secDef;
        }

        private bool AreMissingPrecalcDetailsFound(int secDefId, SecurityDefinition secDef)
        {
            return _securityDefinitionsMissingPrecalcDetails.Contains(secDefId) && secDef.PrecalcDetails.Any();
        }

        public TryGetResult<SecurityDefinition> TryGetSecurityDefinitionByFields(
            Predicate<SecurityDefinition> secDefFilter)
        {
            SecurityDefinition secDef =
                SecurityDefinitions.Values.FirstOrDefault(candidateSecDef => secDefFilter(candidateSecDef));

            return new TryGetRef<SecurityDefinition>() { Value = secDef };
        }

        public bool TryAdd(SecurityDefinition secDef)
        {
            return SecurityDefinitions.TryAdd(secDef.SecurityDefinitionId, secDef);
        }

        public IEnumerable<SecurityDefinition> GetSecurityDefinitions()
        {
            return SecurityDefinitions.Values;
        }

        public IEnumerable<SecurityDefinition> Where(Func<SecurityDefinition, bool> filter)
        {
            return SecurityDefinitions.Values.Where(filter).Select(security => security.ShallowCopy());
        }

        private void LogUpdateResult(int startMaxId, int securityDefinitionsCount = 0)
        {
            if (MaxId != startMaxId)
            {
                _log.Info(
                    "Retrieved Security Definitions with IDs from {0} to {1} ({2} records)",
                    startMaxId + 1,
                    MaxId,
                    securityDefinitionsCount);
            }
            else
            {
                _log.Info("No new Security Definitions were retrieved.");
            }
        }

        public void MarkSecurityDefinitionForReread(int secDefId)
        {
            // The lock is not around the whole method because the protection is only needed on the HashSet.  Other
            // threads might access the security definition that is to be removed, but that's already in a protected
            // collection.  It is possible that another thread will enter TryGetSecurityDefinition, find the instance
            // in the collection after the entry is added to the collection to be reread, but that doesn't matter.  The
            // instance without precalc details will be returned and then it will be removed by this method.  The next
            // time TryGetSecurityDefinition is called for that ID the data will be read from the database again.  If
            // precalc details are then available it will be removed from the collection to reread and inserted in the
            // collection of SecurityDefinitions.
            lock (_hashSetLock)
            {
                _securityDefinitionsMissingPrecalcDetails.Add(secDefId);
            }

            SecurityDefinition removedSecDef;

            SecurityDefinitions.TryRemove(secDefId, out removedSecDef);
        }

        public void CleanupNegativeIdSecurityDefinitions()
        {
            List<int> negativeIdSecurityDefinitions =
                SecurityDefinitions.Keys.Where(sd => sd < 0).ToList();

            bool warningLogged = false;

            foreach (int negativeIdSecurityDefinition in negativeIdSecurityDefinitions)
            {
                SecurityDefinition expiredItem;
                if (!SecurityDefinitions.TryRemove(negativeIdSecurityDefinition, out expiredItem))
                {
                    if (!warningLogged)
                    {
                        _log.Warn("Failed to cleanup negative id security definition, ID: "
                        + negativeIdSecurityDefinition);
                        warningLogged = true;
                    }

                }
            }

        }
    }
}
