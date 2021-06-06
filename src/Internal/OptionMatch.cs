using System;
using System.Collections.Generic;

namespace Decoherence.CommandLineParsing
{
    internal class OptionMatch
    {
        public void FillMatchedArgs(List<string> matchedArgs)
        {
            
        }
        
        /// <summary>
        /// 消耗arg，并返回下一个node
        /// </summary>
        /// <returns>下一个node</returns>
        public LinkedListNode<string> ConsumeArgs()
        {
            throw new NotImplementedException();
        }
    }
}