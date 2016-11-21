namespace JA.PatternMatch
{
    /// <summary>
    /// This class can be used instead of void for return types.
    /// </summary>
    public class Nothing
    {
        public static readonly Nothing Instance = new Nothing();

        private Nothing() { }

        public override string ToString()
        {
            return "Nothing";
        }
    }
}