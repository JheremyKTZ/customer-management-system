using System.Collections.Generic;

namespace Stark.BL.Repositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IList<TEntity> RetrieveAll();
        bool Save(TEntity newEntity);
        bool Delete(int entityId);
        TEntity Retrieve(int entityId);
    }
}
