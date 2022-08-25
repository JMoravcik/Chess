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
        internal static List<TEntity> AddEntities<TEntity, TDto>(this DbSet<TEntity> dbSet, string createdBy, params TEntity[] entities)
            where TEntity : Model<TDto>
            where TDto : Dto
        {
            var result = new List<TEntity>();
            if (entities == null) return result;
            foreach (var entity in entities)
            {
                entity.Id = Guid.NewGuid();
                entity.CreatedAt = DateTime.Now;
                entity.CreatedBy = createdBy;
                entity.UpdatedBy = createdBy;
                entity.UpdatedAt = DateTime.Now;
                result.Add(entity);
                dbSet.Add(entity);
            }
            return result;
        }

        internal static List<TEntity> UpdateEntities<TEntity, TDto>(this DbSet<TEntity> dbSet, string updatedBy, params TEntity[] entities)
            where TEntity : Model<TDto>
            where TDto : Dto
        {
            var result = new List<TEntity>();

            if (entities == null) return result;
            foreach (var entity in entities)
            {
                entity.UpdatedBy = updatedBy;
                entity.UpdatedAt = DateTime.Now;
                result.Add(entity);
                dbSet.Update(entity);
            }
            return result;
        }


        internal static List<TEntity> RemoveEntities<TEntity, TDto>(this DbSet<TEntity> dbSet, string updatedBy, params TEntity[] entities)
            where TEntity : Model<TDto>
            where TDto : Dto
        {
            var result = new List<TEntity>();
            if (entities == null) return result;
            foreach (var entity in entities)
            {
                dbSet.Remove(entity);
            }
            return result;
        }
    }
}
