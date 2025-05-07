using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;

namespace Smart_Kachan_bot_8
{
    internal class CHandlerInitialCommands
    {
        public static async Task HandlerInitialCommand(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {

            if (update.Message?.Date.ToLocalTime() < DateTime.Now.AddMinutes(-1))
            {
                return;
            }

            if (update.Message?.Text != null)
            {
                var me = botClient.GetMe().Result;
                var messageId = update.Message.MessageId;
                var userId = update!.Message!.From!.Id!;
                var chatId = update.Message.Chat.Id;
                var textMessage = update.Message.Text.ToLower();
                int spaceIndex = textMessage.IndexOf(' ');
                string NameUser = $"{update.Message.From.FirstName} {update.Message.From.LastName}";
                string MessagePart2 = spaceIndex >= 0 ? textMessage.Substring(spaceIndex + 1) : string.Empty;
                string MessagePart1 = spaceIndex >= 0 ? textMessage.Substring(0, spaceIndex) : textMessage;
                var userName = update.Message?.From?.Username ?? "Empty";
                if (textMessage.StartsWith("/start"))
                {
                    await botClient.SendMessage(chatId: chatId, text: $"👋Привет, {NameUser}!\n🧠Это умный Качан, сможет ответить почти на любой кошачий вопрос!\n\n📄Узнать информацию о командах можно написав /help\n\n👤Владелец бота @Plasbag\n💻Исходный код бота - https://github.com/Paketv/Smart-Kachan-bot-8", linkPreviewOptions: true, cancellationToken: ct);
                    Console.WriteLine($"{userName}({userId}) запустил бота или просто жмякнул /start");
                }
                if (MessagePart1 == $"/help")
                {
                    string request = MessagePart2 switch
                    {
                        "gptkc" => "Запрос умному Качану(Отправка запроса в OnlySq API) который знает почти всех из фд кошек а так же шарит за качановские мемы. \nПример: /gptkc Привет, расскажи как твои дела",
                        "fgptkc" => "Запрос умному и злому Качану(Отправка апроса в OnlySq API). \nПример: /fgptkc Привет, расскажи как твои дела?",
                        "addprompt" => "Начать создание своего промпта. \n1 часть: нужно написать команду/слово на которое бот будет реагировать(В название промпта можно вписать только 1 слово). \n2 часть написать сам промпт. Пример: 1 часть:\nКачан\n2 часть: \nОтвечай как котик с именем Качан\n Бот отправит ваш промпт и запрос(OnlySq API) если в начале сообщения написать 'Качан' ",
                        "listkc" => "Покажет список всех промптов данного чата созданных с помощью /addprompt",
                        "cancelkc" => "Отменит создание промпта, достаточно ввести команду при создании промпта.",
                        "deleteprompt" => "Удаление промпта из списка. Команда пока что недоработана и доступна только владельцу бота. Если требуется удалить промпт, писать - @Plasbag",
                        "start" => "Базовая команда. Объяснять нечего",
                        "" => "/gptkc - отправить запрос умному качану\n /fgptkc - отправить запрос умному и злому Качану\n/addprompt - начать создание своего Промпта\n/list - выведет список всех промптов данного чата\n/cancelkc - отменить создание Промпта\n/deleteprompt - удалить промпт. Пока доступна только @Plasbag",
                        _ => "Неизвестная команда. Список доступных команд:\n/gptkc - отправить запрос умному качану\n /fgptkc - отправить запрос умному и злому Качану\n/addprompt - начать создание своего Промпта\n/list - выведет список всех промптов данного чата\n/cancelkc - отменить создание Промпта\n/deleteprompt - удалить промпт. Пока доступна только @Plasbag",
                    };
                    await botClient.SendMessage(replyParameters: messageId, chatId: chatId, text: request,cancellationToken: ct);
                    }



            }
        }
    }
    }
