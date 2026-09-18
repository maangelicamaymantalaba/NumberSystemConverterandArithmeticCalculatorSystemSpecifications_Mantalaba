// Number System Converter and Arithmetic Calculator
// Activity No. 1 - System Development Activity
// UPDATED: Added 1's and 2's Complement + Complement-based Subtraction
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
        ShowComplements(inputs);              // NEW: 1's/2's complement of each input
        RunCalculator(inputs);
        RunComplementSubtraction(inputs);     // NEW: subtraction using both complements

        if (!Console.IsInputRedirected)
        {
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
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

    // =====================================================================
    // PART 3: 1's AND 2's COMPLEMENT  (NEW - added for today's activity)
    // =====================================================================

    // Determines how many bits are needed to represent the larger of two
    // magnitudes, plus one extra bit reserved for the sign, so complement
    // subtraction always has room to show a correct negative result.
    static int DetermineBitWidth(long a, long b)
    {
        long max = Math.Max(Math.Abs(a), Math.Abs(b));
        int bits = 1;
        while ((1L << bits) <= max) bits++;
        return bits + 1; // +1 reserved for sign
    }

    // Pads a non-negative decimal value into a fixed-width binary string.
    static string ToBinaryPadded(long value, int bitWidth)
    {
        string bin = Convert.ToString(value, 2);
        if (bin.Length > bitWidth)
            throw new Exception($"bit width too small to represent {value}");
        return bin.PadLeft(bitWidth, '0');
    }

    // 1's complement: flip every bit (0 -> 1, 1 -> 0)
    static string OnesComplementBinary(string binary)
    {
        StringBuilder sb = new StringBuilder();
        foreach (char c in binary)
            sb.Append(c == '0' ? '1' : '0');
        return sb.ToString();
    }

    // 2's complement: 1's complement + 1
    static string TwosComplementBinary(string binary)
    {
        string ones = OnesComplementBinary(binary);
        return AddBinary(ones, ToBinaryPadded(1, binary.Length)).sum;
    }

    // Adds two equal-length binary strings. Returns the sum (same width,
    // extra carry beyond the width is dropped) and the final carry-out bit.
    static (string sum, int carryOut) AddBinary(string a, string b)
    {
        int len = Math.Max(a.Length, b.Length);
        a = a.PadLeft(len, '0');
        b = b.PadLeft(len, '0');
        char[] result = new char[len];
        int carry = 0;

        for (int i = len - 1; i >= 0; i--)
        {
            int bitA = a[i] - '0';
            int bitB = b[i] - '0';
            int total = bitA + bitB + carry;
            result[i] = (char)('0' + (total % 2));
            carry = total / 2;
        }

        return (new string(result), carry);
    }

    // Converts an unsigned binary string into Binary/Octal/Decimal/Hex,
    // formatted the same way as DisplayConversion() above.
    static void DisplayComplementInAllBases(string label, string binary)
    {
        long decimalValue = Convert.ToInt64(binary, 2);
        string octal = Convert.ToString(decimalValue, 8);
        string hexadecimal = Convert.ToString(decimalValue, 16).ToUpper();

        Console.WriteLine($"{label}:");
        Console.WriteLine($"   Binary:      {binary}");
        Console.WriteLine($"   Octal:       {octal}");
        Console.WriteLine($"   Decimal:     {decimalValue}");
        Console.WriteLine($"   Hexadecimal: {hexadecimal}");
    }

    // Shows the 1's and 2's complement of every input number, in all 4 bases.
    static void ShowComplements(List<InputNumber> inputs)
    {
        Console.WriteLine("\n=====================================");
        Console.WriteLine("   1's AND 2's COMPLEMENT OF INPUTS");
        Console.WriteLine("=====================================");

        foreach (var input in inputs)
        {
            int bits = 1;
            while ((1L << bits) <= input.DecimalValue) bits++;

            string binary = ToBinaryPadded(input.DecimalValue, bits);
            string ones = OnesComplementBinary(binary);
            string twos = TwosComplementBinary(binary);

            Console.WriteLine("\n-------------------------------------");
            Console.WriteLine($"Input: {input.Original} (Base {input.BaseNumber}) = {input.DecimalValue} decimal");
            Console.WriteLine($"Original Binary ({bits}-bit): {binary}");
            Console.WriteLine("-------------------------------------");
            DisplayComplementInAllBases("1's Complement", ones);
            Console.WriteLine();
            DisplayComplementInAllBases("2's Complement", twos);
        }
    }

    // Lets the user pick two input numbers (by letter) and subtracts them
    // using both the 1's complement method and the 2's complement method.
    static void RunComplementSubtraction(List<InputNumber> inputs)
    {
        Console.WriteLine("\n=====================================");
        Console.WriteLine("  SUBTRACTION USING COMPLEMENT METHOD");
        Console.WriteLine("=====================================");
        Console.Write("Do you want to try subtraction using complements? (y/n): ");
        string answer = (Console.ReadLine() ?? "").Trim().ToLower();
        if (answer != "y" && answer != "yes") return;

        char letterA = GetInputLetter(inputs.Count, "minuend (A in A - B)");
        char letterB = GetInputLetter(inputs.Count, "subtrahend (B in A - B)");

        InputNumber numA = inputs[letterA - 'a'];
        InputNumber numB = inputs[letterB - 'a'];

        long a = numA.DecimalValue;
        long b = numB.DecimalValue;

        int bitWidth = DetermineBitWidth(a, b);

        Console.WriteLine($"\nExpression: {numA.Original} (Base {numA.BaseNumber}) - {numB.Original} (Base {numB.BaseNumber})");
        Console.WriteLine($"Decimal:    {a} - {b}");
        Console.WriteLine($"Using {bitWidth}-bit representation.\n");

        // ----- 1's complement subtraction -----
        Console.WriteLine("----- 1's Complement Method -----");
        string aBin = ToBinaryPadded(a, bitWidth);
        string bBin = ToBinaryPadded(b, bitWidth);
        string bOnes = OnesComplementBinary(bBin);

        Console.WriteLine($"{letterA} in binary:        {aBin}");
        Console.WriteLine($"{letterB} in binary:        {bBin}");
        Console.WriteLine($"1's complement of {letterB}:  {bOnes}");

        var addOnes = AddBinary(aBin, bOnes);
        Console.WriteLine($"Sum:                 {addOnes.sum}  (carry-out: {addOnes.carryOut})");

        string ones_resultBinary;
        bool ones_negative;
        if (addOnes.carryOut == 1)
        {
            // end-around carry: add the carry back to the least significant bit
            var final = AddBinary(addOnes.sum, ToBinaryPadded(1, bitWidth));
            ones_resultBinary = final.sum;
            ones_negative = false;
            Console.WriteLine($"End-around carry applied -> {ones_resultBinary}");
        }
        else
        {
            // no carry-out: result is negative, magnitude = 1's complement of the sum
            ones_resultBinary = OnesComplementBinary(addOnes.sum);
            ones_negative = true;
            Console.WriteLine($"No carry-out -> result is negative, magnitude = {ones_resultBinary}");
        }

        PrintComplementResult("1's Complement Result", ones_resultBinary, ones_negative);

        // ----- 2's complement subtraction -----
        Console.WriteLine("\n----- 2's Complement Method -----");
        string bTwos = TwosComplementBinary(bBin);
        Console.WriteLine($"{letterA} in binary:        {aBin}");
        Console.WriteLine($"{letterB} in binary:        {bBin}");
        Console.WriteLine($"2's complement of {letterB}:  {bTwos}");

        var addTwos = AddBinary(aBin, bTwos);
        Console.WriteLine($"Sum (carry-out discarded): {addTwos.sum}");

        string twos_resultBinary;
        bool twos_negative;
        if (addTwos.sum[0] == '0')
        {
            twos_resultBinary = addTwos.sum;
            twos_negative = false;
        }
        else
        {
            twos_resultBinary = TwosComplementBinary(addTwos.sum);
            twos_negative = true;
            Console.WriteLine($"Leading bit is 1 -> result is negative, magnitude = {twos_resultBinary}");
        }

        PrintComplementResult("2's Complement Result", twos_resultBinary, twos_negative);

        // ----- cross-check against normal subtraction -----
        long expected = a - b;
        Console.WriteLine($"\nCheck: normal subtraction {a} - {b} = {expected}. Both methods above should match this.");
    }

    static char GetInputLetter(int inputCount, string role)
    {
        while (true)
        {
            Console.Write($"Enter the letter of the {role} (a-{IndexToLetter(inputCount)}): ");
            string line = (Console.ReadLine() ?? "").Trim().ToLower();
            if (line.Length == 1 && line[0] >= 'a' && line[0] < 'a' + inputCount)
                return line[0];
            Console.WriteLine("Invalid letter, try again.");
        }
    }

    static void PrintComplementResult(string label, string magnitudeBinary, bool negative)
    {
        long decimalValue = Convert.ToInt64(magnitudeBinary, 2);
        string octal = Convert.ToString(decimalValue, 8);
        string hexadecimal = Convert.ToString(decimalValue, 16).ToUpper();
        string sign = negative ? "-" : "";

        Console.WriteLine($"{label}:");
        Console.WriteLine($"   Binary:      {sign}{magnitudeBinary}");
        Console.WriteLine($"   Octal:       {sign}{octal}");
        Console.WriteLine($"   Decimal:     {sign}{decimalValue}");
        Console.WriteLine($"   Hexadecimal: {sign}{hexadecimal}");
    }
}   