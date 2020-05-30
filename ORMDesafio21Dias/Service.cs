using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;

namespace ORMDesafio21Dias
{
    public sealed class Service
    {
        private CType cType;

        public Service(CType _cType)
        {
            this.cType = _cType;
        }

        public void Save()
        {
            using (SqlConnection conn = new SqlConnection(this.cType.ConnectionString))
            {
                List<string> cols = new List<string>();
                List<object> values = new List<object>();

                foreach (var p in this.cType.GetType().GetProperties())
                {
                    if (p.GetValue(this.cType) == null) continue;

                    TableAttribute[] propertyAttributes = (TableAttribute[])p.GetCustomAttributes(typeof(TableAttribute), false);
                    if (propertyAttributes != null && propertyAttributes.Length > 0)
                    {
                        if (!propertyAttributes[0].IsNotOnDataBase && string.IsNullOrEmpty(propertyAttributes[0].PrimaryKey))
                        {
                            cols.Add(p.Name);
                            values.Add(p.GetValue(this.cType));
                        }
                    }
                    else
                    {
                        cols.Add(p.Name);
                        values.Add(p.GetValue(this.cType));
                    }
                }


                string table = this.getTableName();


                string sql = string.Empty;
                if (this.cType.Id == 0)
                {
                    sql = $"insert into {table} (";
                    sql += string.Join(',', cols);
                    sql += ") values ( ";
                    sql += "@" + string.Join(", @", cols);
                    sql += ")";
                }
                else
                {
                    sql = $"update {table} set ";

                    var colsUpdate = new List<string>();
                    foreach (string col in cols)
                    {
                        colsUpdate.Add($"{col}=@${col}");
                    }
                    sql += string.Join(',', colsUpdate);

                    sql += $"where {this.getPkName()} = {this.cType.Id}";
                }

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                for (var i = 0; i < cols.Count; i++)
                {
                    var value = values[i];
                    var col = cols[i];

                    cmd.Parameters.Add($"@{col}", GetDbType(value));
                    cmd.Parameters[$"@{col}"].Value = value;
                }

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private string getTableName()
        {
            var table = $"{this.cType.GetType().Name.ToLower()}s";

            TableAttribute[] tableAttributes = (TableAttribute[])this.cType.GetType().GetCustomAttributes(typeof(TableAttribute), false);
            if (tableAttributes != null && tableAttributes.Length > 0)
            {
                table = tableAttributes[0].Name;
            }
            return table;
        }

        private SqlDbType GetDbType(object value)
        {
            var result = SqlDbType.VarChar;

            try
            {
                Type type = value.GetType();

                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Object:
                        result = SqlDbType.Variant;
                        break;
                    case TypeCode.Boolean:
                        result = SqlDbType.Bit;
                        break;
                    case TypeCode.Char:
                        result = SqlDbType.NChar;
                        break;
                    case TypeCode.SByte:
                        result = SqlDbType.SmallInt;
                        break;
                    case TypeCode.Byte:
                        result = SqlDbType.TinyInt;
                        break;
                    case TypeCode.Int16:
                        result = SqlDbType.SmallInt;
                        break;
                    case TypeCode.UInt16:
                        result = SqlDbType.Int;
                        break;
                    case TypeCode.Int32:
                        result = SqlDbType.Int;
                        break;
                    case TypeCode.UInt32:
                        result = SqlDbType.BigInt;
                        break;
                    case TypeCode.Int64:
                        result = SqlDbType.BigInt;
                        break;
                    case TypeCode.UInt64:
                        result = SqlDbType.Decimal;
                        break;
                    case TypeCode.Single:
                        result = SqlDbType.Real;
                        break;
                    case TypeCode.Double:
                        result = SqlDbType.Float;
                        break;
                    case TypeCode.Decimal:
                        result = SqlDbType.Money;
                        break;
                    case TypeCode.DateTime:
                        result = SqlDbType.DateTime;
                        break;
                    case TypeCode.String:
                        result = SqlDbType.VarChar;
                        break;
                }

            }
            catch (Exception ex)
            {
                throw (ex);
            }

            return result;
        }

        public void Destroy()
        {
            using (SqlConnection conn = new SqlConnection(this.cType.ConnectionString))
            {
                string sql = $"delete from {this.getTableName()} where {this.getPkName()} = {this.cType.Id}";

                SqlCommand cmd = new SqlCommand(sql, conn);
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public void Get()
        {
            using (SqlConnection conn = new SqlConnection(this.cType.ConnectionString))
            {
                string sql = $"select * from {this.getTableName()} where {this.getPkName()} = {this.cType.Id}";

                SqlCommand cmd = new SqlCommand(sql, conn);
                try
                {
                    conn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            this.fill(this.cType, dr);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void fill(CType obj, SqlDataReader dr)
        {
            obj.Id = Convert.ToInt32(dr[this.getPkName()]);

            foreach (var p in obj.GetType().GetProperties())
            {
                TableAttribute[] propertyAttributes = (TableAttribute[])p.GetCustomAttributes(typeof(TableAttribute), false);
                if (propertyAttributes != null && propertyAttributes.Length > 0)
                {
                    if (!propertyAttributes[0].IsNotOnDataBase && string.IsNullOrEmpty(propertyAttributes[0].PrimaryKey))
                    {
                        p.SetValue(obj, dr[p.Name]);
                    }
                }
                else
                {
                    p.SetValue(obj, dr[p.Name]);
                }
            }
        }

        private string  getPkName()
        {
            /*TableAttribute[] propertyAttributes = (TableAttribute[])this.cType.GetType().GetProperty("Id").GetCustomAttributes(typeof(TableAttribute), false);
            if (propertyAttributes != null && propertyAttributes.Length > 0 && !string.IsNullOrEmpty(propertyAttributes[0].PrimaryKey))
            {
                return propertyAttributes[0].PrimaryKey;
            }
            else
            {
                return "id";
            }
            */
            return this.cType.GetType().GetProperty("Id").GetCustomAttribute<TableAttribute>().PrimaryKey;
        }

        public List<CType> All()
        {
            string sql;

            var list = new List<CType>();
            using (SqlConnection conn = new SqlConnection(this.cType.ConnectionString))
            {
                sql = $"select * from {this.getTableName()}";

                SqlCommand cmd = new SqlCommand(sql, conn);
                try
                {
                    conn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var instance = (CType)Activator.CreateInstance(this.cType.GetType());
                            this.fill(instance, dr);
                            list.Add(instance);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                return list;
            }
        }

        public static T All<T>()
        {

            var list = Activator.CreateInstance(typeof(T));
            var intance = ((List<object>)list)[0];

            /*using (SqlConnection conn = new SqlConnection(this.cType.ConnectionString))
            {
                string sql;
                sql = $"select * from {this.getTableName()}";

                SqlCommand cmd = new SqlCommand(sql, conn);
                try
                {
                    conn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var instance = (CType)Activator.CreateInstance(this.cType.GetType());
                            this.fill(instance, dr);
                            list.Add(instance);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                return (T)Convert.ChangeType(list, typeof(T));
            }
            */

            return (T)Convert.ChangeType(list, typeof(T));
        }
    }
}
