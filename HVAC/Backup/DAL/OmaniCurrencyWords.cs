using System;
using System.Text;

namespace HVAC.DAL
{
    public static class OmaniCurrencyWords
    {
        private static readonly string[] UnitsMap =
        { "zero","one","two","three","four","five","six","seven","eight","nine",
      "ten","eleven","twelve","thirteen","fourteen","fifteen","sixteen",
      "seventeen","eighteen","nineteen" };

        private static readonly string[] TensMap =
        { "zero","ten","twenty","thirty","forty","fifty","sixty","seventy","eighty","ninety" };

        private static readonly string[] ScaleMap = { "", " thousand", " million", " billion", " trillion" };

        /// <summary>
        /// Convert an amount to words for OMR (Omani Rial & baisa). Rounds to 3 decimals (baisa).
        /// </summary>
        public static string ToOmaniCurrencyWords(decimal amount)
        {
            if (amount == 0m) return "Zero Omani Rials";

            // Round to the nearest baisa (3 decimal places)
            amount = Math.Round(amount, 3, MidpointRounding.AwayFromZero);

            bool isNegative = amount < 0m;
            if (isNegative) amount = Math.Abs(amount);

            long rials = (long)Math.Floor(amount);
            int baisa = (int)Math.Round((amount - rials) * 1000m, MidpointRounding.AwayFromZero);
            if (baisa == 1000) { rials += 1; baisa = 0; } // handle 0.9995 → next rial

            var sb = new StringBuilder();

            if (isNegative) sb.Append("Negative ");

            // Rials
            sb.Append($"{NumberToWords(rials)} Omani {(rials == 1 ? "Rial" : "Rials")}");

            // Baisa
            if (baisa > 0)
            {
                sb.Append(" and ");
                sb.Append($"{NumberToWords(baisa)} {(baisa == 1 ? "Baisa" : "Baisas")}");
            }

            return ToTitleCase(sb.ToString());
        }

        private static string NumberToWords(long number)
        {
            if (number == 0) return "zero";

            var sb = new StringBuilder();
            int scaleIndex = 0;

            while (number > 0 && scaleIndex < ScaleMap.Length)
            {
                int chunk = (int)(number % 1000);
                if (chunk != 0)
                {
                    string chunkWords = ThreeDigitToWords(chunk);
                    if (sb.Length > 0)
                        sb.Insert(0, ", "); // comma between scales

                    sb.Insert(0, chunkWords + ScaleMap[scaleIndex]);
                }

                number /= 1000;
                scaleIndex++;
            }

            return sb.ToString();
        }

        private static string ThreeDigitToWords(int n)
        {
            var sb = new StringBuilder();

            int hundreds = n / 100;
            int remainder = n % 100;

            if (hundreds > 0)
            {
                sb.Append(UnitsMap[hundreds]);
                sb.Append(" hundred");
                if (remainder > 0) sb.Append(" and ");
            }

            if (remainder > 0)
            {
                if (remainder < 20)
                {
                    sb.Append(UnitsMap[remainder]);
                }
                else
                {
                    int tens = remainder / 10;
                    int units = remainder % 10;
                    sb.Append(TensMap[tens]);
                    if (units > 0)
                    {
                        sb.Append("-");
                        sb.Append(UnitsMap[units]);
                    }
                }
            }

            return sb.ToString();
        }

        private static string ToTitleCase(string s)
        {
            // Keep common currency casing tidy: capitalize first letters only.
            // Avoid CultureInfo to keep it deterministic across servers.
            var chars = s.ToCharArray();
            bool newWord = true;
            for (int i = 0; i < chars.Length; i++)
            {
                if (char.IsLetter(chars[i]))
                {
                    chars[i] = newWord ? char.ToUpperInvariant(chars[i]) : char.ToLowerInvariant(chars[i]);
                    newWord = false;
                }
                else
                {
                    newWord = (chars[i] == ' ' || chars[i] == '-' || chars[i] == ',');
                }
            }
            return new string(chars);
        }
    }
}