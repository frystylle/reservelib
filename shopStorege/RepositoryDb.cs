using System;
using Npgsql;
using System.Collections.Generic;
using storege.Interfaces;
using System.Threading.Tasks;
using System.Data;

namespace storege
{
    public class RepositoryDb : IRepositoryDb
    {
        readonly string connString = "Server=localhost;Port=5432;Database=ShopStorage;User Id=postgres;Password=postgres;";

        public RepositoryDb()
        {

        }

        public async Task<string> SetReserve(int idUser, string productName, int countReserve)
        {
            var idProduct = 0;
            var idReserve = 0;
            var freeCount = 0;
            var nowCountReserve = 0;

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                var trans = conn.BeginTransaction();

                // Запрос на id и количество доступного продукта 
                using (var command = new NpgsqlCommand("SELECT free_count, id from public.products where public.products.name=:value1 FOR UPDATE", conn))
                {
                    command.Parameters.Add(new NpgsqlParameter("value1", DbType.String));
                    command.Prepare();
                    command.Parameters[0].Value = productName.ToString();
                    try
                    {
                        NpgsqlDataReader dr = command.ExecuteReader();
                        dr.Read();
                        freeCount = Convert.ToInt32(dr[0]);
                        idProduct = Convert.ToInt32(dr[1]);
                        dr.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        throw ex;
                    }
                    command.Cancel();
                }
                // Если доступного продукта хватает для брони, бронируем
                if (freeCount >= countReserve)
                {
                    using (var command = new NpgsqlCommand("UPDATE public.products SET free_count=:value1 WHERE public.products.id=:value2", conn))
                    {
                        command.Parameters.Add(new NpgsqlParameter("value1", DbType.Int32));
                        command.Parameters.Add(new NpgsqlParameter("value2", DbType.Int32));
                        command.Prepare();
                        command.Parameters[0].Value = (freeCount - countReserve);
                        command.Parameters[1].Value = idProduct;

                        var updCoutn = 0;
                        try
                        {
                            updCoutn = await command.ExecuteNonQueryAsync();
                        }
                        catch (Exception ex)
                        {
                            conn.Close();
                            throw ex;
                        }

                        if (updCoutn != 1)
                        {
                            conn.Close();
                            return await Task.FromResult("out of stock");
                        }
                        command.Cancel();
                    }
                    // Узнаем имеются ли записи о брони данного товара пользователем
                    using (var command = new NpgsqlCommand("SELECT id, count_reserve from public.reserves where (id_product=:value1 and id_user =:value2) FOR UPDATE", conn))
                    {
                        command.Parameters.Add(new NpgsqlParameter("value1", DbType.Int32));
                        command.Parameters.Add(new NpgsqlParameter("value2", DbType.Int32));
                        command.Prepare();
                        command.Parameters[0].Value = idProduct;
                        command.Parameters[1].Value = idUser;
                        try
                        {
                            NpgsqlDataReader dr = command.ExecuteReader();
                            dr.Read();
                            if (dr.HasRows)
                            {
                                idReserve = Convert.ToInt32(dr[0]);
                                nowCountReserve = Convert.ToInt32(dr[1]);
                            }
                            dr.Close();
                        }
                        catch (Exception ex)
                        {
                            conn.Close();
                            throw ex;
                        }
                        command.Cancel();
                    }

                    if (nowCountReserve > 0)
                    {   // Запись имеется, обновляем ее с учетом новой брони
                        using (var command = new NpgsqlCommand("UPDATE public.reserves SET count_reserve=:value1 WHERE public.reserves.id=:value2", conn))
                        {
                            command.Parameters.Add(new NpgsqlParameter("value1", DbType.Int32));
                            command.Parameters.Add(new NpgsqlParameter("value2", DbType.Int32));
                            command.Prepare();
                            command.Parameters[0].Value = (nowCountReserve + countReserve);
                            command.Parameters[1].Value = idReserve;

                            var updCoutn = 0;
                            try
                            {
                                updCoutn = await command.ExecuteNonQueryAsync();
                            }
                            catch (Exception ex)
                            {
                                conn.Close();
                                throw ex;
                            }

                            if (updCoutn != 1)
                            {
                                conn.Close();
                                return await Task.FromResult("failure update");
                            }
                            command.Cancel();
                        }
                        trans.Commit();
                        conn.Close();
                        return await Task.FromResult("success update");
                    }
                    else
                    {   // Бронь производится впервые, добавляем запись
                        using (var command = new NpgsqlCommand("INSERT INTO public.reserves(id_product, id_user, count_reserve)VALUES(:value1, :value2, :value3)", conn))
                        {
                            command.Parameters.Add(new NpgsqlParameter("value1", DbType.Int32));
                            command.Parameters.Add(new NpgsqlParameter("value2", DbType.Int32));
                            command.Parameters.Add(new NpgsqlParameter("value3", DbType.Int32));
                            command.Prepare();
                            command.Parameters[0].Value = idProduct;
                            command.Parameters[1].Value = idUser;
                            command.Parameters[2].Value = countReserve;

                            var updCoutn = 0;
                            try
                            {
                                updCoutn = await command.ExecuteNonQueryAsync();
                            }
                            catch (Exception ex)
                            {
                                conn.Close();
                                throw ex;
                            }

                            if (updCoutn != 1)
                            {
                                conn.Close();
                                return await Task.FromResult("failure insert");
                            }
                            command.Cancel();
                        }
                        trans.Commit();
                        conn.Close();
                        return await Task.FromResult("success insert");
                    }
                }
                else
                {
                    conn.Close();
                    return await Task.FromResult("out of stock");
                }
            }
        }
    }
}
