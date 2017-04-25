using System;
using System.Linq;

namespace Structurizr.InfrastructureAsCode.Policies
{
    public class RandomPasswordPolicy : IPasswordPolicy
    {
        private readonly int _length;
        private readonly char[] _allowedCharacters;
        private readonly Random _random;

        public RandomPasswordPolicy()
            : this(10, AlphaNumberic().ToArray())
        {
        }

        public RandomPasswordPolicy(int length, char[] allowedCharacters)
        {
            _length = length;
            _allowedCharacters = allowedCharacters;
            _random = new Random();
        }

        public string GetPassword()
        {
            var chars = Enumerable.Range(1, _length)
                .Select(i => _allowedCharacters[_random.Next(0, _allowedCharacters.Length)])
                .ToArray();
            return new string(chars);
        }

        private static char[] AlphaNumberic()
        {
            var lowercase = "abcdefghijklmnopqrstuvwxyz";
            var uppercase = lowercase.ToUpperInvariant();
            var numbers = "0123456789";
            return (lowercase + uppercase + numbers).ToCharArray();
        }

        private static char[] SpecialCharacters()
        {
            return "_|-!?".ToCharArray();
        }
    }
}