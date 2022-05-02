using System;
using System.Security.Cryptography;
using System.Text;

namespace SRRC.Service.Repository.Authentication
{
    public interface ISecurityService
    {
        string GetSha256Hash(string input);
		string Decrypt(string encodedString);
		Guid CreateCryptographicallySecureGuid();
    }

    public class SecurityService : ISecurityService
    {
        private readonly RandomNumberGenerator _rand = RandomNumberGenerator.Create();

        public string GetSha256Hash(string input)
        {
            using (var hashAlgorithm = new SHA256CryptoServiceProvider())
            {
                var byteValue = Encoding.UTF8.GetBytes(input);
                var byteHash = hashAlgorithm.ComputeHash(byteValue);
                return Convert.ToBase64String(byteHash);

            }
        }
		
		public string Decrypt(string encodedString)
		{
			byte[] data = Convert.FromBase64String(encodedString);
			string decodedString = Encoding.UTF8.GetString(data);
			return decodedString; 
		}
						
        public Guid CreateCryptographicallySecureGuid()
        {
            var bytes = new byte[16];
            _rand.GetBytes(bytes);
            return new Guid(bytes);
        }
    }
}