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
        Queue<Token> tokens;

        public MyTranslator(string code)
        {
            this.code = code;
        }

        private static Dictionary<(TT, TT), List<TT>> regulations = 
                                new Dictionary<(TT, TT), List<TT>>()
        {
            [(TT.VAR, TT.Program)] = new List<TT>(){ TT.DeclarOfVar, TT.CalculationDescrip },
            [(TT.VAR, TT.DeclarOfVar)] = new List<TT>(){ TT.VAR, TT.IDENT, TT.VariablesList, TT.INTEGER,
                                                       TT.SEMICOLON },
            [(TT.BEGIN, TT.CalculationDescrip)] = new List<TT>(){ TT.BEGIN, TT.InstructionList, TT.END },

            [(TT.READ, TT.InstructionList)] = new List<TT>(){ TT.READ, TT.PARENTHESISOPEN, TT.IDENT,
                                                       TT.VariablesList,
                                                       TT.PARENTHESISCLOSE, TT.SEMICOLON, TT.InstructionList },
            [(TT.WRITE, TT.InstructionList)] = new List<TT>(){ TT.WRITE, TT.PARENTHESISOPEN, TT.IDENT,
                                                       TT.VariablesList,
                                                       TT.PARENTHESISCLOSE, TT.SEMICOLON, TT.InstructionList },
            [(TT.IDENT, TT.InstructionList)] = new List<TT>(){ TT.Assignment, TT.InstructionList },
            [(TT.FOR, TT.InstructionList)] = new List<TT>(){ TT.FOR, TT.IDENT, TT.EQUALLY,
                                                       TT.Expression, TT.TO, TT.Expression, TT.DO, TT.InstructionList,
                                                       TT.END_FOR, TT.SEMICOLON, TT.InstructionList },
            [(TT.END_FOR, TT.InstructionList)] = new List<TT>(){ },
            [(TT.END, TT.InstructionList)] = new List<TT>(){ },

            [(TT.COMMA, TT.VariablesList)] = new List<TT>(){ TT.COMMA, TT.IDENT, TT.VariablesList },
            [(TT.PARENTHESISCLOSE, TT.VariablesList)] = new List<TT>(){  },
            [(TT.INTEGER, TT.VariablesList)] = new List<TT>(){  },

            [(TT.IDENT, TT.Assignment)] = new List<TT>(){ TT.IDENT, TT.EQUALLY, TT.Expression, TT.SEMICOLON },

            [(TT.UNOBYNOPERATION, TT.Expression)] = new List<TT>(){ TT.UNOBYNOPERATION, TT.ExpressionNotMinus },
            [(TT.PARENTHESISOPEN, TT.Expression)] = new List<TT>(){ TT.PARENTHESISOPEN,
                                                        TT.Expression, TT.PARENTHESISCLOSE, TT.Subexpression },
            [(TT.IDENT, TT.Expression)] = new List<TT>(){ TT.IDENT, TT.Subexpression },
            [(TT.NUMLITERAL, TT.Expression)] = new List<TT>(){ TT.NUMLITERAL, TT.Subexpression },

            [(TT.PARENTHESISOPEN, TT.ExpressionNotMinus)] = new List<TT>(){ TT.PARENTHESISOPEN,
                                                        TT.Expression, TT.PARENTHESISCLOSE, TT.Subexpression },
            [(TT.IDENT, TT.ExpressionNotMinus)] = new List<TT>() { TT.IDENT, TT.Subexpression },
            [(TT.NUMLITERAL, TT.ExpressionNotMinus)] = new List<TT>() { TT.NUMLITERAL, TT.Subexpression },

            [(TT.BYNOPERATION, TT.Subexpression)] = new List<TT>() { TT.BYNOPERATION, TT.ExpressionNotMinus },
            [(TT.UNOBYNOPERATION, TT.Subexpression)] = new List<TT>() { TT.UNOBYNOPERATION, TT.ExpressionNotMinus },
            [(TT.SEMICOLON, TT.Subexpression)] = new List<TT>() { },
            [(TT.PARENTHESISCLOSE, TT.Subexpression)] = new List<TT>() { },
            [(TT.DO, TT.Subexpression)] = new List<TT>() { },
            [(TT.TO, TT.Subexpression)] = new List<TT>() { },
        };
        

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
            PARENTHESISOPEN,
            PARENTHESISCLOSE,
            COMMA,
            IDENT, 
            NUMLITERAL,
            BYNOPERATION,
            UNOBYNOPERATION,
            Program,
            DeclarOfVar,
            CalculationDescrip,
            InstructionList,
            VariablesList,
            Assignment,
            Expression,
            ExpressionNotMinus,
            SecondExpression,
            Subexpression,
        };

        /// <summary>
        /// Класс, описывающий лексему
        /// </summary>
        private class Token
        {
            /// <summary>
            /// Тип лексемы
            /// </summary>
            public TT Type;
            /// <summary>
            /// Значение лексемы
            /// </summary>
            public string StringValue;
            /// <summary>
            /// Положение лексемы в коде (считая с конца)
            /// </summary>
            public int offset;
            /// <summary>
            /// Длина лексемы
            /// </summary>
            public int length;

            public Token(TT type)
            {
                Type = type;
                if (MyTranslator.stringValues.TryGetValue(type, out string value))
                {
                    StringValue = value;
                }
                else
                {
                    StringValue = type.ToString();
                }
            }

            public Token(TT type, string stringValue)
            {
                Type = type;
                StringValue = stringValue;
            }

            public Token(TT type, string stringValue, int offset, int length) : this(type, stringValue)
            {
                this.offset = offset;
                this.length = length;
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
        private static Dictionary<TT, Regex> regexes = new Dictionary<TT, Regex>()
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
        public IEnumerable<string> LexicalAnalysis()
        {
            var result = new Queue<Token>();
            // откусываем пробелы
            var remainingText = code.TrimStart(' ', '\n', '\t');
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
                    throw new TranslateExeption("Неизвестная лексема", remainingText.Length, 1);
                }
                if (bestMatch.matchLen > 10)
                {
                    throw new TranslateExeption("Превышена длина идентефикатора", remainingText.Length,
                                                                                    bestMatch.matchLen);
                }
                var token = new Token(bestMatch.tokenType, bestMatch.text, 
                    remainingText.Length, bestMatch.matchLen);
                result.Enqueue(token);

                // откусываем распознанный кусок и пробелы после него
                remainingText = remainingText.Substring(bestMatch.matchLen).TrimStart();
            }
            tokens = result;
            return from i in tokens select i.ToString();
        }

        public void Parse(ListBox listBox)
        {
            Stack<Token> stack = new Stack<Token>();
            stack.Push(new Token(TT.Program));          // Начальный символ
            List<string> identList = new List<string>();// Таблица идентефикаторов
            bool declaration = true;                    // Объявление переменных
            Token inputToken;                           // Текущий символ входной цепочки
            Token stacksToken;                          // Символ, вынутый из стека

            int numberOfToken = 0;
            while (tokens.Count > 0)
            {
                inputToken = tokens.Peek();   
                if (stack.Count == 0)               // Если стек пуст
                {
                    throw new TranslateExeption("Встречено: '" + inputToken.StringValue +
                        "', а ожидался конец файла",
                        inputToken.offset, inputToken.length);
                }
                stacksToken = stack.Pop();
                // Если начался список инструкций - объявление переменных закончено
                if (inputToken.Type == TT.BEGIN)
                {
                    declaration = false;
                }
                if (inputToken.Type == TT.IDENT)    // Если идентефикатор
                {
                    if (declaration)    // Если происходит объявление
                    {
                        identList.Add(inputToken.StringValue);  // Занести в таблицу
                    }
                    else     // Если объявление закончено
                    {
                        // Если идентефикатора, нет в таблице
                        if (!identList.Contains(inputToken.StringValue))    
                        {
                            // Ошибка
                            throw new TranslateExeption("Необъявленный идентефикатор: " +
                                inputToken.StringValue, inputToken.offset, inputToken.length);
                        }
                    }
                }
                if (stacksToken.Equals(inputToken)) // Если стековый символ - терминал и равен входному
                {
                    tokens.Dequeue();   // Чтение символа из входной цепочки
                    listBox.Items[numberOfToken] += "\tOk";
                    numberOfToken++;
                }
                else
                {
                    if (stacksToken.Type < TT.Program) // Если в стеке терминальный символ
                    {
                        // Ошибка
                        throw new TranslateExeption("Ожидалось " + stacksToken.StringValue, 
                            inputToken.offset, inputToken.length);
                    }
                    // Если в стеке нетерминальный символ 
                    List<TT> regulation;    // Хранит правило
                    // Если нет подходящего правила
                    if (!regulations.TryGetValue((inputToken.Type, stacksToken.Type), out regulation))
                    {
                        string errMessage = "Ошибочный синтаксис, ожидалось ";
                        foreach (var (t1, t2) in regulations.Keys)  // Поиск всех правил     
                        {
                            if (t2 == stacksToken.Type) // для текущего стекового символа ожидают    
                            {
                                errMessage += "'" + new Token(t1).StringValue + "'  ";
                            }
                        }
                        //  Вывод всех возможных ожидаемых символов
                        throw new TranslateExeption(errMessage, inputToken.offset, inputToken.length);   
                     }
                    for (int i = regulation.Count - 1; i > -1; i--) // Занесение символов в стек
                    {
                        if (regulation[i] == inputToken.Type)   // Занесение лексем в стек
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
            if (stack.Count != 0)
            {
                // Ошибка
                throw new TranslateExeption("Ожидалось " + stack.Peek().StringValue,
                    0, 0);
            };
        }


        public class TranslateExeption : Exception
        {
            public readonly int offset;
            public readonly int length;

            public TranslateExeption(string message, int offset, int length) : base(message)
            {
                this.offset = offset;
                this.length = length;

            }
        }

        // Соответствие лексемы и ее значения
        private static Dictionary<TT, string> stringValues = new Dictionary<TT, string>()
        {
            [TT.SEMICOLON] = ";",
            [TT.EQUALLY] = "=",
            [TT.PARENTHESISOPEN] = "(",
            [TT.PARENTHESISCLOSE] = ")",
            [TT.COMMA] = ",",
            [TT.IDENT] = "Идентефикатор",
            [TT.NUMLITERAL] = "Число",
            [TT.BYNOPERATION] = "+'  '*",
            [TT.UNOBYNOPERATION] = "-",
            [TT.InstructionList] = "список инструкций",
            [TT.CalculationDescrip] = "описание вычислений",
            [TT.DeclarOfVar] = "объявление переменных",
            [TT.VariablesList] = "список переменных",
            [TT.Program] = "программа",
        };
    }
}
