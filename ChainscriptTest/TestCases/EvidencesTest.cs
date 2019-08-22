using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stratumn.Chainscript.ChainscriptTest.TestCases
{
    class EvidencesTest : ITestCase
    {

        public static readonly string Id = "segment-evidences";
        string ITestCase.Generate()
        {
            Stratumn.Chainscript.Segment Segment = new LinkBuilder("test_process", "test_map").Build().Segmentify();
            Segment.AddEvidence(  new Evidence("0.1.0", "bitcoin", "testnet", new byte[] { 42 })  );
            Segment.AddEvidence(  new Evidence("1.0.3", "ethereum", "mainnet", new byte[] { 24 })  );

            return Convert.ToBase64String(Segment.Serialize());
        }

        string ITestCase.GetId()
        {
            return Id;
        }

        void ITestCase.Validate(string encodedSegment)
        {
            Segment Segment = Segment.Deserialize(Convert.FromBase64String(encodedSegment));
            Segment.Validate();

            if (Segment.Evidences().Length != 2)
                throw new Exception("Invalid evidences count: " + Segment.Evidences().Length);

            Evidence btc = Segment.GetEvidence("bitcoin", "testnet");
            if (btc == null)
                throw new Exception("Missing bitcoin evidence");
            if (
               !btc.Version.Equals("0.1.0") ||
               !btc. Backend .Equals("bitcoin") ||
               !btc. Provider .Equals("testnet") ||
                btc.Proof[0] != 42)
            {
                throw new Exception("Invalid bitcoin evidence:" + JsonConvert.SerializeObject(btc));
            }


            Evidence eth = Segment.GetEvidence("ethereum", "mainnet");
            if (eth == null)
                throw new Exception("Missing ethereum evidence");
            if (
               !eth. Version .Equals("1.0.3") ||
               !eth. Backend .Equals("ethereum") ||
               !eth. Provider .Equals("mainnet") ||
                eth. Proof [0] != 24)
            {
                throw new Exception("Invalid ethereum evidence:" + JsonConvert.SerializeObject(eth));
            }
        }
    }
}
