using System;
using System.Text;

namespace Graph_Drawer
{
	class Equation
	{
		// Constants ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		public const double E = Math.E;
		public const double PI = Math.PI;
		public const double TWO_PI = 2.0d * Math.PI;
		public const double HALF_PI = Math.PI / 2.0d;
		public const double QUARTER_PI = Math.PI / 4.0d;
		public const double ONE_AND_HALF_PI = Math.PI / 2.0d * 3.0d;
		public const double PI_DIVIDED_FROM_180 = 180.0d / Math.PI;
		public const string DOUBLE_FIXED_POINT_FORMAT = "0.###################################################################################################################################################################################################################################################################################################################################################";
		public const string ERROR = "Error!";
		public const string ERROR__INVALID_EQUATION = "Equation is not valid!";
		public const string ERROR__NEGATIVE_NUMBER_IN_LOG_FUNCTION = "Can't apply 'log' function on a negative number!";
		public const string ERROR__NEGATIVE_NUMBER_IN_LN_FUNCTION = "Can't apply 'ln' function on a negative number!";
		public const string ERROR__INFINITY_IN_SIN_FUNCTION = "Can't apply 'sin' function on infinity!";
		public const string ERROR__INFINITY_IN_COS_FUNCTION = "Can't apply 'cos' function on infinity!";
		public const string ERROR__INFINITY_IN_TAN_FUNCTION = "Can't apply 'tan' function on infinity!";
		public const string ERROR__NEGATIVE_NUMBER_IN_SQUARE_ROOT = "Can't apply a square-root on a negative number!";
		public const string ERROR__NON_WHOLE_EXPONENT_IN_POWER_ON_NEGATIVE_NUMBER = "Can't apply a power with a non-whole exponent on a negative number!";
		public const string ERROR__INFINITY_DIVIDED_BY_INFINITY = "Can't divide infinity by infinity!";
		public const string ERROR__ZERO_DIVIDED_BY_ZERO = "Can't divide zero by zero!";
		public const string ERROR__INFINITY_SUBTRACTED_FROM_INFINITY = "Can't subtract infinity from infinity!";
		public const string SOLVING_STEP__ATTEMPTING_TO_SOLVE_THE_EQUATION = "Attempting to solve the equation:";
		public const string SOLVING_STEP__SOLVING_EQUATION_INSIDE_PARENTHESES = "Solving equations inside parentheses:";
		public const string SOLVING_STEP__REPLACING_THE_RESULT_WITH_THE_PARENTHESES = "Replacing the results with the parentheses:";
		public const string SOLVING_STEP__ADDING_MISSING_MULTIPLICATION_SIGNS = "Adding missing multiplication signs:";
		public const string SOLVING_STEP__ASSIGNING_THE_VARIABLE_AND_CONSTANTS = "Assigning the variable and constants:";
		public const string SOLVING_STEP__SOLVING_FUNCTIONS = "Solving functions:";
		public const string SOLVING_STEP__SOLVING_SQUARE_ROOTS = "Solving square-roots:";
		public const string SOLVING_STEP__SOLVING_POWERS = "Solving powers:";
		public const string SOLVING_STEP__DELETING_ALL_THE_PARENTHESES_FROM_THE_EQUATION = "Deleting all the parentheses from the equation:";
		public const string SOLVING_STEP__ADDING_ADDITION_SIGN_BEFORE_SUBTRACTION_SIGNS = "Adding addition sign before subtraction signs:";
		public const string SOLVING_STEP__DELETING_DOUBLE_SUBTRACTION_SIGNS = "Deleting double subtraction signs:";
		public const string SOLVING_STEP__SOLVING_MULTIPLICATIONS_AND_DIVISIONS = "Solving multiplications and divisions:";
		public const string SOLVING_STEP__SOLVING_ADDITIONS = "Solving additions:";
		public const string SOLVING_STEP__EQUATION_RESULT = "Equation result:";
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



		// Variables ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		private string equation;
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



		// Constructors ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		public Equation(string equation)
		{
			if (!IsEquationValid(equation))
				throw new Exception(ERROR__INVALID_EQUATION);
			this.equation = equation;
		}
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



		// Methods /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		public double Solve(double variable)
		{
			if (this.equation == null)
				throw new Exception(ERROR__INVALID_EQUATION);
			return Solve(this.equation, variable, false, false, 0);
		}
		public double Solve(double variable, bool useDegreesInsteadOfRadians)
		{
			if (this.equation == null)
				throw new Exception(ERROR__INVALID_EQUATION);
			return Solve(this.equation, variable, useDegreesInsteadOfRadians, false, 0);
		}
		public double Solve(double variable, bool useDegreesInsteadOfRadians, bool printSolvingSteps)
		{
			if (this.equation == null)
				throw new Exception(ERROR__INVALID_EQUATION);
			return Solve(this.equation, variable, useDegreesInsteadOfRadians, printSolvingSteps, 0);
		}
		private static double Solve(string equation, double variable, bool useDegreesInsteadOfRadians, bool printSolvingSteps, int scopeLevel)
		{
			if (printSolvingSteps)
				PrintASolvingStep(SOLVING_STEP__ATTEMPTING_TO_SOLVE_THE_EQUATION, equation, false, ConsoleColor.Yellow, ConsoleColor.Gray, scopeLevel);
			
			try
			{
				// Solving equations inside parentheses (of the current level) and replacing the results with the parentheses:
				StringBuilder tempEquation = new StringBuilder();
				int parenthesesPairsCount = 0;
				int parenthesesNestingLevel = 0;
				for (int i = 0; i < equation.Length; i++)
				{
					if (equation[i] == '(')
						parenthesesNestingLevel++;
					else if (equation[i] == ')')
					{
						parenthesesNestingLevel--;
						if (parenthesesNestingLevel == 0)
							parenthesesPairsCount++;
					}
				}
				int[] parenthesesEquationStartIndexes = new int[parenthesesPairsCount];
				int[] parenthesesEquationEndIndexes = new int[parenthesesPairsCount];
				parenthesesPairsCount = 0;
				parenthesesNestingLevel = 0;
				for (int i = 0; i < equation.Length; i++)
				{
					if (equation[i] == '(')
					{
						if (parenthesesNestingLevel == 0)
							parenthesesEquationStartIndexes[parenthesesPairsCount] = i + 1;
						parenthesesNestingLevel++;
					}
					else if (equation[i] == ')')
					{
						parenthesesNestingLevel--;
						if (parenthesesNestingLevel == 0)
						{
							parenthesesEquationEndIndexes[parenthesesPairsCount] = i - 1;
							parenthesesPairsCount++;
						}
					}
				}
				string[] parenthesesInnerEquationStrings = new string[parenthesesPairsCount];
				for (int i = 0; i < parenthesesInnerEquationStrings.Length; i++)
					parenthesesInnerEquationStrings[i] = Substring(equation, parenthesesEquationStartIndexes[i], parenthesesEquationEndIndexes[i]);
				int parenthesesUnsolvedInnerEquationsCount = 0;
				for (int i = 0; i < parenthesesInnerEquationStrings.Length; i++)
					if (!IsNumber(parenthesesInnerEquationStrings[i]))
						parenthesesUnsolvedInnerEquationsCount++;
				if (parenthesesUnsolvedInnerEquationsCount > 0)
				{
					if (parenthesesPairsCount > 0)
					{
						if (printSolvingSteps)
							PrintASolvingStep(SOLVING_STEP__SOLVING_EQUATION_INSIDE_PARENTHESES, "", false, ConsoleColor.White, 0, scopeLevel);
						try
						{
							double parenthesesInnerEquationResult;
							if (parenthesesPairsCount == 1)
							{
								tempEquation.Append(Substring(equation, 0, parenthesesEquationStartIndexes[0] - 1));
								if (!IsNumber(parenthesesInnerEquationStrings[0]))
								{
									parenthesesInnerEquationResult = Solve(parenthesesInnerEquationStrings[0], variable, useDegreesInsteadOfRadians, printSolvingSteps, scopeLevel + 1);
									tempEquation.Append(ConvertToString(parenthesesInnerEquationResult));
								}
								else
									tempEquation.Append(parenthesesInnerEquationStrings[0]);
								tempEquation.Append(Substring(equation, parenthesesEquationEndIndexes[0] + 1, equation.Length - 1));
							}
							else if (parenthesesPairsCount == 2)
							{
								tempEquation.Append(Substring(equation, 0, parenthesesEquationStartIndexes[0] - 1));
								if (!IsNumber(parenthesesInnerEquationStrings[0]))
								{
									parenthesesInnerEquationResult = Solve(parenthesesInnerEquationStrings[0], variable, useDegreesInsteadOfRadians, printSolvingSteps, scopeLevel + 1);
									tempEquation.Append(ConvertToString(parenthesesInnerEquationResult));
								}
								else
									tempEquation.Append(parenthesesInnerEquationStrings[0]);
								tempEquation.Append(Substring(equation, parenthesesEquationEndIndexes[0] + 1, parenthesesEquationStartIndexes[1] - 1));
								if (!IsNumber(parenthesesInnerEquationStrings[1]))
								{
									parenthesesInnerEquationResult = Solve(parenthesesInnerEquationStrings[1], variable, useDegreesInsteadOfRadians, printSolvingSteps, scopeLevel + 1);
									tempEquation.Append(ConvertToString(parenthesesInnerEquationResult));
								}
								else
									tempEquation.Append(parenthesesInnerEquationStrings[1]);
								tempEquation.Append(Substring(equation, parenthesesEquationEndIndexes[1] + 1, equation.Length - 1));
							}
							else
							{
								tempEquation.Append(Substring(equation, 0, parenthesesEquationStartIndexes[0] - 1));
								if (!IsNumber(parenthesesInnerEquationStrings[0]))
								{
									parenthesesInnerEquationResult = Solve(parenthesesInnerEquationStrings[0], variable, useDegreesInsteadOfRadians, printSolvingSteps, scopeLevel + 1);
									tempEquation.Append(ConvertToString(parenthesesInnerEquationResult));
								}
								else
									tempEquation.Append(parenthesesInnerEquationStrings[0]);
								for (int i = 1; i < parenthesesPairsCount; i++)
								{
									tempEquation.Append(Substring(equation, parenthesesEquationEndIndexes[i - 1] + 1, parenthesesEquationStartIndexes[i] - 1));
									if (!IsNumber(parenthesesInnerEquationStrings[i]))
									{
										parenthesesInnerEquationResult = Solve(parenthesesInnerEquationStrings[i], variable, useDegreesInsteadOfRadians, printSolvingSteps, scopeLevel + 1);
										tempEquation.Append(ConvertToString(parenthesesInnerEquationResult));
									}
									else
										tempEquation.Append(parenthesesInnerEquationStrings[i]);
								}
								tempEquation.Append(Substring(equation, parenthesesEquationEndIndexes[parenthesesPairsCount - 1] + 1, equation.Length - 1));
							}
							equation = tempEquation.ToString();
							if (printSolvingSteps)
								PrintASolvingStep(SOLVING_STEP__REPLACING_THE_RESULT_WITH_THE_PARENTHESES, equation, false, ConsoleColor.White, ConsoleColor.Gray, scopeLevel);
						}
						catch (Exception e)
						{
							if (printSolvingSteps)
								PrintASolvingStep(SOLVING_STEP__REPLACING_THE_RESULT_WITH_THE_PARENTHESES, equation, true, ConsoleColor.White, ConsoleColor.Gray, scopeLevel);
							throw e;
						}
					}
				}

				// Adding missing multiplication signs before parentheses, square-roots, functions, variables, constants and infinities (and after a number, a variable, a constant or an infinity):
				try
				{
					tempEquation.Clear();
					for (int i = 0; i < equation.Length; i++)
					{
						if (i > 0)
						{
							if (equation[i] == '(' && (IsDigitOrDecimalPointOrInfinitySign(equation[i - 1]) || equation[i - 1] == 'x' || equation[i - 1] == 'π' || equation[i - 1] == 'e' || equation[i - 1] == '∞' || equation[i - 1] == ')'))
								tempEquation.Append("*(");
							else if (equation[i] == '√' && (IsDigitOrDecimalPointOrInfinitySign(equation[i - 1]) || equation[i - 1] == 'x' || equation[i - 1] == 'π' || equation[i - 1] == 'e' || equation[i - 1] == '∞' || equation[i - 1] == ')'))
								tempEquation.Append("*√");
							else if (equation[i] == 'x' && (IsDigitOrDecimalPointOrInfinitySign(equation[i - 1]) || equation[i - 1] == 'x' || equation[i - 1] == 'π' || equation[i - 1] == 'e' || equation[i - 1] == '∞' || equation[i - 1] == ')'))
								tempEquation.Append("*x");
							else if (equation[i] == 'π' && (IsDigitOrDecimalPointOrInfinitySign(equation[i - 1]) || equation[i - 1] == 'x' || equation[i - 1] == 'π' || equation[i - 1] == 'e' || equation[i - 1] == '∞' || equation[i - 1] == ')'))
								tempEquation.Append("*π");
							else if (equation[i] == 'e' && (IsDigitOrDecimalPointOrInfinitySign(equation[i - 1]) || equation[i - 1] == 'x' || equation[i - 1] == 'π' || equation[i - 1] == 'e' || equation[i - 1] == '∞' || equation[i - 1] == ')'))
								tempEquation.Append("*e");
							else if (equation[i] == '∞' && (IsDigitOrDecimalPointOrInfinitySign(equation[i - 1]) || equation[i - 1] == 'x' || equation[i - 1] == 'π' || equation[i - 1] == 'e' || equation[i - 1] == '∞' || equation[i - 1] == ')'))
								tempEquation.Append("*∞");
							else if (IsDigitOrDecimalPointOrInfinitySign(equation[i]) && (equation[i - 1] == 'x' || equation[i - 1] == 'π' || equation[i - 1] == 'e' || equation[i - 1] == '∞' || equation[i - 1] == ')'))
								tempEquation.Append('*').Append(equation[i]);
							else if (i < equation.Length - 1)
							{
								if (equation[i] == 'l' && equation[i + 1] == 'n' && (IsDigitOrDecimalPointOrInfinitySign(equation[i - 1]) || equation[i - 1] == 'x' || equation[i - 1] == 'π' || equation[i - 1] == 'e' || equation[i - 1] == '∞' || equation[i - 1] == ')'))
								{
									tempEquation.Append("*ln");
									i += 1;
								}
								else if (i < equation.Length - 2)
								{
									if (equation[i] == 'a' && equation[i + 1] == 'b' && equation[i + 2] == 's' && (IsDigitOrDecimalPointOrInfinitySign(equation[i - 1]) || equation[i - 1] == 'x' || equation[i - 1] == 'π' || equation[i - 1] == 'e' || equation[i - 1] == '∞' || equation[i - 1] == ')'))
									{
										tempEquation.Append("*abs");
										i += 2;
									}
									else if (equation[i] == 'l' && equation[i + 1] == 'o' && equation[i + 2] == 'g' && (IsDigitOrDecimalPointOrInfinitySign(equation[i - 1]) || equation[i - 1] == 'x' || equation[i - 1] == 'π' || equation[i - 1] == 'e' || equation[i - 1] == '∞' || equation[i - 1] == ')'))
									{
										tempEquation.Append("*log");
										i += 2;
									}
									else if (equation[i] == 's' && equation[i + 1] == 'i' && equation[i + 2] == 'n' && (IsDigitOrDecimalPointOrInfinitySign(equation[i - 1]) || equation[i - 1] == 'x' || equation[i - 1] == 'π' || equation[i - 1] == 'e' || equation[i - 1] == '∞' || equation[i - 1] == ')'))
									{
										tempEquation.Append("*sin");
										i += 2;
									}
									else if (equation[i] == 'c' && equation[i + 1] == 'o' && equation[i + 2] == 's' && (IsDigitOrDecimalPointOrInfinitySign(equation[i - 1]) || equation[i - 1] == 'x' || equation[i - 1] == 'π' || equation[i - 1] == 'e' || equation[i - 1] == '∞' || equation[i - 1] == ')'))
									{
										tempEquation.Append("*cos");
										i += 2;
									}
									else if (equation[i] == 't' && equation[i + 1] == 'a' && equation[i + 2] == 'n' && (IsDigitOrDecimalPointOrInfinitySign(equation[i - 1]) || equation[i - 1] == 'x' || equation[i - 1] == 'π' || equation[i - 1] == 'e' || equation[i - 1] == '∞' || equation[i - 1] == ')'))
									{
										tempEquation.Append("*tan");
										i += 2;
									}
									else
										tempEquation.Append(equation[i]);
								}
								else
									tempEquation.Append(equation[i]);
							}
							else
								tempEquation.Append(equation[i]);
						}
						else
							tempEquation.Append(equation[i]);
					}
					if (!equation.Equals(tempEquation.ToString()))
					{
						equation = tempEquation.ToString();
						if (printSolvingSteps)
							PrintASolvingStep(SOLVING_STEP__ADDING_MISSING_MULTIPLICATION_SIGNS, equation, false, ConsoleColor.White, ConsoleColor.Gray, scopeLevel);
					}
				}
				catch (Exception e)
				{
					if (printSolvingSteps)
						PrintASolvingStep(SOLVING_STEP__ADDING_MISSING_MULTIPLICATION_SIGNS, equation, true, ConsoleColor.White, ConsoleColor.Gray, scopeLevel);
					throw e;
				}
				
				// Assigning the variable and constants:
				try
				{
					tempEquation.Clear();
					for (int i = 0; i < equation.Length; i++)
					{
						if (equation[i] == 'x')
							tempEquation.Append('(').Append(ConvertToString(variable)).Append(')');
						else if (equation[i] == 'π')
							tempEquation.Append('(').Append(ConvertToString(PI)).Append(')');
						else if (equation[i] == 'e')
							tempEquation.Append('(').Append(ConvertToString(E)).Append(')');
						else
							tempEquation.Append(equation[i]);
					}
					if (!equation.Equals(tempEquation.ToString()))
					{
						equation = tempEquation.ToString();
						if (printSolvingSteps)
							PrintASolvingStep(SOLVING_STEP__ASSIGNING_THE_VARIABLE_AND_CONSTANTS, equation, false, ConsoleColor.White, ConsoleColor.Gray, scopeLevel);
					}
				}
				catch (Exception e)
				{
					if (printSolvingSteps)
						PrintASolvingStep(SOLVING_STEP__ASSIGNING_THE_VARIABLE_AND_CONSTANTS, equation, true, ConsoleColor.White, ConsoleColor.Gray, scopeLevel);
					throw e;
				}
				
				// Solving functions ('abs', 'log', 'ln', 'sin', 'cos', 'tan'):
				try
				{
					tempEquation.Clear();
					for (int i = 0; i < equation.Length; i++)
					{
						if (equation[i] == 'a' && equation[i + 1] == 'b' && equation[i + 2] == 's')
						{
							int functionArgumentEndIndex = equation.Length - 1;
							bool isFunctionArgumentHasParentheses = (equation[i + 3] == '(');
							for (int j = i + 3; j < equation.Length; j++)
							{
								if ((isFunctionArgumentHasParentheses && equation[j] == ')') || (!isFunctionArgumentHasParentheses && !IsDigitOrDecimalPointOrInfinitySign(equation[j])))
								{
									functionArgumentEndIndex = j - 1;
									break;
								}
							}
							string functionArgumentString = Substring(equation, i + 2 + (isFunctionArgumentHasParentheses ? 2 : 1), functionArgumentEndIndex);
							double functionArgument = ConvertToDouble(functionArgumentString);
							tempEquation.Clear();
							tempEquation.Append(Substring(equation, 0, i - 1));
							double functionResult = Math.Abs(functionArgument);
							if (double.IsNaN(functionResult))
								throw new Exception(ERROR__NEGATIVE_NUMBER_IN_SQUARE_ROOT);
							if (double.IsPositiveInfinity(functionResult))
								tempEquation.Append("(∞)");
							else if (double.IsNegativeInfinity(functionResult))
								tempEquation.Append("(-∞)");
							else
								tempEquation.Append('(').Append(ConvertToString(functionResult)).Append(')');
							tempEquation.Append(Substring(equation, i + 3 + (isFunctionArgumentHasParentheses ? functionArgumentString.Length + 2 : functionArgumentString.Length), equation.Length - 1));
							equation = tempEquation.ToString();
							i -= 1;
						}
						else if (equation[i] == 'l' && equation[i + 1] == 'o' && equation[i + 2] == 'g')
						{
							int functionArgumentEndIndex = equation.Length - 1;
							bool isFunctionArgumentHasParentheses = (equation[i + 3] == '(');
							for (int j = i + 3; j < equation.Length; j++)
							{
								if ((isFunctionArgumentHasParentheses && equation[j] == ')') || (!isFunctionArgumentHasParentheses && !IsDigitOrDecimalPointOrInfinitySign(equation[j])))
								{
									functionArgumentEndIndex = j - 1;
									break;
								}
							}
							string functionArgumentString = Substring(equation, i + 2 + (isFunctionArgumentHasParentheses ? 2 : 1), functionArgumentEndIndex);
							double functionArgument = ConvertToDouble(functionArgumentString);
							tempEquation.Clear();
							tempEquation.Append(Substring(equation, 0, i - 1));
							double functionResult = Math.Log(functionArgument, 10);
							if (double.IsNaN(functionResult))
								throw new Exception(ERROR__NEGATIVE_NUMBER_IN_LOG_FUNCTION);
							if (double.IsPositiveInfinity(functionResult))
								tempEquation.Append("(∞)");
							else if (double.IsNegativeInfinity(functionResult))
								tempEquation.Append("(-∞)");
							else
								tempEquation.Append('(').Append(ConvertToString(functionResult)).Append(')');
							tempEquation.Append(Substring(equation, i + 3 + (isFunctionArgumentHasParentheses ? functionArgumentString.Length + 2 : functionArgumentString.Length), equation.Length - 1));
							equation = tempEquation.ToString();
							i -= 1;
						}
						else if (equation[i] == 'l' && equation[i + 1] == 'n')
						{
							int functionArgumentEndIndex = equation.Length - 1;
							bool isFunctionArgumentHasParentheses = (equation[i + 2] == '(');
							for (int j = i + 2; j < equation.Length; j++)
							{
								if ((isFunctionArgumentHasParentheses && equation[j] == ')') || (!isFunctionArgumentHasParentheses && !IsDigitOrDecimalPointOrInfinitySign(equation[j])))
								{
									functionArgumentEndIndex = j - 1;
									break;
								}
							}
							string functionArgumentString = Substring(equation, i + 1 + (isFunctionArgumentHasParentheses ? 2 : 1), functionArgumentEndIndex);
							double functionArgument = ConvertToDouble(functionArgumentString);
							tempEquation.Clear();
							tempEquation.Append(Substring(equation, 0, i - 1));
							double functionResult = Math.Log(functionArgument, Math.E);
							if (double.IsNaN(functionResult))
								throw new Exception(ERROR__NEGATIVE_NUMBER_IN_LN_FUNCTION);
							if (double.IsPositiveInfinity(functionResult))
								tempEquation.Append("(∞)");
							else if (double.IsNegativeInfinity(functionResult))
								tempEquation.Append("(-∞)");
							else
								tempEquation.Append('(').Append(ConvertToString(functionResult)).Append(')');
							tempEquation.Append(Substring(equation, i + 2 + (isFunctionArgumentHasParentheses ? functionArgumentString.Length + 2 : functionArgumentString.Length), equation.Length - 1));
							equation = tempEquation.ToString();
							i -= 1;
						}
						else if (equation[i] == 's' && equation[i + 1] == 'i' && equation[i + 2] == 'n')
						{
							int functionArgumentEndIndex = equation.Length - 1;
							bool isFunctionArgumentHasParentheses = (equation[i + 3] == '(');
							for (int j = i + 3; j < equation.Length; j++)
							{
								if ((isFunctionArgumentHasParentheses && equation[j] == ')') || (!isFunctionArgumentHasParentheses && !IsDigitOrDecimalPointOrInfinitySign(equation[j])))
								{
									functionArgumentEndIndex = j - 1;
									break;
								}
							}
							string functionArgumentString = Substring(equation, i + 2 + (isFunctionArgumentHasParentheses ? 2 : 1), functionArgumentEndIndex);
							double functionArgument = ConvertToDouble(functionArgumentString);
							tempEquation.Clear();
							tempEquation.Append(Substring(equation, 0, i - 1));
							double functionResult;
							if (useDegreesInsteadOfRadians)
							{
								double functionArgumentModulo180 = functionArgument % 180;
								double functionArgumentModulo360 = functionArgument % 360;
								if (functionArgumentModulo180 == 0)
									functionResult = 0;
								else if (functionArgumentModulo360 == 90)
									functionResult = 1;
								else if (functionArgumentModulo360 == -90)
									functionResult = -1;
								else if (functionArgumentModulo360 == 270)
									functionResult = -1;
								else if (functionArgumentModulo360 == -270)
									functionResult = 1;
								else
									functionResult = Math.Sin(functionArgument / PI_DIVIDED_FROM_180);
							}
							else
							{
								double functionArgumentModuloPI = functionArgument % PI;
								double functionArgumentModulo2PI = functionArgument % TWO_PI;
								if (functionArgumentModuloPI == 0)
									functionResult = 0;
								else if (functionArgumentModulo2PI == HALF_PI)
									functionResult = 1;
								else if (functionArgumentModulo2PI == -HALF_PI)
									functionResult = -1;
								else if (functionArgumentModulo2PI == ONE_AND_HALF_PI)
									functionResult = -1;
								else if (functionArgumentModulo2PI == -ONE_AND_HALF_PI)
									functionResult = 1;
								else
									functionResult = Math.Sin(functionArgument);
							}
							if (double.IsNaN(functionResult))
								throw new Exception(ERROR__INFINITY_IN_SIN_FUNCTION);
							if (double.IsPositiveInfinity(functionResult))
								tempEquation.Append("(∞)");
							else if (double.IsNegativeInfinity(functionResult))
								tempEquation.Append("(-∞)");
							else
								tempEquation.Append('(').Append(ConvertToString(functionResult)).Append(')');
							tempEquation.Append(Substring(equation, i + 3 + (isFunctionArgumentHasParentheses ? functionArgumentString.Length + 2 : functionArgumentString.Length), equation.Length - 1));
							equation = tempEquation.ToString();
							i -= 1;
						}
						else if (equation[i] == 'c' && equation[i + 1] == 'o' && equation[i + 2] == 's')
						{
							int functionArgumentEndIndex = equation.Length - 1;
							bool isFunctionArgumentHasParentheses = (equation[i + 3] == '(');
							for (int j = i + 3; j < equation.Length; j++)
							{
								if ((isFunctionArgumentHasParentheses && equation[j] == ')') || (!isFunctionArgumentHasParentheses && !IsDigitOrDecimalPointOrInfinitySign(equation[j])))
								{
									functionArgumentEndIndex = j - 1;
									break;
								}
							}
							string functionArgumentString = Substring(equation, i + 2 + (isFunctionArgumentHasParentheses ? 2 : 1), functionArgumentEndIndex);
							double functionArgument = ConvertToDouble(functionArgumentString);
							tempEquation.Clear();
							tempEquation.Append(Substring(equation, 0, i - 1));
							double functionResult;
							if (useDegreesInsteadOfRadians)
							{
								double functionArgumentModulo180 = functionArgument % 180;
								double functionArgumentModulo360 = functionArgument % 360;
								if (functionArgumentModulo180 == 90)
									functionResult = 0;
								else if (functionArgumentModulo180 == -90)
									functionResult = 0;
								else if (functionArgumentModulo360 == 180)
									functionResult = -1;
								else if (functionArgumentModulo360 == -180)
									functionResult = -1;
								else if (functionArgumentModulo360 == 0)
									functionResult = 1;
								else
									functionResult = Math.Cos(functionArgument / PI_DIVIDED_FROM_180);
							}
							else
							{
								double functionArgumentModuloPI = functionArgument % PI;
								double functionArgumentModulo2PI = functionArgument % TWO_PI;
								if (functionArgumentModuloPI == HALF_PI)
									functionResult = 0;
								else if (functionArgumentModuloPI == -HALF_PI)
									functionResult = 0;
								else if (functionArgumentModulo2PI == PI)
									functionResult = -1;
								else if (functionArgumentModulo2PI == -PI)
									functionResult = -1;
								else if (functionArgumentModulo2PI == 0)
									functionResult = 1;
								else
									functionResult = Math.Cos(functionArgument);
							}
							if (double.IsNaN(functionResult))
								throw new Exception(ERROR__INFINITY_IN_COS_FUNCTION);
							if (double.IsPositiveInfinity(functionResult))
								tempEquation.Append("(∞)");
							else if (double.IsNegativeInfinity(functionResult))
								tempEquation.Append("(-∞)");
							else
								tempEquation.Append('(').Append(ConvertToString(functionResult)).Append(')');
							tempEquation.Append(Substring(equation, i + 3 + (isFunctionArgumentHasParentheses ? functionArgumentString.Length + 2 : functionArgumentString.Length), equation.Length - 1));
							equation = tempEquation.ToString();
							i -= 1;
						}
						else if (equation[i] == 't' && equation[i + 1] == 'a' && equation[i + 2] == 'n')
						{
							int functionArgumentEndIndex = equation.Length - 1;
							bool isFunctionArgumentHasParentheses = (equation[i + 3] == '(');
							for (int j = i + 3; j < equation.Length; j++)
							{
								if ((isFunctionArgumentHasParentheses && equation[j] == ')') || (!isFunctionArgumentHasParentheses && !IsDigitOrDecimalPointOrInfinitySign(equation[j])))
								{
									functionArgumentEndIndex = j - 1;
									break;
								}
							}
							string functionArgumentString = Substring(equation, i + 2 + (isFunctionArgumentHasParentheses ? 2 : 1), functionArgumentEndIndex);
							double functionArgument = ConvertToDouble(functionArgumentString);
							tempEquation.Clear();
							tempEquation.Append(Substring(equation, 0, i - 1));
							double functionResult;
							if (useDegreesInsteadOfRadians)
							{
								double functionArgumentModulo180 = functionArgument % 180;
								if (functionArgumentModulo180 == 0)
									functionResult = 0;
								else if (functionArgumentModulo180 == 45)
									functionResult = 1;
								else if (functionArgumentModulo180 == -45)
									functionResult = -1;
								else if (functionArgumentModulo180 == 90)
									functionResult = double.PositiveInfinity;
								else if (functionArgumentModulo180 == -90)
									functionResult = double.NegativeInfinity;
								else
									functionResult = Math.Tan(functionArgument / PI_DIVIDED_FROM_180);
							}
							else
							{
								double functionArgumentModuloPI = functionArgument % PI;
								if (functionArgumentModuloPI == 0)
									functionResult = 0;
								else if (functionArgumentModuloPI == QUARTER_PI)
									functionResult = 1;
								else if (functionArgumentModuloPI == -QUARTER_PI)
									functionResult = -1;
								else if (functionArgumentModuloPI == HALF_PI)
									functionResult = double.PositiveInfinity;
								else if (functionArgumentModuloPI == -HALF_PI)
									functionResult = double.NegativeInfinity;
								else
									functionResult = Math.Tan(functionArgument);
							}
							if (double.IsNaN(functionResult))
								throw new Exception(ERROR__INFINITY_IN_TAN_FUNCTION);
							if (double.IsPositiveInfinity(functionResult))
								tempEquation.Append("(∞)");
							else if (double.IsNegativeInfinity(functionResult))
								tempEquation.Append("(-∞)");
							else
								tempEquation.Append('(').Append(ConvertToString(functionResult)).Append(')');
							tempEquation.Append(Substring(equation, i + 3 + (isFunctionArgumentHasParentheses ? functionArgumentString.Length + 2 : functionArgumentString.Length), equation.Length - 1));
							equation = tempEquation.ToString();
							i -= 1;
						}
					}
					if (!tempEquation.ToString().Equals(""))
					{
						if (printSolvingSteps)
							PrintASolvingStep(SOLVING_STEP__SOLVING_FUNCTIONS, equation, false, ConsoleColor.White, ConsoleColor.Gray, scopeLevel);
					}
				}
				catch (Exception e)
				{
					if (printSolvingSteps)
						PrintASolvingStep(SOLVING_STEP__SOLVING_FUNCTIONS, equation, true, ConsoleColor.White, ConsoleColor.Gray, scopeLevel);
					throw e;
				}
				
				// Solving square-roots:
				try
				{
					tempEquation.Clear();
					for (int i = equation.Length - 1; i >= 0; i--)
					{
						if (equation[i] == '√')
						{
							int squareRootNumberEndIndex = equation.Length - 1;
							bool isSquareRootNumberHasParentheses = (equation[i + 1] == '(');
							for (int j = i + 1; j < equation.Length; j++)
							{
								if ((isSquareRootNumberHasParentheses && equation[j] == ')') || (!isSquareRootNumberHasParentheses && !IsDigitOrDecimalPointOrInfinitySign(equation[j])))
								{
									squareRootNumberEndIndex = j - 1;
									break;
								}
							}
							string squareRootNumberString = Substring(equation, i + (isSquareRootNumberHasParentheses ? 2 : 1), squareRootNumberEndIndex);
							double squareRootNumber = ConvertToDouble(squareRootNumberString);
							tempEquation.Clear();
							tempEquation.Append(Substring(equation, 0, i - 1));
							double squareRootResult = Math.Sqrt(squareRootNumber);
							if (double.IsNaN(squareRootResult))
								throw new Exception(ERROR__NEGATIVE_NUMBER_IN_SQUARE_ROOT);
							if (double.IsPositiveInfinity(squareRootResult))
								tempEquation.Append("(∞)");
							else if (double.IsNegativeInfinity(squareRootResult))
								tempEquation.Append("(-∞)");
							else
								tempEquation.Append('(').Append(ConvertToString(squareRootResult)).Append(')');
							tempEquation.Append(Substring(equation, i + 1 + (isSquareRootNumberHasParentheses ? squareRootNumberString.Length + 2 : squareRootNumberString.Length), equation.Length - 1));
							equation = tempEquation.ToString();
						}
					}
					if (!tempEquation.ToString().Equals(""))
					{
						if (printSolvingSteps)
							PrintASolvingStep(SOLVING_STEP__SOLVING_SQUARE_ROOTS, equation, false, ConsoleColor.White, ConsoleColor.Gray, scopeLevel);
					}
				}
				catch (Exception e)
				{
					if (printSolvingSteps)
						PrintASolvingStep(SOLVING_STEP__SOLVING_SQUARE_ROOTS, equation, true, ConsoleColor.White, ConsoleColor.Gray, scopeLevel);
					throw e;
				}
				
				// Solving powers:
				try
				{
					tempEquation.Clear();
					for (int i = equation.Length - 1; i >= 0; i--)
					{
						if (equation[i] == '^')
						{
							int powerBaseStartIndex = 0, powerExponentEndIndex = equation.Length - 1;
							bool isPowerBaseHasParentheses = (equation[i - 1] == ')');
							bool isPowerExponentHasParentheses = (equation[i + 1] == '(');
							for (int j = i - 1; j >= 0; j--)
							{
								if ((isPowerBaseHasParentheses && equation[j] == '(') || (!isPowerBaseHasParentheses && !IsDigitOrDecimalPointOrInfinitySign(equation[j])))
								{
									powerBaseStartIndex = j + 1;
									break;
								}
							}
							for (int j = i + 1; j < equation.Length; j++)
							{
								if ((isPowerExponentHasParentheses && equation[j] == ')') || (!isPowerExponentHasParentheses && !IsDigitOrDecimalPointOrInfinitySign(equation[j])))
								{
									powerExponentEndIndex = j - 1;
									break;
								}
							}
							string powerBaseString = Substring(equation, powerBaseStartIndex, i - (isPowerBaseHasParentheses ? 2 : 1));
							string powerExponentString = Substring(equation, i + (isPowerExponentHasParentheses ? 2 : 1), powerExponentEndIndex);
							double powerBase = ConvertToDouble(powerBaseString);
							double powerExponent = ConvertToDouble(powerExponentString);
							tempEquation.Clear();
							tempEquation.Append(Substring(equation, 0, i - 1 - (isPowerBaseHasParentheses ? powerBaseString.Length + 2 : powerBaseString.Length)));
							double powerResult = Math.Pow(powerBase, powerExponent);
							if (double.IsNaN(powerResult))
								throw new Exception(ERROR__NON_WHOLE_EXPONENT_IN_POWER_ON_NEGATIVE_NUMBER);
							if (double.IsPositiveInfinity(powerResult))
								tempEquation.Append("(∞)");
							else if (double.IsNegativeInfinity(powerResult))
								tempEquation.Append("(-∞)");
							else
								tempEquation.Append('(').Append(ConvertToString(powerResult)).Append(')');
							tempEquation.Append(Substring(equation, i + 1 + (isPowerExponentHasParentheses ? powerExponentString.Length + 2 : powerExponentString.Length), equation.Length - 1));
							equation = tempEquation.ToString();
							i -= (isPowerBaseHasParentheses ? powerBaseString.Length + 2 : powerBaseString.Length);
						}
					}
					if (!tempEquation.ToString().Equals(""))
					{
						if (printSolvingSteps)
							PrintASolvingStep(SOLVING_STEP__SOLVING_POWERS, equation, false, ConsoleColor.White, ConsoleColor.Gray, scopeLevel);
					}
				}
				catch (Exception e)
				{
					if (printSolvingSteps)
						PrintASolvingStep(SOLVING_STEP__SOLVING_POWERS, equation, true, ConsoleColor.White, ConsoleColor.Gray, scopeLevel);
					throw e;
				}
				
				// Deleting all the parentheses from the equation (because they are no longer needed):
				try
				{
					tempEquation.Clear();
					for (int i = 0; i < equation.Length; i++)
						if (equation[i] != '(' && equation[i] != ')')
							tempEquation.Append(equation[i]);
					if (!equation.Equals(tempEquation.ToString()))
					{
						equation = tempEquation.ToString();
						if (printSolvingSteps)
							PrintASolvingStep(SOLVING_STEP__DELETING_ALL_THE_PARENTHESES_FROM_THE_EQUATION, equation, false, ConsoleColor.White, ConsoleColor.Gray, scopeLevel);
					}
				}
				catch (Exception e)
				{
					if (printSolvingSteps)
						PrintASolvingStep(SOLVING_STEP__DELETING_ALL_THE_PARENTHESES_FROM_THE_EQUATION, equation, true, ConsoleColor.White, ConsoleColor.Gray, scopeLevel);
					throw e;
				}
				
				// Adding addition sign before subtraction signs (that are not a negative sign):
				try
				{
					tempEquation.Clear();
					for (int i = 0; i < equation.Length; i++)
					{
						if (i > 0)
						{
							if (equation[i] == '-' && IsDigitOrDecimalPointOrInfinitySign(equation[i - 1]))
								tempEquation.Append("+-");
							else
								tempEquation.Append(equation[i]);
						}
						else
							tempEquation.Append(equation[i]);
					}
					if (!equation.Equals(tempEquation.ToString()))
					{
						equation = tempEquation.ToString();
						if (printSolvingSteps)
							PrintASolvingStep(SOLVING_STEP__ADDING_ADDITION_SIGN_BEFORE_SUBTRACTION_SIGNS, equation, false, ConsoleColor.White, ConsoleColor.Gray, scopeLevel);
					}
				}
				catch (Exception e)
				{
					if (printSolvingSteps)
						PrintASolvingStep(SOLVING_STEP__ADDING_ADDITION_SIGN_BEFORE_SUBTRACTION_SIGNS, equation, true, ConsoleColor.White, ConsoleColor.Gray, scopeLevel);
					throw e;
				}
				
				// Deleting double subtraction signs (because it means that the number is positive):
				try
				{
					tempEquation.Clear();
					for (int i = 0; i < equation.Length; i++)
					{
						if (i < equation.Length - 1)
						{
							if (equation[i] == '-' && equation[i + 1] == '-')
								i++;
							else
								tempEquation.Append(equation[i]);
						}
						else
							tempEquation.Append(equation[i]);
					}
					if (!equation.Equals(tempEquation.ToString()))
					{
						equation = tempEquation.ToString();
						if (printSolvingSteps)
							PrintASolvingStep(SOLVING_STEP__DELETING_DOUBLE_SUBTRACTION_SIGNS, equation, false, ConsoleColor.White, ConsoleColor.Gray, scopeLevel);
					}
				}
				catch (Exception e)
				{
					if (printSolvingSteps)
						PrintASolvingStep(SOLVING_STEP__DELETING_DOUBLE_SUBTRACTION_SIGNS, equation, true, ConsoleColor.White, ConsoleColor.Gray, scopeLevel);
					throw e;
				}
				
				// Solving multiplications and divisions:
				try
				{
					tempEquation.Clear();
					for (int i = 0; i < equation.Length; i++)
					{
						if (equation[i] == '*' || equation[i] == '/')
						{
							int leftNumberStartIndex = 0, rightNumberEndIndex = equation.Length - 1;
							for (int j = i - 1; j >= 0; j--)
							{
								if (!IsDigitOrDecimalPointOrInfinitySign(equation[j]))
								{
									if (equation[j] == '-')
										leftNumberStartIndex = j;
									else
										leftNumberStartIndex = j + 1;
									break;
								}
							}
							for (int j = i + 1; j < equation.Length; j++)
							{
								if (!(IsDigitOrDecimalPointOrInfinitySign(equation[j]) || equation[j] == '-' && j == i + 1))
								{
									rightNumberEndIndex = j - 1;
									break;
								}
							}
							string leftNumberString = Substring(equation, leftNumberStartIndex, i - 1);
							string rightNumberString = Substring(equation, i + 1, rightNumberEndIndex);
							double leftNumber = ConvertToDouble(leftNumberString);
							double rightNumber = ConvertToDouble(rightNumberString);
							tempEquation.Clear();
							tempEquation.Append(Substring(equation, 0, i - 1 - leftNumberString.Length));
							if (equation[i] == '*')
							{
								double multiplicationResult = leftNumber * rightNumber;
								if (double.IsPositiveInfinity(multiplicationResult))
									tempEquation.Append("∞");
								else if (double.IsNegativeInfinity(multiplicationResult))
									tempEquation.Append("-∞");
								else
									tempEquation.Append(ConvertToString(multiplicationResult));
							}
							else
							{
								double divisionResult = leftNumber / rightNumber;
								if (double.IsNaN(divisionResult))
								{
									if ((leftNumber == double.PositiveInfinity || leftNumber == double.NegativeInfinity) && (rightNumber == double.PositiveInfinity || rightNumber == double.NegativeInfinity))
										throw new Exception(ERROR__INFINITY_DIVIDED_BY_INFINITY);
									else
										throw new Exception(ERROR__ZERO_DIVIDED_BY_ZERO);
								}
								if (double.IsPositiveInfinity(divisionResult))
									tempEquation.Append("∞");
								else if (double.IsNegativeInfinity(divisionResult))
									tempEquation.Append("-∞");
								else
									tempEquation.Append(ConvertToString(divisionResult));
							}
							tempEquation.Append(Substring(equation, i + 1 + rightNumberString.Length, equation.Length - 1));
							equation = tempEquation.ToString();
							i -= (leftNumberString.Length + 1);
						}
					}
					if (!tempEquation.ToString().Equals(""))
					{
						if (printSolvingSteps)
							PrintASolvingStep(SOLVING_STEP__SOLVING_MULTIPLICATIONS_AND_DIVISIONS, equation, false, ConsoleColor.White, ConsoleColor.Gray, scopeLevel);
					}
				}
				catch (Exception e)
				{
					if (printSolvingSteps)
						PrintASolvingStep(SOLVING_STEP__SOLVING_MULTIPLICATIONS_AND_DIVISIONS, equation, true, ConsoleColor.White, ConsoleColor.Gray, scopeLevel);
					throw e;
				}
				
				// Solving additions:
				try
				{
					tempEquation.Clear();
					for (int i = 0; i < equation.Length; i++)
					{
						if (equation[i] == '+')
						{
							int leftNumberStartIndex = 0, rightNumberEndIndex = equation.Length - 1;
							for (int j = i - 1; j >= 0; j--)
							{
								if (!IsDigitOrDecimalPointOrInfinitySign(equation[j]))
								{
									if (equation[j] == '-')
										leftNumberStartIndex = j;
									else
										leftNumberStartIndex = j + 1;
									break;
								}
							}
							for (int j = i + 1; j < equation.Length; j++)
							{
								if (!(IsDigitOrDecimalPointOrInfinitySign(equation[j]) || equation[j] == '-' && j == i + 1))
								{
									rightNumberEndIndex = j - 1;
									break;
								}
							}
							string leftNumberString = Substring(equation, leftNumberStartIndex, i - 1);
							string rightNumberString = Substring(equation, i + 1, rightNumberEndIndex);
							double leftNumber = ConvertToDouble(leftNumberString);
							double rightNumber = ConvertToDouble(rightNumberString);
							tempEquation.Clear();
							tempEquation.Append(Substring(equation, 0, i - 1 - leftNumberString.Length));
							double additionResult = leftNumber + rightNumber;
							if (double.IsNaN(additionResult))
								throw new Exception(ERROR__INFINITY_SUBTRACTED_FROM_INFINITY);
							if (double.IsPositiveInfinity(additionResult))
								tempEquation.Append("∞");
							else if (double.IsNegativeInfinity(additionResult))
								tempEquation.Append("-∞");
							else
								tempEquation.Append(ConvertToString(additionResult));
							tempEquation.Append(Substring(equation, i + 1 + rightNumberString.Length, equation.Length - 1));
							equation = tempEquation.ToString();
							i -= (leftNumberString.Length + 1);
						}
					}
					if (!tempEquation.ToString().Equals(""))
					{
						if (printSolvingSteps)
							PrintASolvingStep(SOLVING_STEP__SOLVING_ADDITIONS, equation, false, ConsoleColor.White, ConsoleColor.Gray, scopeLevel);
					}
				}
				catch (Exception e)
				{
					if (printSolvingSteps)
						PrintASolvingStep(SOLVING_STEP__SOLVING_ADDITIONS, equation, true, ConsoleColor.White, ConsoleColor.Gray, scopeLevel);
					throw e;
				}
				
				if (printSolvingSteps)
					PrintASolvingStep(SOLVING_STEP__EQUATION_RESULT, equation, false, ConsoleColor.Green, ConsoleColor.Gray, scopeLevel);
			}
			catch (Exception e)
			{
				if (printSolvingSteps)
					PrintASolvingStep(SOLVING_STEP__EQUATION_RESULT, equation, true, ConsoleColor.Green, ConsoleColor.Gray, scopeLevel);
				throw e;
			}
			
			return ConvertToDouble(equation);
		}
		private static bool IsDigitOrDecimalPointOrInfinitySign(char character)
		{
			return (character >= '0' && character <= '9') || character == '.' || character == '∞';
		}
		private static bool IsArithmeticOpearionSign(char character)
		{
			return character == '+' || character == '-' || character == '*' || character == '/';
		}
		private static bool IsNumber(string numberString)
		{
			for (int i = 0; i < numberString.Length; i++)
				if (!(IsDigitOrDecimalPointOrInfinitySign(numberString[i]) || (numberString[i] == '-' && i == 0)))
					return false;
			return true;
		}
		private static bool IsEquationValid(string equation)
		{
			// Checking if equation is not empty:
			if (equation.Length == 0)
				return false;

			// Checking if all characters are valid:
			for (int i = 0; i < equation.Length; i++)
			{
				if (!IsDigitOrDecimalPointOrInfinitySign(equation[i]))
				{
					if (!IsArithmeticOpearionSign(equation[i]))
					{
						if (!(equation[i] == '(' || equation[i] == ')'))
						{
							if (!(equation[i] == '^' || equation[i] == '√'))
							{
								if (!(equation[i] == 'x' || equation[i] == 'π' || equation[i] == 'e'))
								{
									if (i <= equation.Length - 3)
									{
										if (!(equation[i] == 'l' && equation[i + 1] == 'n'))
										{
											if (!(equation[i] == 'a' && equation[i + 1] == 'b' && equation[i + 2] == 's'))
												if (!(equation[i] == 'l' && equation[i + 1] == 'o' && equation[i + 2] == 'g'))
													if (!(equation[i] == 's' && equation[i + 1] == 'i' && equation[i + 2] == 'n'))
														if (!(equation[i] == 'c' && equation[i + 1] == 'o' && equation[i + 2] == 's'))
															if (!(equation[i] == 't' && equation[i + 1] == 'a' && equation[i + 2] == 'n'))
																return false;
											i += 2;
										}
										i++;
									}
									else
										return false;
								}
							}
						}
					}
				}
			}

			// Checking if for each opening parentheses there is a closing parentheses:
			int parenthesesNestingLevel = 0;
			for (int i = 0; i < equation.Length; i++)
			{
				if (equation[i] == '(')
					parenthesesNestingLevel++;
				else if (equation[i] == ')')
				{
					if (parenthesesNestingLevel == 0)
						return false;
					parenthesesNestingLevel--;
				}
			}
			if (parenthesesNestingLevel != 0)
				return false;
			
			// Checking if there are not empty equations inside parentheses:
			for (int i = 0; i < equation.Length - 1; i++)
				if (equation[i] == '(' && equation[i + 1] == ')')
					return false;
			
			// Checking if for each function there is a valid argument:
			for (int i = 0; i < equation.Length; i++)
			{
				if (i <= equation.Length - 3)
				{
					if ((equation[i] == 'a' && equation[i + 1] == 'b' && equation[i + 2] == 's') ||
					   (equation[i] == 'l' && equation[i + 1] == 'o' && equation[i + 2] == 'g') ||
					   (equation[i] == 's' && equation[i + 1] == 'i' && equation[i + 2] == 'n') ||
					   (equation[i] == 'c' && equation[i + 1] == 'o' && equation[i + 2] == 's') ||
					   (equation[i] == 't' && equation[i + 1] == 'a' && equation[i + 2] == 'n'))
					{
						if (equation.Length - i == 3)
							return false;
						if (!(IsDigitOrDecimalPointOrInfinitySign(equation[i + 3]) || equation[i + 3] == 'x' || equation[i + 3] == 'π' || equation[i + 3] == 'e' || equation[i + 3] == '('))
							return false;
					}
				}
				if (i <= equation.Length - 2)
				{
					if (equation[i] == 'l' && equation[i + 1] == 'n')
					{
						if (equation.Length - i == 2)
							return false;
						if (!(IsDigitOrDecimalPointOrInfinitySign(equation[i + 2]) || equation[i + 2] == 'x' || equation[i + 2] == 'π' || equation[i + 2] == 'e' || equation[i + 2] == '('))
							return false;
					}
				}
			}

			// Checking if all powers and square-roots have valid surrounding characters:
			for (int i = 0; i < equation.Length; i++)
			{
				if (i == 0 && equation[i] == '^')
					return false;
				else if (i == equation.Length - 1)
				{
					if (equation[i] == '^' || equation[i] == '√')
						return false;
				}
				else
				{
					if (equation[i] == '^')
					{
						if (!(IsDigitOrDecimalPointOrInfinitySign(equation[i - 1]) || equation[i - 1] == 'x' || equation[i - 1] == 'π' || equation[i - 1] == 'e' || equation[i - 1] == ')'))
							return false;
						if (!(IsDigitOrDecimalPointOrInfinitySign(equation[i + 1]) || equation[i + 1] == 'x' || equation[i + 1] == 'π' || equation[i + 1] == 'e' || equation[i + 1] == '(' || equation[i + 1] == '√' || equation[i + 1] == 'a' || equation[i + 1] == 'l' || equation[i + 1] == 's' || equation[i + 1] == 'c' || equation[i + 1] == 't'))
							return false;
					}
					else if (equation[i] == '√')
					{
						if (!(IsDigitOrDecimalPointOrInfinitySign(equation[i + 1]) || equation[i + 1] == 'x' || equation[i + 1] == 'π' || equation[i + 1] == 'e' || equation[i + 1] == '(' || equation[i + 1] == '√' || equation[i + 1] == 'a' || equation[i + 1] == 'l' || equation[i + 1] == 's' || equation[i + 1] == 'c' || equation[i + 1] == 't'))
							return false;
					}
				}
			}
			
			// Checking if all arithmetic operations signs are surrounded by characters (and the characters are valid):
			for (int i = 0; i < equation.Length; i++)
			{
				if (i == 0)
				{
					if (IsArithmeticOpearionSign(equation[i]) && equation[i] != '-')
						return false;
				}
				else if (i == equation.Length - 1)
				{
					if (IsArithmeticOpearionSign(equation[i]))
						return false;
				}
				else
				{
					if (IsArithmeticOpearionSign(equation[i]) && equation[i] != '-')
					{
						if (!(IsDigitOrDecimalPointOrInfinitySign(equation[i - 1]) || equation[i - 1] == 'x' || equation[i - 1] == 'π' || equation[i - 1] == 'e' || equation[i - 1] == ')') ||
							!(IsDigitOrDecimalPointOrInfinitySign(equation[i + 1]) || equation[i + 1] == 'x' || equation[i + 1] == 'π' || equation[i + 1] == 'e' || equation[i + 1] == '(' || equation[i + 1] == '√' || equation[i + 1] == 'a' || equation[i + 1] == 'l' || equation[i + 1] == 's' || equation[i + 1] == 'c' || equation[i + 1] == 't'))
							return false;
					}
					else if (IsArithmeticOpearionSign(equation[i]) && equation[i] == '-' && equation[i - 1] == '-')
						return false;
				}
			}
			
			// Checking if for each decimal point there is a number from the left side and/or from the right side:
			for (int i = 0; i < equation.Length; i++)
			{
				if (equation[i] == '.')
				{
					if (i == 0)
					{
						if (equation.Length == 1)
							return false;
						if (!(equation[i + 1] >= '0' && equation[i + 1] <= '9'))
							return false;
					}
					else if (i == equation.Length - 1)
					{
						if (!(equation[i - 1] >= '0' && equation[i - 1] <= '9'))
							return false;
					}
					else
					{
						if (!((equation[i - 1] >= '0' && equation[i - 1] <= '9') || (equation[i + 1] >= '0' && equation[i + 1] <= '9')))
							return false;
					}
				}
			}
			
			// Checking if each number has maximum one decimal point:
			for (int i = 0; i < equation.Length; i++)
			{
				int numberSegmentStartIndex = 0, numberSegmentEndIndex = equation.Length - 1;
				if (equation[i] == '.')
				{
					for (int j = i - 1; j >= 0; j--)
					{
						if (!IsDigitOrDecimalPointOrInfinitySign(equation[j]))
						{
							numberSegmentStartIndex = j + 1;
							break;
						}
					}
					for (int j = i + 1; j < equation.Length; j++)
					{
						if (!IsDigitOrDecimalPointOrInfinitySign(equation[j]))
						{
							numberSegmentEndIndex = j - 1;
							break;
						}
					}
					int decimalPointsCount = 0;
					for (int j = numberSegmentStartIndex; j <= numberSegmentEndIndex; j++)
						if (equation[j] == '.')
							decimalPointsCount++;
					if (decimalPointsCount > 1)
						return false;
				}
			}
			
			return true;
		}
		private static string Substring(string str, int startIndex, int endIndex)
		{
			return endIndex >= startIndex ? str.Substring(startIndex, endIndex - startIndex + 1) : "";
		}
		private static double ConvertToDouble(string numberString)
		{
			if (numberString.Equals("∞"))
				return double.PositiveInfinity;
			if (numberString.Equals("-∞"))
				return double.NegativeInfinity;
			double number = 0;
			try
			{
				number = Convert.ToDouble(numberString);
			}
			catch (OverflowException e)
			{
				return numberString[0] == '-' ? double.NegativeInfinity : double.PositiveInfinity;
			}
			catch
			{
				return double.NaN;
			}
			return number;
		}
		private static string ConvertToString(double number)
		{
			if (number == double.PositiveInfinity)
				return "∞";
			if (number == double.NegativeInfinity)
				return "-∞";
			string numberString = number.ToString();
			bool isThereAnE = false;
			for (int i = numberString.Length - 1; i >= numberString.Length - 6 && i >= 0; i--)
			{
				if (numberString[i] == 'E')
				{
					isThereAnE = true;
					break;
				}
			}
			if (isThereAnE)
				return number.ToString(DOUBLE_FIXED_POINT_FORMAT);
			return number.ToString("r");
		}
		private static void PrintASolvingStep(string stepDescription, string stepResult, bool isThereAnError, ConsoleColor stepDescriptionForegroundColor, ConsoleColor stepResultForegroundColor, int scopeLevel)
		{
			for (int i = 0; i < scopeLevel; i++)
				Console.Write("    ");
			Console.ForegroundColor = stepDescriptionForegroundColor;
			Console.Write(stepDescription);
			Console.Write(' ');
			Console.ForegroundColor = stepResultForegroundColor;
			if (!isThereAnError)
				Console.WriteLine(stepResult);
			else
			{
				Console.Write(stepResult);
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Write(' ');
				Console.WriteLine(ERROR);
			}
			Console.ResetColor();
		}
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	}
}
