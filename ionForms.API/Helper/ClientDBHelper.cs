using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace ionForms.API.Helper
{
    public class ClientDBHelper
    {
        int _accountId = 0;
        int _formId = 0;
        string _connectionString = "";
        string _clientTablePrefix = "";

        public ClientDBHelper(int accountId, int formId, string connectionString, string clientTablePrefix = "tblClient_")
        {
            _accountId = accountId;
            _formId = formId;
            _connectionString = connectionString;
            _clientTablePrefix = clientTablePrefix;
        }

        public void ExecuteNonQuery(string queryString)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public string GetClientTableName()
        {
            return _clientTablePrefix + _accountId + "_" + _formId;
        }

        public bool TableExists(string tableName)
        {
            bool tableExists = false;
            SqlConnection connection = new SqlConnection(_connectionString);

            //IF OBJECT_ID (N'tblClient_3_5', N'U') IS NOT NULL SELECT 1 AS table_exists ELSE SELECT 0 AS table_exists;
            var sqlQuery = "IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = '" + tableName + "') SELECT 1 AS table_exists ELSE SELECT 0 AS table_exists;";
            using (connection)
            {
                SqlCommand command = new SqlCommand(
                  sqlQuery,
                  connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        tableExists = reader.GetInt32(0) == 1 ? true : false;
                    }
                }
                else
                {
                    throw new System.Exception("Error reading TableExists");
                }
                reader.Close();
            }

            return tableExists;
        }

        /// <summary>
        /// Create client table from columns
        /// </summary>
        /// <param name="columnEntity"></param>
        public void CreateTable(IEnumerable<Entities.Column> columnEntity)
        {
            string tableName = _clientTablePrefix + _accountId + "_" + _formId + "";
            string tableNameFull = "[dbo].[" + tableName + "]";
            string cols = "[Id] [int] IDENTITY(1,1) NOT NULL,";

            foreach (Entities.Column column in columnEntity)
            {
                cols += column.ColumnName + " varchar(100),";
            }

            var constrains = "CONSTRAINT [PK_" + _clientTablePrefix + _accountId + "_" + _formId + "] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]";
            var sqlQuery = "CREATE TABLE " + tableNameFull + " (" + cols + constrains + ") ON [PRIMARY]";
            ExecuteNonQuery(sqlQuery);
        }

        public void UpdateTable(IEnumerable<Entities.Column> columnEntity)
        {
            List<Entities.Column> deltaColumns = GetDeltaColumns(columnEntity);
            string tableName = GetClientTableName();
            string tableNameFull = "[dbo].[" + tableName + "]";
            var alterTableSql = "";
            var alterColumnSql = "";

            foreach (Entities.Column deltaColumn in deltaColumns)
            {
                //var c_col = deltaColumn.ColumnName;
                var colType = deltaColumn.ColumnType;
                var colLength = deltaColumn.ColumnLength;
                var colTypeLength = "";
                if (colType == "" || colType == null) { colType = "varchar"; }
                if (colLength == "" || colLength == null) { colLength = "200"; }
                colTypeLength = colType + "(" + colLength + ")";

                alterColumnSql += deltaColumn.ColumnName + " " + colTypeLength + ",";
            }

            alterColumnSql = (alterColumnSql.TrimEnd(','));
            if (alterColumnSql.Length > 0)
            {
                alterTableSql = "ALTER TABLE " + tableNameFull + " ADD " + alterColumnSql;
                ExecuteNonQuery(alterTableSql);
            }
        }

        public List<Entities.Column> GetDeltaColumns(IEnumerable<Entities.Column> columnEntity)
        {
            List<Entities.Column> columnEntityDelta = new List<Entities.Column>();
            List<object[]> rowData = GetData("SELECT TABLE_SCHEMA, COLUMN_NAME, DATA_TYPE, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'tblClient_3_5'");
            foreach (Entities.Column column in columnEntity)
            {
                var colToCompare = column.ColumnName;
                bool colExists = false;
                foreach (object[] dbRow in rowData)
                {
                    if (colToCompare.ToLowerInvariant() == dbRow[1].ToString().ToLowerInvariant())
                    {
                        colExists = true;
                        break;
                    }
                }

                //If column does not exist in DB then add the delta column
                if (!colExists)
                {
                    columnEntityDelta.Add(column);
                }
            }

            return columnEntityDelta;
        }

        public string GetClientDataReadQuery()
        {
            string readQuery = "";
            string tableName = GetClientTableName();
            string tableNameFull = "[dbo].[" + tableName + "]";

            readQuery = "SELECT * FROM " + tableNameFull;
            return readQuery;
        }

        public List<object[]> GetData(string sqlQuery)
        {
            List<object[]> outData = new List<object[]>();

            SqlConnection connection = new SqlConnection(_connectionString);

            using (connection)
            {
                SqlCommand command = new SqlCommand(
                  sqlQuery,
                  connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    object[] rowData = null;
                    while (reader.Read())
                    {
                        rowData = new object[reader.FieldCount];
                        var count = reader.GetSqlValues(rowData);
                        outData.Add(rowData);
                    }
                }

                reader.Close();
            }

            return outData;
        }

        public void InsertClientData(IEnumerable<Entities.Column> columnEntity)
        {
            var sqlQuery = "";
            var colNames = "";
            var colValues = "";
            string tableName = GetClientTableName();
            string tableNameFull = "[dbo].[" + tableName + "]";

            //INSERT INTO table_name(column1, column2, column3, ...)
            //VALUES(value1, value2, value3, ...);

            foreach (Entities.Column col in columnEntity)
            {
                colNames += col.ColumnName + ",";
                colValues += "'" + col.ColumnValue + "',";
            }

            colNames = colNames.TrimEnd(',');
            colValues = colValues.TrimEnd(',');
            sqlQuery = "INSERT INTO " + tableNameFull + "(" + colNames + ") VALUES(" + colValues + ")";

            ExecuteNonQuery(sqlQuery);
        }

        public StringBuilder GetClientDataPyDataFrameCSV(string sqlQuery)
        {
            //sqlQuery = "SELECT ROW_NUMBER() OVER(ORDER BY id ASC) as [sno], [Id] ,[Col1] ,[Col1_] ,[Col1_1] ,[Col1_2] ,[Col1_3] ,[Col1_4] ,[Col2] FROM [dbo].[tblClient_3_5]";
            StringBuilder sb = new StringBuilder();

            SqlConnection connection = new SqlConnection(_connectionString);

            using (connection)
            {
                SqlCommand command = new SqlCommand(
                  sqlQuery,
                  connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    bool hasHeader = false;
                    while (reader.Read())
                    {
                        int fldCount = reader.FieldCount;
                        var colName = "";

                        for (int intI=0; intI < fldCount; intI++)
                        {
                            var dataValue = "";
                            var dataType = reader.GetDataTypeName(intI);
                            if(!hasHeader) { colName += "\"" + reader.GetName(intI) + "\","; }

                            switch (dataType)
                            {
                                case "varchar":
                                    if (!reader.IsDBNull(intI)) { dataValue = "\"" + reader.GetString(intI) + "\""; }
                                    break;
                                case "int":
                                    if (!reader.IsDBNull(intI)) { dataValue = reader.GetInt32(intI).ToString(); }
                                    break;
                                case "bigint":
                                    break;
                                default:
                                    dataValue = "\"\"";
                                    break;
                            }

                            sb.Append(dataValue);
                            sb.Append(",");
                        }

                        if(!hasHeader)
                        {
                            colName = colName.TrimEnd(',');
                            sb.Insert(0, colName + "\n");
                            hasHeader = true;
                        }

                        sb.AppendLine();
                    }
                }
                else
                {
                    sb.Append("No Records Found");
                }
                reader.Close();
            }

            return sb;
        }

        public void DropClientTable()
        {
            var sqlQuery = "";
            string tableName = GetClientTableName();
            string tableNameFull = "[dbo].[" + tableName + "]";

            //IF EXISTS(Select * from INFORMATION_SCHEMA.TABLES where table_name = 'tblClient_1252_1950' and TABLE_SCHEMA = 'dbo') DROP TABLE [dbo].[tblClient_1251_194d9]
            sqlQuery = "IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES where table_name = '" + tableName + "' and TABLE_SCHEMA = 'dbo') DROP TABLE " + tableNameFull;
            //sqlQuery = "IF EXISTS(SELECT * FROM " + tableNameFull + ") DROP TABLE " + tableNameFull;
            ExecuteNonQuery(sqlQuery);
        }
    }
}