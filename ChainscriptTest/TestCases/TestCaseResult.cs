namespace Stratumn.Chainscript.ChainscriptTest.TestCases
{
    public class TestCaseResult
    {
        public string Id { get; private set; }
        public string Data { get; private set; }

        public TestCaseResult(string Id, string Data)
        {
            this.Id = Id;
            this.Data = Data;
        }
    }
}
