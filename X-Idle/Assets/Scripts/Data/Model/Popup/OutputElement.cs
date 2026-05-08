namespace Data.Model.Popup
{
    public class OutputElement
    {
        public OutputElement(string currentOutput, string nextOutput)
        {
            CurrentOutput = currentOutput;
            NextOutput = nextOutput;
        }

        public string CurrentOutput { get; }
        public string NextOutput { get; }
    }
}