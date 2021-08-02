using System;

namespace ChatCore
{
    public enum MessageType
    {
        UserJoined,
        Chat,
    }

    public class MessageEnvelope
    {
        public Guid MessageId { get; set; } = Guid.NewGuid();
        public MessageType MessageType { get; set; }
        public DateTime DateCreatedUtc { get; set; } = DateTime.UtcNow;
        public UserDetails Author { get; set; }

        /// <summary>
        /// Serialized message in json format
        /// </summary>
        public string Payload { get; set; }

    }
}
