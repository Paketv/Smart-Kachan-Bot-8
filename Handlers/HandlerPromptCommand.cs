using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;

namespace Smart_Kachan_bot_8
{
    internal class CHandlerPromptCommand
        
    {
        private static Dictionary<long, UserState> userStates = new Dictionary<long, UserState>();
        public static async Task HandlerPromptCommand(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {

            if (update.Message?.Date.ToLocalTime() < DateTime.Now.AddMinutes(-1))
            {
                return;
            }

            if (update.Message?.Text != null)
            {
                
                var messageId = update.Message.MessageId;
                var userId = update!.Message!.From!.Id!;
                var chatId = update.Message.Chat.Id;
                var textMessage = update.Message.Text.ToLower();
                int spaceIndex = textMessage.IndexOf(' ');
                string MessagePart2 = spaceIndex >= 0 ? textMessage.Substring(spaceIndex + 1) : string.Empty;
                string MessagePart1 = spaceIndex >= 0 ? textMessage.Substring(0, spaceIndex) : textMessage;
                var userName = update.Message?.From?.Username ?? "Empty";

                if (textMessage.StartsWith("/cancelkc"))
                {
                    if (userStates.ContainsKey(userId))
                    {
                        userStates.Remove(userId);
                        await botClient.SendMessage(
                            chatId: chatId,
                            text: "Создание промпта отменено",
                            cancellationToken: ct);
                        Console.WriteLine($"{userName}({userId}) Отменил создание промпта'");
                    }
                    return;
                }

                if (userStates.TryGetValue(userId, out var state))
                {
                    if (state.UserId != userId && state.ChatId != chatId)
                    {
                        return;
                    }
                  
                        if (string.IsNullOrEmpty(state.PromptName))
                        {

                            state.PromptName = textMessage;
                            await botClient.SendMessage(
                                chatId: chatId,
                                text: "Теперь введи Промпт:",
                                cancellationToken: ct);
                            return;
                        }
                        else
                        {
                            await DataBaseOperations.SavePromptToDatabase(userId, state.PromptName, chatId, textMessage);
                            await botClient.SendMessage(
                                chatId: chatId,
                                text: $"Промпт '{state.PromptName}' успешно сохранён ✔️",
                                cancellationToken: ct);
                            Console.WriteLine($"{userName} ({userId}) закончил создание промпта'");
                            userStates.Remove(userId);

                            return;
                        }
                    
                }

                else if (textMessage.StartsWith("/addprompt"))
                { 
                    userStates[userId] = new UserState
                    {
                        CurrentCommand = "/addprompt",
                        UserId = userId,
                        ChatId = chatId,
                    };
                    Console.WriteLine($"{userName}({userId}) начал создание промпта'");
                    await botClient.SendMessage(
                        chatId: chatId,
                        text: "Введи название для нового промпта:",
                        cancellationToken: ct);
                    return;
                }




                if (textMessage.StartsWith("/listkc"))
                {
                    string dildo = DataBaseOperations.GetAllPromptsForChatAsString(chatId);
                    await botClient.SendMessage(chatId, dildo, parseMode: ParseMode.Html, cancellationToken: ct);
                }
                else if (textMessage.StartsWith("/deleteprompt "))
                {

                    if (HelpClass.IsAdmin(userId))
                    {
                        string promptName = MessagePart2;
                        bool deleted = await DataBaseOperations.DeletePromptByName(promptName);

                        await botClient.SendMessage(
                            chatId: chatId,
                            text: deleted
                                ? $"Промпт '{promptName}' успешно удалён!"
                                : $"Промпт '{promptName}' не найден или нет прав на удаление",
                            cancellationToken: ct);

                        Console.WriteLine(deleted ? $"{userName}({userId})успешно удалил '{promptName}'" : $"Промпт '{promptName}' не найден или нет прав на удаление у {userName}({userId})");
                    }
                    else
                    {
                        await botClient.SendMessage(
                            chatId: chatId,
                            text: "У вас нет прав на удаление промптов",
                            cancellationToken: ct);
                    }
                }
            }
        }
    }
}
