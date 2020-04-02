using System;

namespace Stratumn.Chainscript.ChainscriptTest.TestCases
{
    class ReferencesTest : ITestCase
    {

        public static readonly string Id = "segment-references";
        string ITestCase.Generate()
        {
            Segment segment = new LinkBuilder("test_process", "test_map")
         .WithRefs(new LinkReference[] {
            new LinkReference( new byte[] {42} , "p1"),
            new LinkReference( new byte[]  {24} , "p2")
         }).Build().Segmentify();

            

            return Convert.ToBase64String(segment.Serialize());
        }

        string ITestCase.GetId()
        {
            return Id;
        }

        bool ITestCase.Validate(string encodedSegment)
        {
            Segment Segment = Segment.Deserialize(Convert.FromBase64String(encodedSegment));
            Segment.Validate();


            Link link = Segment.Link();
            LinkReference[] linkRefs = link.Refs();

            if (linkRefs.Length != 2)
            {
                throw new Exception("Invalid references count: " + linkRefs.Length);
            }
            if (!linkRefs[0].Process.Equals("p1"))
            {
                throw new Exception("Invalid first reference process: " + linkRefs[0]. Process );
            }

            if (!System.Linq.Enumerable.SequenceEqual(linkRefs[0].LinkHash, (new byte[] { 42 })))
            {
                throw new Exception("Invalid first reference link hash: " + String.Join(",", linkRefs[0].LinkHash));
            }

            if (!linkRefs[1].Process.Equals("p2"))
            {
                throw new Exception("Invalid second reference process: " + linkRefs[1].Process);
            }

            if (!System.Linq.Enumerable.SequenceEqual(linkRefs[1].LinkHash, (new byte[] { 24 })))
            {
                throw new Exception("Invalid second reference link hash: " + String.Join(",", linkRefs[1].LinkHash)); 
            }

            return true;
        }
    }
}
