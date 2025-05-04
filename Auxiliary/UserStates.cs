using System;

namespace Smart_Kachan_bot_8
{
    public class UserState
    {
        public long UserId { get; set; }
        public string? CurrentCommand { get; set; }
        public string? PromptName { get; set; }
        public long ChatId { get; set; }
    }
}
