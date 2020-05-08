using System;
using System.Windows;
using System.Windows.Media;

namespace Graph_Drawer
{
	class DrawingVisualHost : UIElement
	{
		// Variables ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		private DrawingVisual drawingVisual;
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



		// Constructors ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		public DrawingVisualHost(DrawingVisual drawingVisual) : base()
		{
			this.drawingVisual = drawingVisual;
		}
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



		// Methods /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		protected override int VisualChildrenCount
		{
			get { return drawingVisual != null ? 1 : 0; }
		}
		protected override Visual GetVisualChild(int index)
		{
			return this.drawingVisual;
		}
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	}
}