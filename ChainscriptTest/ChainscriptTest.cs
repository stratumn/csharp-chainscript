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
           Assert.IsTrue( Generate(dataFile ),"Generate failed");
            Assert.IsTrue(Validate(dataFile), "Validate failed"); 
        }


        private Boolean Generate(String dataFile )
        {
            Console.WriteLine("Saving encoded segments to " + Path.GetFullPath(dataFile));
            List<TestCaseResult> results = new List<TestCaseResult>();
            Boolean result = true;
            foreach (ITestCase tcase in TestCases)
            {
                try
                {
                    results.Add(new TestCaseResult(tcase.GetId(), tcase.Generate()));
                }
                catch (Exception e)
                {
                    Debug.WriteLine(tcase.GetId() + "  " + e.ToString());
                    result &= false;
                    results.Add(new TestCaseResult(tcase.GetId(), e.Message));
                }
            }

            String json = Newtonsoft.Json.JsonConvert.SerializeObject(results, Newtonsoft.Json.Formatting.Indented);

            File.WriteAllText(Path.GetFullPath(dataFile), json);
            return result;

        }

        private Boolean Validate(String inputFile)
        {
            Console.WriteLine("Loading encoded segments from " + Path.GetFullPath(inputFile));
            if (!File.Exists(Path.GetFullPath(inputFile)))
            {
                throw new Exception("File not found " + Path.GetFullPath(inputFile));
            }
            String jsonData = File.ReadAllText(Path.GetFullPath(inputFile));
            Boolean result = true;
            TestCaseResult[] resultArr = JsonConvert.DeserializeObject<TestCaseResult[]>(jsonData);

            ITestCase testCase;
            foreach (TestCaseResult testCaseResult in resultArr)
            {
                if (testCaseResult.Id.Equals(SimpleSegmentTest.Id, StringComparison.CurrentCultureIgnoreCase))
                    testCase = new SimpleSegmentTest();
                 else if (testCaseResult.Id .Equals (SignaturesTest.Id,StringComparison.CurrentCultureIgnoreCase))
                    testCase = new SignaturesTest();
                else if (testCaseResult.Id.Equals(ReferencesTest.Id, StringComparison.CurrentCultureIgnoreCase))
                    testCase = new ReferencesTest();
                else if (testCaseResult.Id.Equals(EvidencesTest.Id, StringComparison.CurrentCultureIgnoreCase))
                    testCase = new EvidencesTest();
                else
                {
                    Console.Error.WriteLine("Unknown test case : " + testCaseResult.Id);
                    continue;
                }

                try
                {
                    testCase.Validate(testCaseResult.Data) ;
                    Console.WriteLine(testCaseResult.Id + " SUCCESS ");
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString() );
                    Console.Error.WriteLine(testCaseResult.Id + " FAILED " + e.Message);
                    result &= false;
                }
            }
            return result;
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
