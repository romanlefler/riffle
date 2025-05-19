// Copyright 2025 Roman Lefler


namespace Riffle.Models
{
    public class RoomMember : IEquatable<RoomMember?>
    {
        public string Name { get; }
        public string ConnectionId { get; }

        public RoomMember(string connectionId, string name)
        {
            Name = name;
            ConnectionId = connectionId;
        }

        public override int GetHashCode()
        {
            return ConnectionId.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as RoomMember);
        }

        public bool Equals(RoomMember? other)
        {
            return other is not null &&
                   ConnectionId == other.ConnectionId;
        }

        public static bool operator ==(RoomMember? left, RoomMember? right)
        {
            return EqualityComparer<RoomMember>.Default.Equals(left, right);
        }

        public static bool operator !=(RoomMember? left, RoomMember? right)
        {
            return !(left == right);
        }

    }
}
