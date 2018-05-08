namespace Grooph
{
    public class Edge : IHaveValue
    {
        public Edge(Id id, object value, Id from, Id to)
        {
            Id = id;
            Value = value;
            From = from;
            To = to;
        }

        public Id From { get; private set; }
        public Id To { get; private set; }
        public Id Id { get; private set; }

        public object Value { get; set; }

        public override string ToString() => $"({Id})";
    }
}
