using System;

namespace Mandara.Entities
{
    public abstract class ReplaceableDbContext
    {
        protected Func<MandaraEntities> _dbContextCreator;

        protected ReplaceableDbContext()
        {
            _dbContextCreator = GetMandaraEntities;
        }

        protected MandaraEntities GetMandaraEntities()
        {
            return new MandaraEntities(MandaraEntities.DefaultConnStrName, nameof(ReplaceableDbContext));
        }

        public void SupercedeDbContextCreator(Func<MandaraEntities> dbCxtCreator)
        {
            _dbContextCreator = dbCxtCreator;
        }
    }
}
