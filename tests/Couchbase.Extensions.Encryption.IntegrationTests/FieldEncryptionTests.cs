﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Couchbase.Configuration.Client;
using Couchbase.Extensions.Encryption.Providers;
using Couchbase.Extensions.Encryption.Stores;
using Xunit;

namespace Couchbase.Extensions.Encryption.IntegrationTests
{
    public class FieldEncryptionTests
    {
        [Fact]
        public void Test_Encrypt_String()
        {
            using (var aes = Aes.Create())
            {
                aes.GenerateKey();
                var key = Encoding.Unicode.GetString(aes.Key);

                var config = new ClientConfiguration
                {
                    Servers = new List<Uri>
                    {
                        new Uri("http://10.142.171.101:8091/")
                    }
                };
                config.EnableFieldEncryption(new AesCryptoProvider(new InsecureKeyStore("mypublickey", key))
                {
                    KeyName = "mypublickey"
                });

                using (var cluster = new Cluster(config))
                {
                    cluster.Authenticate("Administrator", "password");
                    var bucket = cluster.OpenBucket();

                    var poco = new Poco
                    {
                        Bar = "Bar",
                        Foo = 90,
                        ChildObject = new PocoMoco
                        {
                            Bar = "Bar2"
                        },
                        Fizz = "fizz",
                        Baz = new List<int>{3,4}
                    };

                    var result = bucket.Upsert("thepoco", poco);

                    Assert.True(result.Success);

                    var get = bucket.Get<Poco>("thepoco");
                    Assert.True(get.Success);
                    Assert.Equal("Bar", get.Value.Bar);
                    Assert.Equal(90, get.Value.Foo);
                    Assert.Equal(new List<int> { 3, 4 }, get.Value.Baz);
                    Assert.Equal("Bar2", get.Value.ChildObject.Bar);
                }
            }
        }

        public class Poco
        {
            [EncryptedField(Provider = "AESCryptoProvider")]
            public string Bar { get; set; }

            [EncryptedField(Provider = "AESCryptoProvider")]
            public int Foo { get; set; }

            [EncryptedField(Provider = "AESCryptoProvider")]
            public List<int> Baz { get; set; }

            [EncryptedField(Provider = "AESCryptoProvider")]
            public PocoMoco ChildObject { get; set; }

            public string Fizz { get; set; }
        }

        public class PocoMoco
        {
            public string Bar { get; set; }
        }
    }
}
