﻿namespace Microsoft.MecSolutionAccelerator.Services.Commons
{
    public interface IBaseRepository<TEntity, IdType> where TEntity : AEntity
    {
        Task<TEntity> GetById(IdType id);
        Task<IEnumerable<TEntity>> List();
        Task<IEnumerable<TEntity>> List(int skip, int take);
        Task Create(TEntity entity);
        Task Update(TEntity entity);
        Task Delete(IdType id);
    }
}
