using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Stratumn.Chainscript.ChainscriptTest.TestCases;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Stratumn.Chainscript.ChainscriptTest
{ 
    [TestClass]
    public class ChainscriptTest
    {
        static List<ITestCase> TestCases =
            new List<ITestCase>() { 
                new SimpleSegmentTest(),
                new ReferencesTest(),
                new EvidencesTest(),
                new SignaturesTest()
            };

        [TestMethod]
        public void RunTests()
        {
            string dataFile = "./1.0.0_T.json";
            Generate(dataFile);
            Validate(dataFile);
        }


        private void Generate(String dataFile)
        {
            Console.WriteLine("Saving encoded segments to " + Path.GetFullPath(dataFile));
            List<TestCaseResult> results = new List<TestCaseResult>();
            foreach (ITestCase tcase in TestCases)
            {
                try
                {
                    results.Add(new TestCaseResult(tcase.GetId(), tcase.Generate()));
                }
                catch (Exception e)
                {
                    Debug.WriteLine(tcase.GetId() + "  " + e.ToString());
                    
                    results.Add(new TestCaseResult(tcase.GetId(), e.Message));
                }
            }

            String json = Newtonsoft.Json.JsonConvert.SerializeObject(results, Newtonsoft.Json.Formatting.Indented);

            File.WriteAllText(Path.GetFullPath(dataFile), json);


        }

        private void Validate(String inputFile)
        {
            Console.WriteLine("Loading encoded segments from " + Path.GetFullPath(inputFile));
            if (!File.Exists(Path.GetFullPath(inputFile)))
            {
                throw new Exception("File not found " + Path.GetFullPath(inputFile));
            }
            String jsonData = File.ReadAllText(Path.GetFullPath(inputFile));

            TestCaseResult[] resultArr = JsonConvert.DeserializeObject<TestCaseResult[]>(jsonData);

            ITestCase testCase;
            foreach (TestCaseResult result in resultArr)
            {
                if (result.Id.Equals(SimpleSegmentTest.Id, StringComparison.CurrentCultureIgnoreCase))
                    testCase = new SimpleSegmentTest();
                //else if (result.Id .Equals (SignaturesTest.id,StringComparison.CurrentCultureIgnoreCase))
                //   testCase = new SignaturesTest();
                else if (result.Id.Equals(ReferencesTest.Id, StringComparison.CurrentCultureIgnoreCase))
                    testCase = new ReferencesTest();
                else if (result.Id.Equals(EvidencesTest.Id, StringComparison.CurrentCultureIgnoreCase))
                    testCase = new EvidencesTest();
                else
                {
                    Console.Error.WriteLine("Unknown test case : " + result.Id);
                    continue;
                }

                try
                {
                    testCase.Validate(result.Data) ;
                    Console.WriteLine(result.Id + " SUCCESS ");
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString() );
                    Console.Error.WriteLine(result.Id + " FAILED " + e.Message);

                }
            }
        }



        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: ChainscriptTest action[generate|validate] <file Path> ");
                Console.WriteLine("For generate, filePath is output file.");
                Console.WriteLine("For validate, filePath is input file.");
                Console.ReadLine();
            }
            String action = args[0].Trim();
            String path = args[1].Trim();
            ChainscriptTest test = new ChainscriptTest();
            if (action.Equals("generate", StringComparison.InvariantCultureIgnoreCase))
            {
                test.Generate(args[1]);
            }
            else if (action.Equals("validate", StringComparison.InvariantCultureIgnoreCase))
            {
                test.Validate(args[1]);
            }
            else
                Console.Error.WriteLine("Unknown action " + action);
            Console.ReadLine();
        }
    }
}
