using System;
using Npgsql;
using System.Collections.Generic;
using storege.Interfaces;
using System.Threading.Tasks;

namespace storege
{
    public class ConnectToDb : IConnectToDb
    {
        readonly string connString = "Server=localhost;Port=5432;Database=ShopStorage;User Id=postgres; Password=postgres;";

        public ConnectToDb()
        {

        }

        public async Task<string> SetReserve(string token, string productName, int count)
        {
            using (var connection = new NpgsqlConnection(connString))
            {
                connection.Open();
                var trans = connection.BeginTransaction();

                using (var command = new NpgsqlCommand("reserve", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "user_token";
                    parameter.DbType = System.Data.DbType.AnsiString;
                    parameter.Value = $"{token}";
                    command.Parameters.Add(parameter);

                    var parameter2 = command.CreateParameter();
                    parameter2.ParameterName = "product_name";
                    parameter2.DbType = System.Data.DbType.AnsiString;
                    parameter2.Value = $"{productName}";
                    command.Parameters.Add(parameter2);

                    var parameter3 = command.CreateParameter();
                    parameter3.ParameterName = "reserve_count";
                    parameter3.DbType = System.Data.DbType.VarNumeric;
                    parameter3.Value = count;
                    command.Parameters.Add(parameter3);

                    Object result = await command.ExecuteScalarAsync();
                    
                    trans.Commit();
                    connection.Close();
                    return result.ToString();

                }
            }
        }





    }
}
