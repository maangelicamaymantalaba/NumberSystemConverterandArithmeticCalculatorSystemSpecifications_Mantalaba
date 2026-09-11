// Number System Converter and Arithmetic Calculator
// Activity No. 1 - System Development Activity
// Expressions use letters (a, b, c, d, ...) mapped in order to your inputs.

using System;
using System.Collections.Generic;
using System.Text;

class Program
{
    struct InputNumber
    {
        public string Original;
        public int BaseNumber;
        public long DecimalValue;
    }

    static void Main()
    {
        List<InputNumber> inputs = RunConverter();
        RunCalculator(inputs);

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    // =====================================================================
    // PART 1: NUMBER SYSTEM CONVERTER
    // =====================================================================

    static List<InputNumber> RunConverter()
    {
        Console.WriteLine("=====================================");
        Console.WriteLine("       NUMBER SYSTEM CONVERTER");
        Console.WriteLine("=====================================");

        int numberOfInputs = GetNumberOfInputs();
        List<InputNumber> inputs = new List<InputNumber>();

        for (int i = 1; i <= numberOfInputs; i++)
        {
            Console.WriteLine("\n-------------------------------------");
            Console.WriteLine($"INPUT NUMBER {i} (variable '{IndexToLetter(i)}')");
            Console.WriteLine("-------------------------------------");

            int baseNumber = GetBaseChoice();
            string inputNumber = GetValidNumber(baseNumber);
            long decimalValue = Convert.ToInt64(inputNumber, baseNumber);

            DisplayConversion(inputNumber, decimalValue);

            inputs.Add(new InputNumber
            {
                Original = inputNumber,
                BaseNumber = baseNumber,
                DecimalValue = decimalValue
            });
        }

        Console.WriteLine("\n=====================================");
        Console.WriteLine("     CONVERSION COMPLETED!");
        Console.WriteLine("=====================================");

        return inputs;
    }

    static int GetNumberOfInputs()
    {
        int numberOfInputs;
        while (true)
        {
            Console.Write("\nEnter the number of inputs (minimum of 3): ");

            if (int.TryParse(Console.ReadLine(), out numberOfInputs)
                && numberOfInputs >= 3 && numberOfInputs <= 26)
            {
                return numberOfInputs;
            }

            Console.WriteLine("Invalid input! Please enter a number from 3 to 26.");
        }
    }

    static int GetBaseChoice()
    {
        while (true)
        {
            Console.WriteLine("\nSelect Number System:");
            Console.WriteLine("[1] Binary (Base 2)");
            Console.WriteLine("[2] Octal (Base 8)");
            Console.WriteLine("[3] Decimal (Base 10)");
            Console.WriteLine("[4] Hexadecimal (Base 16)");
            Console.Write("Enter your choice: ");

            if (int.TryParse(Console.ReadLine(), out int choice)
                && choice >= 1 && choice <= 4)
            {
                switch (choice)
                {
                    case 1: return 2;
                    case 2: return 8;
                    case 3: return 10;
                    case 4: return 16;
                }
            }

            Console.WriteLine("Invalid choice! Please select from 1 to 4.");
        }
    }

    static string GetValidNumber(int baseNumber)
    {
        while (true)
        {
            Console.Write($"\nEnter a Base {baseNumber} number: ");
            string inputNumber = Console.ReadLine() ?? "";

            try
            {
                Convert.ToInt64(inputNumber, baseNumber);
                return inputNumber;
            }
            catch
            {
                Console.WriteLine($"Invalid Base {baseNumber} number! Please try again.");
            }
        }
    }

    static void DisplayConversion(string originalInput, long decimalValue)
    {
        string binary = Convert.ToString(decimalValue, 2);
        string octal = Convert.ToString(decimalValue, 8);
        string hexadecimal = Convert.ToString(decimalValue, 16).ToUpper();

        Console.WriteLine("\n=====================================");
        Console.WriteLine("         CONVERSION RESULT");
        Console.WriteLine("=====================================");
        Console.WriteLine($"Original Input: {originalInput}");
        Console.WriteLine($"Binary:         {binary}");
        Console.WriteLine($"Octal:          {octal}");
        Console.WriteLine($"Decimal:        {decimalValue}");
        Console.WriteLine($"Hexadecimal:    {hexadecimal}");
    }

    // Converts input index (1, 2, 3...) to a letter (a, b, c...)
    static char IndexToLetter(int index) => (char)('a' + (index - 1));

    // =====================================================================
    // PART 2: ARITHMETIC CALCULATOR
    // =====================================================================

    static void RunCalculator(List<InputNumber> inputs)
    {
        Console.WriteLine("\n=====================================");
        Console.WriteLine("       ARITHMETIC CALCULATOR");
        Console.WriteLine("=====================================");

        for (int i = 0; i < inputs.Count; i++)
        {
            char letter = IndexToLetter(i + 1);
            Console.WriteLine($"{letter} = {inputs[i].Original} (Base {inputs[i].BaseNumber}) = {inputs[i].DecimalValue} decimal");
        }

        List<string> tokens = GetCalculationTokens(inputs.Count);

        try
        {
            ValidateTokenSequence(tokens);
            List<string> postfix = ToPostfix(tokens);
            double result = EvaluatePostfix(postfix, inputs);

            string expressionDisplay = BuildExpressionDisplay(tokens, inputs);
            Console.WriteLine("\n=====================================");
            Console.WriteLine("         ARITHMETIC EXPRESSION");
            Console.WriteLine("=====================================");
            Console.WriteLine($"Expression: {expressionDisplay}");
            Console.WriteLine($"Result (Decimal): {result}");

            DisplayFinalResult(result);
        }
        catch (DivideByZeroException)
        {
            Console.WriteLine("\nArithmetic Error: Division by zero is not allowed. Calculation aborted.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nArithmetic Error: {ex.Message}. Calculation aborted.");
        }
    }

    static List<string> GetCalculationTokens(int inputCount)
    {
        while (true)
        {
            Console.WriteLine("\nChoose calculation mode:");
            Console.WriteLine("[1] Simple operation applied to all inputs (a op b op c ...)");
            Console.WriteLine("[2] Custom expression with parentheses (e.g. (a+b-c)*d)");
            Console.Write("Enter your choice: ");

            if (!int.TryParse(Console.ReadLine(), out int mode) || (mode != 1 && mode != 2))
            {
                Console.WriteLine("Invalid choice! Please select 1 or 2.");
                continue;
            }

            string expression;

            if (mode == 1)
            {
                string op = GetOperatorChoice();
                StringBuilder sb = new StringBuilder();
                for (int i = 1; i <= inputCount; i++)
                {
                    sb.Append(IndexToLetter(i));
                    if (i < inputCount) sb.Append(" " + op + " ");
                }
                expression = sb.ToString();
                Console.WriteLine($"Generated expression: {expression}");
            }
            else
            {
                char lastLetter = IndexToLetter(inputCount);
                Console.WriteLine($"\nYou have variables 'a' to '{lastLetter}' available.");
                Console.WriteLine("Example: (a+b-c)*d");
                Console.Write("Enter your expression: ");
                expression = Console.ReadLine() ?? "";
            }

            try
            {
                return Tokenize(expression, inputCount);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Invalid expression: {ex.Message}. Please try again.");
            }
        }
    }

    static string GetOperatorChoice()
    {
        while (true)
        {
            Console.WriteLine("\nSelect Arithmetic Operation:");
            Console.WriteLine("[1] Addition (+)");
            Console.WriteLine("[2] Subtraction (-)");
            Console.WriteLine("[3] Multiplication (*)");
            Console.WriteLine("[4] Division (/)");
            Console.Write("Enter your choice: ");

            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                switch (choice)
                {
                    case 1: return "+";
                    case 2: return "-";
                    case 3: return "*";
                    case 4: return "/";
                }
            }
            Console.WriteLine("Invalid choice! Please select 1 to 4.");
        }
    }

    // Breaks an expression string into tokens: "a", "+", "(", ")", etc.
    // Letters a, b, c... refer to inputs in order (a = input 1, b = input 2, ...)
    static List<string> Tokenize(string expr, int inputCount)
    {
        List<string> tokens = new List<string>();
        int i = 0;

        while (i < expr.Length)
        {
            char c = expr[i];

            if (char.IsWhiteSpace(c)) { i++; continue; }

            if (c == '(' || c == ')' || c == '+' || c == '-' || c == '*' || c == '/')
            {
                tokens.Add(c.ToString());
                i++;
            }
            else if (char.IsLetter(c))
            {
                char letter = char.ToLower(c);
                int idx = letter - 'a' + 1;

                if (idx < 1 || idx > inputCount)
                    throw new Exception($"'{letter}' is out of range (you have 'a' to '{IndexToLetter(inputCount)}')");

                tokens.Add(letter.ToString());
                i++;
            }
            else
            {
                throw new Exception($"unexpected character '{c}'");
            }
        }

        return tokens;
    }

    static void ValidateTokenSequence(List<string> tokens)
    {
        if (tokens.Count == 0)
            throw new Exception("expression is empty");

        int parenDepth = 0;
        bool expectOperand = true;

        foreach (string tok in tokens)
        {
            if (tok == "(")
            {
                if (!expectOperand) throw new Exception("unexpected '('");
                parenDepth++;
            }
            else if (tok == ")")
            {
                if (expectOperand) throw new Exception("unexpected ')'");
                parenDepth--;
                if (parenDepth < 0) throw new Exception("unmatched ')'");
            }
            else if (tok == "+" || tok == "-" || tok == "*" || tok == "/")
            {
                if (expectOperand) throw new Exception($"unexpected operator '{tok}'");
                expectOperand = true;
            }
            else
            {
                if (!expectOperand) throw new Exception($"unexpected value '{tok}'");
                expectOperand = false;
            }
        }

        if (expectOperand) throw new Exception("expression ends unexpectedly (missing a value)");
        if (parenDepth != 0) throw new Exception("unmatched '('");
    }

    static int Precedence(string op) => (op == "*" || op == "/") ? 2 : 1;

    // Converts infix tokens to postfix (RPN) using the Shunting-Yard algorithm.
    // This enforces operator precedence and left-to-right associativity.
    static List<string> ToPostfix(List<string> tokens)
    {
        List<string> output = new List<string>();
        Stack<string> opStack = new Stack<string>();

        foreach (string tok in tokens)
        {
            if (tok.Length == 1 && char.IsLetter(tok[0]))
            {
                output.Add(tok);
            }
            else if (tok == "(")
            {
                opStack.Push(tok);
            }
            else if (tok == ")")
            {
                while (opStack.Count > 0 && opStack.Peek() != "(")
                    output.Add(opStack.Pop());
                if (opStack.Count > 0) opStack.Pop(); // discard "("
            }
            else // operator
            {
                while (opStack.Count > 0 && opStack.Peek() != "("
                       && Precedence(opStack.Peek()) >= Precedence(tok))
                {
                    output.Add(opStack.Pop());
                }
                opStack.Push(tok);
            }
        }

        while (opStack.Count > 0) output.Add(opStack.Pop());
        return output;
    }

    // Evaluates a postfix expression using the decimal values of the inputs.
    static double EvaluatePostfix(List<string> postfix, List<InputNumber> inputs)
    {
        Stack<double> stack = new Stack<double>();

        foreach (string tok in postfix)
        {
            if (tok.Length == 1 && char.IsLetter(tok[0]))
            {
                int idx = tok[0] - 'a' + 1;
                stack.Push(inputs[idx - 1].DecimalValue);
            }
            else
            {
                double b = stack.Pop();
                double a = stack.Pop();
                double res;

                switch (tok)
                {
                    case "+": res = a + b; break;
                    case "-": res = a - b; break;
                    case "*": res = a * b; break;
                    case "/":
                        if (b == 0) throw new DivideByZeroException();
                        res = a / b;
                        break;
                    default: throw new Exception("unknown operator");
                }

                stack.Push(res);
            }
        }

        return stack.Pop();
    }

    // Rebuilds the expression using original input values instead of letters
    static string BuildExpressionDisplay(List<string> tokens, List<InputNumber> inputs)
    {
        StringBuilder sb = new StringBuilder();

        foreach (string tok in tokens)
        {
            if (tok.Length == 1 && char.IsLetter(tok[0]))
            {
                int idx = tok[0] - 'a' + 1;
                sb.Append(inputs[idx - 1].Original);
            }
            else
            {
                sb.Append(" " + tok + " ");
            }
        }

        return sb.ToString().Trim();
    }

    static void DisplayFinalResult(double result)
    {
        Console.WriteLine("\n=====================================");
        Console.WriteLine("            FINAL RESULT");
        Console.WriteLine("=====================================");
        Console.WriteLine($"Decimal:        {result}");

        if (result == Math.Floor(result) && !double.IsInfinity(result))
        {
            long intResult = (long)result;
            bool negative = intResult < 0;
            long magnitude = Math.Abs(intResult);

            string binary = (negative ? "-" : "") + Convert.ToString(magnitude, 2);
            string octal = (negative ? "-" : "") + Convert.ToString(magnitude, 8);
            string hexadecimal = (negative ? "-" : "") + Convert.ToString(magnitude, 16).ToUpper();

            Console.WriteLine($"Binary:         {binary}");
            Console.WriteLine($"Octal:          {octal}");
            Console.WriteLine($"Hexadecimal:    {hexadecimal}");
        }
        else
        {
            Console.WriteLine("Binary/Octal/Hexadecimal: N/A (result is not a whole number)");
        }
    }
}