using System;
using System.IO;
using System.Linq;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
using SQLite.Net;
using SQLite.Net.Cipher.Interfaces;
using SQLite.Net.Cipher.Security;
using SQLiteNetCipher.Windows.Sample.Windows.Sample;

namespace SQLiteNetCipher.Windows.Sample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

	    protected override void OnNavigatedTo(NavigationEventArgs e)
	    {
		    base.OnNavigatedTo(e);

			var dbFilePath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "sqliteCipher.db3");
		    var platform = new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT();
			ISecureDatabase database = new MyDatabase(platform, dbFilePath, new CryptoService());
			var keySeed = "my very very secure key seed. You should use PCLCrypt strong random generator for this";

			var user = new SampleUser()
			{
				Name = "Has AlTaiar",
				Password = "very secure password :)",
				Bio = "Very cool guy :) ",
				Id = Guid.NewGuid().ToString()
			};

			var inserted = database.SecureInsert<SampleUser>(user, keySeed);

			System.Diagnostics.Debug.WriteLine("Sample Object was inserted securely? {0} ", inserted);

			var userFromDb = database.SecureGet<SampleUser>(user.Id, keySeed);

			System.Diagnostics.Debug.WriteLine("User was accessed back from the database: username= {0}, password={1}", userFromDb.Name, userFromDb.Password);

			// need to establish a direct connection to the database and get the object to test the encrypted value. 
			var directAccessDb = (SQLiteConnection)database;
			var userAccessedDirectly = directAccessDb.Query<SampleUser>("SELECT * FROM SampleUser").FirstOrDefault();

			System.Diagnostics.Debug.WriteLine("User was accessed Directly from the database (with no decryption): username= {0}, password={1}", userAccessedDirectly.Name, userAccessedDirectly.Password);
	    }
    }
}
