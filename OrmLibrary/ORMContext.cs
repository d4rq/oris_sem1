using System.Data;
using System.Linq.Expressions;
using System.Text;
using Microsoft.Data.SqlClient;

namespace OrmLibrary
{
    public class OrmContext<T>(IDbConnection dbConnection)
        where T : class, new()
    {
        public bool Create(T entity)
        {
                dbConnection.Open();
                string query = GenerateInsertQuery();

                using (var command = dbConnection.CreateCommand())
                {
                    command.CommandText = query;
                    AddParameters(command, entity);
                    int rowsAffected = command.ExecuteNonQuery();
                    
                    dbConnection.Close();
                    return rowsAffected > 0;
                }
        }

        public T ReadById(Guid id)
        {
            string query = $"SELECT * FROM {typeof(T).Name}s WHERE Id=@id";
            using (var command = dbConnection.CreateCommand())
            {
                command.CommandText = query;
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@id";
                parameter.Value = id;
                command.Parameters.Add(parameter);

                dbConnection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                        return Map(reader);
                }
            }
            return new T();
        }

        public List<T> ReadAll()
        {
            string query = $"SELECT * FROM {typeof(T).Name}s";

            var result = new List<T>();
            using (var command = dbConnection.CreateCommand())
            {
                command.CommandText = query;

                dbConnection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        result.Add(Map(reader));
                }
            }
            dbConnection.Close();
            return result;
        }

        public bool Update(T entity)
        {
            //dbConnection.Open();
            string query = GenerateUpdateQuery();

            using (var command = dbConnection.CreateCommand())
            {
                AddParameters(command, entity);
                command.CommandText = query;
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public bool Delete(int id)
        {
            string query = $"DELETE FROM {typeof(T).Name}s WHERE Id=@id";

            using (var command = dbConnection.CreateCommand())
            {
                command.CommandText = query;
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@id";
                parameter.Value = id;
                command.Parameters.Add(parameter);

                dbConnection.Open();
                var result = command.ExecuteNonQuery() == 1;
                dbConnection.Close();
                return result;
            }
        }

        public List<T> Where(Expression<Func<T, bool>> predicate)
        {
            var sqlQuery = ExpressionParser<T>.BuildSqlQuery(predicate, singleResult: false);
            return ExecuteQueryMultiple(sqlQuery).ToList();
        }
        
        public T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            var sqlQuery = ExpressionParser<T>.BuildSqlQuery(predicate, singleResult: true);
            return ExecuteQuerySingle(sqlQuery);
        }

        private T Map(IDataReader reader)
        {
            var entity = new T();
            var props = typeof(T).GetProperties();

            foreach (var property in props)
            {
                if (reader[property.Name] != DBNull.Value)
                {
                    property.SetValue(entity, reader[property.Name]);
                }
            }
            return entity;
        }
        
        private T ExecuteQuerySingle(string query)
        {
            using (var command = dbConnection.CreateCommand())
            {
                command.CommandText = query;
                dbConnection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return Map(reader);
                    }
                }
                dbConnection.Close();
            }
 
            return null;
        }
 
        private IEnumerable<T> ExecuteQueryMultiple(string query)
        {
            var results = new List<T>();
            using (var command = dbConnection.CreateCommand())
            {
                command.CommandText = query;
                dbConnection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(Map(reader));
                    }
                }
                dbConnection.Close();
            }
            return results;
        }
        
        private string GenerateInsertQuery()
        {
            var properties = typeof(T).GetProperties();
            if (properties == null) throw new Exception(); 

            var columns = string.Join(", ", properties.Where(p => p.Name != "Id").Select(p => $"{p.Name}"));
            var values = string.Join(", ", properties.Where(p => p.Name != "Id").Select(p => $"@{p.Name}"));
            return $"INSERT INTO {typeof(T).Name.ToLower()}s ({columns}) VALUES ({values})";
        }
        
        private void AddParameters(IDbCommand command, T entity)
        {
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                if (property.Name == "Id") continue;
                var value = property.GetValue(entity) ?? DBNull.Value;
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@" + property.Name;
                parameter.Value = value;
                command.Parameters.Add(parameter);
            }
        }
        
        private string GenerateUpdateQuery()
        {
            var properties = typeof(T).GetProperties();
            var setClauses = string.Join(", ", properties.Select(p => $"{p.Name} = @{p.Name}"));
            return $"UPDATE {typeof(T).Name}s SET {setClauses} WHERE Id = @Id";
        }
    }
}
