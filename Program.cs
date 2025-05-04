

using System;
using Telegram.Bot;
using System.Data.SQLite;
namespace Smart_Kachan_bot_8
{
  internal class Program
    {
        private static ITelegramBotClient botClient = new TelegramBotClient("BOT_API_TOKEN");
        public static string dbPath = "DataBasePrompt.sqlite";

        static void Main(string[] args)
        {
            if (!System.IO.File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
                Console.WriteLine("База Данных создана>");
            }

            using (var connection = new SQLiteConnection($"Data Source = {dbPath};Version=3;"))
            {
                connection.Open();

                string createTableSql = @"
                    CREATE TABLE IF NOT EXISTS Users(
                        IdUser INTEGER NOT NULL,
                        NameCommand TEXT NOT NULL,
                        ChatId INTEGER NOT NULL, 
                        Prompt TEXT NOT NULL
                    )";

                using (var command = new SQLiteCommand(createTableSql, connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("Таблица создана>");
                }
            }

            var cts = new CancellationTokenSource();

            var me = botClient.GetMe().Result;
            Console.WriteLine($"Бот запущен: {me.Username}>");

            botClient.StartReceiving
                (
                updateHandler: async (bot, update, ct) =>
                {
                    if (update.Message is not { Text: { } text } message)
                        return;
                    await CHandlerSendRequest.HandlerSendRequest(botClient, update, ct);
                    await CHandlerPromptCommand.HandlerPromptCommand(botClient, update, ct);

                },
                errorHandler: async (bot, ex, ct) => await HandleError(bot, ex, ct)
            );

            Console.WriteLine("Главный процесс завершён, бот получает обновления");
            Console.ReadLine();
        }
       
        private static Task HandleError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Error! {exception.Message}");
            return Task.CompletedTask;
        }
    }
}