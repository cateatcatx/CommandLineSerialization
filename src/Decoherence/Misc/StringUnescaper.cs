﻿using System.Collections.Generic;

namespace Decoherence.CommandLineSerialization
{
    public class StringUnescaper
    {
        private string? mStr;
        private int mIndex;
        
        private readonly Dictionary<char, int> mCh2Value;
        private readonly char mEscapeCh;

        /// <param name="ch2Value">需要转义的字符映射，转义的字符会被映射成int值</param>
        /// <param name="escapeCh">转义字符</param>
        public StringUnescaper(Dictionary<char, int> ch2Value, char escapeCh = '\\')
        {
            mCh2Value = ch2Value;
            mEscapeCh = escapeCh;
        }
        
        public void Reset(string str)
        {
            mStr = str;
            mIndex = -1;
        }

        /// <summary>
        /// 读取一个字符
        /// </summary>
        /// <param name="ch">读到的字符，null代表没读到</param>
        /// <param name="value">字符的映射值，null代表无映射值</param>
        /// <returns>是否成功读取</returns>
        public bool ReadChar(out char? ch, out int? value)
        {
            ch = null;
            value = null;
            
            ++mIndex;
            if (mIndex >= mStr.Length)
            {
                return false;
            }
            
            ch = mStr[mIndex];
            
            if (ch == mEscapeCh && mIndex + 1 < mStr.Length)
            {// 转义
                var nextCh = mStr[mIndex + 1];
                if (nextCh == mEscapeCh || mCh2Value.ContainsKey(nextCh))
                {
                    ch = nextCh;
                    ++mIndex;
                }

                return mIndex < mStr.Length;
            }
            
            if (mCh2Value.TryGetValue(ch.Value, out var tmp))
            {
                ch = null;
                value = tmp;
            }
            
            return true;
        }
    }
}