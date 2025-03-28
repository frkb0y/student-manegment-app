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
                new ChatModel { Name = "Nathan Scott", LastMessage = "One day you're seventeen...",
                                ProfileImage = "profile_placeholder.png", UnreadMessages = 0 },
                new ChatModel { Name = "Brooke Davis", LastMessage = "I am who I am. No excuses.",
                                ProfileImage = "profile_placeholder.png", UnreadMessages = 2 },
                new ChatModel { Name = "Lucas Scott", LastMessage = "The only way out is through.",
                                ProfileImage = "profile_placeholder.png", UnreadMessages = 1 },
                new ChatModel { Name = "Peyton Sawyer", LastMessage = "Life is short, enjoy the ride.",
                                ProfileImage = "profile_placeholder.png", UnreadMessages = 3 },
                new ChatModel { Name = "Haley James", LastMessage = "Family means everything to me.",
                                ProfileImage = "profile_placeholder.png", UnreadMessages = 0 },
                new ChatModel { Name = "Dan Scott", LastMessage = "I did what I had to do.",
                                ProfileImage = "profile_placeholder.png", UnreadMessages = 5 },
                new ChatModel { Name = "Skills Taylor", LastMessage = "You're only as good as your next play.",
                                ProfileImage = "profile_placeholder.png", UnreadMessages = 4 },
                new ChatModel { Name = "Mouth McFadden", LastMessage = "Life is full of surprises.",
                                ProfileImage = "profile_placeholder.png", UnreadMessages = 7 },
                new ChatModel { Name = "Karen Roe", LastMessage = "A mother's love is endless.",
                                ProfileImage = "profile_placeholder.png", UnreadMessages = 6 },
                new ChatModel { Name = "Keith Scott", LastMessage = "The heart knows what it wants.",
                                ProfileImage = "profile_placeholder.png", UnreadMessages = 1 },
                new ChatModel { Name = "Deb Scott", LastMessage = "You don't always get what you want.",
                                ProfileImage = "profile_placeholder.png", UnreadMessages = 8 },
                new ChatModel { Name = "Julian Baker", LastMessage = "This is just the beginning.",
                                ProfileImage = "profile_placeholder.png", UnreadMessages = 0 },
                new ChatModel { Name = "Quinn James", LastMessage = "Don't let fear hold you back.",
                                ProfileImage = "profile_placeholder.png", UnreadMessages = 0 },
                new ChatModel { Name = "Chase Adams", LastMessage = "Life's too short for regrets.",
                                ProfileImage = "profile_placeholder.png", UnreadMessages = 9 },
                new ChatModel { Name = "Chris Keller", LastMessage = "You have to be willing to fail.",
                                ProfileImage = "profile_placeholder.png", UnreadMessages = 3 }
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
