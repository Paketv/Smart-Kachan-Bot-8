using System;

namespace Smart_Kachan_bot_8
{
    internal class HelpClass
    {
        public static bool IsAdmin(long userId)
        {

            long[] adminIds = {};//Ваши Айди пользователей которым разрешаете использовать команду 
            return adminIds.Contains(userId);
        }


        public static string EscapeHtml(string text)
        {
            return text
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;");
        }
    }
}
