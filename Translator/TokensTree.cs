using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Translator
{
    class TokensTree
    {


        MyTranslator.TT type;
        List<TokensTree> sons;

        public void Add(int count, MyTranslator.TT type)
        {
            switch (type)
            {
                case MyTranslator.TT.InstructionList:
                    break;
                case MyTranslator.TT.FOR:
                    break;
                case MyTranslator.TT.READ:
                    break;
                case MyTranslator.TT.WRITE:
                    break;
                case MyTranslator.TT.Assignment:
                    break;
                case MyTranslator.TT.Expression:
                    break;
                case MyTranslator.TT.ExpressionNotMinus:
                    break;
                case MyTranslator.TT.SecondExpression:
                    break;
                case MyTranslator.TT.Subexpression:
                    break;
                default:
                    break;
            }
        }

        class TokensNode
        {
            MyTranslator.TT type;
            List<TokensTree> sons;
        }
    }
}
