using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Text;
using Utils;

namespace Stratumn.Chainscript.ChainscriptTest.TestCases
{
    class SignaturesTest : ITestCase
    {
        string publicKeyString = "-----BEGIN ED25519 PUBLIC KEY-----\nMCowBQYDK2VwAyEAEIwjKUueKwu2s+ie5aFAsYBn8OEL7GHjEPML3JgxOEs=\n-----END ED25519 PUBLIC KEY-----";
        string privateKeyString = "-----BEGIN ED25519 PRIVATE KEY-----\nMFACAQAwBwYDK2VwBQAEQgRAG4bBxUz5/UFzaCCxlhmpbKtZE313fsfY+hviGNRr\n5RYQjCMpS54rC7az6J7loUCxgGfw4QvsYeMQ8wvcmDE4Sw==\n-----END ED25519 PRIVATE KEY-----\n";

        public static readonly string Id = "segment-signatures";
        string ITestCase.Generate()
        {
            Org.BouncyCastle.Crypto.Parameters.Ed25519PrivateKeyParameters privateKey = CryptoUtils.DecodeEd25519PrivateKey(privateKeyString);
            Link link = new LinkBuilder("test_process", "test_map")
            .WithAction("ʙᴀᴛᴍᴀɴ").Build();
            link.Sign(privateKey.GetEncoded(), "");
            link.Sign(privateKey.GetEncoded(), "[version,meta.mapId]");
            //      link.sign(rsaKey.getEncoded(), "[version,meta.mapId]");

            Segment segment = link.Segmentify();

            return Convert.ToBase64String(segment.Serialize());
        }

        string ITestCase.GetId()
        {
            return Id;
        }

        void ITestCase.Validate(string encodedSegment)
        {
            Segment Segment = Segment.Deserialize(Convert.FromBase64String(encodedSegment));
            Segment.Validate();
            Signature[] signatures = Segment.Link().Signatures();
            if (signatures.Length != 2)
                throw new Exception("Invalid number of signatures: " + signatures.Length);

            signatures[0].Validate(Segment.Link());
            signatures[1].Validate(Segment.Link());

            if (!signatures[0].PayloadPath().Equals("[version,data,meta]"))
            {
                throw new Exception("Invalid first signature payload path: " + signatures[0].PayloadPath());
            }
            if (!signatures[1].PayloadPath().Equals("[version,meta.mapId]"))
            {
                throw new Exception("Invalid second signature payload path: " + signatures[1].PayloadPath());
            }
        }
    }
}
