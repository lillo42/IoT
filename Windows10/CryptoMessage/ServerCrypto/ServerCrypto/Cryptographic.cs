using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace ServerCrypto
{
    internal class Cryptographic
    {
        public static byte[] Encrypt(string text, string passPhrase)
        {
            IBuffer iv = null;

            //Create SymmetricKeyAlgorithmProvider
            var symetric = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);

            //Create hash for passPhrase to create symmetric Key
            IBuffer keyBuffer = Hash(passPhrase);
            //Create symmetric key
            CryptographicKey key = symetric.CreateSymmetricKey(keyBuffer);

            //Convert texto to binary, for encrypt
            IBuffer data = CryptographicBuffer.ConvertStringToBinary(text, BinaryStringEncoding.Utf8);
            
            //Encrypt data
            //Encrypt method return IBuffer
            return CryptographicEngine.Encrypt(key, data, iv).ToArray();
        }

        public static string Decrypt(IBuffer data, string passPhrase)
        {
            IBuffer iv = null;

            //Create SymmetricKeyAlgorithmProvider
            var symetric = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);

            //Create hash for passPhrase to create symmetric Key
            IBuffer keyBuffer = Hash(passPhrase);
            //Create symmetric key
            CryptographicKey key = symetric.CreateSymmetricKey(keyBuffer);

            //Decrypt data
            IBuffer bufferDecrypted = CryptographicEngine.Decrypt(key, data, iv);
            //Convert binary to string
            return CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, bufferDecrypted);
        }

        public static IBuffer Hash(string text)
        {
            //Create HashAlgorithmProvider 
            var hash = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            //Create Hash object
            CryptographicHash cryptographicHash = hash.CreateHash();
            //Convert string to binary
            IBuffer data = CryptographicBuffer.ConvertStringToBinary(text, BinaryStringEncoding.Utf8);
            //Append data to generate Hash
            cryptographicHash.Append(data);
            //Generate Hash
            return cryptographicHash.GetValueAndReset();
        }
    }
}
