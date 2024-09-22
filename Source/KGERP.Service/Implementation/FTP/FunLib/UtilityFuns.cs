using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.IO.Compression;

namespace KGERP.FunLib
{
    [Serializable]
    public static class UtilityFuns
    {
        public static string FilterString(string SourceString)
        {
            SourceString = SourceString.Trim();
            SourceString = SourceString.Replace("'", "''");
            return SourceString;
        }

        public static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        public static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }

        public static string Left(string host, int index)
        {
            return host.Substring(0, index);

        }
        public static string Right(string host, int index)
        {
            return host.Substring(host.Length - index);
        }
        public static string ExprToValue(string cExpr)
        {
            string mExpr1 = cExpr.Trim().Replace(",", "");
            mExpr1 = mExpr1.Replace("/", " div ");
            XmlDocument xmlDoc = new XmlDocument();
            XPathNavigator xPathNavigator = xmlDoc.CreateNavigator();
            mExpr1 = xPathNavigator.Evaluate(mExpr1).ToString();
            return mExpr1;
        }

        public static string Text2Value(string InputValue)
        {
            // Calculator
            string OutputValue = "0.00";
            #region javascript calculat

            Type scriptType = Type.GetTypeFromCLSID(Guid.Parse("0E59F1D5-1FBE-11D0-8FF2-00A0D10038BC"));
            dynamic obj = Activator.CreateInstance(scriptType, false);
            obj.Language = "Javascript";
            string str = null;
            try
            {
                dynamic res = obj.Eval(InputValue);
                str = Convert.ToString(res);
                //this.txtbFResult.Text = this.txtResult.Text + "=" + str;
                OutputValue = str;
            }
            catch (Exception)
            {
                return OutputValue;
                //throw;
            }
            #endregion
            return OutputValue;
        }

        public static string Text2IntValue(string InputValue)
        {
            string OutputValue = "0.00";
            string[] num = Regex.Split(InputValue, @"\-|\+|\*|\/").Where(s => !String.IsNullOrEmpty(s)).ToArray(); // get Array for numbers
            string[] op = Regex.Split(InputValue, @"\d{1,3}").Where(s => !String.IsNullOrEmpty(s)).ToArray(); // get Array for mathematical operators +,-,/,*
            int numCtr = 0, lastVal = 0; // number counter and last Value accumulator
            string lastOp = ""; // last Operator
            foreach (string n in num)
            {
                numCtr++;
                if (numCtr == 1)
                {
                    lastVal = int.Parse(n); // if first loop lastVal will have the first numeric value
                }
                else
                {
                    if (!String.IsNullOrEmpty(lastOp)) // if last Operator not empty
                    {
                        // Do the mathematical computation and accumulation
                        switch (lastOp)
                        {
                            case "+":
                                lastVal = lastVal + int.Parse(n);
                                break;
                            case "-":
                                lastVal = lastVal - int.Parse(n);
                                break;
                            case "*":
                                lastVal = lastVal * int.Parse(n);
                                break;
                            case "/":
                                lastVal = lastVal / int.Parse(n);
                                break;

                        }
                    }
                }
                int opCtr = 0;
                foreach (string o in op)
                {
                    opCtr++;
                    if (opCtr == numCtr) //will make sure it will get the next operator
                    {
                        lastOp = o;  // get the last operator
                        break;
                    }
                }
                OutputValue = lastVal.ToString();
            }
            return OutputValue;
        }

        public static byte[] GetBytes(object obj1)
        {
            try
            {
                return (byte[])obj1;
            }
            catch
            {
                //string msg1 = exp.Message;
                return null;
            }
        }

        public static string Trans(double XX1, int Index)
        {
            Index = (Index == 0 ? 1 : Index);
            string[] X1 = new string[101];
            string[] Y1 = new string[6];
            string[] Z1 = new string[3];

            X1[0] = "Zero ";
            X1[1] = "One ";
            X1[2] = "Two ";
            X1[3] = "Three ";
            X1[4] = "Four ";
            X1[5] = "Five ";
            X1[6] = "Six ";
            X1[7] = "Seven ";
            X1[8] = "Eight ";
            X1[9] = "Nine ";
            X1[10] = "Ten ";
            X1[11] = "Eleven ";
            X1[12] = "Twelve ";
            X1[13] = "Thirteen ";
            X1[14] = "Fourteen ";
            X1[15] = "Fifteen ";
            X1[16] = "Sixteen ";
            X1[17] = "Seventeen ";
            X1[18] = "Eighteen ";
            X1[19] = "Nineteen ";
            X1[20] = "Twenty ";
            X1[30] = "Thirty ";
            X1[40] = "Forty ";
            X1[50] = "Fifty ";
            X1[60] = "Sixty ";
            X1[70] = "Seventy ";
            X1[80] = "Eighty ";
            X1[90] = "Ninety ";

            for (int J1 = 20; J1 <= 90; J1 = J1 + 10)
                for (int I1 = 1; I1 <= 9; I1++)
                    X1[J1 + I1] = X1[J1] + X1[I1];

            Y1[1] = "Hundred ";
            Y1[2] = "Thousand ";
            Y1[3] = (Index >= 3 ? "Million " : "Lac ");
            Y1[4] = (Index >= 3 ? "Billion " : "Crore ");
            Y1[5] = "Trillion ";
            Z1[1] = "Minus ";
            Z1[2] = "Zero ";
            long N_1 = System.Convert.ToInt64(Math.Floor(XX1));
            string N_2 = XX1.ToString();
            while (!(N_2.Length == 0))
            {
                if (N_2.Substring(0, 1) == ".")
                    break;
                N_2 = N_2.Substring(1);
            }
            N_2 = (N_2.Length == 0 ? " " : N_2);
            switch (Index)
            {
                case 1:
                case 3:
                    N_2 = ((N_2.Substring(0, 1) == ".") ? ((string)(N_2.Substring(1) + "00000")).Substring(0, 5) : "00000");
                    break;
                case 2:
                case 4:
                    N_2 = ((N_2.Substring(0, 1) == ".") ? ((string)(N_2.Substring(1) + "00000")).Substring(0, 2) : "00");
                    break;
            }
            string S_GN = (Math.Sign(N_1) == -1 ? Z1[1] : "");
            string Z1_ER = (N_1 == 0 ? Z1[2] : "");
            string N_O = Right("00000000000000000" + Math.Abs(N_1).ToString(), 17);
            string[] L = new string[100];
            switch (Index)
            {
                case 1:
                case 2:
                    L[0] = "";
                    L[1] = ((Convert.ToInt32(N_O.Substring(0, 1)) == 0) ? "" : X1[Int32.Parse(N_O.Substring(0, 1))] + Y1[1]);
                    L[2] = ((Convert.ToInt32(N_O.Substring(1, 2)) == 0) ? "" : X1[Int32.Parse(N_O.Substring(1, 2))] + Y1[4]);
                    L[3] = ((Convert.ToInt32(N_O.Substring(3, 2)) == 0) ? "" : X1[Int32.Parse(N_O.Substring(3, 2))] + Y1[3]);
                    L[4] = ((Convert.ToInt32(N_O.Substring(5, 2)) == 0) ? "" : X1[Int32.Parse(N_O.Substring(5, 2))] + Y1[2]);
                    L[5] = ((Convert.ToInt32(N_O.Substring(7, 1)) == 0) ? "" : X1[Int32.Parse(N_O.Substring(7, 1))] + Y1[1]);
                    L[6] = ((Convert.ToInt32(N_O.Substring(8, 2)) == 0) ? ((Convert.ToInt32(N_O.Substring(0, 10))) == 0 ? "" : Y1[4]) : X1[Int32.Parse(N_O.Substring(8, 2))] + Y1[4]);
                    L[7] = ((Convert.ToInt32(N_O.Substring(10, 2)) == 0) ? "" : X1[Int32.Parse(N_O.Substring(10, 2))] + Y1[3]);
                    L[8] = ((Convert.ToInt32(N_O.Substring(12, 2)) == 0) ? "" : X1[Int32.Parse(N_O.Substring(12, 2))] + Y1[2]);
                    L[9] = ((Convert.ToInt32(N_O.Substring(14, 1)) == 0) ? "" : X1[Int32.Parse(N_O.Substring(14, 1))] + Y1[1]);
                    L[10] = (Convert.ToInt32(N_O.Substring(15, 2)) == 0) ? "" : X1[Int32.Parse(N_O.Substring(15, 2))];
                    break;
                case 3:
                case 4:
                    L[0] = ((Convert.ToInt32(N_O.Substring(0, 2)) == 0) ? "" : X1[Int32.Parse(N_O.Substring(0, 2))] + Y1[2]);
                    L[1] = ((Convert.ToInt32(N_O.Substring(2, 1)) == 0) ? "" : X1[Int32.Parse(N_O.Substring(2, 1))] + Y1[1]);
                    L[2] = ((Convert.ToInt32(N_O.Substring(3, 2)) == 0) ? ((Convert.ToInt32(N_O.Substring(2, 1)) == 0) ? "" : Y1[5]) : X1[Int32.Parse(N_O.Substring(3, 2))] + Y1[5]);
                    L[3] = ((Convert.ToInt32(N_O.Substring(5, 1)) == 0) ? "" : X1[Int32.Parse(N_O.Substring(5, 1))] + Y1[1]);
                    L[4] = ((Convert.ToInt32(N_O.Substring(6, 2)) == 0) ? ((Convert.ToInt32(N_O.Substring(5, 1)) == 0) ? "" : Y1[4]) : X1[Int32.Parse(N_O.Substring(6, 2))] + Y1[4]);
                    L[5] = ((Convert.ToInt32(N_O.Substring(8, 1)) == 0) ? "" : X1[Int32.Parse(N_O.Substring(8, 1))] + Y1[1]);
                    L[6] = ((Convert.ToInt32(N_O.Substring(9, 2)) == 0) ? ((Convert.ToInt32(N_O.Substring(8, 1)) == 0) ? "" : Y1[3]) : X1[Int32.Parse(N_O.Substring(9, 2))] + Y1[3]);
                    L[7] = ((Convert.ToInt32(N_O.Substring(11, 1)) == 0) ? "" : X1[Int32.Parse(N_O.Substring(11, 1))] + Y1[1]);
                    L[8] = ((Convert.ToInt32(N_O.Substring(12, 2)) == 0) ? ((Convert.ToInt32(N_O.Substring(11, 1)) == 0) ? "" : Y1[2]) : X1[Int32.Parse(N_O.Substring(12, 2))] + Y1[2]);
                    L[9] = ((Convert.ToInt32(N_O.Substring(14, 1)) == 0) ? "" : X1[Int32.Parse(N_O.Substring(14, 1))] + Y1[1]);
                    L[10] = (Convert.ToInt32(N_O.Substring(15, 2)) == 0) ? "" : X1[Int32.Parse(N_O.Substring(15, 2))];
                    break;
            }
            string O = S_GN + Z1_ER + L[0] + L[1] + L[2] + L[3] + L[4] + L[5] + L[6] + L[7] + L[8] + L[9] + L[10];
            string[] M = new string[100];
            string Q_ = "";
            string P = "";

            switch (Index)
            {
                case 1:
                case 3:
                    M[1] = ((Convert.ToInt32(N_2) >= 1) ? X1[Int32.Parse(N_2.Substring(0, 1))] : "");
                    M[2] = ((Convert.ToInt32(N_2) >= 1 & Convert.ToInt32(N_2.Substring(1)) >= 1) ? X1[Int32.Parse(N_2.Substring(1, 1))] : "");
                    M[3] = ((Convert.ToInt32(N_2) >= 1 & Convert.ToInt32(N_2.Substring(2)) >= 1) ? X1[Int32.Parse(N_2.Substring(2, 1))] : "");
                    M[4] = ((Convert.ToInt32(N_2) >= 1 & Convert.ToInt32(N_2.Substring(3 - 1)) >= 1) ? X1[Int32.Parse(N_2.Substring(3, 1))] : "");
                    M[5] = ((Convert.ToInt32(N_2) >= 1 & Convert.ToInt32(N_2.Substring(4)) >= 1) ? X1[Convert.ToInt32(N_2.Substring(4, 1))] : "");
                    M[6] = ((Convert.ToInt32(N_2) > 0) ? "Point " : "");
                    P = M[6] + M[1] + M[2] + M[3] + M[4] + M[5];
                    Q_ = O + P;
                    break;
                case 2:
                    M[1] = ((Convert.ToInt32(N_2) >= 1) ? X1[Int32.Parse(N_2)] : "");
                    M[6] = ((Convert.ToInt32(N_2) > 0) ? "And Paisa " : "");
                    P = M[6] + M[1];
                    Q_ = "( Taka " + O + P + "Only )";
                    break;
                case 4:
                    M[1] = ((Convert.ToInt32(N_2) >= 1) ? X1[Int32.Parse(N_2)] : "");
                    M[6] = ((Convert.ToInt32(N_2) > 0) ? "And Cent " : "");
                    P = M[6] + M[1];
                    Q_ = "( Dollar " + O + P + "Only )";
                    break;
            }
            return Q_;
        }

        //--------------------------------------------------------------------------------------------------------
        public static string DefComa(double AA) // Bangla Coma
        {
            string[] A = new string[21];
            A[1] = ((Math.Sign(AA) >= 0) ? "" : "-");
            A[2] = Math.Abs(AA).ToString("###0.00");
            A[3] = Math.Abs(AA).ToString("###0.000");
            A[3] = ((double.Parse(A[3]) - (double.Parse(A[2])))).ToString();
            A[2] = A[2] + ((Double.Parse(A[3]) >= 0.005) ? 0.01 : 0);
            A[2] = Left(A[2], A[2].Length - 1);
            A[4] = ((string)(string.Empty.PadLeft(24) + A[2])).Substring(((string)(string.Empty.PadLeft(24) + A[2])).Length - 24);
            A[5] = A[4].Substring(0, 2);
            A[6] = A[4].Substring(2, 2);
            A[7] = A[4].Substring(4, 3);
            A[8] = A[4].Substring(7, 2);
            A[9] = A[4].Substring(9, 2);
            A[10] = A[4].Substring(11, 3);
            A[11] = A[4].Substring(14, 2);
            A[12] = A[4].Substring(16, 2);
            A[13] = A[4].Substring(18, 3);
            A[14] = A[5] + "," + A[6] + "," + A[7] + "," + A[8] + "," + A[9] + "," + A[10] + "," + A[11] + "," + A[12] + "," + A[13];
            A[14] = A[14].Trim();

            while (A[14].Substring(0, 1) == ",")
            {
                A[14] = A[14].Substring(1, A[14].Length - 1);
                A[14] = A[14].Trim();
            }
            A[15] = A[14] + A[4].Substring(21, 3);
            A[16] = ((string)(string.Empty.PadLeft(24) + A[15])).Substring(((string)(string.Empty.PadLeft(24) + A[15])).Length - 24) + " ";
            A[17] = ((A[1] != "") ? "(" : "") + A[16].Trim() + ((A[1] != "") ? ")" : "");
            return A[17];
        }
        //-------------------------------------------------------------------------------------------------------       
        public static string Concat(string compname, string username, string printdate)
        {
            string concat = "";
            concat = concat + "Printed from Computer Address:" + compname + ", User:" + username + ", Time:" + printdate;
            return concat;
        }

        public static string Cominformation()
        {
            return "Software By: www.idealinfo.com, E-Mail: info@idealinfo.com";
        }

        public static string ToRoman(int N)
        {
            const string Digits = "IVXLCDM";
            int I = 0;
            int Digit = 0;
            string Temp = null;
            string Temp1 = null;
            int N1 = 0;
            Temp1 = "";
            if (N >= 1000)
            {
                String s = "MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM";
                Temp1 = s.Substring(0, N1);
                N1 = N / (1000);
                N = N - N1 * 1000;
            }
            I = 1;
            Temp = "";
            while (N > 0)
            {
                Digit = N % 10;
                N = N / 10;
                switch (Digit)
                {
                    case 1:
                        Temp = Digits.Substring(I - 1, 1) + Temp;
                        break;
                    case 2:
                        Temp = Digits.Substring(I - 1, 1) + Digits.Substring(I - 1, 1) + Temp;
                        break;
                    case 3:
                        Temp = Digits.Substring(I - 1, 1) + Digits.Substring(I - 1, 1) + Digits.Substring(I - 1, 1) + Temp;
                        break;
                    case 4:
                        Temp = Digits.Substring(I - 1, 2) + Temp;
                        break;
                    case 5:
                        Temp = Digits.Substring(I, 1) + Temp;
                        break;
                    case 6:
                        Temp = Digits.Substring(I, 1) + Digits.Substring(I - 1, 1) + Temp;
                        break;
                    case 7:
                        Temp = Digits.Substring(I, 1) + Digits.Substring(I - 1, 1) + Digits.Substring(I - 1, 1) + Temp;
                        break;
                    case 8:
                        Temp = Digits.Substring(I, 1) + Digits.Substring(I - 1, 1) + Digits.Substring(I - 1, 1) + Digits.Substring(I - 1, 1) + Temp;
                        break;
                    case 9:
                        Temp = Digits.Substring(I - 1, 1) + Digits.Substring(I + 2 - 1, 1) + Temp;
                        break;
                }
                I = I + 2;
            }
            return Temp1 + Temp;


        }

        public static string EncMD5(string password)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            UTF8Encoding encoder = new UTF8Encoding();
            Byte[] originalBytes = encoder.GetBytes(password);
            Byte[] encodedBytes = md5.ComputeHash(originalBytes);
            password = BitConverter.ToString(encodedBytes).Replace("-", "");
            var result = password.ToLower();
            return result;
        }

        public static string NumberOnly(string value)
        {
            Regex _isNumber = new Regex(@"^\d+$");
            string result = "0";
            string[] data = Regex.Split(value, "([0-9]|[.])");
            List<string> list = new List<string>();
            for (int i = 0; i < data.Length; i++)
            {
                Match m = _isNumber.Match(data[i]);
                if (m.Success)
                {
                    list.Add(data[i]);
                }
                if (data[i] == ".")
                {
                    list.Add(data[i]);
                }
            }
            string ans = "";
            for (int j = 0; j < list.Count; j++)
            {
                ans += list[j];
            }

            int count = 0;
            for (int i = 0; i < ans.Length; i++)
                if ('.' == ans[i])
                    count++;

            for (int k = count; count > 1; count--)
            {
                if (ans.Contains('.'))
                {
                    int i = ans.IndexOf('.');
                    ans = ans.Remove(i, 1);
                }
            }
            int index = 0;
            if (value.StartsWith("-") || (value.StartsWith("(") && value.EndsWith(")")))
                index = 1;
            else if (value == "")
                index = 2;

            result = (index == 0 ? ans : (index == 1 ? "-" + ans : result));
            return result;
        }
        public static string EncodePassword(string originalPassword)
        {
            Byte[] originalBytes;
            Byte[] encodedBytes;
            MD5 md5;
            md5 = new MD5CryptoServiceProvider();
            originalBytes = ASCIIEncoding.Default.GetBytes(originalPassword);
            encodedBytes = md5.ComputeHash(originalBytes);
            return BitConverter.ToString(encodedBytes);
        }

        public static string XmlSerialize(object dataToSerialize)
        {
            if (dataToSerialize == null) return null;

            using (StringWriter stringwriter = new System.IO.StringWriter())
            {
                var serializer = new XmlSerializer(dataToSerialize.GetType());
                serializer.Serialize(stringwriter, dataToSerialize);
                return stringwriter.ToString();
            }
        }

        public static T XmlDeserialize<T>(string xmlText)
        {
            if (String.IsNullOrWhiteSpace(xmlText)) return default(T);

            using (StringReader stringReader = new System.IO.StringReader(xmlText))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(stringReader);
            }
        }

        public static string UppercaseWords(string value)
        {
            char[] array = value.ToCharArray();
            // Handle the first letter in the string.
            if (array.Length >= 1)
            {
                if (char.IsLower(array[0]))
                {
                    array[0] = char.ToUpper(array[0]);
                }
            }
            // Scan through the letters, checking for spaces.
            // ... Uppercase the lowercase letters following spaces.
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i - 1] == ' ')
                {
                    if (char.IsLower(array[i]))
                    {
                        array[i] = char.ToUpper(array[i]);
                    }
                }
            }
            return new string(array);
        }

        public static List<T> DataTableToList<T>(this DataTable table) where T : class, new()
        {
            try
            {
                List<T> list = new List<T>();

                foreach (var row in table.AsEnumerable())
                {
                    T obj = new T();

                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        try
                        {
                            PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                            propertyInfo.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    list.Add(obj);
                }

                return list;
            }
            catch
            {
                return null;
            }
        }

        public static DataTable ListToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
        public static string ConvertDtToXmlString(DataTable table)
        {
            TextWriter writer = new StringWriter();
            table.WriteXml(writer);
            string xml = writer.ToString();
            return xml;
        }

        public static string UnicodeNumEng2Ban(string input1 = "")
        {
            //char[] ar1 = {'0','1','2','3','4','5','6','7','8','9' };
            char[] ar2 = { '০', '১', '২', '৩', '৪', '৫', '৬', '৭', '৮', '৯' };
            char[] ar3 = input1.ToCharArray();
            string output1 = "";
            foreach (char item in ar3)
            {
                int i1 = -1;
                bool isInt = int.TryParse(item.ToString(), out i1);
                output1 += (isInt && i1 >= 0 ? ar2[i1].ToString() : item.ToString());
            }

            return output1;
        }
        public static string UnicodeNumBan2Eng(string input1 = "")
        {
            char[] ar3 = input1.ToCharArray();
            string output1 = "";
            foreach (char item in ar3)
            {
                string item1 = item.ToString();
                switch (item1)
                {
                    case "০": output1 += "0"; break;
                    case "১": output1 += "1"; break;
                    case "২": output1 += "2"; break;
                    case "৩": output1 += "3"; break;
                    case "৪": output1 += "4"; break;
                    case "৫": output1 += "5"; break;
                    case "৬": output1 += "6"; break;
                    case "৭": output1 += "7"; break;
                    case "৮": output1 += "8"; break;
                    case "৯": output1 += "9"; break;
                    default: output1 += item1; break;
                }
            }
            return output1;
        }

        public static string Eng2BanMonthsDays()
        {
            string[] EngMonthse = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            string[] EndMonthsb = { "জানুয়ারী", "ফেব্রুয়ারী", "মার্চ", "এপ্রিল", "মে", "জুন", "জুলাই ", "আগষ্ট", "সেপ্টেম্বর", "অক্টোবর", "নভেম্বর", "ডিসেম্বর" };

            string[] EngSMonthse = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            string[] EndSMonthsb = { "জানুঃ", "ফেব্রু", "মার্চ", "এপ্রিল", "মে", "জুন", "জুলাই ", "আগষ্ট", "সেপ্টেঃ", "অক্টোঃ", "নভেঃ", "ডিসেঃ" };

            string[] BanMonthse = { "Boishakh", "Joishtho", "Asharh", "Srabon", "Bhadro", "Ashshin", "Kartik", "Ogrohaeon", "Poush", "Magh", "Falgun", "Choitro" };
            string[] BanMonthsb = { "বৈশাখ", "জৈষ্ঠ্য", "আষাঢ়", "শ্রাবণ", "ভাদ্র", "আশ্বিন", "কার্তিক", "অগ্রাহায়ন", "পৌষ", "মাঘ", "ফাল্গুন" };

            string[] EngDays = { "Saturday", "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };
            string[] BanDays = { "শনিবার", "রবিবার", "সোমবার", "মঙ্গলবার", "বুধবার", "বৃহস্পতিবার", "শুক্রবার" };

            string[] EngsDays = { "Sat", "Sun", "Mon", "Tue", "Wed", "Thu", "Fri" };
            string[] BansDays = { "শনি", "রবি", "সোম", "মঙ্গল", "বুধ", "বৃহস্পতি", "শুক্র" };

            // January       February       March   April   May     June    July    August  September   October     November        December
            // জানুয়ারী        ফেব্রুয়ারী       মার্চ     এপ্রিল        মে       জুন      জুলাই     আগষ্ট        সেপ্টেম্বর      অক্টোবর       নভেম্বর       ডিসেম্বর

            // বৈশাখ  জৈষ্ঠ্য      আষাঢ়     শ্রাবণ        ভাদ্র     আশ্বিন        কার্তিক        অগ্রাহায়ন      পৌষ      মাঘ      ফাল্গুন

            //Saturday      Sunday      Monday      Tuesday     Wednesday       Thursday, Firday
            // শনিবার         রবিবার         সোমবার   মঙ্গলবার  বুধবার    বৃহস্পতিবার শুক্রবার
            // Boishakh            Joishtho            Asharh Srabon    Bhadro  Ashshin Kartik  Ogrohaeon   Poush   Magh    Falgun  Choitro
            // PM = 12-2= দুপুর, 3-4 = অপরাহ্ন, 5 = বিকেল, 6-7, সন্ধ্যা, 8-11: রাত, AM = পূর্বাহ্ণ
            return "";
        }
        public static List<string> EngBanCalculator(string input1 = "0")
        {
            input1 = input1.Trim();
            string BanNum1 = "০১২৩৪৫৬৭৮৯";
            bool isBan1 = false;
            char[] arr1 = input1.ToCharArray();
            foreach (var item in arr1)
            {
                if (BanNum1.Contains(item.ToString()))
                {
                    isBan1 = true;
                    break;
                }
            }
            string EngNum1 = (isBan1 ? UtilityFuns.UnicodeNumBan2Eng(input1) : input1);
            string output1 = UtilityFuns.Text2Value(EngNum1);
            if (isBan1)
            {
                input1 = UtilityFuns.UnicodeNumEng2Ban(input1);
                output1 = UtilityFuns.UnicodeNumEng2Ban(output1);
            }

            var lst1 = new List<string>();
            lst1.Add(input1);
            lst1.Add(output1);
            return lst1;
        }

        public static void ObjectToObject(object source, object destination)
        {
            // Purpose : Use reflection to set property values of objects that share the same property names.
            Type s = source.GetType();
            Type d = destination.GetType();

            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

            var objSourceProperties = s.GetProperties(flags);
            var objDestinationProperties = d.GetProperties(flags);

            var propertyNames = objSourceProperties
            .Select(c => c.Name)
            .ToList();

            foreach (var properties in objDestinationProperties.Where(properties => propertyNames.Contains(properties.Name)))
            {
                try
                {
                    PropertyInfo piSource = source.GetType().GetProperty(properties.Name);

                    properties.SetValue(destination, piSource.GetValue(source, null), null);
                }
                catch
                {
                    throw;
                }

            }
        }
        public static List<T> CopyList<T>(this List<T> lst)
        {
            List<T> lstCopy = new List<T>();

            foreach (var item in lst)
            {
                var instanceOfT = Activator.CreateInstance<T>();
                ObjectToObject(item, instanceOfT);
                lstCopy.Add(instanceOfT);
            }
            return lstCopy;
        }

        public static List<T> JsonStringToList<T>(this string JsonDs1, string partName1) //where T : class, new()
        {
            dynamic obj = JsonConvert.DeserializeObject(JsonDs1);
            List<T> lstCopy = JsonConvert.DeserializeObject<List<T>>(JsonConvert.SerializeObject(obj[partName1]));
            return lstCopy;
        }

        public static String ListToJsonString<T>(this List<T> list1, string partName1)
        {
            string JsonStr1 = JsonConvert.SerializeObject(list1);
            JsonStr1 = "{\"" + partName1 + "\":" + JsonStr1 + "}";
            return JsonStr1;


            //DataTable tbl1 = UtilityFuns.ListToDataTable<T>(list1);
            //tbl1.TableName = partName1;
            //DataSet ds1 = new DataSet();
            //ds1.Tables.Add(tbl1);
            //string JsonStr2 = JsonConvert.SerializeObject(ds1);
            //return JsonStr2;
        }

        public static string StrCompress(string s)
        {
            var bytes = Encoding.Unicode.GetBytes(s);
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    msi.CopyTo(gs);
                }
                return Convert.ToBase64String(mso.ToArray());
            }
        }

        public static string StrDecompress(string s)
        {
            var bytes = Convert.FromBase64String(s);
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    gs.CopyTo(mso);
                }
                return Encoding.Unicode.GetString(mso.ToArray());
            }
        }

        //List<MyTable1> User = JsonConvert.DeserializeObject<List<MyTable1>>(JsonConvert.SerializeObject(obj["Table"]));

        /*
         * DataTable dtTable = GetEmployeeDataTable();
         * List<Employee> employeeList = dtTable.DataTableToList<Employee>();
         */
        // MH
        public static string TakaEnToBanWithCommaSeparator(decimal enTaka)
        {


            string enNumInp = Comma_SeparatorTaka(Math.Round(Convert.ToDecimal(enTaka), 2));


            //    enNumInp = this.txtboxInput.Text.Trim();  //12-February-2018
            char[] bnNum = { '০', '১', '২', '৩', '৪', '৫', '৬', '৭', '৮', '৯' };
            string banNumOutput = "";

            foreach (var c in enNumInp.ToArray())
            {
                char c1 = c;
                for (int i = 0; i <= 9; i++)
                {
                    if (c1.ToString() == i.ToString())
                    {
                        c1 = bnNum[i];
                        break;
                    }
                }
                banNumOutput += c1.ToString();  // 
            }

            return banNumOutput;
        }
        public static String Comma_SeparatorTaka(decimal amount)
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
                case 12:
                    if (amt_paisa == "")
                    {
                        result = amt.Substring(0, 1) + "," + amt.Substring(2, 2) + "," +
                                 amt.Substring(4, 2) + "," + amt.Substring(6, 3);
                    }
                    else
                    {
                        //result = amt.Substring(0, 1) + "," + amt.Substring(1, 3) + "," + amt.Substring(4, 2) + "," +
                        //        amt.Substring(6, 2) + "," + amt.Substring(8, 3) + "." +
                        //        amt_paisa;
                        result = amt.Substring(0, 2) + "," + amt.Substring(2, 3) + "," + amt.Substring(5, 2) + "," +
                                 amt.Substring(7, 2) + "," + amt.Substring(9, 3) + "." +
                                 amt_paisa;
                    }
                    break;
                case 11:
                    if (amt_paisa == "")
                    {
                        result = amt.Substring(0, 1) + "," + amt.Substring(2, 2) + "," +
                                 amt.Substring(4, 2) + "," + amt.Substring(6, 3);
                    }
                    else
                    {
                        //result = amt.Substring(0, 3) + "," + amt.Substring(3, 2) + "," +
                        //         amt.Substring(5, 2) + "," + amt.Substring(7, 3) + "." +
                        //         amt_paisa;
                        result = amt.Substring(0, 1) + "," + amt.Substring(1, 3) + "," + amt.Substring(4, 2) + "," +
                                 amt.Substring(6, 2) + "," + amt.Substring(8, 3) + "." +
                                 amt_paisa;
                    }
                    break;
                case 10:
                    if (amt_paisa == "")
                    {
                        result = amt.Substring(0, 1) + "," + amt.Substring(2, 2) + "," +
                                 amt.Substring(4, 2) + "," + amt.Substring(6, 3);
                    }
                    else
                    {
                        result = amt.Substring(0, 3) + "," + amt.Substring(3, 2) + "," +
                                 amt.Substring(5, 2) + "," + amt.Substring(7, 3) + "." +
                                 amt_paisa;
                    }
                    break;
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

        #region customFunctions > dateToBangla, DateBanglatoenglish, NumEntoBan
        public static string DateToBangla(string enNumInp, string type = "")
        {
            //    enNumInp = this.txtboxInput.Text.Trim();  //12-February-2018
            char[] bnNum = { '০', '১', '২', '৩', '৪', '৫', '৬', '৭', '৮', '৯' };
            string[] bn = { "জানু", "ফেব্রু", "মার্চ", "এপ্রি", "মে", "জুন", "জুলাই", "আগস্ট", "সেপ্টে", "অক্টো", "নভে", "ডিসে" };
            string[] bnFull = { "জানুয়ারি", "ফেব্রুয়ারি", "মার্চ", "এপ্রিল", "মে", "জুন", "জুলাই", "আগস্ট", "সেপ্টেম্বর", "অক্টোবর", "নভেম্বর", "ডিসেম্বর" };
            string[] enFull = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            //   string[] en = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            string banNumOutput = "";

            foreach (var c in enNumInp.ToArray())
            {
                char c1 = c;
                for (int i = 0; i <= 9; i++)
                {
                    if (c1.ToString() == i.ToString())
                    {
                        c1 = bnNum[i];
                        break;
                    }
                }
                banNumOutput += c1.ToString();  // 
            }
            for (int i = 0; i < 12; i++)
            {
                if (banNumOutput.Contains(enFull[i]))
                {
                    banNumOutput = banNumOutput.Replace(enFull[i], bnFull[i].ToString());
                    break;
                }
                else if (banNumOutput.Contains(enFull[i].Substring(0, 3)))
                {
                    if (type == "FULLNAME")
                        banNumOutput = banNumOutput.Replace(enFull[i].Substring(0, 3), bnFull[i]);
                    else
                        banNumOutput = banNumOutput.Replace(enFull[i].Substring(0, 3), bn[i]);
                    break;
                }
            }
            //   this.lblShow.Content = banNumOutput;


            return banNumOutput;
        }

        public static string DateToBanglaMin(string enNumInp, string type = "")
        {
            //    enNumInp = this.txtboxInput.Text.Trim();  //12-February-2018
            char[] bnNum = { '০', '১', '২', '৩', '৪', '৫', '৬', '৭', '৮', '৯' };
            string[] bn = { "জানু", "ফেব্রু", "মার্চ", "এপ্রি", "মে", "জুন", "জুলাই", "আগস্ট", "সেপ্টে", "অক্টো", "নভে", "ডিসে" };
            string[] bnFull = { "জানুয়ারি", "ফেব্রুয়ারি", "মার্চ", "এপ্রিল", "মে", "জুন", "জুলাই", "আগস্ট", "সেপ্টেম্বর", "অক্টোবর", "নভেম্বর", "ডিসেম্বর" };
            string[] enFull = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            //   string[] en = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            string banNumOutput = "";

            foreach (var c in enNumInp.ToArray())
            {
                char c1 = c;
                for (int i = 0; i <= 9; i++)
                {
                    if (c1.ToString() == i.ToString())
                    {
                        c1 = bnNum[i];
                        break;
                    }
                }
                banNumOutput += c1.ToString();  // 
            }


            for (int i = 0; i < 12; i++)
            {
                if (banNumOutput.Contains(enFull[i]))
                {
                    banNumOutput = banNumOutput.Replace(enFull[i], bnFull[i].ToString());
                    break;
                }
                else if (banNumOutput.Contains(enFull[i].Substring(0, 3)))
                {
                    if (type == "FULLNAME")
                        banNumOutput = banNumOutput.Replace(enFull[i].Substring(0, 3), bnFull[i]);
                    else
                        banNumOutput = banNumOutput.Replace(enFull[i].Substring(0, 3), bn[i]);
                    break;
                }
            }
            //   this.lblShow.Content = banNumOutput;


            return banNumOutput;
        }

        public static string DateBanglaToEnglish(string bnNumInp)
        {
            // ২৪-মে-২০২১	
            //    enNumInp = this.txtboxInput.Text.Trim();  //12-February-2018
            char[] bnNum = { '০', '১', '২', '৩', '৪', '৫', '৬', '৭', '৮', '৯' };
            char[] enNum = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            string[] bn = { "জানু", "ফেব্রু", "মার্চ", "এপ্রি", "মে", "জুন", "জুলা", "আগ", "সেপ্টে", "অক্টো", "নভে", "ডিসে" };
            string[] bnFull = { "জানুয়ারি", "ফেব্রুয়ারি", "মার্চ", "এপ্রিল", "মে", "জুন", "জুলাই", "আগস্ট", "সেপ্টেম্বর", "অক্টোবর", "নভেম্বর", "ডিসেম্বর" };
            string[] enFull = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            string[] en = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            string banNumOutput = "";

            foreach (var c in bnNumInp.ToArray())
            {
                char c1 = c;
                for (int i = 0; i <= 9; i++)
                {
                    if (c1.ToString() == bnNum[i].ToString())
                    {
                        c1 = bnNum[i];
                        break;
                    }
                }
                banNumOutput += c1.ToString();  // 
            }
            for (int i = 0; i < 12; i++)
            {
                if (banNumOutput.Contains(bnFull[i]))
                {
                    banNumOutput = banNumOutput.Replace(bnFull[i], enFull[i].ToString());
                    break;
                }
                else if (banNumOutput.Contains(bn[i]))
                {
                    banNumOutput = banNumOutput.Replace(bnFull[i].Substring(0, 3), en[i]);
                    break;
                }
            }
            //   this.lblShow.Content = banNumOutput;


            return banNumOutput;
        }

        public static string NumEnToBan(string enNumInp)
        {
            //    enNumInp = this.txtboxInput.Text.Trim();  //12-February-2018
            char[] bnNum = { '০', '১', '২', '৩', '৪', '৫', '৬', '৭', '৮', '৯' };
            string banNumOutput = "";

            foreach (var c in enNumInp.ToArray())
            {
                char c1 = c;
                for (int i = 0; i <= 9; i++)
                {
                    if (c1.ToString() == i.ToString())
                    {
                        c1 = bnNum[i];
                        break;
                    }
                }
                banNumOutput += c1.ToString();  // 
            }

            return banNumOutput;
        }

        #endregion

        static readonly string[] SizeSuffixes =
                   { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        public static string GetFileSizeSuffix(Int64 value, int decimalPlaces = 1)
        {
            if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
            if (value < 0) { return "-" + GetFileSizeSuffix(-value, decimalPlaces); }
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }


        public static Dictionary<string, string> GetMimeTypes() // its called and working
        {
            return new Dictionary<string, string>
            {
                {".323", "text/h323"},
                {".3g2", "video/3gpp2"},
                {".3gp", "video/3gpp"},
                {".3gp2", "video/3gpp2"},
                {".3gpp", "video/3gpp"},
                {".7z", "application/x-7z-compressed"},
                {".aa", "audio/audible"},
                {".AAC", "audio/aac"},
                {".aaf", "application/octet-stream"},
                {".aax", "audio/vnd.audible.aax"},
                {".ac3", "audio/ac3"},
                {".aca", "application/octet-stream"},
                {".accda", "application/msaccess.addin"},
                {".accdb", "application/msaccess"},
                {".accdc", "application/msaccess.cab"},
                {".accde", "application/msaccess"},
                {".accdr", "application/msaccess.runtime"},
                {".accdt", "application/msaccess"},
                {".accdw", "application/msaccess.webapplication"},
                {".accft", "application/msaccess.ftemplate"},
                {".acx", "application/internet-property-stream"},
                {".AddIn", "text/xml"},
                {".ade", "application/msaccess"},
                {".adobebridge", "application/x-bridge-url"},
                {".adp", "application/msaccess"},
                {".ADT", "audio/vnd.dlna.adts"},
                {".ADTS", "audio/aac"},
                {".afm", "application/octet-stream"},
                {".ai", "application/postscript"},
                {".aif", "audio/x-aiff"},
                {".aifc", "audio/aiff"},
                {".aiff", "audio/aiff"},
                {".air", "application/vnd.adobe.air-application-installer-package+zip"},
                {".amc", "application/x-mpeg"},
                {".application", "application/x-ms-application"},
                {".art", "image/x-jg"},
                {".asa", "application/xml"},
                {".asax", "application/xml"},
                {".ascx", "application/xml"},
                {".asd", "application/octet-stream"},
                {".asf", "video/x-ms-asf"},
                {".ashx", "application/xml"},
                {".asi", "application/octet-stream"},
                {".asm", "text/plain"},
                {".asmx", "application/xml"},
                {".aspx", "application/xml"},
                {".asr", "video/x-ms-asf"},
                {".asx", "video/x-ms-asf"},
                {".atom", "application/atom+xml"},
                {".au", "audio/basic"},
                {".avi", "video/x-msvideo"},
                {".axs", "application/olescript"},
                {".bas", "text/plain"},
                {".bcpio", "application/x-bcpio"},
                {".bin", "application/octet-stream"},
                {".bmp", "image/bmp"},
                {".c", "text/plain"},
                {".cab", "application/octet-stream"},
                {".caf", "audio/x-caf"},
                {".calx", "application/vnd.ms-office.calx"},
                {".cat", "application/vnd.ms-pki.seccat"},
                {".cc", "text/plain"},
                {".cd", "text/plain"},
                {".cdda", "audio/aiff"},
                {".cdf", "application/x-cdf"},
                {".cer", "application/x-x509-ca-cert"},
                {".chm", "application/octet-stream"},
                {".class", "application/x-java-applet"},
                {".clp", "application/x-msclip"},
                {".cmx", "image/x-cmx"},
                {".cnf", "text/plain"},
                {".cod", "image/cis-cod"},
                {".config", "application/xml"},
                {".contact", "text/x-ms-contact"},
                {".coverage", "application/xml"},
                {".cpio", "application/x-cpio"},
                {".cpp", "text/plain"},
                {".crd", "application/x-mscardfile"},
                {".crl", "application/pkix-crl"},
                {".crt", "application/x-x509-ca-cert"},
                {".cs", "text/plain"},
                {".csdproj", "text/plain"},
                {".csh", "application/x-csh"},
                {".csproj", "text/plain"},
                {".css", "text/css"},
                {".csv", "text/csv"},
                {".cur", "application/octet-stream"},
                {".cxx", "text/plain"},
                {".dat", "application/octet-stream"},
                {".datasource", "application/xml"},
                {".dbproj", "text/plain"},
                {".dcr", "application/x-director"},
                {".def", "text/plain"},
                {".deploy", "application/octet-stream"},
                {".der", "application/x-x509-ca-cert"},
                {".dgml", "application/xml"},
                {".dib", "image/bmp"},
                {".dif", "video/x-dv"},
                {".dir", "application/x-director"},
                {".disco", "text/xml"},
                {".dll", "application/x-msdownload"},
                {".dll.config", "text/xml"},
                {".dlm", "text/dlm"},
                {".doc", "application/msword"},
                {".docm", "application/vnd.ms-word.document.macroEnabled.12"},
                {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
                {".dot", "application/msword"},
                {".dotm", "application/vnd.ms-word.template.macroEnabled.12"},
                {".dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template"},
                {".dsp", "application/octet-stream"},
                {".dsw", "text/plain"},
                {".dtd", "text/xml"},
                {".dtsConfig", "text/xml"},
                {".dv", "video/x-dv"},
                {".dvi", "application/x-dvi"},
                {".dwf", "drawing/x-dwf"},
                {".dwp", "application/octet-stream"},
                {".dxr", "application/x-director"},
                {".eml", "message/rfc822"},
                {".emz", "application/octet-stream"},
                {".eot", "application/octet-stream"},
                {".eps", "application/postscript"},
                {".etl", "application/etl"},
                {".etx", "text/x-setext"},
                {".evy", "application/envoy"},
                {".exe", "application/octet-stream"},
                {".exe.config", "text/xml"},
                {".fdf", "application/vnd.fdf"},
                {".fif", "application/fractals"},
                {".filters", "Application/xml"},
                {".fla", "application/octet-stream"},
                {".flr", "x-world/x-vrml"},
                {".flv", "video/x-flv"},
                {".fsscript", "application/fsharp-script"},
                {".fsx", "application/fsharp-script"},
                {".generictest", "application/xml"},
                {".gif", "image/gif"},
                {".group", "text/x-ms-group"},
                {".gsm", "audio/x-gsm"},
                {".gtar", "application/x-gtar"},
                {".gz", "application/x-gzip"},
                {".h", "text/plain"},
                {".hdf", "application/x-hdf"},
                {".hdml", "text/x-hdml"},
                {".hhc", "application/x-oleobject"},
                {".hhk", "application/octet-stream"},
                {".hhp", "application/octet-stream"},
                {".hlp", "application/winhlp"},
                {".hpp", "text/plain"},
                {".hqx", "application/mac-binhex40"},
                {".hta", "application/hta"},
                {".htc", "text/x-component"},
                {".htm", "text/html"},
                {".html", "text/html"},
                {".htt", "text/webviewhtml"},
                {".hxa", "application/xml"},
                {".hxc", "application/xml"},
                {".hxd", "application/octet-stream"},
                {".hxe", "application/xml"},
                {".hxf", "application/xml"},
                {".hxh", "application/octet-stream"},
                {".hxi", "application/octet-stream"},
                {".hxk", "application/xml"},
                {".hxq", "application/octet-stream"},
                {".hxr", "application/octet-stream"},
                {".hxs", "application/octet-stream"},
                {".hxt", "text/html"},
                {".hxv", "application/xml"},
                {".hxw", "application/octet-stream"},
                {".hxx", "text/plain"},
                {".i", "text/plain"},
                {".ico", "image/x-icon"},
                {".ics", "application/octet-stream"},
                {".idl", "text/plain"},
                {".ief", "image/ief"},
                {".iii", "application/x-iphone"},
                {".inc", "text/plain"},
                {".inf", "application/octet-stream"},
                {".inl", "text/plain"},
                {".ins", "application/x-internet-signup"},
                {".ipa", "application/x-itunes-ipa"},
                {".ipg", "application/x-itunes-ipg"},
                {".ipproj", "text/plain"},
                {".ipsw", "application/x-itunes-ipsw"},
                {".iqy", "text/x-ms-iqy"},
                {".isp", "application/x-internet-signup"},
                {".ite", "application/x-itunes-ite"},
                {".itlp", "application/x-itunes-itlp"},
                {".itms", "application/x-itunes-itms"},
                {".itpc", "application/x-itunes-itpc"},
                {".IVF", "video/x-ivf"},
                {".jar", "application/java-archive"},
                {".java", "application/octet-stream"},
                {".jck", "application/liquidmotion"},
                {".jcz", "application/liquidmotion"},
                {".jfif", "image/pjpeg"},
                {".jnlp", "application/x-java-jnlp-file"},
                {".jpb", "application/octet-stream"},
                {".jpe", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".jpg", "image/jpeg"},
                {".js", "application/x-javascript"},
                {".json", "application/json"},
                {".jsx", "text/jscript"},
                {".jsxbin", "text/plain"},
                {".latex", "application/x-latex"},
                {".library-ms", "application/windows-library+xml"},
                {".lit", "application/x-ms-reader"},
                {".loadtest", "application/xml"},
                {".lpk", "application/octet-stream"},
                {".lsf", "video/x-la-asf"},
                {".lst", "text/plain"},
                {".lsx", "video/x-la-asf"},
                {".lzh", "application/octet-stream"},
                {".m13", "application/x-msmediaview"},
                {".m14", "application/x-msmediaview"},
                {".m1v", "video/mpeg"},
                {".m2t", "video/vnd.dlna.mpeg-tts"},
                {".m2ts", "video/vnd.dlna.mpeg-tts"},
                {".m2v", "video/mpeg"},
                {".m3u", "audio/x-mpegurl"},
                {".m3u8", "audio/x-mpegurl"},
                {".m4a", "audio/m4a"},
                {".m4b", "audio/m4b"},
                {".m4p", "audio/m4p"},
                {".m4r", "audio/x-m4r"},
                {".m4v", "video/x-m4v"},
                {".mac", "image/x-macpaint"},
                {".mak", "text/plain"},
                {".man", "application/x-troff-man"},
                {".manifest", "application/x-ms-manifest"},
                {".map", "text/plain"},
                {".master", "application/xml"},
                {".mda", "application/msaccess"},
                {".mdb", "application/x-msaccess"},
                {".mde", "application/msaccess"},
                {".mdp", "application/octet-stream"},
                {".me", "application/x-troff-me"},
                {".mfp", "application/x-shockwave-flash"},
                {".mht", "message/rfc822"},
                {".mhtml", "message/rfc822"},
                {".mid", "audio/mid"},
                {".midi", "audio/mid"},
                {".mix", "application/octet-stream"},
                {".mk", "text/plain"},
                {".mmf", "application/x-smaf"},
                {".mno", "text/xml"},
                {".mny", "application/x-msmoney"},
                {".mod", "video/mpeg"},
                {".mov", "video/quicktime"},
                {".movie", "video/x-sgi-movie"},
                {".mp2", "video/mpeg"},
                {".mp2v", "video/mpeg"},
                {".mp3", "audio/mpeg"},
                {".mp4", "video/mp4"},
                {".mp4v", "video/mp4"},
                {".mpa", "video/mpeg"},
                {".mpe", "video/mpeg"},
                {".mpeg", "video/mpeg"},
                {".mpf", "application/vnd.ms-mediapackage"},
                {".mpg", "video/mpeg"},
                {".mpp", "application/vnd.ms-project"},
                {".mpv2", "video/mpeg"},
                {".mqv", "video/quicktime"},
                {".ms", "application/x-troff-ms"},
                {".msi", "application/octet-stream"},
                {".mso", "application/octet-stream"},
                {".mts", "video/vnd.dlna.mpeg-tts"},
                {".mtx", "application/xml"},
                {".mvb", "application/x-msmediaview"},
                {".mvc", "application/x-miva-compiled"},
                {".mxp", "application/x-mmxp"},
                {".nc", "application/x-netcdf"},
                {".nsc", "video/x-ms-asf"},
                {".nws", "message/rfc822"},
                {".ocx", "application/octet-stream"},
                {".oda", "application/oda"},
                {".odc", "text/x-ms-odc"},
                {".odh", "text/plain"},
                {".odl", "text/plain"},
                {".odp", "application/vnd.oasis.opendocument.presentation"},
                {".ods", "application/oleobject"},
                {".odt", "application/vnd.oasis.opendocument.text"},
                {".one", "application/onenote"},
                {".onea", "application/onenote"},
                {".onepkg", "application/onenote"},
                {".onetmp", "application/onenote"},
                {".onetoc", "application/onenote"},
                {".onetoc2", "application/onenote"},
                {".orderedtest", "application/xml"},
                {".osdx", "application/opensearchdescription+xml"},
                {".p10", "application/pkcs10"},
                {".p12", "application/x-pkcs12"},
                {".p7b", "application/x-pkcs7-certificates"},
                {".p7c", "application/pkcs7-mime"},
                {".p7m", "application/pkcs7-mime"},
                {".p7r", "application/x-pkcs7-certreqresp"},
                {".p7s", "application/pkcs7-signature"},
                {".pbm", "image/x-portable-bitmap"},
                {".pcast", "application/x-podcast"},
                {".pct", "image/pict"},
                {".pcx", "application/octet-stream"},
                {".pcz", "application/octet-stream"},
                {".pdf", "application/pdf"},
                {".pfb", "application/octet-stream"},
                {".pfm", "application/octet-stream"},
                {".pfx", "application/x-pkcs12"},
                {".pgm", "image/x-portable-graymap"},
                {".pic", "image/pict"},
                {".pict", "image/pict"},
                {".pkgdef", "text/plain"},
                {".pkgundef", "text/plain"},
                {".pko", "application/vnd.ms-pki.pko"},
                {".pls", "audio/scpls"},
                {".pma", "application/x-perfmon"},
                {".pmc", "application/x-perfmon"},
                {".pml", "application/x-perfmon"},
                {".pmr", "application/x-perfmon"},
                {".pmw", "application/x-perfmon"},
                {".png", "image/png"},
                {".pnm", "image/x-portable-anymap"},
                {".pnt", "image/x-macpaint"},
                {".pntg", "image/x-macpaint"},
                {".pnz", "image/png"},
                {".pot", "application/vnd.ms-powerpoint"},
                {".potm", "application/vnd.ms-powerpoint.template.macroEnabled.12"},
                {".potx", "application/vnd.openxmlformats-officedocument.presentationml.template"},
                {".ppa", "application/vnd.ms-powerpoint"},
                {".ppam", "application/vnd.ms-powerpoint.addin.macroEnabled.12"},
                {".ppm", "image/x-portable-pixmap"},
                {".pps", "application/vnd.ms-powerpoint"},
                {".ppsm", "application/vnd.ms-powerpoint.slideshow.macroEnabled.12"},
                {".ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow"},
                {".ppt", "application/vnd.ms-powerpoint"},
                {".pptm", "application/vnd.ms-powerpoint.presentation.macroEnabled.12"},
                {".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
                {".prf", "application/pics-rules"},
                {".prm", "application/octet-stream"},
                {".prx", "application/octet-stream"},
                {".ps", "application/postscript"},
                {".psc1", "application/PowerShell"},
                {".psd", "application/octet-stream"},
                {".psess", "application/xml"},
                {".psm", "application/octet-stream"},
                {".psp", "application/octet-stream"},
                {".pub", "application/x-mspublisher"},
                {".pwz", "application/vnd.ms-powerpoint"},
                {".qht", "text/x-html-insertion"},
                {".qhtm", "text/x-html-insertion"},
                {".qt", "video/quicktime"},
                {".qti", "image/x-quicktime"},
                {".qtif", "image/x-quicktime"},
                {".qtl", "application/x-quicktimeplayer"},
                {".qxd", "application/octet-stream"},
                {".ra", "audio/x-pn-realaudio"},
                {".ram", "audio/x-pn-realaudio"},
                {".rar", "application/octet-stream"},
                {".ras", "image/x-cmu-raster"},
                {".rat", "application/rat-file"},
                {".rc", "text/plain"},
                {".rc2", "text/plain"},
                {".rct", "text/plain"},
                {".rdlc", "application/xml"},
                {".resx", "application/xml"},
                {".rf", "image/vnd.rn-realflash"},
                {".rgb", "image/x-rgb"},
                {".rgs", "text/plain"},
                {".rm", "application/vnd.rn-realmedia"},
                {".rmi", "audio/mid"},
                {".rmp", "application/vnd.rn-rn_music_package"},
                {".roff", "application/x-troff"},
                {".rpm", "audio/x-pn-realaudio-plugin"},
                {".rqy", "text/x-ms-rqy"},
                {".rtf", "application/rtf"},
                {".rtx", "text/richtext"},
                {".ruleset", "application/xml"},
                {".s", "text/plain"},
                {".safariextz", "application/x-safari-safariextz"},
                {".scd", "application/x-msschedule"},
                {".sct", "text/scriptlet"},
                {".sd2", "audio/x-sd2"},
                {".sdp", "application/sdp"},
                {".sea", "application/octet-stream"},
                {".searchConnector-ms", "application/windows-search-connector+xml"},
                {".setpay", "application/set-payment-initiation"},
                {".setreg", "application/set-registration-initiation"},
                {".settings", "application/xml"},
                {".sgimb", "application/x-sgimb"},
                {".sgml", "text/sgml"},
                {".sh", "application/x-sh"},
                {".shar", "application/x-shar"},
                {".shtml", "text/html"},
                {".sit", "application/x-stuffit"},
                {".sitemap", "application/xml"},
                {".skin", "application/xml"},
                {".sldm", "application/vnd.ms-powerpoint.slide.macroEnabled.12"},
                {".sldx", "application/vnd.openxmlformats-officedocument.presentationml.slide"},
                {".slk", "application/vnd.ms-excel"},
                {".sln", "text/plain"},
                {".slupkg-ms", "application/x-ms-license"},
                {".smd", "audio/x-smd"},
                {".smi", "application/octet-stream"},
                {".smx", "audio/x-smd"},
                {".smz", "audio/x-smd"},
                {".snd", "audio/basic"},
                {".snippet", "application/xml"},
                {".snp", "application/octet-stream"},
                {".sol", "text/plain"},
                {".sor", "text/plain"},
                {".spc", "application/x-pkcs7-certificates"},
                {".spl", "application/futuresplash"},
                {".src", "application/x-wais-source"},
                {".srf", "text/plain"},
                {".SSISDeploymentManifest", "text/xml"},
                {".ssm", "application/streamingmedia"},
                {".sst", "application/vnd.ms-pki.certstore"},
                {".stl", "application/vnd.ms-pki.stl"},
                {".sv4cpio", "application/x-sv4cpio"},
                {".sv4crc", "application/x-sv4crc"},
                {".svc", "application/xml"},
                {".swf", "application/x-shockwave-flash"},
                {".t", "application/x-troff"},
                {".tar", "application/x-tar"},
                {".tcl", "application/x-tcl"},
                {".testrunconfig", "application/xml"},
                {".testsettings", "application/xml"},
                {".tex", "application/x-tex"},
                {".texi", "application/x-texinfo"},
                {".texinfo", "application/x-texinfo"},
                {".tgz", "application/x-compressed"},
                {".thmx", "application/vnd.ms-officetheme"},
                {".thn", "application/octet-stream"},
                {".tif", "image/tiff"},
                {".tiff", "image/tiff"},
                {".tlh", "text/plain"},
                {".tli", "text/plain"},
                {".toc", "application/octet-stream"},
                {".tr", "application/x-troff"},
                {".trm", "application/x-msterminal"},
                {".trx", "application/xml"},
                {".ts", "video/vnd.dlna.mpeg-tts"},
                {".tsv", "text/tab-separated-values"},
                {".ttf", "application/octet-stream"},
                {".tts", "video/vnd.dlna.mpeg-tts"},
                {".txt", "text/plain"},
                {".u32", "application/octet-stream"},
                {".uls", "text/iuls"},
                {".user", "text/plain"},
                {".ustar", "application/x-ustar"},
                {".vb", "text/plain"},
                {".vbdproj", "text/plain"},
                {".vbk", "video/mpeg"},
                {".vbproj", "text/plain"},
                {".vbs", "text/vbscript"},
                {".vcf", "text/x-vcard"},
                {".vcproj", "Application/xml"},
                {".vcs", "text/plain"},
                {".vcxproj", "Application/xml"},
                {".vddproj", "text/plain"},
                {".vdp", "text/plain"},
                {".vdproj", "text/plain"},
                {".vdx", "application/vnd.ms-visio.viewer"},
                {".vml", "text/xml"},
                {".vscontent", "application/xml"},
                {".vsct", "text/xml"},
                {".vsd", "application/vnd.visio"},
                {".vsi", "application/ms-vsi"},
                {".vsix", "application/vsix"},
                {".vsixlangpack", "text/xml"},
                {".vsixmanifest", "text/xml"},
                {".vsmdi", "application/xml"},
                {".vspscc", "text/plain"},
                {".vss", "application/vnd.visio"},
                {".vsscc", "text/plain"},
                {".vssettings", "text/xml"},
                {".vssscc", "text/plain"},
                {".vst", "application/vnd.visio"},
                {".vstemplate", "text/xml"},
                {".vsto", "application/x-ms-vsto"},
                {".vsw", "application/vnd.visio"},
                {".vsx", "application/vnd.visio"},
                {".vtx", "application/vnd.visio"},
                {".wav", "audio/wav"},
                {".wave", "audio/wav"},
                {".wax", "audio/x-ms-wax"},
                {".wbk", "application/msword"},
                {".wbmp", "image/vnd.wap.wbmp"},
                {".wcm", "application/vnd.ms-works"},
                {".wdb", "application/vnd.ms-works"},
                {".wdp", "image/vnd.ms-photo"},
                {".webarchive", "application/x-safari-webarchive"},
                {".webtest", "application/xml"},
                {".wiq", "application/xml"},
                {".wiz", "application/msword"},
                {".wks", "application/vnd.ms-works"},
                {".WLMP", "application/wlmoviemaker"},
                {".wlpginstall", "application/x-wlpg-detect"},
                {".wlpginstall3", "application/x-wlpg3-detect"},
                {".wm", "video/x-ms-wm"},
                {".wma", "audio/x-ms-wma"},
                {".wmd", "application/x-ms-wmd"},
                {".wmf", "application/x-msmetafile"},
                {".wml", "text/vnd.wap.wml"},
                {".wmlc", "application/vnd.wap.wmlc"},
                {".wmls", "text/vnd.wap.wmlscript"},
                {".wmlsc", "application/vnd.wap.wmlscriptc"},
                {".wmp", "video/x-ms-wmp"},
                {".wmv", "video/x-ms-wmv"},
                {".wmx", "video/x-ms-wmx"},
                {".wmz", "application/x-ms-wmz"},
                {".wpl", "application/vnd.ms-wpl"},
                {".wps", "application/vnd.ms-works"},
                {".wri", "application/x-mswrite"},
                {".wrl", "x-world/x-vrml"},
                {".wrz", "x-world/x-vrml"},
                {".wsc", "text/scriptlet"},
                {".wsdl", "text/xml"},
                {".wvx", "video/x-ms-wvx"},
                {".x", "application/directx"},
                {".xaf", "x-world/x-vrml"},
                {".xaml", "application/xaml+xml"},
                {".xap", "application/x-silverlight-app"},
                {".xbap", "application/x-ms-xbap"},
                {".xbm", "image/x-xbitmap"},
                {".xdr", "text/plain"},
                {".xht", "application/xhtml+xml"},
                {".xhtml", "application/xhtml+xml"},
                {".xla", "application/vnd.ms-excel"},
                {".xlam", "application/vnd.ms-excel.addin.macroEnabled.12"},
                {".xlc", "application/vnd.ms-excel"},
                {".xld", "application/vnd.ms-excel"},
                {".xlk", "application/vnd.ms-excel"},
                {".xll", "application/vnd.ms-excel"},
                {".xlm", "application/vnd.ms-excel"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsb", "application/vnd.ms-excel.sheet.binary.macroEnabled.12"},
                {".xlsm", "application/vnd.ms-excel.sheet.macroEnabled.12"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".xlt", "application/vnd.ms-excel"},
                {".xltm", "application/vnd.ms-excel.template.macroEnabled.12"},
                {".xltx", "application/vnd.openxmlformats-officedocument.spreadsheetml.template"},
                {".xlw", "application/vnd.ms-excel"},
                {".xml", "text/xml"},
                {".xmta", "application/xml"},
                {".xof", "x-world/x-vrml"},
                {".XOML", "text/plain"},
                {".xpm", "image/x-xpixmap"},
                {".xps", "application/vnd.ms-xpsdocument"},
                {".xrm-ms", "text/xml"},
                {".xsc", "application/xml"},
                {".xsd", "text/xml"},
                {".xsf", "text/xml"},
                {".xsl", "text/xml"},
                {".xslt", "text/xml"},
                {".xsn", "application/octet-stream"},
                {".xss", "application/xml"},
                {".xtp", "application/octet-stream"},
                {".xwd", "image/x-xwindowdump"},
                {".z", "application/x-compress"},
                {".zip", "application/x-zip-compressed"},
                //{".txt", "text/plain" },
                //{".pdf", "application/pdf" },
                //{".doc", "application/vnd.ms-word" },
                //{".docx", "application/vnd.ms-word" },
                //{".ppt", "application/vnd.ms-powerpoint" },
                //{".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
                //{".xls", "application/vnd.ms-excel" },
                //{".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
                //{".png", "image/png" },
                //{".jpg", "image/jpeg" },
                //{".jpeg", "image/jpeg" },
                //{".gif", "image/gif" },
                //{".csv", "text/csv" },
                //{".sql", "application/octet-stream" },
                //{".apk", "application/vnd.android.package-archive" },
            };
        }
    }
}
