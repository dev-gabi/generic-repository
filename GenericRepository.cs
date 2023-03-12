using Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dal
{


    public class GenericRepository <TEntity> where TEntity: class, IGenericEntity
    {
        internal AppContext context;
        internal DbSet<TEntity> dbSet;

        public GenericRepository(AppContext _context)
        {
            context = _context;
            this.dbSet = this.context.Set<TEntity>();
        }
        public IEnumerable<TEntity> GetAll()
        {
            return dbSet.ToList();
        }

        public TEntity GetOneFromSqlRaw(string spName, Dictionary<string, string> colomnNamesWithValues)
        {
            string command = CreateStoredProcedureCommand(spName, colomnNamesWithValues);
            IEnumerable<SqlParameter> sqlParameters = GetSqlParameterList(colomnNamesWithValues);
            return dbSet.FromSqlRaw(command, sqlParameters.ToArray()).AsEnumerable<TEntity>().FirstOrDefault();
        }

        public List<TEntity> GetManyFromSqlRaw(string spName, Dictionary<string, string> colomnNamesWithValues)
        {
            string command = CreateStoredProcedureCommand(spName, colomnNamesWithValues);
            IEnumerable<SqlParameter> sqlParameters = GetSqlParameterList(colomnNamesWithValues);
            return dbSet.FromSqlRaw(command, sqlParameters.ToArray()).AsEnumerable<TEntity>().ToList();
        }
        public List<TEntity> GetManyFromSqlRaw(string viewName)
        {
            return dbSet.FromSqlRaw(viewName).AsEnumerable<TEntity>().ToList();
        }

        public void ExecuteSqlRawSP(string spName, Dictionary<string, string> colomnNamesWithValues)
        {

            string command = CreateStoredProcedureCommand(spName, colomnNamesWithValues);
            IEnumerable<SqlParameter> sqlParameters = GetSqlParameterList(colomnNamesWithValues);

            context.Database.ExecuteSqlRaw(command, sqlParameters.ToArray());
        }

        public IQueryable<TEntity> FromSqlRawSP(string spName, Dictionary<string, string> colomnNamesWithValues)
        {

            string command = CreateStoredProcedureCommand(spName, colomnNamesWithValues);
            IEnumerable<SqlParameter> sqlParameters = GetSqlParameterList(colomnNamesWithValues);

            return dbSet.FromSqlRaw(command, sqlParameters.ToArray());
        }

        private string CreateStoredProcedureCommand(string spName, Dictionary<string, string> colomnNamesWithValues)
        {
            string command = spName + ' ';

            for (int i = 0; i < colomnNamesWithValues.Count(); i++)
            {
                command += $"@{colomnNamesWithValues.ElementAt(i).Key}";
                if (i + 1 < colomnNamesWithValues.Count) { command += ','; }
            }
            return command;
        }

        private IEnumerable<SqlParameter> GetSqlParameterList(Dictionary<string, string> colomnNamesWithValues)
        {
            foreach (var item in colomnNamesWithValues)
            {
                yield return new SqlParameter($"@{item.Key}", item.Value);
            }
        }

        public void Delete(string spName, int id)
        {
            context.Database.ExecuteSql($"{spName} {id}");
        }

    }
}
