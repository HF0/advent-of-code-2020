using QuikGraph;
using System.Diagnostics.CodeAnalysis;

namespace AdventCode7
{
    public class ColorEdge : Edge<string>
    {
        public ColorEdge([NotNull] string source, [NotNull] string target) : base(source, target)
        {
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !GetType().Equals(obj.GetType()))
            {
                return false;
            }
            ColorEdge other = (ColorEdge)obj;
            return Source.Equals(other.Source) && Target.Equals(other.Target);
        }

        public override int GetHashCode()
        {
            return Source.GetHashCode() + Target.GetHashCode();
        }
    }
}
