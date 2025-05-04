using System.Text;
using System.Data.SQLite;
namespace Smart_Kachan_bot_8
{
    internal class DataBaseOperations
    {
        public static async Task SavePromptToDatabase(long idUser, string nameCommand, long chatId, string prompt)
        {
            using (var connection = new SQLiteConnection($"Data Source = {Program.dbPath};Version=3;"))
            {
                await connection.OpenAsync();

                string sql = @"
            INSERT INTO Users (IdUser, NameCommand, ChatId, Prompt)
            VALUES (@idUser, @nameCommand, @chatId, @prompt)";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idUser", idUser);
                    command.Parameters.AddWithValue("@nameCommand", nameCommand);
                    command.Parameters.AddWithValue("@chatId", chatId);
                    command.Parameters.AddWithValue("@prompt", prompt);

                    await command.ExecuteNonQueryAsync();
                    Console.WriteLine($"Новый промпт добавлен: {nameCommand}");
                }
            }

            Console.WriteLine($"Новое сохранение в БД:\n" +
                     $"ID пользователя: {idUser}\n" +
                     $"Username: {idUser}\n" +
                     $"Название команды: {nameCommand}\n" +
                     $"Chat ID: {chatId}\n" +
                     $"Промпт: {prompt}\n" +
                     $"Дата сохранения: {DateTime.Now}\n" +
                     "----------------------------------");
        }
        public static async Task<bool> DeletePromptByName(string promptName)
        {
            using (var connection = new SQLiteConnection($"Data Source={Program.dbPath};Version=3;"))
            {
                await connection.OpenAsync();

                string sql = @"
            DELETE FROM Users 
            WHERE NameCommand = @promptName";

                using (var cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@promptName", promptName);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }
       public static string GetAllPromptsForChatAsString(long chatId)
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine($"📂 Все промпты чата {chatId}:");
            result.AppendLine("----------------------------------");

            using (var connection = new SQLiteConnection($"Data Source={Program.dbPath};Version=3;"))
            {
                connection.Open();

                string sql = "SELECT IdUser, NameCommand, Prompt FROM Users WHERE ChatId = @chatId";

                using (var cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@chatId", chatId);
                  
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            result.AppendLine($"<a href=\"tg://openmessage?user_id={reader["IdUser"]}\">Создатель</a> | {HelpClass.EscapeHtml(reader["NameCommand"].ToString()!)}");
                            result.AppendLine($"{HelpClass.EscapeHtml(reader["Prompt"].ToString()!)}");
                            result.AppendLine("----------------------------------");



                        }
                    }
                }
            }

            if (result.Length == 0)
            {
                return "В этом чате нет сохраненных промптов";
            }

            return result.ToString();
        }
      public static async Task<long?> FindChatIdByPromptName(string commandName)
        {
            try
            {
                using (var connection = new SQLiteConnection($"Data Source={Program.dbPath};Version=3;"))
                {
                    await connection.OpenAsync();

                    string sql = @"
                SELECT ChatId 
                FROM Users 
                WHERE NameCommand = @commandName
                LIMIT 1";

                    using (var cmd = new SQLiteCommand(sql, connection))
                    {
                        var param = cmd.CreateParameter();
                        param.ParameterName = "@commandName";
                        param.Value = commandName;
                        param.DbType = System.Data.DbType.String;
                        cmd.Parameters.Add(param);

                        object? result = await cmd.ExecuteScalarAsync();

                        if (result != null && result != DBNull.Value)
                        {
                            return Convert.ToInt64(result);
                        }
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при поиске ChatId: {ex.Message}");
                return null;
            }
        }
        public static async Task<string> FindPromptByName(string commandName)
        {
            using (var connection = new SQLiteConnection($"Data Source={Program.dbPath};Version=3;"))
            {
                await connection.OpenAsync();

                string sql = @"
            SELECT Prompt 
            FROM Users 
            WHERE NameCommand = @commandName
            LIMIT 1";

                using (var cmd = new SQLiteCommand(sql, connection))
                {

                    cmd.Parameters.AddWithValue("@commandName", commandName);

                    object? result = await cmd.ExecuteScalarAsync();
                    return result?.ToString() ?? "Промпт не найден";
                }
            }
     }
       public static List<string> GetAllNames(string dbPath)
        {
            List<string> promptNames = new List<string>();

            using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                connection.Open();
                string sql = "SELECT DISTINCT NameCommand FROM Users";

                using (var cmd = new SQLiteCommand(sql, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        promptNames.Add(reader["NameCommand"].ToString()!);
                    }
                }
            }


            return promptNames;
        }
    }
}
