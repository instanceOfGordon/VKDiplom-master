using System;

namespace SymbolicDifferentiation
{
    static class StringExtensions
    {
        public static int First(this string str, char character)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == character)
                    return i;
            }
            return -1;
        }

        public static int Last(this string str, char character)
        {
            for (int i = str.Length - 1; i >= 0; i--)
            {
                if (str[i] == character)
                    return i;
            }
            return -1;
        }

        public static string OptimizeSign(this string str)
        {
            var nIndex = 0;
            // var operators = new[] {'(', '+', '-', '/', '*', '^'};
            // replace "--" with "" or "+"
            while ((nIndex = str.IndexOf("--", nIndex, StringComparison.Ordinal)) != -1)
                if (nIndex == 0 || "(+-/*^".Contains(str[nIndex - 1].ToString()))
                    str = str.Remove(nIndex, 2);
                else
                {
                    str = str.Remove(nIndex, 1);
                    var strChars = str.ToCharArray();
                    strChars[nIndex] = '+';
                    str = new string(strChars);
                }

            nIndex = 0;
            // replace "+-" with "-"

            while ((nIndex = str.IndexOf("+-", nIndex, StringComparison.Ordinal)) != -1)
                str = str.Remove(nIndex);

            return str;
        }

        public static bool IsNumeric(this string lpcs)
        {
            var p = 0;
            if (lpcs[p] == '-' || lpcs[p] == '+')
                p++;
            if (lpcs[p] == 'e' && lpcs[p + 1] == 0)
                return true;
            if (lpcs[p] == 'p' && lpcs[p + 1] == 'i' && lpcs[p + 2] == 0)
                return true;
            while (p < lpcs.Length)
            {
                if (!Char.IsDigit(lpcs[p]))
                    return false;
                p++;
            }
            return true;
        }

        public static int GetClose(this string p, int i)
        {
            // initialize parenthesis count to one
            var nOpen = 1;
            // loop in the input to find the close parenthesis
            //var i = 0;
            while (++i < p.Length)
                if (p[i] == '(')
                    nOpen++;
                else if (p[i] == ')')
                    if (--nOpen == 0)
                        return ++i;
            return -1;
        }

        public static string Optimize(this string str)
        {
            var nLength = str.Length;

            str = OptimizeSign(str);

            var pClose = 0;
            var nIndex = -1;
            // replace "((....))"  with "(....)"
            while ((nIndex<str.Length)&&(nIndex = str.IndexOf("((", nIndex + 1, StringComparison.Ordinal)) != -1)
                pClose = GetClose(str, nIndex + 1);
            if (str[pClose] == ')')
            {
                str = str.Remove(pClose,1);
                str = str.Remove(nIndex,1);
            }

            nIndex = -1;
            // remove any 1*
            while ((nIndex < str.Length) && (nIndex = str.IndexOf("1*", nIndex + 1, StringComparison.Ordinal)) != -1)
                if (nIndex == 0 || "+-*(".Contains(str[nIndex - 1].ToString()))
                    str = str.Remove(nIndex, 2);

            nIndex = -1;
            // remove any *1
            while ((nIndex < str.Length) && (nIndex = str.IndexOf("*1", nIndex + 1, StringComparison.Ordinal)) != -1)
                if (nIndex + 2 == str.Length || "+-*(".Contains(str[nIndex + 2].ToString()))
                    str = str.Remove(nIndex, 2);

            nIndex = -1;
            // remove any exponent equal 1
            while ((nIndex < str.Length) && (nIndex = str.IndexOf("^1", nIndex + 1, StringComparison.Ordinal)) != -1)
                if (nIndex + 2 == str.Length || "+-*(".Contains(str[nIndex + 2].ToString()))
                    str = str.Remove(nIndex, 2);

            nIndex = 0;
            // remove unneeded parentheses 
            while ((nIndex < str.Length) && (nIndex = str.IndexOf('(', nIndex)) != -1)
            {
                // "nscthg0" is the end characters of all supported functions
                if (nIndex > 0 && "nscthg0".Contains(str[nIndex - 1].ToString()))
                {
                    nIndex++;
                    continue;
                }
                // find the parenthesis close
                pClose = GetClose(str, nIndex);
                if (pClose == -1)
                    return str;
                // get the index of the close char
                var nCloseIndex = pClose - 1;
                // check if the parentheses in the start and the end of the string
                if ((nIndex == 0 && nCloseIndex == str.Length - 1) ||
                    nCloseIndex == nIndex + 2 ||
                    // check if the string doesn't include any operator
                    IsNumeric(str.Substring(nIndex + 1, nCloseIndex - nIndex - 1)))
                {
                    // delete the far index of ')'
                    str =str.Remove(nCloseIndex,1);
                    // delete the near index of '('
                    str =str.Remove(nIndex,1);
                }
                else
                    nIndex++;
            }

            if (nLength != str.Length)
                return str.Optimize();
            return str;
        }

        public static string InsertTabs(this string str)
        {
            if (str == "")
                return str;
            str = str.Insert(0, "\t");
            str = str.Replace("\n", "\n\t");
            str = str.TrimEnd('\t');
            return str;
        }
    }
}
