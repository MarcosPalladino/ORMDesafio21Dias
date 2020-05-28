using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Runtime.CompilerServices;

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
            throw new NotImplementedException();
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
                        this.cType.Id = Convert.ToInt32(dr[this.getPkName()]);

                        foreach (var p in this.cType.GetType().GetProperties())
                        {
                            TableAttribute[] propertyAttributes = (TableAttribute[])p.GetCustomAttributes(typeof(TableAttribute), false);
                            if (propertyAttributes != null && propertyAttributes.Length > 0)
                            {
                                if (!propertyAttributes[0].IsNotOnDataBase && string.IsNullOrEmpty(propertyAttributes[0].PrimaryKey))
                                {
                                    p.SetValue(this.cType, dr[p.Name]);
                                }
                            }
                            else
                            {
                                p.SetValue(this.cType, dr[p.Name]);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }




        }

        private string  getPkName()
        {
            return this.cType.Id.GetType().GetCustomAttribute<TableAttribute>().PrimaryKey;
        }

        public static T All<T>()
        {
            throw new NotImplementedException();
        }
        /*
        public static T Todos<T>()
        {
            string sql;

            DataTable table = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConectionString()))
            {
                if (typeof(T) == typeof(List<Juridica>))
                    sql = "select * from pessoa where tipo = 'J' and id is not null";
                else
                    sql = "select * from pessoa where tipo = 'F' and id is not null";

                SqlCommand cmd = new SqlCommand(sql, conn);
                try
                {
                    conn.Open();
                    using (SqlDataAdapter a = new SqlDataAdapter(cmd))
                    {
                        a.Fill(table);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            if (typeof(T) == typeof(List<Juridica>))
            {
                var listaJuridica = new List<Juridica>();

                foreach (DataRow linha in table.Rows)
                {
                    listaJuridica.Add(new Juridica()
                    {
                        Nome = linha["nome"].ToString(),
                        Id = Convert.ToInt32(linha["id"]),
                        Endereco = linha["endereco"].ToString(),
                        Cnpj = linha["cpfcnpj"].ToString()
                    });
                }
                return (T)Convert.ChangeType(listaJuridica, typeof(T));
            }
            else
            {
                var listaFisica = new List<Fisica>();

                foreach (DataRow linha in table.Rows)
                {
                    listaFisica.Add(new Fisica()
                    {
                        Nome = linha["nome"].ToString(),
                        Id = Convert.ToInt32(linha["id"]),
                        Endereco = linha["endereco"].ToString(),
                        Cpf = linha["cpfcnpj"].ToString()
                    });
                }
                return (T)Convert.ChangeType(listaFisica, typeof(T));
            }
        }
        */

    }
}
