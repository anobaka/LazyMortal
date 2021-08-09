// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using System.Net;
// using System.Text;
// using System.Threading.Tasks;
//
// namespace Bootstrap.Extensions
// {
//     public static class CookieExtensions
//     {
//         public static CookieCollection GetAllCookiesFromHeader(string headerValue, string host)
//         {
//             var cc = new CookieCollection();
//             if (headerValue.IsNotEmpty())
//             {
//                 var al = ConvertCookieHeaderToArrayList(headerValue);
//                 cc = ConvertCookieArraysToCookieCollection(al, host);
//             }
//
//             return cc;
//         }
//
//         private static ArrayList ConvertCookieHeaderToArrayList(string strCookHeader)
//         {
//             strCookHeader = strCookHeader.Replace("\r", "");
//             strCookHeader = strCookHeader.Replace("\n", "");
//             var strCookTemp = strCookHeader.Split(',');
//             var al = new ArrayList();
//             var i = 0;
//             var n = strCookTemp.Length;
//             while (i < n)
//             {
//                 if (strCookTemp[i].IndexOf("expires=", StringComparison.OrdinalIgnoreCase) > 0)
//                 {
//                     al.Add(strCookTemp[i] + "," + strCookTemp[i + 1]);
//                     i += 1;
//                 }
//                 else
//                 {
//                     al.Add(strCookTemp[i]);
//                 }
//
//                 i += 1;
//             }
//
//             return al;
//         }
//
//         private static CookieCollection ConvertCookieArraysToCookieCollection(IList al, string strHost)
//         {
//             var cc = new CookieCollection();
//             var alCount = al.Count;
//             for (var i = 0; i < alCount; i++)
//             {
//                 var strEachCook = al[i].ToString();
//                 var strEachCookParts = strEachCook.Split(';');
//                 var intEachCookPartsCount = strEachCookParts.Length;
//                 var cookTemp = new Cookie();
//
//                 for (var j = 0; j < intEachCookPartsCount; j++)
//                 {
//                     if (j == 0)
//                     {
//                         var strCNameAndCValue = strEachCookParts[j];
//                         if (strCNameAndCValue != string.Empty)
//                         {
//                             var firstEqual = strCNameAndCValue.IndexOf("=");
//                             var firstName = strCNameAndCValue[..firstEqual];
//                             var allValue = strCNameAndCValue.Substring(firstEqual + 1,
//                                 strCNameAndCValue.Length - (firstEqual + 1));
//                             cookTemp.Name = firstName;
//                             cookTemp.Value = allValue;
//                         }
//
//                         continue;
//                     }
//
//                     string[] nameValuePairTemp;
//                     if (strEachCookParts[j].IndexOf("path", StringComparison.OrdinalIgnoreCase) >= 0)
//                     {
//                         var strPNameAndPValue = strEachCookParts[j];
//                         if (strPNameAndPValue != string.Empty)
//                         {
//                             nameValuePairTemp = strPNameAndPValue.Split('=');
//                             if (nameValuePairTemp[1] != string.Empty)
//                             {
//                                 cookTemp.Path = nameValuePairTemp[1];
//                             }
//                             else
//                             {
//                                 cookTemp.Path = "/";
//                             }
//                         }
//
//                         continue;
//                     }
//
//                     if (strEachCookParts[j].IndexOf("domain", StringComparison.OrdinalIgnoreCase) >= 0)
//                     {
//                         var strDNameAndDValue = strEachCookParts[j];
//                         if (strDNameAndDValue != string.Empty)
//                         {
//                             nameValuePairTemp = strDNameAndDValue.Split('=');
//
//                             cookTemp.Domain = nameValuePairTemp[1] != string.Empty ? nameValuePairTemp[1] : strHost;
//                         }
//                     }
//                 }
//
//                 if (cookTemp.Path == string.Empty)
//                 {
//                     cookTemp.Path = "/";
//                 }
//
//                 if (cookTemp.Domain == string.Empty)
//                 {
//                     cookTemp.Domain = strHost;
//                 }
//
//                 cc.Add(cookTemp);
//             }
//
//             return cc;
//         }
//     }
// }