using Mandara.Date.Time;
using Mandara.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using Mandara.Entities.Entities;
using Mandara.Entities.Exceptions;
using NLog;
using Optional;
using Optional.Unsafe;

namespace Mandara.Business
{
    public class SourceDataManager : ReplaceableDbContext
    {
        private static readonly Dictionary<string, SecDefPrecalcInsertFailure> precalcsFromWhichToRunAway =
            new Dictionary<string, SecDefPrecalcInsertFailure>();
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public List<String> GetAccounts()
        {
            using (MandaraEntities productsDb = _dbContextCreator())
            {
                productsDb.Database.CommandTimeout = 0;
                return productsDb.SourceDetails.Where(abnDetailGrp => abnDetailGrp.AccountNumber != null)
                                 .Select(abnDetailGrp => abnDetailGrp.AccountNumber)
                                 .Distinct()
                                 .ToList();
            }
        }

        public List<Exchange> GetExchanges()
        {
            using (MandaraEntities productsDb = _dbContextCreator())
            {
                return productsDb.Exchanges.ToList();
            }
        }

        public SourceData GetSourceData(DateTime effectiveDate, SourceDataType sourceDataType)
        {
            using (MandaraEntities productsDb = _dbContextCreator())
            {
                return productsDb.SourceDataSet.FirstOrDefault(
                    abnDetailGrp =>
                        abnDetailGrp.Date == effectiveDate && abnDetailGrp.TypeDb == (short)sourceDataType);
            }
        }

        public void ClearSourceDetails(SourceData abnDataGroup)
        {
            using (MandaraEntities productsDb = _dbContextCreator())
            {
                productsDb.Database.ExecuteSqlCommand(
                    "DELETE FROM source_data_details WHERE source_data_id={0}",
                    abnDataGroup.SourceDataId);
            }
        }

        /// <summary>
        /// Clean current seal details before import of new ones with same ExchangeID
        /// </summary>
        /// <param name="abnGroupId"></param>
        /// <param name="duplicatedID"></param>
        public void ClearSealDetails(int abnGroupId, List<string> duplicatedID)
        {
            using (MandaraEntities productsDb = _dbContextCreator())
            {
                SourceData abnDataGroup = productsDb.SourceDataSet.Include(abnGrp => abnGrp.SealDetails)
                                                    .FirstOrDefault(abnGrp => abnGrp.SourceDataId == abnGroupId);

                productsDb.SealDeatails.RemoveRange(
                    productsDb.SealDeatails.Where(seal => duplicatedID.Contains(seal.ExchangeId)).ToList());
                productsDb.SaveChanges();
            }
        }

        public Hashtable GetAllSourceData()
        {
            Hashtable sourceDates = new Hashtable();

            using (MandaraEntities productsDb = _dbContextCreator())
            {
                productsDb.Database.CommandTimeout = 0;

                foreach (SourceData abnDataPoint in productsDb.SourceDataSet)
                {

                    if (sourceDates.ContainsKey(abnDataPoint.Date))
                    {
                        if (abnDataPoint.Type != SourceDataType.Seals)
                        {
                            sourceDates[abnDataPoint.Date] =
                                Convert.ToInt32(sourceDates[abnDataPoint.Date]) + ((short)abnDataPoint.Type + 1);
                        }
                    }
                    else
                    {
                        if (abnDataPoint.Type != SourceDataType.Seals)
                        {
                            sourceDates.Add(abnDataPoint.Date, (short)abnDataPoint.Type + 1);
                        }
                        else
                        {
                            sourceDates.Add(abnDataPoint.Date, 0);
                        }
                    }
                }

            }

            return sourceDates;
        }

        //        public static Hashtable GetAllPositionsDates()
        //        {
        //            Hashtable sourceDates = new Hashtable();
        //            using (var context = _dbContextCreator())
        //            {
        //                context.Database.CommandTimeout = 0;
        //
        //                var sourceData = context.SourceDataSet
        //                    .Where(x => x.TypeDb == (int)SourceDataType.OpenPositions)
        //                    .OrderByDescending(x => x.Date);
        //
        //                foreach (SourceData sd in sourceData)
        //                {
        //                    if (!sourceDates.ContainsKey(sd.Date))
        //                    {
        //                        sourceDates.Add(sd.Date, 3);
        //                    }
        //                }
        //            }
        //
        //            return sourceDates;
        //        }

        public Dictionary<DateTime, int> GetAllPositionsDates()
        {
            Dictionary<DateTime, int> sourceDates = new Dictionary<DateTime, int>();

            using (MandaraEntities productsDb = _dbContextCreator())
            {
                productsDb.Database.CommandTimeout = 0;

                IOrderedQueryable<SourceData> abnPositionGroups = productsDb.SourceDataSet
                    .Where(x => x.TypeDb == (int)SourceDataType.OpenPositions)
                    .OrderByDescending(x => x.Date);

                foreach (SourceData position in abnPositionGroups.Where(pos => !sourceDates.ContainsKey(pos.Date)))
                {
                    sourceDates.Add(position.Date, 3);
                }
            }

            return sourceDates;
        }

        public DateTime? GetLatestSourceDate()
        {
            using (MandaraEntities productsDb = _dbContextCreator())
            {
                SourceData abnDataGroup = productsDb.SourceDataSet
                                                    .Where(
                                                        abnDataGrp =>
                                                            abnDataGrp.TypeDb == (int)SourceDataType.OpenPositions)
                                                    .OrderByDescending(abnDataGrp => abnDataGrp.Date)
                                                    .FirstOrDefault();

                return abnDataGroup?.Date;
            }
        }

        public void SaveSourceData(
            SourceData abnDataGroup,
            List<SourceDetail> abnDataPoints,
            List<PrecalcSourcedetailsDetail> abnPrecalcs,
            List<PrecalcSdDetail> secDefPrecalcs)
        {
            ProductManager products = new ProductManager();

            //try
            {
                using (MandaraEntities productsDb = _dbContextCreator())
                {
                    InsertAbnData(
                        abnDataGroup,
                        abnDataPoints,
                        products,
                        abnPrecalcs ?? new List<PrecalcSourcedetailsDetail>(),
                        secDefPrecalcs ?? new List<PrecalcSdDetail>(),
                        productsDb);
                    precalcsFromWhichToRunAway.Clear();
                }
            }
            //catch (InsertException<SecDefPrecalcInsertFailure>)
            //{
            //    SaveSourceData(
            //        abnDataGroup,
            //        abnDataPoints,
            //        abnPrecalcs ?? new List<PrecalcSourcedetailsDetail>(),
            //        secDefPrecalcs ?? new List<PrecalcSdDetail>());
            //}
        }

        private static void InsertAbnData(
            SourceData abnDataGroup,
            List<SourceDetail> abnDataPoints,
            ProductManager productManager,
            List<PrecalcSourcedetailsDetail> abnPrecalcs,
            List<PrecalcSdDetail> secDefPrecalcs,
            MandaraEntities productsDb)
        {
            try
            {
                using (DbContextTransaction insertTransaction = productsDb.Database.BeginTransaction())
                {
                    productsDb.Database.CommandTimeout = 0;
                    productsDb.Configuration.AutoDetectChangesEnabled = false;

                    if (!abnDataGroup.IsNew())
                    {
                        productsDb.SourceDataSet.Attach(abnDataGroup);
                        abnDataGroup.ImportedDateTime = SystemTime.Now();
                    }

                    SetUpAbnDataForSave(abnDataGroup, abnDataPoints, productManager, productsDb);
                    AddAbnTransactionGroupForSave(abnDataGroup, productsDb);
                    productsDb.SourceDetails.AddRange(abnDataPoints);
                    AddAbnCustomPeriodTradePrecalcsForSave(abnPrecalcs, productsDb);
                    AddSecDefPrecalcsForSave(secDefPrecalcs, productsDb);
                    productsDb.SaveChanges();
                    insertTransaction.Commit();
                }
            }
            catch (SqlException insertErr)
            {
                if (SqlErrorNumbers.KeyViolation == insertErr.Number)
                {
                    HandleKeyConstraintViolation(insertErr);
                }

                throw;
            }
            catch (DbUpdateException updateErr)
            {
                Option<SqlException> innerSqlExc = FindSqlException(updateErr);

                if (innerSqlExc.HasValue && innerSqlExc.ValueOrDefault()?.Number == SqlErrorNumbers.KeyViolation)
                {
                    HandleKeyConstraintViolation(innerSqlExc.ValueOrFailure());
                }

                throw;
            }

            Option<SqlException> FindSqlException(Exception exceptionToCheck)
            {
                if (exceptionToCheck is SqlException found)
                {
                    return Option.Some(found);
                }

                return null != exceptionToCheck.InnerException
                    ? FindSqlException(exceptionToCheck.InnerException)
                    : Option.None<SqlException>();
            }
        }

        private static void SetUpAbnDataForSave(
                SourceData abnDataGroup,
                List<SourceDetail> abnDataPoints,
                ProductManager productManager,
                MandaraEntities productsDb)
        {
            List<Product> products = productManager.GetProducts(productsDb);

            abnDataPoints.ForEach(trade => { SetUpAbnTradeForSave(abnDataGroup, productsDb, trade, products); });
        }

        private static void SetUpAbnTradeForSave(
            SourceData abnTransactionGroup,
            MandaraEntities productsDb,
            SourceDetail trade,
            List<Product> products)
        {
            trade.Product = GetTradedProduct(products, trade);
            PrepareAbnTradedProductForSave(productsDb, trade);
            SetAbnTradeGroup(abnTransactionGroup, trade);
        }

        private static Product GetTradedProduct(List<Product> products, SourceDetail abnDataPoint)
        {
            return products.Single(p => p.ProductId == abnDataPoint.Product.ProductId);
        }

        private static void PrepareAbnTradedProductForSave(MandaraEntities productsDb, SourceDetail abnDataPoint)
        {
            // could be null in case of daily products
            if (abnDataPoint.SecurityDefinition != null)
            {
                abnDataPoint.SecurityDefinition.Product = abnDataPoint.Product;
                productsDb.SecurityDefinitions.Add(abnDataPoint.SecurityDefinition);

                if (abnDataPoint.SecurityDefinition.SecurityDefinitionId != 0)
                {
                    productsDb.Entry(abnDataPoint.SecurityDefinition).State = EntityState.Unchanged;
                }
            }
        }

        private static void SetAbnTradeGroup(SourceData abnTransactionGroup, SourceDetail abnDataPoint)
        {
            if (!abnTransactionGroup.IsNew())
            {
                abnDataPoint.SourceDataId = abnTransactionGroup.SourceDataId;
            }
            else
            {
                abnDataPoint.SourceData = abnTransactionGroup;
            }
        }

        private static void AddAbnTransactionGroupForSave(SourceData abnTransactionGroup, MandaraEntities productsDb)
        {
            if (abnTransactionGroup.IsNew())
            {
                productsDb.SourceDataSet.Add(abnTransactionGroup);
            }
        }

        private static void AddAbnCustomPeriodTradePrecalcsForSave(
            List<PrecalcSourcedetailsDetail> abnPrecalcs,
            MandaraEntities productsDb)
        {
            productsDb.PrecalcSourcedetailsDetails.AddRange(abnPrecalcs);
        }

        private static void AddSecDefPrecalcsForSave(List<PrecalcSdDetail> secDefPrecalcs, MandaraEntities productsDb)
        {
            productsDb.PrecalcSdDetails.AddRange(secDefPrecalcs.Where(PrecalcsAreNew));

        }

        private static bool PrecalcsAreNew(PrecalcSdDetail precalcs)
        {
            return !precalcsFromWhichToRunAway.TryGetValue(
                GetPrecalcKey(precalcs.SecurityDefinitionId, precalcs.ProductId, precalcs.Month),
                out SecDefPrecalcInsertFailure _);
        }

        private static string GetPrecalcKey(int secDef, int product, DateTime month)
        {
            return $"{secDef}{product}{month}";
        }

        private static void HandleKeyConstraintViolation(SqlException keyConstrViolation)
        {
            Match uniqueKeyViolation =
                KeyConstraintViolation.UniqueKeyViolationPattern.Match(keyConstrViolation.Message);

            if (uniqueKeyViolation.Success && IsSecDefPrecalcUniquenessViolation())
            {
                LogPrecalcUniqueKeyViolation(uniqueKeyViolation);
                ThrowPrecalcInsertException(keyConstrViolation, uniqueKeyViolation);
            }

            bool IsSecDefPrecalcUniquenessViolation()
            {
                return TableNameSource.GetTableName(typeof(PrecalcSdDetail))
                       == uniqueKeyViolation.Groups[KeyConstraintViolation.TableMatchName].Value;
            }
        }

        private static void LogPrecalcUniqueKeyViolation(Match uniqueKeyViolation)
        {
            string[] keyFields = uniqueKeyViolation.Groups[KeyConstraintViolation.KeyMatchName]
                                                   .Value.Split(',')
                                                   .Select(keyField => keyField.Trim())
                                                   .ToArray();
            SecDefPrecalcInsertFailure failure = new SecDefPrecalcInsertFailure(keyFields);

            Logger.Error(
                "Security precalcs key violation; duplicate found - Security ID = {0}, Product ID = {1}, Month = {2}",
                failure.SecurityDef,
                failure.Product,
                failure.Month);
        }

        private static void ThrowPrecalcInsertException(
            SqlException precalcUniquenessViolation,
            Match uniqueKeyViolation)
        {
            string[] keyFields = uniqueKeyViolation.Groups[KeyConstraintViolation.KeyMatchName]
                                                   .Value.Split(',')
                                                   .Select(keyField => keyField.Trim())
                                                   .ToArray();
            SecDefPrecalcInsertFailure failure = new SecDefPrecalcInsertFailure(keyFields);

            precalcsFromWhichToRunAway.Add(GetPrecalcKey(failure.SecurityDef, failure.Product, failure.Month), failure);

            throw new InsertException<SecDefPrecalcInsertFailure>(
                "Error inserting precalcs for Security Definition - duplicate found",
                InsertFailureSource.SecurityPrecalc,
                keyFields,
                failure,
                precalcUniquenessViolation);
        }

        public void SaveSealData(SourceData abnSealGroup, List<SealDetail> seals)
        {
            if (abnSealGroup.SourceDataId > -1)
            {
                ClearSealDetails(abnSealGroup.SourceDataId, seals.Select(seal => seal.ExchangeId).Distinct().ToList());
            }

            using (MandaraEntities productsDb = _dbContextCreator())
            {
                if (abnSealGroup.SourceDataId != -1)
                {
                    productsDb.SourceDataSet.Attach(abnSealGroup);
                    abnSealGroup.ImportedDateTime = SystemTime.Now();
                }

                foreach (SealDetail seal in seals)
                {
                    abnSealGroup.SealDetails.Add(seal);
                }

                if (abnSealGroup.SourceDataId == -1)
                {
                    productsDb.SourceDataSet.Add(abnSealGroup);
                }

                productsDb.SaveChanges();
            }
        }

        public List<SourceDataMessage> GetSourceDataMessages()
        {
            using (MandaraEntities productsDb = _dbContextCreator())
            {
                return productsDb.SourceDataMessages.OrderByDescending(m => m.MessageDate).ToList();
            }
        }

        public void SaveSourceDataMessages(List<SourceDataMessage> messages, List<AuditMessage> auditMessages)
        {
            using (MandaraEntities productsDb = _dbContextCreator())
            {
                foreach (AuditMessage auditMessage in auditMessages)
                {
                    productsDb.AuditMessages.Add(auditMessage);
                }

                foreach (SourceDataMessage m in messages)
                {
                    productsDb.SourceDataMessages.Add(m);
                }

                productsDb.SaveChanges();
            }
        }

        public static void CleanSealDetailsOnNewImport()
        {


        }

    }
}
