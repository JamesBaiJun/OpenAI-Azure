using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPTChat.Models
{
    public class MessageItem
    {
        public MessageItem(string content, Role role, bool isRec)
        {
            Role = role;
            Content = content;
            IsReceive = isRec;
        }
        public string Content { get; set; }

        public bool IsReceive { get; set; }

        public Role Role { get; set; }
    }
}
