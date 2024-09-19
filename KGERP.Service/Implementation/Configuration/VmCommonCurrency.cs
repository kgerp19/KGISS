using System;

namespace KG.Core.Services.Configuration
{
    public enum CurrencyType
    {
        USD,
        BDT
    }
    public static class VmCommonCurrency
    {
        public static string CurrencyConverter(decimal d, CurrencyType ct)
        {
            string s = d.ToString("F");
            string s1 = "";
            string s2 = "";
            int n1 = 0;
            int n2 = 0;
            if (s.Contains("."))
            {
                s1 = s.Substring(0, s.IndexOf("."));
                s2 = s.Replace(s1 + ".", "");
            }
            else
            {
                s1 = s;
                s2 = "00";
            }
            int.TryParse(s1, out n1);
            int.TryParse(s2, out n2);
            if (ct == CurrencyType.USD)
            {
                s1 = USDNumberToWords(n1);
                s2 = USDNumberToWords(n2);
                s = " U.S Dollar " + s1 + " & Cents " + s2 + " Only";
            }
            else if (ct == CurrencyType.BDT)
            {
                s = TakaConvertToWords(s); ///s1 + " Taka  & " + s2 + " Paisa Only";
            }
            return s;
        }

        public static string NumberToWords(decimal d, CurrencyType ct)
        {
            string s = d.ToString("F");
            string s1 = "";
            string s2 = "";
            int n1 = 0;
            int n2 = 0;
            if (s.Contains("."))
            {
                s1 = s.Substring(0, s.IndexOf("."));
                s2 = s.Replace(s1 + ".", "");
            }
            else
            {
                s1 = s;
                s2 = "00";
            }
            int.TryParse(s1, out n1);
            int.TryParse(s2, out n2);
            s1 = USDNumberToWords(n1);
            s2 = USDNumberToWords(n2);
            if (ct == CurrencyType.USD)
            {
                s = s1 + " U.S Dollar & " + s2 + " Cents Only";
            }
            else if (ct == CurrencyType.BDT)
            {
                s = TakaConvertToWords(s);//s1 + " Taka  & " + s2 + " Paisa Only";
            }
            return s;
        }
        public static String NumberToComma(decimal amount)
        {
            string result = "";
            string amt = "";
            string amt_paisa = "";

            amt = amount.ToString();
            int aaa = amount.ToString().IndexOf(".", 0);
            amt_paisa = amount.ToString().Substring(aaa + 1);

            if (amt == amt_paisa)
            {
                amt_paisa = "";
            }
            else
            {
                amt = amount.ToString().Substring(0, amount.ToString().IndexOf(".", 0));
                amt = (amt.Replace(",", "")).ToString();
            }
            switch (amt.Length)
            {
                case 9:
                    if (amt_paisa == "")
                    {
                        result = amt.Substring(0, 2) + "," + amt.Substring(2, 2) + "," +
                                 amt.Substring(4, 2) + "," + amt.Substring(6, 3);
                    }
                    else
                    {
                        result = amt.Substring(0, 2) + "," + amt.Substring(2, 2) + "," +
                                 amt.Substring(4, 2) + "," + amt.Substring(6, 3) + "." +
                                 amt_paisa;
                    }
                    break;
                case 8:
                    if (amt_paisa == "")
                    {
                        result = amt.Substring(0, 1) + "," + amt.Substring(1, 2) + "," +
                                 amt.Substring(3, 2) + "," + amt.Substring(5, 3);
                    }
                    else
                    {
                        result = amt.Substring(0, 1) + "," + amt.Substring(1, 2) + "," +
                                 amt.Substring(3, 2) + "," + amt.Substring(5, 3) + "." +
                                 amt_paisa;
                    }
                    break;
                case 7:
                    if (amt_paisa == "")
                    {
                        result = amt.Substring(0, 2) + "," + amt.Substring(2, 2) + "," +
                                 amt.Substring(4, 3);
                    }
                    else
                    {
                        result = amt.Substring(0, 2) + "," + amt.Substring(2, 2) + "," +
                                 amt.Substring(4, 3) + "." + amt_paisa;
                    }
                    break;
                case 6:
                    if (amt_paisa == "")
                    {
                        result = amt.Substring(0, 1) + "," + amt.Substring(1, 2) + "," +
                                 amt.Substring(3, 3);
                    }
                    else
                    {
                        result = amt.Substring(0, 1) + "," + amt.Substring(1, 2) + "," +
                                 amt.Substring(3, 3) + "." + amt_paisa;
                    }
                    break;
                case 5:
                    if (amt_paisa == "")
                    {
                        result = amt.Substring(0, 2) + "," + amt.Substring(2, 3);
                    }
                    else
                    {
                        result = amt.Substring(0, 2) + "," + amt.Substring(2, 3) + "." +
                                 amt_paisa;
                    }
                    break;
                case 4:
                    if (amt_paisa == "")
                    {
                        result = amt.Substring(0, 1) + "," + amt.Substring(1, 3);
                    }
                    else
                    {
                        result = amt.Substring(0, 1) + "," + amt.Substring(1, 3) + "." +
                                 amt_paisa;
                    }
                    break;
                default:
                    if (amt_paisa == "")
                    {
                        result = amt;
                    }
                    else
                    {
                        result = amt + "." + amt_paisa;
                    }
                    break;
            }
            return result;
        }
        private static string USDNumberToWords(int number)
        {
            if (number == 0)
                return "Zero";

            if (number < 0)
                return "Minus " + USDNumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += USDNumberToWords(number / 1000000) + " Million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += USDNumberToWords(number / 1000) + " Thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += USDNumberToWords(number / 100) + " Hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[]
                {
                    "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven",
                    "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen"
                };
                var tensMap = new[]
                    {"Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety"};

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        //words += "-" + unitsMap[number % 10];
                        words += unitsMap[number % 10];
                }
            }
            return words;
        }

        public static String TakaConvertToWords(String numb)
        {
            String val, wholeNo = numb;
            String andStr = "", pointStr = "";
            String endStr = "Only";
            try
            {
                int decimalPlace = numb.IndexOf(".", StringComparison.Ordinal);
                if (decimalPlace > 0)
                {
                    wholeNo = numb.Substring(0, decimalPlace);
                    var points = numb.Substring(decimalPlace + 1);
                    if (Convert.ToInt32(points) > 0)
                    {
                        andStr = "and"; // just to separate whole numbers from points/cents  
                        endStr = "Paisa " + endStr; //Cents  
                        pointStr = ConvertDecimals(points);
                    }
                }
                val = String.Format("{0} {1}{2} {3}", TakaConvertWholeNumber(wholeNo).Trim() + " Taka ", andStr, pointStr, endStr);
            }
            catch
            {
                return "Invalid number......";
            }
            return val;
        }
        private static String TakaConvertWholeNumber(String number)
        {
            string word = "";
            try
            {
                bool isDone = false; //test if already translated
                double dblAmt = (Convert.ToDouble(number));
                //if ((dblAmt > 0) && number.StartsWith("0"))
                if (dblAmt > 0)
                {
                    //test for zero or digit zero in a nuemric
                    //double numDigits = dblAmt
                    int numDigits = dblAmt.ToString().Length;
                    int pos = 0; //store digit grouping
                    String place = ""; //digit grouping name:hundres,thousand,etc...
                    switch (numDigits)
                    {
                        case 1: //ones' range

                            word = TakaOnes(dblAmt.ToString());
                            isDone = true;
                            break;
                        case 2: //tens' range
                            word = TakaTens(dblAmt.ToString());
                            isDone = true;
                            break;
                        case 3: //hundreds' range
                            pos = (numDigits % 3) + 1;
                            place = " Hundred ";
                            break;
                        case 4: //thousands' range
                        case 5:
                            pos = (numDigits % 4) + 1;
                            place = " Thousand ";
                            break;
                        case 6: //Lakh' range
                        case 7:
                            pos = (numDigits % 6) + 1;
                            place = " Lakh ";
                            break;
                        case 8: //Lakh's range
                        case 9:
                        case 10:
                        case 11:
                        case 12:
                        case 13:
                        case 14:
                        case 15:
                        case 16:
                            pos = (numDigits % 8) + 1;
                            place = " Crore ";
                            break;
                        //add extra case options for anything above Billion...
                        default:
                            isDone = true;
                            break;
                    }
                    if (!isDone)
                    {
                        //if transalation is not done, continue...(Recursion comes in now!!)
                        if (number.Substring(0, pos) != "0" && number.Substring(pos) != "0")
                        {
                            try
                            {
                                word = TakaConvertWholeNumber(number.Substring(0, pos)) + place +
                                       TakaConvertWholeNumber(number.Substring(pos));
                            }
                            catch
                            {
                                //ignore
                            }
                        }
                        else
                        {
                            word = TakaConvertWholeNumber(number.Substring(0, pos)) +
                                   TakaConvertWholeNumber(number.Substring(pos));
                        }

                        //check for trailing zeros
                        //if (beginsZero) word = " and " + word.Trim();
                    }
                    //ignore digit grouping names
                    if (word.Trim().Equals(place.Trim())) word = "";
                }
            }
            catch
            {
                return "Invalid number.....";
            }
            return word.Trim();
        }

        private static String TakaTens(String number)
        {
            int numberiInt32 = Convert.ToInt32(number);
            String name = null;
            switch (numberiInt32)
            {
                case 10:
                    name = "Ten";
                    break;
                case 11:
                    name = "Eleven";
                    break;
                case 12:
                    name = "Twelve";
                    break;
                case 13:
                    name = "Thirteen";
                    break;
                case 14:
                    name = "Fourteen";
                    break;
                case 15:
                    name = "Fifteen";
                    break;
                case 16:
                    name = "Sixteen";
                    break;
                case 17:
                    name = "Seventeen";
                    break;
                case 18:
                    name = "Eighteen";
                    break;
                case 19:
                    name = "Nineteen";
                    break;
                case 20:
                    name = "Twenty";
                    break;
                case 30:
                    name = "Thirty";
                    break;
                case 40:
                    name = "Fourty";
                    break;
                case 50:
                    name = "Fifty";
                    break;
                case 60:
                    name = "Sixty";
                    break;
                case 70:
                    name = "Seventy";
                    break;
                case 80:
                    name = "Eighty";
                    break;
                case 90:
                    name = "Ninety";
                    break;
                default:
                    if (numberiInt32 > 0)
                    {
                        name = TakaTens(number.Substring(0, 1) + "0") + " " + TakaOnes(number.Substring(1));
                    }
                    break;
            }
            return name;
        }
        private static String TakaOnes(String number)
        {
            int numberiInt32 = Convert.ToInt32(number);
            String name = "";
            switch (numberiInt32)
            {

                case 1:
                    name = "One";
                    break;
                case 2:
                    name = "Two";
                    break;
                case 3:
                    name = "Three";
                    break;
                case 4:
                    name = "Four";
                    break;
                case 5:
                    name = "Five";
                    break;
                case 6:
                    name = "Six";
                    break;
                case 7:
                    name = "Seven";
                    break;
                case 8:
                    name = "Eight";
                    break;
                case 9:
                    name = "Nine";
                    break;
            }
            return name;
        }

        private static String ConvertDecimals(String number)
        {
            String cd = "";
            foreach (char t in number)
            {
                var digit = t.ToString();
                string engOne;
                if (digit.Equals("0"))
                {
                    engOne = "Zero";
                }
                else
                {
                    engOne = TakaOnes(digit);
                }
                cd += " " + engOne;
            }
            return cd;
        }
    }
}
