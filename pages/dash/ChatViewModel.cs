using System.Collections.ObjectModel;

namespace plz_fix.pages.dash
{
    public class ChatViewModel
    {
        public ObservableCollection<ChatModel> Chats { get; set; }

        public ChatViewModel()
        {
            Chats = new ObservableCollection<ChatModel>
            {
                new ChatModel { Name = "Chat - GPT", LastMessage = "AI model that can help you",
                                ProfileImage = "profile_placeholder.png", UnreadMessages = 9 },
                new ChatModel { Name = "School Principal", LastMessage = "Important school updates.",
                                ProfileImage = "profile_placeholder.png", UnreadMessages = 5 },
                new ChatModel { Name = "Group Chat", LastMessage = "Let's meet at 3 PM!",
                                ProfileImage = "profile_placeholder.png", UnreadMessages = 2 }
            };
        }
    }

    public class ChatModel
    {
        public string Name { get; set; }
        public string LastMessage { get; set; }
        public string ProfileImage { get; set; }
        public int UnreadMessages { get; set; }
        public bool HasUnreadMessages => UnreadMessages > 0;
    }
}
