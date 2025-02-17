using System.Security.Cryptography;
using System.Text;

namespace Domain.Entities
{
    public class CatFact
    {
        public Guid Id { get; set; }
        public string Fact { get; set; } = string.Empty;
        public string FactHash { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public CatFact(string fact)
        {
            Fact = fact;
            FactHash = ComputeHash(fact);
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(string fact)
        {
            Fact = fact;
            FactHash = ComputeHash(fact);
            CreatedAt = DateTime.UtcNow;
        }

        public static string ComputeHash(string fact)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(fact);
            var hashBytes = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hashBytes);
        }
    }
}
