using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Translator
{
    class MyTranslator
    {
        String code;
        String[] keyWords = new string[]
        {
            "VAR",
            "BEGIN",
            "END",
            "INTEGER",
            "FOR",
            "TO",
            "DO",
            "END_FOR",
        };
        Dictionary<(TT, TT), List<TT>> regulations = new Dictionary<(TT, TT), List<TT>>()
        {
            [(TT.VAR, TT.Program)] = new List<TT>(){ TT.DeclarOfVar, TT.CalculationDescrip },
            [(TT.VAR, TT.DeclarOfVar)] = new List<TT>(){ TT.VAR, TT.VariablesList, TT.INTEGER, TT.SEMICOLON },
            [(TT.BEGIN, TT.CalculationDescrip)] = new List<TT>(){ TT.BEGIN, TT.InstructionList, TT.END },
            [(TT.READ, TT.InstructionList)] = new List<TT>(){ TT.READ, TT.PARENTHESISOPEN, TT.VariablesList,
                                                       TT.PARENTHESISCLOSE, TT.SEMICOLON, TT.InstructionList },
            [(TT.WRITE, TT.InstructionList)] = new List<TT>(){ TT.WRITE, TT.PARENTHESISOPEN, TT.VariablesList,
                                                       TT.PARENTHESISCLOSE, TT.SEMICOLON, TT.InstructionList },
            [(TT.IDENT, TT.InstructionList)] = new List<TT>(){ TT.Assignment, TT.InstructionList },
            [(TT.FOR, TT.InstructionList)] = new List<TT>(){ TT.FOR, TT.IDENT, TT.EQUALLY,
                                                       TT.Expression, TT.TO, TT.Expression, TT.DO, TT.InstructionList,
                                                       TT.END_FOR, TT.SEMICOLON, TT.InstructionList },
            [(TT.END_FOR, TT.InstructionList)] = new List<TT>(){ },
            [(TT.END, TT.InstructionList)] = new List<TT>(){ },
            [(TT.IDENT, TT.VariablesList)] = new List<TT>(){ TT.IDENT, TT.VariablesList },
            [(TT.COMMA, TT.VariablesList)] = new List<TT>(){ TT.COMMA, TT.IDENT, TT.VariablesList },
            [(TT.PARENTHESISCLOSE, TT.VariablesList)] = new List<TT>(){  },
            [(TT.INTEGER, TT.VariablesList)] = new List<TT>(){  },
            [(TT.IDENT, TT.Assignment)] = new List<TT>(){ TT.IDENT, TT.EQUALLY, TT.Expression, TT.SEMICOLON },
            [(TT.UNOBYNOPERATION, TT.Expression)] = new List<TT>(){ TT.UNOBYNOPERATION, TT.Expression },
            [(TT.PARENTHESISOPEN, TT.Expression)] = new List<TT>(){ TT.PARENTHESISOPEN,
                                                        TT.Expression, TT.PARENTHESISOPEN, TT.Subexpression },
            [(TT.IDENT, TT.Expression)] = new List<TT>(){ TT.IDENT, TT.Subexpression },
            [(TT.NUMLITERAL, TT.Expression)] = new List<TT>(){ TT.NUMLITERAL, TT.Subexpression },
            [(TT.BYNOPERATION, TT.Subexpression)] = new List<TT>() { TT.BYNOPERATION, TT.Expression },
            [(TT.UNOBYNOPERATION, TT.Subexpression)] = new List<TT>() { TT.UNOBYNOPERATION, TT.Expression },
            [(TT.SEMICOLON, TT.Subexpression)] = new List<TT>() { },
            [(TT.DO, TT.Subexpression)] = new List<TT>() { },
            [(TT.TO, TT.Subexpression)] = new List<TT>() { },
        };

        public MyTranslator(String code)
        {
            this.code = code;
        }


        void Translate()
        {
            
        }


        class key
        {
            char c1, c2;
        }










        public enum TT
        {
            VAR,
            BEGIN,
            END,
            INTEGER,
            FOR,
            TO,
            DO,
            END_FOR,
            WRITE,
            READ,
            SEMICOLON,
            EQUALLY,
            COMMA,
            PARENTHESISOPEN,
            PARENTHESISCLOSE,
            IDENT, 
            NUMLITERAL,
            BYNOPERATION,
            UNOBYNOPERATION,
            Program,
            DeclarOfVar,
            CalculationDescrip,
            InstructionList,
            Instruction,
            VariablesList,
            Assignment,
            Expression,
            Subexpression,
        };

        public class Token
        {
            public TT Type;
            public string StringValue;
            public int offset;

            public Token(TT type)
            {
                Type = type;
                StringValue = type.ToString();
            }

            public Token(TT type, string stringValue)
            {
                Type = type;
                StringValue = stringValue;
            }

            public Token(TT type, string stringValue, int offset) : this(type, stringValue)
            {
                this.offset = offset;
            }

            public override bool Equals(object obj)
            {
                var token = obj as Token;
                return token != null &&
                       Type == token.Type;
            }

            public override int GetHashCode()
            {
                var hashCode = 622955006;
                hashCode = hashCode * -1521134295 + Type.GetHashCode();
                return hashCode;
            }

            public override string ToString()
            {
                return Type.ToString() + ": " + StringValue;
            }
        }

        // регулярки, описывающие лексему, начинаются с ^, чтобы матчить только в начале
        Dictionary<TT, Regex> regexes = new Dictionary<TT, Regex>()
        {
            [TT.SEMICOLON] = new Regex(@"^;", RegexOptions.Compiled),
            [TT.COMMA] = new Regex(@"^,", RegexOptions.Compiled),
            [TT.EQUALLY] = new Regex(@"^=", RegexOptions.Compiled),
            [TT.PARENTHESISOPEN] = new Regex(@"^\(", RegexOptions.Compiled),
            [TT.PARENTHESISCLOSE] = new Regex(@"^\)", RegexOptions.Compiled),
            [TT.VAR] = new Regex(@"^VAR\b", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            [TT.BEGIN] = new Regex(@"^BEGIN\b", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            [TT.END] = new Regex(@"^END\b", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            [TT.INTEGER] = new Regex(@"^: *INTEGER\b", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            [TT.FOR] = new Regex(@"^FOR\b", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            [TT.TO] = new Regex(@"^TO\b", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            [TT.DO] = new Regex(@"^DO\b", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            [TT.END_FOR] = new Regex(@"^END_FOR\b", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            [TT.READ] = new Regex(@"^READ\b", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            [TT.WRITE] = new Regex(@"^WRITE\b", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            [TT.IDENT] = new Regex(@"^[a-zA-Z][a-zA-Z0-9]*", RegexOptions.Compiled),
            [TT.NUMLITERAL] = new Regex(@"^[0-9]+", RegexOptions.Compiled),
            [TT.BYNOPERATION] = new Regex(@"^[\*\+]", RegexOptions.Compiled),
            [TT.UNOBYNOPERATION] = new Regex(@"^-", RegexOptions.Compiled),
        };
        

        // а вот и вся логика:
        public Queue<Token> LexicalAnalysis(string text)
        {
            var result = new Queue<Token>();
            // откусываем пробелы
            var remainingText = text.TrimStart(' ', '\n', '\t');
            while (remainingText != "")
            {
                // находим наиболее подходящий паттерн:
                var bestMatch =
                   (from pair in regexes
                    let tokenType = pair.Key
                    let regex = pair.Value
                    let match = regex.Match(remainingText)
                    let matchLen = match.Length
                // упорядочиваем по длине, а если длина одинаковая - по типу токена
                orderby matchLen descending, tokenType
                    select new { tokenType, text = match.Value, matchLen }).First();

                // если везде только 0, ошибка
                if (bestMatch.matchLen == 0)
                {
                    throw new TranslateExeption("Неизвестная лексема", remainingText.Length);
                }
                if (bestMatch.matchLen > 10)
                {
                    throw new TranslateExeption("Превышена длина идентефикатора", remainingText.Length);
                }
                var token = new Token(bestMatch.tokenType, bestMatch.text, remainingText.Length);
                result.Enqueue(token);

                // откусываем распознанный кусок и пробелы после него
                remainingText = remainingText.Substring(bestMatch.matchLen).TrimStart();
            }
            return result;
        }

        public bool Parse(Queue<Token> tokens, ListBox listBox)
        {
            Stack<Token> stack = new Stack<Token>();
            stack.Push(new Token(TT.Program));
            List<string> identList = new List<string>();
            bool declaration = true;

            int j = 0; //////////////////////////// temp
            while (tokens.Count > 0)
            {
                Token stacksToken = stack.Pop();
                Token inputToken = tokens.Peek();
                if (inputToken.Type == TT.IDENT)
                {
                    if (declaration)
                    {
                        identList.Add(inputToken.StringValue);
                    }
                    else
                    {
                        if (!identList.Contains(inputToken.StringValue))
                        {
                            throw new TranslateExeption("Необъявленный идентефикатор: " +
                                inputToken.StringValue, inputToken.offset);
                        }
                    }
                }
                if (stacksToken.Equals(inputToken))
                {
                    tokens.Dequeue();
                    //////////////////////////// temp
                    listBox.Items[j] += "\tOk";
                    j++;
                }
                else
                {
                    if (stacksToken.Type < TT.Program) // Терминальный символ
                    {
                        throw new TranslateExeption("Ожидалось " + stacksToken.StringValue, inputToken.offset);
                    }
                    List<TT> regulation;
                    if (!regulations.TryGetValue((inputToken.Type, stacksToken.Type), out regulation))
                    {
                        throw new TranslateExeption("Ошибочный синтаксис", inputToken.offset);
                    }
                    for (int i = regulation.Count - 1; i > -1; i--)
                    {
                        if (regulation[i] == TT.InstructionList)
                        {
                            declaration = false;
                        }
                        if (regulation[i] == inputToken.Type)
                        {
                            stack.Push(inputToken);
                        }
                        else
                        {
                            stack.Push(new Token(regulation[i]));
                        }
                    }
                }
            }
            return stack.Count == 0;
        }


        public class TranslateExeption : Exception
        {
            public readonly int offset;

            public TranslateExeption(string message, int offset) : base(message)
            {
                this.offset = offset;

            }
        }
    }
}
