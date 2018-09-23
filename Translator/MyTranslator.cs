using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
        Dictionary<(Token, Token), string> regulation = new Dictionary<(Token, Token), string>()
        {
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










        public enum TokenType
        {
            VAR,
            BEGIN,
            END,
            INTEGER,
            FOR,
            TO,
            DO,
            END_FOR,
            Punct,
            IDENT, 
            NUMLITERAL
        };

        public class Token
        {
            public TokenType Type;
            public string StringValue;
            public int offset;

            public Token(TokenType type)
            {
                Type = type;
            }

            public Token(TokenType type, string stringValue)
            {
                Type = type;
                StringValue = stringValue;
            }

            public override string ToString()
            {
                return Type.ToString() + ": " + StringValue;
            }
        }

        // регулярки, описывающие лексему, начинаются с ^, чтобы матчить только в начале
        Dictionary<TokenType, Regex> regexes = new Dictionary<TokenType, Regex>()
        {
            [TokenType.Punct] = new Regex(@"^[,)(;:=\*+-]", RegexOptions.Compiled),
            [TokenType.VAR] = new Regex(@"^VAR\b", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            [TokenType.BEGIN] = new Regex(@"^BEGIN\b", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            [TokenType.END] = new Regex(@"^END\b", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            [TokenType.INTEGER] = new Regex(@"^INTEGER\b", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            [TokenType.TO] = new Regex(@"^TO\b", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            [TokenType.DO] = new Regex(@"^DO\b", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            [TokenType.END_FOR] = new Regex(@"^END_FOR\b", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            [TokenType.IDENT] = new Regex(@"^[a-zA-Z][a-zA-Z0-9]{0,9}", RegexOptions.Compiled),
            [TokenType.NUMLITERAL] = new Regex(@"^[0-9]+", RegexOptions.Compiled)
        };
        

        // а вот и вся логика:
        public List<Token> LexicalAnalysis(string text)
        {
            var result = new List<Token>();
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
                    //throw new Exception("Unknown lexeme");
                    continue;
                }
                var token = new Token(bestMatch.tokenType, bestMatch.text);
                result.Add(token);

                // откусываем распознанный кусок и пробелы после него
                remainingText = remainingText.Substring(bestMatch.matchLen).TrimStart();
            }
            return result;
        }
    }
}
