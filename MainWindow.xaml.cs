using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;

namespace Graph_Drawer
{
	public partial class MainWindow : Window
	{
		// Constants ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		public const int MAX_EQUATIONS_COUNT = 5;
		public const char EQUATIONS_DELIMITER = ';';
		public readonly Brush AXIS_COLOR = Brushes.Black;
		public readonly Brush[] EQUATIONS_COLORS = new Brush[] { Brushes.Red, Brushes.Green, Brushes.Blue, Brushes.Purple, Brushes.Yellow };
		public const int AXIS_INITIAL_ZOOM_AND_STEP_SIZE = 100;
		public const int GRAPH_NUMBERS_LINES_SIZE = 10;
		public const string GRAPH_NUMBERS_FONT = "Tahoma";
		public const int GRAPH_NUMBERS_FONT_SIZE = 20;
		public readonly Size GRAPH_NUMBERS_CHARACTER_SIZE_FOR_THE_SELECTED_FONT = new Size(6, 14); // Manually calculated size for one character in the selected font
		public readonly Size GRAPH_NUMBERS_TEXT_MARGIN_FOR_THE_SELECTED_FONT = new Size(15, 10); // Manually calculated margin for the text in selected font
		public const string ERROR = "Error!";
		public const string ERROR__ONE_OR_MORE_OF_THE_EQUATIONS_ARE_NOT_VALID = "One or more of the equations are not valid!";
		public const string ERROR__REACHED_MAX_EQUATIONS_COUNT = "You can only draw up to 5 equations!";
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



		// Variables ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		DrawingVisual drawingVisual;
		Point axisOriginLocation;
		Equation[] equations;
		bool draggingGraph;
		Point currentMouseLocation;
		double xZoomAmount, yZoomAmount;
		bool xAxisZoomLock, yAxisZoomLock;
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



		// Constructors ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		public MainWindow()
		{
			InitializeComponent();
			drawingVisual = new DrawingVisual();
			axisOriginLocation = new Point(canvas_graphArea.ActualWidth / 2, canvas_graphArea.ActualHeight / 2);
			equations = new Equation[MAX_EQUATIONS_COUNT];
			draggingGraph = false;
			currentMouseLocation = new Point(-1, -1);
			xZoomAmount = 100;
			yZoomAmount = 100;
			xAxisZoomLock = false;
			yAxisZoomLock = false;
		}
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



		// Events //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		private void OnWindowSizeChangedEvent(object sender, SizeChangedEventArgs e)
		{
			axisOriginLocation = new Point(canvas_graphArea.ActualWidth / 2, canvas_graphArea.ActualHeight / 2);
			RedrawAxisAndGraphs();
		}
		private void OnWindowKeyDownEvent(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.X:
					yAxisZoomLock = true;
					break;
				case Key.Y:
					xAxisZoomLock = true;
					break;
			}
		}
		private void OnWindowKeyUpEvent(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.X:
					yAxisZoomLock = false;
					break;
				case Key.Y:
					xAxisZoomLock = false;
					break;
			}
		}
		private void OnCanvasLoadedEvent(object sender, RoutedEventArgs e)
		{
			Canvas senderCanvas = (Canvas)sender;
			if (senderCanvas == canvas_graphArea)
				RedrawAxisAndGraphs();
		}
		private void OnCanvasMouseDownEvent(object sender, MouseButtonEventArgs e)
		{
			Canvas senderCanvas = (Canvas)sender;
			if (senderCanvas == canvas_graphArea)
			{
				if (e.LeftButton == MouseButtonState.Pressed)
				{
					currentMouseLocation = e.GetPosition(senderCanvas);
					draggingGraph = true;
				}
			}
		}
		private void OnCanvasMouseUpEvent(object sender, MouseButtonEventArgs e)
		{
			Canvas senderCanvas = (Canvas)sender;
			if (senderCanvas == canvas_graphArea)
				if (e.LeftButton == MouseButtonState.Released)
					draggingGraph = false;
		}
		private void OnCanvasMouseMoveEvent(object sender, MouseEventArgs e)
		{
			Canvas senderCanvas = (Canvas)sender;
			if (senderCanvas == canvas_graphArea)
			{
				if (draggingGraph)
				{
					double xDelta = e.GetPosition(senderCanvas).X - currentMouseLocation.X;
					double yDelta = e.GetPosition(senderCanvas).Y - currentMouseLocation.Y;
					axisOriginLocation = new Point(axisOriginLocation.X + xDelta, axisOriginLocation.Y + yDelta);
					currentMouseLocation = e.GetPosition(senderCanvas);
					RedrawAxisAndGraphs();
				}
			}
		}
		private void OnCanvasMouseWheelEvent(object sender, MouseWheelEventArgs e)
		{
			Canvas senderCanvas = (Canvas)sender;
			if (senderCanvas == canvas_graphArea)
			{
				if (e.Delta > 0)
				{
					axisOriginLocation = new Point(xAxisZoomLock ? axisOriginLocation.X : (axisOriginLocation.X - ((e.GetPosition(senderCanvas).X - axisOriginLocation.X) / xZoomAmount)), yAxisZoomLock ? axisOriginLocation.Y : (axisOriginLocation.Y - ((e.GetPosition(senderCanvas).Y - axisOriginLocation.Y) / yZoomAmount)));
					if (!xAxisZoomLock)
						xZoomAmount++;
					if (!yAxisZoomLock)
						yZoomAmount++;
				}
				else if (e.Delta < 0)
				{
					axisOriginLocation = new Point(xAxisZoomLock ? axisOriginLocation.X : (axisOriginLocation.X + ((e.GetPosition(senderCanvas).X - axisOriginLocation.X) / xZoomAmount)), yAxisZoomLock ? axisOriginLocation.Y : (axisOriginLocation.Y + ((e.GetPosition(senderCanvas).Y - axisOriginLocation.Y) / yZoomAmount)));
					if (!xAxisZoomLock)
						xZoomAmount--;
					if (!yAxisZoomLock)
						yZoomAmount--;
				}
				RedrawAxisAndGraphs();
			}
		}
		private void OnButtonClickEvent(object sender, RoutedEventArgs e)
		{
			Button senderButton = (Button)sender;
			if (senderButton == button_drawGraph)
				AttemptToDrawGraphs();
		}
		private void OnTextBoxKeyDownEvent(object sender, KeyEventArgs e)
		{
			TextBox senderTextBox = (TextBox)sender;
			if (senderTextBox == textBox_equation)
				if (e.Key == Key.Enter)
					AttemptToDrawGraphs();
		}
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



		// Methods /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		private void AttemptToDrawGraphs()
		{
			equations = new Equation[MAX_EQUATIONS_COUNT];
			string[] equationsStrings = textBox_equation.Text.Split(EQUATIONS_DELIMITER);
			if (equationsStrings.Length > MAX_EQUATIONS_COUNT)
			{
				RedrawAxisAndGraphs();
				MessageBox.Show(ERROR__REACHED_MAX_EQUATIONS_COUNT);
				return;
			}
			bool anEquationIsNotValid = false;
			for (int i = 0; i < equationsStrings.Length; i++)
			{
				try
				{
					equations[i] = new Equation(equationsStrings[i]);
				}
				catch (Exception ex)
				{
					equations[i] = null;
					anEquationIsNotValid = true;
				}
			}
			RedrawAxisAndGraphs();
			if (anEquationIsNotValid)
				MessageBox.Show(ERROR__ONE_OR_MORE_OF_THE_EQUATIONS_ARE_NOT_VALID, ERROR);
		}
		private void RedrawAxisAndGraphs()
		{
			canvas_graphArea.Children.Clear();
			canvas_graphArea.Children.Add(new DrawingVisualHost(drawingVisual));
			DrawingContext drawingContext = drawingVisual.RenderOpen();

			AddLineToCanvas(drawingContext, new Point(0, axisOriginLocation.Y), new Point(canvas_graphArea.ActualWidth, axisOriginLocation.Y), AXIS_COLOR, 2);
			AddLineToCanvas(drawingContext, new Point(axisOriginLocation.X, 0), new Point(axisOriginLocation.X, canvas_graphArea.ActualHeight), AXIS_COLOR, 2);
			int onScreenXNegativeNumbersLinesCount = (int)(axisOriginLocation.X / xZoomAmount) + 1;
			int onScreenXPositiveNumbersLinesCount = (int)((canvas_graphArea.ActualWidth - 1 - axisOriginLocation.X) / xZoomAmount) + 1;
			double xSteps = GetNearestBeautifulNumber((double)AXIS_INITIAL_ZOOM_AND_STEP_SIZE / xZoomAmount); // Make the 'x' axis steps amount to be on "beautiful" numbers (for example: 0.25, 0.5, 1, 2, 4, 5)
			double ySteps = GetNearestBeautifulNumber((double)AXIS_INITIAL_ZOOM_AND_STEP_SIZE / yZoomAmount); // Make the 'y' axis steps amount to be on "beautiful" numbers (for example: 0.25, 0.5, 1, 2, 4, 5)
			for (double i = 0; i < onScreenXNegativeNumbersLinesCount; i += xSteps)
			{
				AddLineToCanvas(
					drawingContext,
					new Point(axisOriginLocation.X - xZoomAmount * (i + xSteps), axisOriginLocation.Y - GRAPH_NUMBERS_LINES_SIZE),
					new Point(axisOriginLocation.X - xZoomAmount * (i + xSteps), axisOriginLocation.Y + GRAPH_NUMBERS_LINES_SIZE),
					Brushes.Black,
					2
				);
				string iString = Convert.ToString(-(i + xSteps));
				AddTextToCanvas(
					drawingContext,
					new Point(axisOriginLocation.X - xZoomAmount * (i + xSteps) - GRAPH_NUMBERS_CHARACTER_SIZE_FOR_THE_SELECTED_FONT.Width * iString.Length, axisOriginLocation.Y + GRAPH_NUMBERS_TEXT_MARGIN_FOR_THE_SELECTED_FONT.Height),
					Convert.ToString(iString),
					GRAPH_NUMBERS_FONT,
					GRAPH_NUMBERS_FONT_SIZE,
					Brushes.Black,
					2
				);
			}
			for (double i = 0; i < onScreenXPositiveNumbersLinesCount; i += xSteps)
			{
				AddLineToCanvas(
					drawingContext,
					new Point(axisOriginLocation.X + xZoomAmount * (i + xSteps), axisOriginLocation.Y - GRAPH_NUMBERS_LINES_SIZE),
					new Point(axisOriginLocation.X + xZoomAmount * (i + xSteps), axisOriginLocation.Y + GRAPH_NUMBERS_LINES_SIZE),
					Brushes.Black,
					2
				);
				string iString = Convert.ToString(i + xSteps);
				AddTextToCanvas(
					drawingContext,
					new Point(axisOriginLocation.X + xZoomAmount * (i + xSteps) - GRAPH_NUMBERS_CHARACTER_SIZE_FOR_THE_SELECTED_FONT.Width * iString.Length, axisOriginLocation.Y + GRAPH_NUMBERS_TEXT_MARGIN_FOR_THE_SELECTED_FONT.Height),
					Convert.ToString(iString),
					GRAPH_NUMBERS_FONT,
					GRAPH_NUMBERS_FONT_SIZE,
					Brushes.Black,
					2
				);
			}
			int onScreenYNegativeNumbersLinesCount = (int)((canvas_graphArea.ActualHeight - 1 - axisOriginLocation.Y) / yZoomAmount) + 1;
			int onScreenYPositiveNumbersLinesCount = (int)(axisOriginLocation.Y / yZoomAmount) + 1;
			for (double i = 0; i < onScreenYNegativeNumbersLinesCount; i += ySteps)
			{
				AddLineToCanvas(
					drawingContext,
					new Point(axisOriginLocation.X - GRAPH_NUMBERS_LINES_SIZE, axisOriginLocation.Y + yZoomAmount * (i + ySteps)),
					new Point(axisOriginLocation.X + GRAPH_NUMBERS_LINES_SIZE, axisOriginLocation.Y + yZoomAmount * (i + ySteps)),
					Brushes.Black,
					2
				);
				string iString = Convert.ToString(-(i + ySteps));
				AddTextToCanvas(
					drawingContext,
					new Point(axisOriginLocation.X + GRAPH_NUMBERS_TEXT_MARGIN_FOR_THE_SELECTED_FONT.Width, axisOriginLocation.Y + yZoomAmount * (i + ySteps) - GRAPH_NUMBERS_CHARACTER_SIZE_FOR_THE_SELECTED_FONT.Height),
					Convert.ToString(iString),
					GRAPH_NUMBERS_FONT,
					GRAPH_NUMBERS_FONT_SIZE,
					Brushes.Black,
					2
				);
			}
			for (double i = 0; i < onScreenYPositiveNumbersLinesCount; i += ySteps)
			{
				AddLineToCanvas(
					drawingContext,
					new Point(axisOriginLocation.X - GRAPH_NUMBERS_LINES_SIZE, axisOriginLocation.Y - yZoomAmount * (i + ySteps)),
					new Point(axisOriginLocation.X + GRAPH_NUMBERS_LINES_SIZE, axisOriginLocation.Y - yZoomAmount * (i + ySteps)),
					Brushes.Black,
					2
				);
				string iString = Convert.ToString(i + ySteps);
				AddTextToCanvas(
					drawingContext,
					new Point(axisOriginLocation.X + GRAPH_NUMBERS_TEXT_MARGIN_FOR_THE_SELECTED_FONT.Width, axisOriginLocation.Y - yZoomAmount * (i + ySteps) - GRAPH_NUMBERS_CHARACTER_SIZE_FOR_THE_SELECTED_FONT.Height),
					Convert.ToString(iString),
					GRAPH_NUMBERS_FONT,
					GRAPH_NUMBERS_FONT_SIZE,
					Brushes.Black,
					2
				);
			}

			for (int i = 0; i < equations.Length; i++)
			{
				if (equations[i] != null)
				{
					for (double x = -axisOriginLocation.X / xZoomAmount; x < (-axisOriginLocation.X + canvas_graphArea.ActualWidth) / xZoomAmount; x += (1.0D / xZoomAmount))
					{
						try
						{
							AddLineToCanvas(
								drawingContext,
								new Point(axisOriginLocation.X + x * xZoomAmount, axisOriginLocation.Y - equations[i].Solve(x, (bool)radioButton_degrees.IsChecked) * yZoomAmount),
								new Point(axisOriginLocation.X + ((x + (1.0D / xZoomAmount)) * xZoomAmount), axisOriginLocation.Y - equations[i].Solve(x + (1.0D / xZoomAmount), (bool)radioButton_degrees.IsChecked) * yZoomAmount),
								EQUATIONS_COLORS[i],
								2
							);
						}
						catch { }
					}
				}
			}

			drawingContext.Close();
		}
		private void AddLineToCanvas(DrawingContext drawingContext, Point startLocation, Point endLocation, Brush brush, double strokeThickness)
		{
			drawingContext.DrawLine(new Pen(brush, strokeThickness), startLocation, endLocation);
		}
		private void AddTextToCanvas(DrawingContext drawingContext, Point location, string text, string font, int size, Brush brush, double strokeThickness)
		{
			drawingContext.DrawText(new FormattedText(text, CultureInfo.CurrentUICulture, System.Windows.FlowDirection.LeftToRight, new Typeface(font), size, brush), location);
		}
		private static double GetNearestBeautifulNumber(double number)
		{
			// TODO: Improve
			if (number < 1)
			{
				int inversedRoundedNumber = (int)(1.0D / number);
				while (!((inversedRoundedNumber % 2 == 0 && inversedRoundedNumber % 6 != 0) || inversedRoundedNumber % 4 == 0 || inversedRoundedNumber % 5 == 0 || inversedRoundedNumber == 1))
					inversedRoundedNumber--;
				return 1.0D / inversedRoundedNumber;
			}
			else if (number > 1)
			{
				int roundedNumber = (int)number;
				while (!((roundedNumber % 2 == 0 && roundedNumber % 6 != 0) || roundedNumber % 4 == 0 || roundedNumber % 5 == 0 || roundedNumber == 1))
					roundedNumber--;
				return roundedNumber;
			}
			else
				return number;
		}
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	}
}
