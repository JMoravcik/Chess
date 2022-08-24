using Chess.Entities.Models;
using Chess.Shared.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Entities
{
    internal static class DbSetExtensions
    {
        internal static void AddEntities<TEntity, TDto>(this DbSet<TEntity> dbSet, string createdBy, params TDto[] dtos)
            where TEntity : Model<TDto>
            where TDto : Dto
        {
            if (dtos == null) return;
            foreach (var dto in dtos)
            {
                TEntity entity = Convert.ChangeType(dto, typeof(TEntity)) as TEntity;
                entity.Id = Guid.NewGuid();
                entity.CreatedAt = DateTime.Now;
                entity.CreatedBy = createdBy;
                entity.UpdatedBy = createdBy;
                entity.UpdatedAt = DateTime.Now;
                dbSet.Add(entity);
            }
        }

        internal static void UpdateEntities<TEntity, TDto>(this DbSet<TEntity> dbSet, string updatedBy, params TDto[] dtos)
            where TEntity : Model<TDto>
            where TDto : Dto
        {
            if (dtos == null) return;
            foreach (var dto in dtos)
            {
                TEntity entity = Convert.ChangeType(dto, typeof(TEntity)) as TEntity;
                entity.UpdatedBy = updatedBy;
                entity.UpdatedAt = DateTime.Now;
                dbSet.Update(entity);
            }
        }

        internal static void RemoveEntities<TEntity, TDto>(this DbSet<TEntity> dbSet, string updatedBy, params TDto[] dtos)
            where TEntity : Model<TDto>
            where TDto : Dto
        {
            if (dtos == null) return;
            foreach (var dto in dtos)
            {
                TEntity entity = Convert.ChangeType(dto, typeof(TEntity)) as TEntity;
                dbSet.Remove(entity);
            }
        }
    }
}
