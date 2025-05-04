using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
namespace Smart_Kachan_bot_8
{

    internal class HttpRequest
    {


        public static async Task<string> RequestToKachan(string TextMessage, string apiURL, string YouKachan)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {

                    var MessageJson = new
                    {
                        model = "gemini-2.0-flash",
                        request = new
                        {
                            messages = new[]
                            {
                                new
                        {
                            role = "system",
                            content = YouKachan
                        },
                        new
                        {
                            role = "user",
                            content = TextMessage
                        }
                    }
                        }
                    };

                    string jsonxz = JsonConvert.SerializeObject(MessageJson);
                    var content = new StringContent(jsonxz, Encoding.UTF8, "application/json");

                    HttpResponseMessage respone = await client.PostAsync(apiURL, content);
                    string replyRespone = await respone.Content.ReadAsStringAsync();

                    if (respone.IsSuccessStatusCode)
                    {
                        try
                        {
                            Console.WriteLine("");
                            var jsonResponse = JObject.Parse(replyRespone);
                            if (jsonResponse["choices"]?[0]?["message"]?["content"] != null)
                            {
                                string answer = jsonResponse["choices"]![0]!["message"]!["content"]!.ToString().Trim();
                                Console.WriteLine("Ответ: " + answer);
                                return answer;
                            }
                            return replyRespone;
                        }
                        catch (System.Text.Json.JsonException jsonEx)
                        {
                            Console.WriteLine("Ошибка парсинга JSON: " + jsonEx.Message);
                            return replyRespone;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Ошибка:" + respone.StatusCode);
                        Console.WriteLine("Ответ от сервера: " + replyRespone);
                        return "прасти я слишком тупов для этого запроса. Ошибка:" + respone.StatusCode + replyRespone;
                    }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("Ошибка запроса:" + e.Message);
                    return "прасти я слишком тупов для этого запроса. Ошибка:" + e.Message;
                }
            }
        }
    }
}