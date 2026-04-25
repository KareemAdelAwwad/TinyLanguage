using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridViewOutput.Rows.Clear();
            string input = richTextBoxInput.Text;

            string pattern =
                @"(?<Comment>/\*[\s\S]*?\*/)|" +
                @"(?<String>"".*?"")|" +
                @"(?<Keyword>\b(int|float|string|read|write|repeat|until|if|elseif|else|then|return|endl|main)\b)|" +
                @"(?<Identifier>[a-zA-Z][a-zA-Z0-9]*)|" +
                @"(?<Number>\d+(\.\d+)?)|" +
                @"(?<Assignment>:=)|" +
                @"(?<Condition><|>|=|<>)|" +
                @"(?<Boolean>&&|\|\|)|" +
                @"(?<Arithmetic>\+|-|\*|/)|" +
                @"(?<Symbol>[;,\(\)\{\}])";

            Regex regex = new Regex(pattern);
            MatchCollection matches = regex.Matches(input);

            List<Token> allTokens = new List<Token>();

            foreach (Match match in matches)
            {
                string tokenType = "";
                string lexeme = match.Value;

                if (match.Groups["Comment"].Success) tokenType = "Comment_Statement";
                else if (match.Groups["String"].Success) tokenType = "String";
                else if (match.Groups["Keyword"].Success) tokenType = "Reserved_Keyword";
                else if (match.Groups["Identifier"].Success) tokenType = "Identifier";
                else if (match.Groups["Number"].Success) tokenType = "Number";
                else if (match.Groups["Assignment"].Success) tokenType = "Assignment_Statement";
                else if (match.Groups["Condition"].Success) tokenType = "Condition_Operator";
                else if (match.Groups["Boolean"].Success) tokenType = "Boolean_Operator";
                else if (match.Groups["Arithmetic"].Success) tokenType = "Arithmetic_Operator";
                else if (match.Groups["Symbol"].Success) tokenType = "Symbol";

                if (!string.IsNullOrWhiteSpace(tokenType))
                {
                    dataGridViewOutput.Rows.Add(lexeme, tokenType);

                    if (tokenType != "Comment_Statement")
                    {
                        allTokens.Add(new Token { Value = lexeme, Type = tokenType });
                    }
                }
            }

            try
            {
                if (allTokens.Count == 0) return;

                TinyParser parser = new TinyParser(allTokens);
                parser.ParseProgram();

                MessageBox.Show("Success: Your Tiny PL code is correct and follows the grammar!", "Parser Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Syntax Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridViewOutput_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void richTextBoxInput_TextChanged(object sender, EventArgs e) { }
    }

    public class Token
    {
        public string Value { get; set; }
        public string Type { get; set; }
    }

    public class TinyParser
    {
        private List<Token> tokens;
        private int index = 0;
        private Token currentToken;

        public TinyParser(List<Token> tokens)
        {
            this.tokens = tokens;
            if (tokens.Count > 0)
                currentToken = tokens[index];
        }

        private void Match(string expected)
        {
            if (index < tokens.Count && (currentToken.Value == expected || currentToken.Type == expected))
            {
                index++;
                if (index < tokens.Count)
                    currentToken = tokens[index];
            }
            else
            {
                string found = currentToken != null ? currentToken.Value : "End of File";
                throw new Exception($"Syntax Error: Expected '{expected}' but found '{found}'");
            }
        }

        public void ParseProgram()
        {
            while (index < tokens.Count)
            {
                if (index + 1 < tokens.Count && tokens[index + 1].Value == "main")
                {
                    break;
                }
                ParseFunctionStatement();
            }
            ParseMainFunction();

            if (index < tokens.Count)
            {
                throw new Exception($"Syntax Error: Unexpected code at the end starting with '{currentToken.Value}'");
            }
        }

        private void ParseFunctionStatement()
        {
            ParseDatatype();
            Match("Identifier");
            Match("(");
            ParseParameters();
            Match(")");
            ParseFunctionBody();
        }

        private void ParseMainFunction()
        {
            ParseDatatype();
            Match("main");
            Match("(");
            Match(")");
            ParseFunctionBody();
        }

        private void ParseParameters()
        {
            if (currentToken.Value == ")") return;

            ParseDatatype();
            Match("Identifier");
            while (index < tokens.Count && currentToken.Value == ",")
            {
                Match(",");
                ParseDatatype();
                Match("Identifier");
            }
        }

        private void ParseFunctionBody()
        {
            Match("{");
            ParseStatements();
            ParseReturnStatement();
            Match("}");
        }

        private void ParseDatatype()
        {
            if (currentToken.Value == "int" || currentToken.Value == "float" || currentToken.Value == "string")
            {
                Match(currentToken.Value);
            }
            else
            {
                throw new Exception($"Syntax Error: Expected a Datatype (int, float, string) but found '{currentToken.Value}'");
            }
        }

        private void ParseStatements()
        {
            while (index < tokens.Count)
            {
                string val = currentToken.Value;
                if (val == "return" || val == "until" || val == "end" || val == "elseif" || val == "else" || val == "}")
                {
                    break;
                }
                ParseStatement();
            }
        }

        private void ParseStatement()
        {
            if (index >= tokens.Count) return;

            if (currentToken.Value == "int" || currentToken.Value == "float" || currentToken.Value == "string")
            {
                ParseDeclaration();
            }
            else if (currentToken.Type == "Identifier")
            {
                ParseAssignment();
            }
            else if (currentToken.Value == "write")
            {
                ParseWrite();
            }
            else if (currentToken.Value == "read")
            {
                ParseRead();
            }
            else if (currentToken.Value == "if")
            {
                ParseIf();
            }
            else if (currentToken.Value == "repeat")
            {
                ParseRepeat();
            }
            else
            {
                throw new Exception($"Error: A statement cannot start with '{currentToken.Value}' ({currentToken.Type})");
            }
        }

        private void ParseDeclaration()
        {
            ParseDatatype();
            ParseDeclItem();
            while (index < tokens.Count && currentToken.Value == ",")
            {
                Match(",");
                ParseDeclItem();
            }
            Match(";");
        }

        private void ParseDeclItem()
        {
            Match("Identifier");
            if (currentToken.Value == ":=")
            {
                Match(":=");
                ParseExpression();
            }
        }

        private void ParseAssignment()
        {
            Match("Identifier");
            Match(":=");
            ParseExpression();
            Match(";");
        }

        private void ParseWrite()
        {
            Match("write");
            if (currentToken.Value == "endl")
            {
                Match("endl");
            }
            else
            {
                ParseExpression();
            }
            Match(";");
        }

        private void ParseRead()
        {
            Match("read");
            Match("Identifier");
            Match(";");
        }

        private void ParseReturnStatement()
        {
            Match("return");
            ParseExpression();
            Match(";");
        }

        private void ParseIf()
        {
            Match("if");
            ParseConditionStatement();
            Match("then");
            ParseStatements();

            while (index < tokens.Count && currentToken.Value == "elseif")
            {
                Match("elseif");
                ParseConditionStatement();
                Match("then");
                ParseStatements();
            }

            if (index < tokens.Count && currentToken.Value == "else")
            {
                Match("else");
                ParseStatements();
            }

            Match("end");
        }

        private void ParseRepeat()
        {
            Match("repeat");
            ParseStatements();
            Match("until");
            ParseConditionStatement();
        }

        private void ParseConditionStatement()
        {
            ParseCondition();
            while (index < tokens.Count && currentToken.Type == "Boolean_Operator")
            {
                Match("Boolean_Operator");
                ParseCondition();
            }
        }

        private void ParseCondition()
        {
            ParseExpression();
            if (currentToken.Type == "Condition_Operator")
            {
                Match("Condition_Operator");
            }
            else
            {
                throw new Exception($"Expected Condition Operator (<, >, =, <>) but found '{currentToken.Value}'");
            }
            ParseExpression();
        }

        private void ParseExpression()
        {
            if (currentToken.Type == "String")
            {
                Match("String");
                return;
            }

            ParseTerm();
            while (index < tokens.Count && currentToken.Type == "Arithmetic_Operator")
            {
                Match("Arithmetic_Operator");
                ParseTerm();
            }
        }

        private void ParseTerm()
        {
            if (currentToken.Type == "Identifier")
            {
                Match("Identifier");
                // Check if it's a function call e.g., sum(a,b)
                if (index < tokens.Count && currentToken.Value == "(")
                {
                    Match("(");
                    if (currentToken.Value != ")")
                    {
                        ParseExpression();
                        while (index < tokens.Count && currentToken.Value == ",")
                        {
                            Match(",");
                            ParseExpression();
                        }
                    }
                    Match(")");
                }
            }
            else if (currentToken.Type == "Number")
            {
                Match("Number");
            }
            else if (currentToken.Value == "(")
            {
                Match("(");
                ParseExpression();
                Match(")");
            }
            else
            {
                throw new Exception($"Expected Identifier, Number, or '(' but found '{currentToken.Value}'");
            }
        }
    }
}