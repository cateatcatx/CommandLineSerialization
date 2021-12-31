// using System;
// using System.Collections.Generic;
// using System.Text;
//
// namespace Decoherence.CommandLineSerialization
// {
//     public class StringUnescaper
//     {
//         private string? mStr;
//         private int mIndex;
//         private Dictionary<char, int>? mCharMapping;
//         
//         private readonly char mEscapeCh;
//
//         /// <param name="escapeCh">转义符</param>
//         public StringUnescaper(char escapeCh = '\\')
//         {
//             mEscapeCh = escapeCh;
//         }
//         
//         public void Reset(string str)
//         {
//             mStr = str;
//             mIndex = -1;
//         }
//
//         /// <param name="charMapping">需要转义的字符映射，转义的字符会被映射成int值</param>
//         public void SetCharMapping(Dictionary<char, int>? charMapping)
//         {
//             mCharMapping = charMapping;
//         }
//
//         /// <summary>
//         /// 判断是否还有可读的字符
//         /// </summary>
//         public bool HasAnyChar()
//         {
//             
//         }
//
//         /// <summary>
//         /// 读取一个字符
//         /// </summary>
//         /// <param name="ch">读到的字符，null代表没读到</param>
//         /// <param name="value">字符的映射值，null代表无映射值</param>
//         /// <returns>是否成功读取</returns>
//         public bool ReadChar(out char? ch, out int? value)
//         {
//             ch = null;
//             value = null;
//             
//             ++mIndex;
//             if (mIndex >= mStr.Length)
//             {
//                 return false;
//             }
//             
//             ch = mStr[mIndex];
//             
//             if (ch == mEscapeCh && mIndex + 1 < mStr.Length)
//             {// 转义
//                 var nextCh = mStr[mIndex + 1];
//                 if (nextCh == mEscapeCh || mCharMapping.ContainsKey(nextCh))
//                 {
//                     ch = nextCh;
//                     ++mIndex;
//                 }
//
//                 return mIndex < mStr.Length;
//             }
//             
//             if (mCharMapping.TryGetValue(ch.Value, out var tmp))
//             {
//                 ch = null;
//                 value = tmp;
//             }
//             
//             return true;
//         }
//
//         public bool MovePrev()
//         {
//             if (mStr == null)
//                 throw _NewNonResetException();
//             
//             if (mIndex <= -1)
//             {
//                 return false;
//             }
//
//             if (mIndex - 2 >= 0 && mStr[mIndex - 2] == mEscapeCh)
//             {
//                 mIndex -= 2;
//             }
//             else
//             {
//                 --mIndex;
//             }
//
//             return true;
//         }
//
//         public void ReadToEnd(StringBuilder stringBuilder)
//         {
//             while (ReadChar(out var ch, out var value))
//             {
//                 if (ch != null)
//                 {
//                     stringBuilder.Append(ch);
//                 }
//             }
//             
//         }
//
//         private InvalidOperationException _NewNonResetException()
//         {
//             return new InvalidOperationException("Need invoke Reset first.");
//         }
//     }
// }