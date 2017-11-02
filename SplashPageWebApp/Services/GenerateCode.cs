using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApp1
{
    public class GenerateCode
    {
        public static string GeneratedCode()
        {
            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            byte[] key = Guid.NewGuid().ToByteArray();
            String generatedCode = Convert.ToBase64String(time.Concat(key).ToArray());
            return new String(generatedCode.Where(Char.IsLetter).ToArray()).Remove(7);
        } 
    }
}
