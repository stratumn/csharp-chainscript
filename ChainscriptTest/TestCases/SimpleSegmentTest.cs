using System;
using System.Collections.Generic;

namespace Stratumn.Chainscript.ChainscriptTest.TestCases
{
    class SimpleSegmentTest : ITestCase
    {

        public static readonly string Id = "simple-segment";
        string ITestCase.Generate()
        {
            Segment segment = new LinkBuilder("test_process", "test_map").WithAction("init")
         .WithData(CanonicalJson.Canonicalizer.Parse("{ \"name\": \"ʙᴀᴛᴍᴀɴ\", \"age\": 42 }"))
         .WithDegree(3).WithMetadata("bruce wayne")
         .WithParent(new byte[] { 42, 42 }).WithPriority(42).WithProcessState("started").WithStep("setup")
         .WithTags(new String[] { "tag1", "tag2" })
         .Build().Segmentify(); 

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
            if (!link.Action().Equals("init"))
                throw new Exception("Invalid action: " + link.Action());

            IDictionary<String, Object> data = (SortedDictionary<String, Object>)link.Data()  ;
            if (!data["age"].ToString()  .Equals( "42" ))
            {
                throw new Exception( "Invalid data: " +  CanonicalJson.Canonicalizer.Stringify(link.Data()));
            }
            if (!data["name"].Equals("ʙᴀᴛᴍᴀɴ"))
            {
                throw new Exception( "Invalid data: " + CanonicalJson.Canonicalizer.Stringify(link.Data()));
            }
            if (link.OutDegree() != 3)
            {
                throw new Exception ( "Invalid degree:" + link.OutDegree()) ;
            }
            if (!link.MapId().Equals("test_map"))
            {
                throw new Exception("Invalid map id:  " +  link.MapId() );
            }
            if (!link.Metadata().Equals("bruce wayne"))
            {
                throw new Exception("Invalid metadata:  " + CanonicalJson.Canonicalizer.Stringify(link.Metadata() ) );
            }
            if (link.PrevLinkHash()[0] != 42 || link.PrevLinkHash()[1] != 42)
            {
                throw new Exception("Invalid parent: "  + System.Text.Encoding.Default.GetString( link.PrevLinkHash() )  );
            }
            if (link.Priority() != 42)
            {
                throw new Exception("Invalid priority:  " +  link.Priority()) ;
            }
            if (!link.Process().Name .Equals("test_process"))
            {
                throw new Exception("Invalid process name:" +  link.Process().Name ) ;
            }
            if (!link.Process().State .Equals("started"))
            {
                throw new Exception("Invalid process state: " + link.Process().State ) ;
            }
            if (!link.Step().Equals("setup"))
            {
                throw new Exception("Invalid step:  " + link.Step()) ;
            }
            if (!"tag1".Equals(link.Tags()[0]) || !"tag2".Equals(link.Tags()[1]))
            {
                throw new Exception("Invalid tags:  " + (Object)link.Tags()) ;
            }

            return true;
        }
    }
}
