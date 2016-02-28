using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace ClientCrypto
{
    public static class Cryptographic
    {
        public static byte[] Encrypt(string text, string passPhrase)
        {
            IBuffer iv = null;
            var symetric = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);

            IBuffer keyBuffer = Hash(passPhrase);
            CryptographicKey key = symetric.CreateSymmetricKey(keyBuffer);

            IBuffer data = CryptographicBuffer.ConvertStringToBinary(text, BinaryStringEncoding.Utf8);
            return CryptographicEngine.Encrypt(key, data, iv).ToArray();
        }

        public static string Decrypt(IBuffer data, string passPhrase)
        {
            IBuffer iv = null;

            var symetric = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);

            IBuffer keyBuffer = Hash(passPhrase);
            CryptographicKey key = symetric.CreateSymmetricKey(keyBuffer);

            IBuffer bufferDecrypted = CryptographicEngine.Decrypt(key, data, iv);
            return CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, bufferDecrypted);
        }

        public static IBuffer Hash(string text)
        {
            var hash = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            CryptographicHash cryptographicHash = hash.CreateHash();
            IBuffer data = CryptographicBuffer.ConvertStringToBinary(text, BinaryStringEncoding.Utf8);
            cryptographicHash.Append(data);
            return cryptographicHash.GetValueAndReset();
        }
    }
}
