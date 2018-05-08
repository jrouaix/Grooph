using System;

namespace Grooph
{
    public sealed class Id : IEquatable<Id>
    {
        private readonly string _id;

        public Id(string collection, string key)
        {
            Collection = collection;
            Key = key;
            _id = $"{Collection}/{Key}";
        }

        public string Collection { get; private set; }
        public string Key { get; private set; }


        public bool Equals(Id other) => this._id == other._id;
        public override string ToString() => _id;
        public override int GetHashCode() => ToString().GetHashCode();

        public override bool Equals(object obj) => (obj is Id other) && Equals(other);

        public static bool operator ==(Id i1, Id i2) => i1?.Equals(i2) ?? ReferenceEquals(i1, i2);
        public static bool operator !=(Id i1, Id i2) => !(i1 == i2);

    }
}
