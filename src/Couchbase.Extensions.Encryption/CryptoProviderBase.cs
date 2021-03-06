﻿using System.Security.Cryptography;

namespace Couchbase.Extensions.Encryption
{
    public abstract class CryptoProviderBase : ICryptoProvider
    {
        public IKeystoreProvider KeyStore { get; set; }

        public abstract byte[] Decrypt(byte[] encryptedBytes, byte[] iv, string keyName = null);

        public abstract byte[] Encrypt(byte[] plainBytes, out byte[] iv);

        public byte[] GetSignature(byte[] cipherBytes, string password)
        {
            var passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
            using (var hmac = new HMACSHA256(passwordBytes))
            {
                return hmac.ComputeHash(cipherBytes);
            }
        }

        public string Name { get; protected set; }
        public string KeyName { get; set; }
        public string PrivateKeyName { get; set; }
        public abstract bool RequiresAuthentication { get; }
    }
}

#region [ License information          ]
/* ************************************************************
 *
 *    @author Couchbase <info@couchbase.com>
 *    @copyright 2017 Couchbase, Inc.
 *
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * ************************************************************/
#endregion
