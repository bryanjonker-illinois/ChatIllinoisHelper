namespace ChatIllinoisHelper.Models {
    public class ChatbotTransport {
#pragma warning disable IDE1006 // Naming Styles
        public string model { get; set; } = "";
        public IEnumerable<ChatbotTransportMessage> messages { get; set; } = [];

        public string api_key { get; set; } = "";
        public string course_name { get; set; } = "";
        public bool stream { get; set; } = true;
        public float temperature { get; set; } = 0.1f;
        public bool retrieval_only { get; set; } = false;
#pragma warning restore IDE1006 // Naming Styles
    }
}
