using System;
using MonoTouch.UIKit;
using System.Threading.Tasks;
using MonoTouch.Foundation;
using System.Collections.Generic;
using Simple.OData.Client;
using Infragistics;
using System.Drawing;

namespace NorthwindSalesiOS
{
	public class HomeViewController : UIViewController
	{
		public HomeViewController ()
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
		}

		public override void ViewDidLoad ()
		{
			this.Title = "Northwind Sales Dashboard";
			this.EdgesForExtendedLayout = UIRectEdge.None;

			IGChartView chart = null;


			var fetchDataButton = new UIBarButtonItem ("Fetch Data", UIBarButtonItemStyle.Plain, async (object sender, EventArgs e) => {

				// if chart was added previously, remove it from the view before constructing it with new values
				if(chart!=null)
					chart.RemoveFromSuperview();

				chart = new IGChartView(this.View.Bounds);
				chart.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
				chart.Theme = IGChartGradientThemes.IGThemeDark();

				var progressView = new IGProgressView (IGProgressViewStyle.IGProgressViewStyleRadialIndeterminate);
				progressView.Frame = new RectangleF ((this.View.Bounds.Size.Width / 2) - 100, (this.View.Bounds.Size.Height / 2) - 100, 200, 200);
				progressView.AutoresizingMask = UIViewAutoresizing.FlexibleTopMargin | UIViewAutoresizing.FlexibleBottomMargin | UIViewAutoresizing.FlexibleLeftMargin | UIViewAutoresizing.FlexibleRightMargin;
			
				this.View.Add (progressView);

				var data = await GetDataAsync ();

				// set data source
				IGCategorySeriesDataSourceHelper barSeriesSource = new IGCategorySeriesDataSourceHelper();
				barSeriesSource.Values = data.ProductSales.ToArray();
				barSeriesSource.Labels = data.ProductName.ToArray();

			
				// Create axis types and add it to the chart
				IGNumericXAxis xAxisBar = new IGNumericXAxis ("xAxis");
				IGCategoryYAxis yAxisBar = new IGCategoryYAxis ("yAxis");
				yAxisBar.LabelAlignment = IGHorizontalAlign.IGHorizontalAlignRight;

				chart.AddAxis(xAxisBar);
				chart.AddAxis(yAxisBar);

				// decide on what series need to be displayed on the chart
				IGBarSeries barSeries= new IGBarSeries ("series");
				barSeries.XAxis = xAxisBar;
				barSeries.YAxis = yAxisBar;

				// set the appropriate data sources
				barSeries.DataSource = barSeriesSource;
				chart.AddSeries(barSeries);

				progressView.RemoveFromSuperview();

				this.View.Add(chart);


			});

			NavigationItem.SetRightBarButtonItem (fetchDataButton, false);	
		}


		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}

		async Task<SalesByCategory> GetDataAsync()
		{
			var client = new ODataClient("http://services.odata.org/V3/Northwind/Northwind.svc/");
			var salesByCategory = await client.For ("Sales_by_Categories").Top(15).OrderBy("ProductSales").FindEntriesAsync();
	
			SalesByCategory saleByCtg = new SalesByCategory();
			foreach (var sale in salesByCategory) {
				saleByCtg.ProductName.Add (NSObject.FromObject(sale["ProductName"].ToString()));
				saleByCtg.ProductSales.Add (NSObject.FromObject(sale["ProductSales"].ToString()));
			}

			return saleByCtg;
		}
	}

	public class SalesByCategory
	{
		public SalesByCategory ()
		{
			ProductName = new List<NSObject> ();
			ProductSales = new List<NSObject> ();

		}
		public List<NSObject> ProductName{
			get;
			set;
		}

		public List<NSObject> ProductSales{
			get;
			set;
		}
	}

}

