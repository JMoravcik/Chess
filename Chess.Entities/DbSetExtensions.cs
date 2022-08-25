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
        internal static List<TEntity> AddEntities<TEntity, TDto>(this DbSet<TEntity> dbSet, string createdBy, params TDto[] dtos)
            where TEntity : Model<TDto>, new()
            where TDto : Dto
        {
            var result = new List<TEntity>();
            if (dtos == null)  return result;
            foreach (var dto in dtos)
            {
                TEntity entity = new();
                entity.SetFrom(dto);
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

        internal static List<TEntity> UpdateEntities<TEntity, TDto>(this DbSet<TEntity> dbSet, string updatedBy, params TDto[] dtos)
            where TEntity : Model<TDto>, new()
            where TDto : Dto
        {
            if (dtos == null) return new List<TEntity>();
            var ids = dtos.Select(d => d.Id).ToList();
            var entities = dbSet.Where(e => ids.Contains(e.Id)).ToList();
            foreach (var dto in dtos)
            {
                var entity = entities.FirstOrDefault(e => e.Id == dto.Id);
                if (entity == null) continue;
                entity.SetFrom(dto);
                entity.UpdatedBy = updatedBy;
                entity.UpdatedAt = DateTime.Now;
            }
            return entities;
        }

        internal static List<TEntity> RemoveEntities<TEntity, TDto>(this DbSet<TEntity> dbSet, string updatedBy, params TDto[] dtos)
            where TEntity : Model<TDto>, new()
            where TDto : Dto
        {
            if (dtos == null) return new List<TEntity>();
            var ids = dtos.Select(d => d.Id).ToList();
            var entities = dbSet.Where(e => ids.Contains(e.Id)).ToList();
            foreach (var dto in dtos)
            {
                dbSet.RemoveRange(entities);
            }
            return entities;
        }
    }
}
