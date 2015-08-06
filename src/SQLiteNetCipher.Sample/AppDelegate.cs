using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CoreGraphics;
using Foundation;
using SQLite.Net;
using SQLite.Net.Cipher.Interfaces;
using SQLite.Net.Cipher.Security;
using SQLiteNetCipher.Sample.Data;
using UIKit;

namespace SQLiteNetCipher.Sample
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		UIWindow window;

		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			// create a new window instance based on the screen size
			window = new UIWindow(UIScreen.MainScreen.Bounds);

			// If you have defined a view, add it here:
			 window.RootViewController  = new UINavigationController( new MainViewController());

			// make the window visible
			window.MakeKeyAndVisible();

			return true;
		}
	}

	public class MainViewController : UIViewController
	{
		public MainViewController()
		{
			
		}

		private UILabel _label;

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var button = new UIButton(new CGRect(50, 100, 200, 30));
			button.SetTitle("Go", UIControlState.Application);
			button.BackgroundColor = UIColor.Blue;
			button.SetTitleColor(UIColor.Red, UIControlState.Normal);
			button.TouchUpInside += OnTouched;

			_label = new UILabel(new CGRect(50, 150, 200, 50));
			
			View.BackgroundColor = UIColor.White;
			View.Add(button);
			View.Add(_label);
		}

		private void OnTouched(object sender, EventArgs e)
		{
			var dbFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "mysequredb.db3");
			var platform = new SQLite.Net.Platform.XamarinIOS.SQLitePlatformIOS();
			ISecureDatabase database = new MyDatabase(platform, dbFilePath, new CryptoService());
			var keySeed = "my very very secure key seed. You should use PCLCrypt strong random generator for this";

			var user = new SampleUser()
			{
				Name = "Has AlTaiar", Password = "very secure password :)", Bio = "Very cool guy :) ", Id = Guid.NewGuid().ToString()
			};

			var inserted = database.SecureInsert<SampleUser>(user, keySeed);

			Console.WriteLine("Sample Object was inserted securely? {0} ", inserted);

			var userFromDb = database.SecureGet<SampleUser>(user.Id, keySeed);

			Console.WriteLine("User was accessed back from the database: username= {0}, password={1}", userFromDb.Name, userFromDb.Password);

			// need to establish a direct connection to the database and get the object to test the encrypted value. 
			var directAccessDb = (SQLiteConnection)database;
			var userAccessedDirectly = directAccessDb.Query<SampleUser>("SELECT * FROM SampleUser", 0).FirstOrDefault();

			Console.WriteLine("User was accessed Directly from the database (with no decryption): username= {0}, password={1}", userAccessedDirectly.Name, userAccessedDirectly.Password);
		}
	}
}
