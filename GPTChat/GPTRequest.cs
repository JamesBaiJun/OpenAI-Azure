using OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenAI.Models;
using OpenAI.Chat;
using System.Diagnostics;

namespace GPTChat
{
    public class GPTRequest
    {
        static GPTRequest()
        {
            key = ConfigHelper.GetConfig("OpenAI");
            api = new OpenAIClient(key);
        }

        static string key = string.Empty;
        static OpenAIClient api;

        public static event Action<string> OnReceiveReply;
        public static event Action<string> OnReceivePartial;
        public static async Task AskAsync(List<Message> messages)
        {
            var chatRequest = new ChatRequest(messages, Model.GPT3_5_Turbo);
            await foreach (var result in api.ChatEndpoint.StreamCompletionEnumerableAsync(chatRequest))
            {
                foreach (var choice in result.Choices.Where(choice => choice.Delta?.Content != null))
                {
                    // Partial response content
                    //Debug.WriteLine(choice.Delta.Content);
                    OnReceivePartial?.Invoke(choice.Delta.Content);
                }

                foreach (var choice in result.Choices.Where(choice => choice.Message?.Content != null))
                {
                    // Completed response content
                    //Debug.WriteLine($"{choice.Message.Role}: {choice.Message.Content}");
                    OnReceiveReply?.Invoke(choice.Message.Content);
                }
            }
        }

        public static void SetKey(string key)
        {
            api = new OpenAIClient(key);
        }
    }
}
