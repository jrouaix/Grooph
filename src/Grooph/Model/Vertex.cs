namespace Grooph.Model
{
    public class Vertex : IHaveValue
    {
        public Vertex(Id id, object value)
        {
            Id = id;
            Value = value;
        }

        public Id Id { get; private set; }

        public object Value { get; set; }

        public override string ToString() => $"[{Id}]";
    }
}
