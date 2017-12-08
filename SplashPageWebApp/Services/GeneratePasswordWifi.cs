using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SplashPageWebApp.Services
{
    public class GeneratePasswordWifi
    {
        private static readonly int DEFAULT_MIN_PASSWORD_LENGTH = 8;
        private static readonly int DEFAULT_MAX_PASSWORD_LENGTH = 10;
        private static readonly string PASSWORD_CHARS_LCASE = "abcdefgijkmnopqrstwxyz";
        private static readonly string PASSWORD_CHARS_UCASE = "ABCDEFGHJKLMNPQRSTWXYZ";
        private static readonly string PASSWORD_CHARS_NUMERIC = "23456789";

        public static string Generate()
        {
            return Generate(DEFAULT_MIN_PASSWORD_LENGTH,
                DEFAULT_MAX_PASSWORD_LENGTH);
        }

        public static string Generate(int length)
        {
            return Generate(length, length);
        }

        public static string Generate(int minLength,
            int maxLength)
        {
            if (minLength <= 0 || maxLength <= 0 || minLength > maxLength)
                return null;

            char[][] charGroups =
            {
                PASSWORD_CHARS_LCASE.ToCharArray(),
                PASSWORD_CHARS_UCASE.ToCharArray(),
                PASSWORD_CHARS_NUMERIC.ToCharArray()
            };

            var charsLeftInGroup = new int[charGroups.Length];

            for (var i = 0; i < charsLeftInGroup.Length; i++)
                charsLeftInGroup[i] = charGroups[i].Length;

            var leftGroupsOrder = new int[charGroups.Length];

            for (var i = 0; i < leftGroupsOrder.Length; i++)
                leftGroupsOrder[i] = i;
            var randomBytes = new byte[4];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes);

            var seed = BitConverter.ToInt32(randomBytes, 0);

            var random = new Random(seed);

            char[] password = null;

            password = minLength < maxLength ? new char[random.Next(minLength, maxLength + 1)] : new char[minLength];

            var lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;

            for (var i = 0; i < password.Length; i++)
            {
                int nextLeftGroupsOrderIdx;
                if (lastLeftGroupsOrderIdx == 0)
                    nextLeftGroupsOrderIdx = 0;
                else
                    nextLeftGroupsOrderIdx = random.Next(0,
                        lastLeftGroupsOrderIdx);

                var nextGroupIdx = leftGroupsOrder[nextLeftGroupsOrderIdx];

                var lastCharIdx = charsLeftInGroup[nextGroupIdx] - 1;

                var nextCharIdx = lastCharIdx == 0 ? 0 : random.Next(0, lastCharIdx + 1);

                password[i] = charGroups[nextGroupIdx][nextCharIdx];

                if (lastCharIdx == 0)
                {
                    charsLeftInGroup[nextGroupIdx] =
                        charGroups[nextGroupIdx].Length;
                }
                else
                {
                    if (lastCharIdx != nextCharIdx)
                    {
                        var temp = charGroups[nextGroupIdx][lastCharIdx];
                        charGroups[nextGroupIdx][lastCharIdx] =
                            charGroups[nextGroupIdx][nextCharIdx];
                        charGroups[nextGroupIdx][nextCharIdx] = temp;
                    }
                    charsLeftInGroup[nextGroupIdx]--;
                }

                if (lastLeftGroupsOrderIdx == 0)
                {
                    lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
                }
                else
                {
                    if (lastLeftGroupsOrderIdx != nextLeftGroupsOrderIdx)
                    {
                        var temp = leftGroupsOrder[lastLeftGroupsOrderIdx];
                        leftGroupsOrder[lastLeftGroupsOrderIdx] =
                            leftGroupsOrder[nextLeftGroupsOrderIdx];
                        leftGroupsOrder[nextLeftGroupsOrderIdx] = temp;
                    }
                    lastLeftGroupsOrderIdx--;
                }
            }
            return new string(password);
        }
    }
}
