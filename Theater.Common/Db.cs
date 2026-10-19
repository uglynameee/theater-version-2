using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Theater.Common
{
    public sealed class FieldDefinition
    {
        public string Name, Label, Kind, LookupTable, LookupLabel;
        public FieldDefinition(string name, string label, string kind, string lookupTable = null, string lookupLabel = null)
        { Name = name; Label = label; Kind = kind; LookupTable = lookupTable; LookupLabel = lookupLabel; }
    }

    public sealed class TableDefinition
    {
        public string Name, Label;
        public FieldDefinition[] Fields;
        public TableDefinition(string name, string label, params FieldDefinition[] fields) { Name = name; Label = label; Fields = fields; }
    }

    public sealed class AuthUser
    {
        public string Login { get; private set; }
        public string Role { get; private set; }
        public int? SpectatorId { get; private set; }
        public bool IsEmployee { get { return Role == "Сотрудник"; } }

        public AuthUser(string login, string role, int? spectatorId = null) { Login = login; Role = role; SpectatorId = spectatorId; }
    }

    public static class Db
    {
        private static string _path;
        private static string _connectionString;

        public static readonly TableDefinition[] Tables = {
            new TableDefinition("Театры", "Театры",
                new FieldDefinition("НазваниеТеатра", "Название", "text"), new FieldDefinition("Адрес", "Адрес", "text")),
            new TableDefinition("Режиссёры", "Режиссёры",
                new FieldDefinition("Имя", "Имя", "text"), new FieldDefinition("Фамилия", "Фамилия", "text")),
            new TableDefinition("Актёры", "Актёры",
                new FieldDefinition("Имя", "Имя", "text"), new FieldDefinition("Фамилия", "Фамилия", "text")),
            new TableDefinition("Спектакли", "Спектакли",
                new FieldDefinition("Название", "Название", "text"), new FieldDefinition("КодТеатра", "Театр", "fk", "Театры", "НазваниеТеатра"),
                new FieldDefinition("КодРежиссёра", "Режиссёр", "fk", "Режиссёры", "Фамилия"), new FieldDefinition("ДатаВремя", "Дата и время", "date"),
                new FieldDefinition("ПродолжительностьМинут", "Продолжительность, мин.", "int"), new FieldDefinition("Жанр", "Жанр", "text"),
                new FieldDefinition("ЦенаБилета", "Цена билета", "money"), new FieldDefinition("Описание", "Описание", "memo")),
            new TableDefinition("СоставСпектакля", "Состав спектаклей",
                new FieldDefinition("КодСпектакля", "Спектакль", "fk", "Спектакли", "Название"), new FieldDefinition("КодАктёра", "Актёр", "fk", "Актёры", "Фамилия"),
                new FieldDefinition("Роль", "Роль", "text")),
            new TableDefinition("Зрители", "Зрители",
                new FieldDefinition("Имя", "Имя", "text"), new FieldDefinition("Фамилия", "Фамилия", "text"), new FieldDefinition("Телефон", "Телефон", "text")),
            new TableDefinition("Билеты", "Билеты",
                new FieldDefinition("КодЗрителя", "Зритель", "fk", "Зрители", "Фамилия"), new FieldDefinition("КодСпектакля", "Спектакль", "fk", "Спектакли", "Название"),
                new FieldDefinition("Место", "Место", "text"), new FieldDefinition("ЦенаБилета", "Цена", "money"),
                new FieldDefinition("ДатаПокупки", "Дата покупки", "date"), new FieldDefinition("Статус", "Статус", "text")),
            new TableDefinition("Оценки", "Оценки",
                new FieldDefinition("КодЗрителя", "Зритель", "fk", "Зрители", "Фамилия"), new FieldDefinition("КодСпектакля", "Спектакль", "fk", "Спектакли", "Название"),
                new FieldDefinition("Оценка", "Оценка (1-5)", "int"), new FieldDefinition("Отзыв", "Отзыв", "memo"), new FieldDefinition("ДатаОценки", "Дата", "date"))
        };

        public static string DatabasePath { get { Initialize(); return _path; } }

        public static void Initialize()
        {
            if (_connectionString != null) return;
            string configuredPath = Environment.GetEnvironmentVariable("THEATER_DATABASE_PATH");
            if (!string.IsNullOrWhiteSpace(configuredPath))
            {
                _path = Path.GetFullPath(configuredPath);
                Directory.CreateDirectory(Path.GetDirectoryName(_path));
            }
            else
            {
                string root = FindProjectRoot(AppDomain.CurrentDomain.BaseDirectory);
                string dataDir = Path.Combine(root, "Database");
                Directory.CreateDirectory(dataDir);
                _path = Path.Combine(dataDir, "Театр.accdb");
            }
            _connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + _path + ";";
            if (!File.Exists(_path)) CreateAccessFile(_connectionString);
            try
            {
                using (OleDbConnection connection = Open())
                {
                    DataTable existingTables = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    bool hasTheatres = false;
                    bool hasAccounts = false;
                    foreach (DataRow row in existingTables.Rows)
                    {
                        string tableName = Convert.ToString(row["TABLE_NAME"]);
                        if (string.Equals(tableName, "Театры", StringComparison.OrdinalIgnoreCase)) hasTheatres = true;
                        if (string.Equals(tableName, "ПользователиСистемы", StringComparison.OrdinalIgnoreCase)) hasAccounts = true;
                    }
                    if (hasTheatres)
                    {
                        EnsureAccounts(connection, hasAccounts);
                        return;
                    }
                    string[] ddl = {
                        "CREATE TABLE [Театры] ([Код] COUNTER CONSTRAINT PK_Theatres PRIMARY KEY, [НазваниеТеатра] TEXT(120) NOT NULL, [Адрес] TEXT(255))",
                        "CREATE TABLE [Режиссёры] ([Код] COUNTER CONSTRAINT PK_Directors PRIMARY KEY, [Имя] TEXT(80) NOT NULL, [Фамилия] TEXT(80) NOT NULL)",
                        "CREATE TABLE [Актёры] ([Код] COUNTER CONSTRAINT PK_Actors PRIMARY KEY, [Имя] TEXT(80) NOT NULL, [Фамилия] TEXT(80) NOT NULL)",
                        "CREATE TABLE [Спектакли] ([Код] COUNTER CONSTRAINT PK_Plays PRIMARY KEY, [Название] TEXT(150) NOT NULL, [КодТеатра] LONG, [КодРежиссёра] LONG, [ДатаВремя] DATETIME, [ПродолжительностьМинут] LONG, [Жанр] TEXT(80), [ЦенаБилета] CURRENCY, [Описание] MEMO)",
                        "CREATE TABLE [СоставСпектакля] ([Код] COUNTER CONSTRAINT PK_CastMembers PRIMARY KEY, [КодСпектакля] LONG NOT NULL, [КодАктёра] LONG NOT NULL, [Роль] TEXT(100))",
                        "CREATE TABLE [Зрители] ([Код] COUNTER CONSTRAINT PK_Spectators PRIMARY KEY, [Имя] TEXT(80) NOT NULL, [Фамилия] TEXT(80) NOT NULL, [Телефон] TEXT(40))",
                        "CREATE TABLE [Билеты] ([Код] COUNTER CONSTRAINT PK_Tickets PRIMARY KEY, [КодЗрителя] LONG NOT NULL, [КодСпектакля] LONG NOT NULL, [Место] TEXT(30), [ЦенаБилета] CURRENCY, [ДатаПокупки] DATETIME, [Статус] TEXT(30))",
                        "CREATE TABLE [Оценки] ([Код] COUNTER CONSTRAINT PK_Ratings PRIMARY KEY, [КодЗрителя] LONG NOT NULL, [КодСпектакля] LONG NOT NULL, [Оценка] LONG NOT NULL, [Отзыв] MEMO, [ДатаОценки] DATETIME)",
                        "ALTER TABLE [Спектакли] ADD CONSTRAINT FK_Plays_Theatres FOREIGN KEY ([КодТеатра]) REFERENCES [Театры] ([Код])",
                        "ALTER TABLE [Спектакли] ADD CONSTRAINT FK_Plays_Directors FOREIGN KEY ([КодРежиссёра]) REFERENCES [Режиссёры] ([Код])",
                        "ALTER TABLE [СоставСпектакля] ADD CONSTRAINT FK_Cast_Play FOREIGN KEY ([КодСпектакля]) REFERENCES [Спектакли] ([Код])",
                        "ALTER TABLE [СоставСпектакля] ADD CONSTRAINT FK_Cast_Actor FOREIGN KEY ([КодАктёра]) REFERENCES [Актёры] ([Код])",
                        "ALTER TABLE [Билеты] ADD CONSTRAINT FK_Ticket_Spectator FOREIGN KEY ([КодЗрителя]) REFERENCES [Зрители] ([Код])",
                        "ALTER TABLE [Билеты] ADD CONSTRAINT FK_Ticket_Play FOREIGN KEY ([КодСпектакля]) REFERENCES [Спектакли] ([Код])",
                        "ALTER TABLE [Оценки] ADD CONSTRAINT FK_Rating_Spectator FOREIGN KEY ([КодЗрителя]) REFERENCES [Зрители] ([Код])",
                        "ALTER TABLE [Оценки] ADD CONSTRAINT FK_Rating_Play FOREIGN KEY ([КодСпектакля]) REFERENCES [Спектакли] ([Код])"
                    };
                    foreach (string sql in ddl) using (OleDbCommand command = new OleDbCommand(sql, connection)) command.ExecuteNonQuery();
                    EnsureAccounts(connection, false);
                }
            }
            catch
            {
                _connectionString = null;
                throw;
            }
        }

        private static void EnsureAccounts(OleDbConnection connection, bool tableExists)
        {
            if (!tableExists)
                using (OleDbCommand command = new OleDbCommand("CREATE TABLE [ПользователиСистемы] ([Код] COUNTER CONSTRAINT PK_SystemUsers PRIMARY KEY, [Логин] TEXT(80) NOT NULL, [ХэшПароля] TEXT(64) NOT NULL, [Роль] TEXT(20) NOT NULL, [КодЗрителя] LONG)", connection))
                    command.ExecuteNonQuery();
            else
            {
                DataTable columns = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] { null, null, "ПользователиСистемы", null });
                bool hasSpectatorId = false;
                foreach (DataRow column in columns.Rows)
                    if (string.Equals(Convert.ToString(column["COLUMN_NAME"]), "КодЗрителя", StringComparison.OrdinalIgnoreCase)) hasSpectatorId = true;
                if (!hasSpectatorId)
                    using (OleDbCommand command = new OleDbCommand("ALTER TABLE [ПользователиСистемы] ADD COLUMN [КодЗрителя] LONG", connection)) command.ExecuteNonQuery();
            }

            SeedAccount(connection, "sotrudnik", "1234", "Сотрудник");
            SeedAccount(connection, "zritel", "1234", "Зритель");
        }

        private static void SeedAccount(OleDbConnection connection, string login, string password, string role)
        {
            using (OleDbCommand check = new OleDbCommand("SELECT COUNT(*) FROM [ПользователиСистемы] WHERE [Логин]=?", connection))
            {
                check.Parameters.AddWithValue("?", login);
                if (Convert.ToInt32(check.ExecuteScalar()) > 0) return;
            }
            using (OleDbCommand insert = new OleDbCommand("INSERT INTO [ПользователиСистемы] ([Логин],[ХэшПароля],[Роль]) VALUES (?,?,?)", connection))
            {
                insert.Parameters.AddWithValue("?", login);
                insert.Parameters.AddWithValue("?", PasswordHash(password));
                insert.Parameters.AddWithValue("?", role);
                insert.ExecuteNonQuery();
            }
        }

        private static string PasswordHash(string password)
        {
            using (SHA256 sha = SHA256.Create())
                return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(password))).Replace("-", "");
        }

        public static AuthUser Authenticate(string login, string password)
        {
            Initialize();
            using (OleDbConnection connection = Open())
            using (OleDbCommand command = new OleDbCommand("SELECT [Логин],[Роль],[КодЗрителя] FROM [ПользователиСистемы] WHERE [Логин]=? AND [ХэшПароля]=?", connection))
            {
                command.Parameters.AddWithValue("?", login.Trim());
                command.Parameters.AddWithValue("?", PasswordHash(password));
                using (OleDbDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read()) return null;
                    object spectatorId = reader["КодЗрителя"];
                    return new AuthUser(Convert.ToString(reader["Логин"]), Convert.ToString(reader["Роль"]), spectatorId == DBNull.Value ? (int?)null : Convert.ToInt32(spectatorId));
                }
            }
        }

        public static AuthUser RegisterViewer(string login, string password, string firstName, string lastName, string phone)
        {
            Initialize();
            using (OleDbConnection connection = Open())
            using (OleDbTransaction transaction = connection.BeginTransaction())
            {
                using (OleDbCommand check = new OleDbCommand("SELECT COUNT(*) FROM [ПользователиСистемы] WHERE [Логин]=?", connection, transaction))
                {
                    check.Parameters.AddWithValue("?", login);
                    if (Convert.ToInt32(check.ExecuteScalar()) > 0) throw new InvalidOperationException("Этот логин уже занят.");
                }
                using (OleDbCommand addSpectator = new OleDbCommand("INSERT INTO [Зрители] ([Имя],[Фамилия],[Телефон]) VALUES (?,?,?)", connection, transaction))
                {
                    addSpectator.Parameters.AddWithValue("?", firstName);
                    addSpectator.Parameters.AddWithValue("?", lastName);
                    addSpectator.Parameters.AddWithValue("?", string.IsNullOrWhiteSpace(phone) ? (object)DBNull.Value : phone);
                    addSpectator.ExecuteNonQuery();
                }
                int spectatorId;
                using (OleDbCommand identity = new OleDbCommand("SELECT @@IDENTITY", connection, transaction)) spectatorId = Convert.ToInt32(identity.ExecuteScalar());
                using (OleDbCommand addAccount = new OleDbCommand("INSERT INTO [ПользователиСистемы] ([Логин],[ХэшПароля],[Роль],[КодЗрителя]) VALUES (?,?,?,?)", connection, transaction))
                {
                    addAccount.Parameters.AddWithValue("?", login);
                    addAccount.Parameters.AddWithValue("?", PasswordHash(password));
                    addAccount.Parameters.AddWithValue("?", "Зритель");
                    addAccount.Parameters.AddWithValue("?", spectatorId);
                    addAccount.ExecuteNonQuery();
                }
                transaction.Commit();
                return new AuthUser(login, "Зритель", spectatorId);
            }
        }

        private static string FindProjectRoot(string start)
        {
            DirectoryInfo dir = new DirectoryInfo(start);
            while (dir != null)
            {
                if (Directory.GetFiles(dir.FullName, "*.sln").Length > 0) return dir.FullName;
                dir = dir.Parent;
            }
            return start;
        }

        private static void CreateAccessFile(string connectionString)
        {
            object catalog = null;
            try
            {
                Type type = Type.GetTypeFromProgID("ADOX.Catalog");
                if (type == null) throw new InvalidOperationException("Не найден компонент Access Database Engine (ACE). Установите Microsoft Access или Access Database Engine.");
                catalog = Activator.CreateInstance(type);
                type.InvokeMember("Create", System.Reflection.BindingFlags.InvokeMethod, null, catalog, new object[] { connectionString + "Jet OLEDB:Engine Type=6;" });
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Не удалось создать файл Access. Проверьте, что установлен Microsoft Access Database Engine той же разрядности, что и программа. " + ex.GetBaseException().Message, ex);
            }
            finally { if (catalog != null && Marshal.IsComObject(catalog)) Marshal.FinalReleaseComObject(catalog); }
        }

        private static OleDbConnection Open()
        {
            OleDbConnection connection = new OleDbConnection(_connectionString);
            connection.Open();
            return connection;
        }

        public static DataTable Read(string sql)
        {
            Initialize();
            using (OleDbConnection connection = Open())
            using (OleDbDataAdapter adapter = new OleDbDataAdapter(sql, connection))
            {
                DataTable table = new DataTable(); adapter.Fill(table); return table;
            }
        }

        public static DataTable ReadTable(TableDefinition table)
        { return Read("SELECT * FROM [" + table.Name + "] ORDER BY [Код] DESC"); }

        public static DataTable ReadTickets()
        {
            return Read("SELECT [Билеты].*, [Зрители].[Имя] & ' ' & [Зрители].[Фамилия] AS [Покупатель], [Спектакли].[Название] AS [НазваниеСпектакля] " +
                "FROM ([Билеты] INNER JOIN [Зрители] ON [Билеты].[КодЗрителя]=[Зрители].[Код]) " +
                "INNER JOIN [Спектакли] ON [Билеты].[КодСпектакля]=[Спектакли].[Код] ORDER BY [Билеты].[Код] DESC");
        }

        public static void BuyTicket(int spectatorId, int playId, string seat)
        {
            Initialize();
            using (OleDbConnection connection = Open())
            using (OleDbTransaction transaction = connection.BeginTransaction())
            {
                decimal price;
                using (OleDbCommand play = new OleDbCommand("SELECT [ЦенаБилета] FROM [Спектакли] WHERE [Код]=?", connection, transaction))
                {
                    play.Parameters.AddWithValue("?", playId);
                    object value = play.ExecuteScalar();
                    if (value == null || value == DBNull.Value) throw new InvalidOperationException("Спектакль не найден или цена не указана.");
                    price = Convert.ToDecimal(value);
                }
                using (OleDbCommand occupied = new OleDbCommand("SELECT COUNT(*) FROM [Билеты] WHERE [КодСпектакля]=? AND [Место]=?", connection, transaction))
                {
                    occupied.Parameters.AddWithValue("?", playId);
                    occupied.Parameters.AddWithValue("?", seat);
                    if (Convert.ToInt32(occupied.ExecuteScalar()) > 0) throw new InvalidOperationException("Это место уже занято. Выберите другое.");
                }
                using (OleDbCommand insert = new OleDbCommand("INSERT INTO [Билеты] ([КодЗрителя],[КодСпектакля],[Место],[ЦенаБилета],[ДатаПокупки],[Статус]) VALUES (?,?,?,?,?,?)", connection, transaction))
                {
                    insert.Parameters.AddWithValue("?", spectatorId);
                    insert.Parameters.AddWithValue("?", playId);
                    insert.Parameters.AddWithValue("?", seat);
                    insert.Parameters.AddWithValue("?", price);
                    insert.Parameters.AddWithValue("?", DateTime.Now);
                    insert.Parameters.AddWithValue("?", "Куплен");
                    insert.ExecuteNonQuery();
                }
                transaction.Commit();
            }
        }

        public static int Add(TableDefinition table, IDictionary<string, object> values)
        { return ExecuteWrite(table, values, null); }

        public static int Update(TableDefinition table, int id, IDictionary<string, object> values)
        { return ExecuteWrite(table, values, id); }

        public static void UpdateBasicPlay(int id, string title, DateTime playDate, int duration, decimal price)
        {
            Initialize();
            using (OleDbConnection connection = Open())
            using (OleDbCommand command = new OleDbCommand("UPDATE [Спектакли] SET [Название]=?, [ДатаВремя]=?, [ПродолжительностьМинут]=?, [ЦенаБилета]=? WHERE [Код]=?", connection))
            {
                command.Parameters.AddWithValue("?", title);
                command.Parameters.AddWithValue("?", playDate);
                command.Parameters.AddWithValue("?", duration);
                command.Parameters.AddWithValue("?", price);
                command.Parameters.AddWithValue("?", id);
                command.ExecuteNonQuery();
            }
        }

        private static int ExecuteWrite(TableDefinition table, IDictionary<string, object> values, int? id)
        {
            Initialize();
            List<string> names = new List<string>(); List<object> parameters = new List<object>();
            foreach (FieldDefinition field in table.Fields) { names.Add("[" + field.Name + "]"); parameters.Add(values.ContainsKey(field.Name) ? values[field.Name] : DBNull.Value); }
            List<string> assignments = names.ConvertAll(n => n + "=?");
            string sql = id.HasValue
                ? "UPDATE [" + table.Name + "] SET " + string.Join(",", assignments) + " WHERE [Код]=?"
                : "INSERT INTO [" + table.Name + "] (" + string.Join(",", names) + ") VALUES (" + string.Join(",", names.ConvertAll(n => "?")) + ")";
            if (id.HasValue) parameters.Add(id.Value);
            using (OleDbConnection connection = Open()) using (OleDbCommand command = new OleDbCommand(sql, connection))
            {
                foreach (object value in parameters) command.Parameters.AddWithValue("?", value ?? DBNull.Value);
                return command.ExecuteNonQuery();
            }
        }

        public static void Delete(TableDefinition table, int id)
        {
            Initialize();
            using (OleDbConnection connection = Open()) using (OleDbCommand command = new OleDbCommand("DELETE FROM [" + table.Name + "] WHERE [Код]=?", connection))
            { command.Parameters.AddWithValue("?", id); command.ExecuteNonQuery(); }
        }

        public static string FullName(string tableName, string firstNameColumn, string lastNameColumn)
        { return "Trim(Nz([" + firstNameColumn + "],'') & ' ' & Nz([" + lastNameColumn + "],''))"; }
    }
}
