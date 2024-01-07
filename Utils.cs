namespace astronomy
{
    public delegate bool ShouldContinue(string input);
    public delegate dynamic ProcessInput(string input);

    internal class Utils
    {
        public static dynamic GetInput(string description, ShouldContinue shouldContinue, ProcessInput? processInput = null)
        {
            string input;
            do
            {
                Console.Write(description + ": ");
                input = Console.ReadLine();
            } while (input == null || !shouldContinue(input));

            if (processInput == null) return input;
            return processInput(input);
        }
        
        public static bool IsValidWindowsPath(string path)
        {
            return !new[] { '<', '>', ':', '"', '/', '\\', '|', '?', '*' }.Any(c => path.Contains(c));
        }
    }
}
