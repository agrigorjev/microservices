using Mandara.Business.Audit;
using Mandara.Entities;
using Mandara.Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;


namespace Mandara.Business
{
    public class BrokerManager
    {
        public List<Broker> GetBrokers()
        {
            using (MandaraEntities context = CreateMandaraProductsDbContext())
            {
                return
                    context.Brokers.Include(c => c.Company)
                                   .Include(c => c.ParserDefaultProduct)
                                   .Include(c => c.ParserDefaultProduct.OfficialProduct).ToList();
            }
        }

        private MandaraEntities CreateMandaraProductsDbContext()
        {
            return new MandaraEntities(MandaraEntities.DefaultConnStrName, nameof(BrokerManager));
        }

        public List<Region> GetRegions()
        {
            using (MandaraEntities context = CreateMandaraProductsDbContext())
            {
                return context.Regions.ToList();
            }
        }

        public void SaveBrokers(List<Broker> brokersToSave, AuditContext auditContext)
        {
            using (var context = CreateMandaraProductsDbContext())
            {
                var brokers = context.Brokers.Include(c => c.Company)
                                             .Include(c => c.ParserDefaultProduct)
                                             .Include(c => c.ParserDefaultProduct.OfficialProduct).ToList();

                context.Configuration.AutoDetectChangesEnabled = false;

                foreach (var updatedBroker in brokersToSave)
                {
                    var existingBroker = brokers.SingleOrDefault(x => x.BrokerId == updatedBroker.BrokerId);

                    if (existingBroker != null)
                    {
                        if (updatedBroker.Company != null)
                        {
                            existingBroker.Company =
                                context.Companies.Single(x => x.CompanyId == updatedBroker.Company.CompanyId);
                        }
                        else
                            existingBroker.Company = null;

                        if (updatedBroker.DefaultProduct != null)
                        {
                            if (existingBroker.ParserDefaultProduct != null)
                            {
                                existingBroker.ParserDefaultProduct.OfficialProduct =
                                    context.OfficialProducts.Single(x => x.OfficialProductId == updatedBroker.DefaultProduct.OfficialProductId);
                            }
                            else
                            {
                                var parserDefaultProduct = new ParserDefaultProduct
                                {
                                    BrokerId = updatedBroker.BrokerId,
                                    OfficialProduct = context.OfficialProducts.Single(x => x.OfficialProductId == updatedBroker.DefaultProduct.OfficialProductId)
                                };

                                existingBroker.ParserDefaultProduct = parserDefaultProduct;

                                context.ParserDefaultProducts.Add(parserDefaultProduct);
                            }
                        }
                        else
                            existingBroker.ParserDefaultProduct = null;
                    }
                    else
                    {
                        if (updatedBroker.Company != null)
                            updatedBroker.Company = context.Companies.Single(x => x.CompanyId == updatedBroker.Company.CompanyId);

                        if (updatedBroker.DefaultProduct != null)
                        {
                            var parserDefaultProduct =
                                context.ParserDefaultProducts.SingleOrDefault(x => x.BrokerId == updatedBroker.ParserDefaultProduct.BrokerId);

                            if (parserDefaultProduct == null)
                            {
                                parserDefaultProduct = new ParserDefaultProduct
                                {
                                    BrokerId = updatedBroker.BrokerId,
                                    OfficialProduct = context.OfficialProducts.Single(x => x.OfficialProductId == updatedBroker.DefaultProduct.OfficialProductId)
                                };

                                updatedBroker.ParserDefaultProduct = parserDefaultProduct;

                                context.ParserDefaultProducts.Add(parserDefaultProduct);
                            }
                            else
                            {
                                updatedBroker.ParserDefaultProduct.OfficialProduct =
                                    context.OfficialProducts.Single(x => x.OfficialProductId == updatedBroker.DefaultProduct.OfficialProductId);
                            }
                        }

                        context.Brokers.Add(updatedBroker);
                    }
                }

                var updatedEntities = brokersToSave.Intersect(brokers).ToList();
                var deletedEntities = brokers.Except(updatedEntities).ToList();


                // works faster that Remove method in foreach
                context.Brokers.RemoveRange(deletedEntities);

                context.ChangeTracker.DetectChanges();
                context.SaveChanges();
            }
        }

        public List<Company> GetCompanies()
        {
            using (MandaraEntities context = CreateMandaraProductsDbContext())
            {
                return context.Companies.Include(c => c.Brokerages).Include("Brokerages.Unit").Include(c => c.Region).ToList();
            }
        }

        public void SaveCompanies(List<Company> companiesToSave, AuditContext auditContext, out List<string> hasBrokers, out List<string> hasBrokerage)
        {
            using (var cxt = CreateMandaraProductsDbContext())
            using (var transaction = cxt.Database.BeginTransaction())
            {
                cxt.Configuration.AutoDetectChangesEnabled = false;

                List<Company> deletedEntities;
                cxt.Save(cxt.Companies, companiesToSave, c => c.CompanyId, c => true, false, out deletedEntities);

                hasBrokers = new List<string>();
                hasBrokerage = new List<string>();
                foreach (var deletedCompany in deletedEntities)
                {
                    //check if company may be deleted has brockers
                    if (deletedCompany.Brokers.Count > 0)
                    {
                        hasBrokers.Add(deletedCompany.CompanyName);
                    }
                    //or product GetProductBrokerages reference
                    else if (deletedCompany.ProductsBrokerages.Count > 0)
                    {
                        hasBrokerage.Add(deletedCompany.CompanyName);
                    }
                    else
                    {
                        //delete aliases first
                        if (deletedCompany.CompanyAliases.Count > 0)
                        {
                            cxt.CompanyAlias.RemoveRange(deletedCompany.CompanyAliases);
                            deletedCompany.CompanyAliases.Clear();
                        }
                        //delete object
                        cxt.Companies.Remove(deletedCompany);
                    }
                }

                cxt.ChangeTracker.DetectChanges();
                cxt.SaveChanges();
                transaction.Commit();
            }
        }

        public List<CompanyAlias> GetCompanyAliases()
        {
            using (MandaraEntities context = CreateMandaraProductsDbContext())
            {
                return context.CompanyAlias.Include(c => c.Company).Include(c => c.Company.Brokerages).Include("Company.Brokerages.Unit").ToList();
            }
        }

        public List<ProductBrokerage> GetProductBrokerages()
        {
            using (MandaraEntities context = CreateMandaraProductsDbContext())
            {
                return context.ProductBrokerage.ToList();
            }
        }

        public Company GetCompanyById(int companyId)
        {
            using (MandaraEntities context = CreateMandaraProductsDbContext())
            {
                return context.Companies
                    .Include(c => c.Region)
                    .Include(c => c.CompanyAliases)
                    .Include(c => c.Brokerages)
                    .Include("Brokerages.Unit")
                    .SingleOrDefault(c => c.CompanyId == companyId);
            }
        }

        public void SaveCompany(Company companyToSave, AuditContext auditContext)
        {
            using (MandaraEntities cxt = CreateMandaraProductsDbContext())
            using (var transaction = cxt.Database.BeginTransaction())
            {
                Func<int> newIdFunc;
                cxt.Configuration.AutoDetectChangesEnabled = false;

                cxt
                    .Save(cxt.Companies, companyToSave, c => c.CompanyId, out newIdFunc)
                    .Save(cxt.CompanyAlias, companyToSave.CompanyAliases, ca => ca.AliasId, ca => ca.CompanyId == companyToSave.CompanyId)
                    .Save(cxt.CompanyBrokerages, companyToSave.Brokerages, br => br.CompanyBrokerageId, br => br.CompanyId == companyToSave.CompanyId);

                cxt.ChangeTracker.DetectChanges();
                cxt.SaveChanges();
                transaction.Commit();

                companyToSave.CompanyId = newIdFunc();
            }
        }

        public bool CheckAliasesUnique(List<string> aliases, int companyId)
        {
            if (aliases == null || aliases.Count == 0)
                return true;

            int numAliasesThatSpecifiedMoreThanOnce = aliases.GroupBy(x => x.Trim())
                .Select(x => new { alias = x.Key, count = x.Sum(y => 1) })
                .Count(alias => alias.count > 1);

            if (numAliasesThatSpecifiedMoreThanOnce > 0)
                return false;

            using (MandaraEntities cxt = CreateMandaraProductsDbContext())
            {
                int count = cxt.CompanyAlias.Count(x => aliases.Contains(x.AliasName.Trim()) && x.Company.CompanyId != companyId);

                return count == 0;
            }
        }
    }
}
