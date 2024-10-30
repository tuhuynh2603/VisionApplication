﻿
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using VisionApplication.MVVM.ViewModel;
using Binding = System.Windows.Data.Binding;
using Brushes = System.Windows.Media.Brushes;
using Image = System.Windows.Controls.Image;
using Panel = System.Windows.Controls.Panel;
using Point = System.Windows.Point;


namespace VisionApplication.MVVM.View
{
	/// <summary>
	/// Interaction logic for PixelRuler.xaml
	/// </summary>


	public partial class PixelRuler : System.Windows.Controls.UserControl
	{

		//private bool isDragging = false;
		//private Point mouseOffset;


		public PixelRuler()
		{
			InitializeComponent();
			initPixelRuleDelegate = InitPixelRule;

        }

		private int width = 0;
		private int height = 0;

		private double scaleX = 1;
		private double scaleY = 1;

		bool isDock = false;

		Grid[] uIElements;

		int trackID = 0;

		public void SetUp(Grid[] uIElements, bool isDock)
		{
			this.isDock = isDock;
			this.uIElements = uIElements;

			if (uIElements == null) { return; }
			for (int i = 0; i < uIElements.Length; i++)
			{
				this.uIElements[i].PreviewMouseLeftButtonDown += Pixe_MouseLeftButtonDown;
				this.uIElements[i].PreviewMouseMove += Pixel_PreviewMouseMove;
			}
			currentDock = this.uIElements[0];
			polyline = new Polyline();
			polyline.StrokeThickness = 0.3;
			polyline.Stroke = Brushes.Blue;
			Panel.SetZIndex(polyline, 5);
			valuepixX.Text = "--";
			valuepixY.Text = "--";
			valuepixel.Text = "--";
			valueum.Text = "--";
			valuedeg.Text = "--";
			valuerad.Text = "--";
			currentGrid = CreteaGrid();
			currentGrid.Children.Add(polyline);
			currentDock.Children.Add(currentGrid);
		}
		public void Finish()
		{
			if (uIElements == null) return;
			for (int i = 0; i < uIElements.Length; i++)
			{
				uIElements[i].PreviewMouseLeftButtonDown -= Pixe_MouseLeftButtonDown;
				uIElements[i].PreviewMouseMove -= Pixel_PreviewMouseMove;
			}
			if (currentDock.Children.Contains(currentGrid))
				currentDock.Children.Remove(currentGrid);
		}


		Polyline polyline;
		Grid currentGrid;
		Grid currentDock;
		Image GetImage()
		{
			for (int i = 0; i < currentDock.Children.Count; i++)
			{
				if (currentDock.Children[i].GetType() == typeof(Image))
					return currentDock.Children[i] as Image;
			}
			return null;
		}
		Grid CreteaGrid()
		{
			Grid grid = new Grid();
			grid.VerticalAlignment = VerticalAlignment.Top;
			grid.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
			Binding bndW = new Binding("ActualWidth");
			Binding bndH = new Binding("ActualHeight");
			var image = GetImage();
			bndH.Source = image;
			bndW.Source = image;
			grid.Width = image.ActualHeight;
			grid.Height = image.ActualWidth;
			grid.SetBinding(WidthProperty, bndW);
			grid.SetBinding(HeightProperty, bndH);
			Grid.SetZIndex(grid, 3);
			return grid;
		}
		int count = 0;
		public void Pixe_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			int trackID = MainWindowVM.activeImageDock.trackID;
			//int docID = 0;
			var dock = sender as Grid;
			if (dock != currentDock)
			{
				if (currentDock.Children.Contains(currentGrid))
				{
					currentDock.Children.Remove(currentGrid);
				}
				currentDock = dock;
				currentGrid.Children.Clear();
				currentGrid = CreteaGrid();
				currentGrid.Children.Add(polyline);
				currentDock.Children.Add(currentGrid);
				count = 0;
			}

			System.Windows.Point m_start = (e.GetPosition(dock));
			if (count == 0)
			{
				polyline.Points.Clear();
				polyline.Points.Add(m_start);
				polyline.Points.Add(m_start);
				count += 1;
			}
			else if (count == 1)
			{
				count = 0;
			}
		}

		void UpdateDatas()
		{
			double resX = 0.0;
			double resY = 0.0;
			trackID = MainWindowVM.activeImageDock.trackID;
			width = MainWindowVM.master.m_Tracks[trackID].m_imageViews[0]._imageWidth;
			height = MainWindowVM.master.m_Tracks[trackID].m_imageViews[0]._imageHeight;
			//ToDO need to determine the resolutoion both camera
			if (trackID == 0)
			{
				resX = 24.0 / 650;
				resY = 24.0 / 650;
			}
			else if (trackID == 1)
			{
				resX = 240 / 650;
				resY = 240 / 650;
			}
			//else if (trackID == 2)
			//{
			//	resX = (double)(Application.categoriesInsParam.SR_resolutionSf) / (1000);
			//	resY = (double)(Application.categoriesInsParam.SR_resolutionSf) / (1000);
			//}

			try
			{
				var image = GetImage();
				scaleX = image.ActualWidth / (isDock ? width : (image.Source as BitmapSource).PixelWidth);
				scaleY = image.ActualHeight / (isDock ? height : (image.Source as BitmapSource).PixelHeight);
			}
			catch { }
			Point start = new Point(polyline.Points[0].X / scaleX, polyline.Points[0].Y / scaleY);
			Point end = new Point(polyline.Points[1].X / scaleX, polyline.Points[1].Y / scaleY);
			double res = 1;

			res = (resX + resY) / 2;

			double dPixel = Math.Sqrt((end.X - start.X) * (end.X - start.X) + (end.Y - start.Y) * (end.Y - start.Y));
			valuepixX.Text = ((int)(end.X - start.X)).ToString();
			valuepixY.Text = ((int)(end.Y - start.Y)).ToString();

			valuepixel.Text = String.Format("{0:0.00}", dPixel);

			valueum.Text = string.Format("{0:0.00}", Math.Round(dPixel * res, 2));

			double xDist = end.X - start.X;
			double yDist = end.Y - start.Y;
			double dangle = Math.Atan2(yDist, xDist);

			valuedeg.Text = String.Format("{0:0.00}", dangle * 180 / Math.PI);
			valuerad.Text = String.Format("{0:0.00}", dangle);
		}
		private void Pixel_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (count == 1 && polyline.Points.Count > 0)
			{
				Point m_start = (e.GetPosition(sender as Grid));
				polyline.Points.RemoveAt(1);
				polyline.Points.Add(m_start);
				UpdateDatas();
			}
		}
		private void CloseRuler(object sender, RoutedEventArgs e)
		{
            InitPixelRule(Visibility.Collapsed);

        }


		public delegate void InitPixelRuleDelegate(Visibility bIsVisible);
		public static InitPixelRuleDelegate initPixelRuleDelegate;


        public void InitPixelRule(Visibility bIsVisible)
		{
            if (bIsVisible != Visibility.Visible)
            {
                Finish();
                return;
            }

            
            Grid[] tempGrid = new Grid[AppMagnus.m_nTrack];
            for (int index_track = 0; index_track < AppMagnus.m_nTrack; index_track++)
            {
                tempGrid[index_track] = MainWindowVM.master.m_Tracks[index_track].m_imageViews[0].grd_Dock;
            }

            SetUp(tempGrid, true);
        }

	}
}
