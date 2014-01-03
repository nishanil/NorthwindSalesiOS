using System;
using MonoTouch.UIKit;
using System.Threading.Tasks;
using MonoTouch.Foundation;
using System.Collections.Generic;
using Simple.OData.Client;
using Infragistics;
using BigTed;

namespace NorthwindSalesiOS
{
	public class HomeViewController : UIViewController
	{
		readonly ODataClient client;
		IGChartView chart;

		public HomeViewController ()
		{
			client = new ODataClient ("http://services.odata.org/v3/Northwind/Northwind.svc/");
		}

		public override void ViewDidLoad ()
		{
			Title = "Northwind Sales Dashboard";
			EdgesForExtendedLayout = UIRectEdge.None;

			var fetchDataButton = new UIBarButtonItem ("Fetch Data", UIBarButtonItemStyle.Plain, OnFetchDataTap);

			NavigationItem.SetRightBarButtonItem (fetchDataButton, false);	
		}

	

		async void OnFetchDataTap (object sender, EventArgs e)
		{
			InitialiseChart ();

			BTProgressHUD.Show ("Fetching data");

			var data = await GetDataAsync ();

			// set data source
			var barSeriesSource = new IGCategorySeriesDataSourceHelper ();
			barSeriesSource.Values = data.ProductSales.ToArray ();
			barSeriesSource.Labels = data.ProductName.ToArray ();

			// Create axis types and add it to the chart
			var xAxisBar = new IGNumericXAxis ("xAxis");
			var yAxisBar = new IGCategoryYAxis ("yAxis");
			yAxisBar.LabelAlignment = IGHorizontalAlign.IGHorizontalAlignRight;

			chart.AddAxis (xAxisBar);
			chart.AddAxis (yAxisBar);

			// decide on what series need to be displayed on the chart
			var barSeries = new IGBarSeries ("series");
			barSeries.XAxis = xAxisBar;
			barSeries.YAxis = yAxisBar;

			// set the appropriate data sources
			barSeries.DataSource = barSeriesSource;
			chart.AddSeries (barSeries);

			BTProgressHUD.Dismiss ();


		}

		async Task<SalesByCategory> GetDataAsync ()
		{
<<<<<<< HEAD
			var client = new ODataClient("http://services.odata.org/V3/Northwind/Northwind.svc/");
			var salesByCategory = await client.For ("Sales_by_Categories").Top(15).OrderBy("ProductSales").FindEntriesAsync();
=======
			var salesByCategory = await client.For("Sales_by_Categories")
				.Top (15).OrderBy ("ProductSales").FindEntriesAsync ();
>>>>>>> pr/1
	
			var saleByCtg = new SalesByCategory ();
			foreach (var sale in salesByCategory) {
				saleByCtg.ProductName.Add (NSObject.FromObject (sale ["ProductName"].ToString ()));
				saleByCtg.ProductSales.Add (NSObject.FromObject (sale ["ProductSales"].ToString ()));
			}

			return saleByCtg;
		}

		void InitialiseChart ()
		{
			// if chart was added previously, remove it from the view before constructing it with new values
			if (chart != null)
				chart.RemoveFromSuperview ();
			chart = new IGChartView (View.Bounds);
			chart.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
			chart.Theme = IGChartGradientThemes.IGThemeDark ();
			View.Add (chart);
		}
	}

	public class SalesByCategory
	{
		public SalesByCategory ()
		{
			ProductName = new List<NSObject> ();
			ProductSales = new List<NSObject> ();

		}

		public List<NSObject> ProductName {
			get;
			set;
		}

		public List<NSObject> ProductSales {
			get;
			set;
		}
	}
}

