using Microsoft.Extensions.DependencyInjection;
using MauiAppMinhasCompras.Helpers;

namespace MauiAppMinhasCompras
{
    public partial class App : Application
    {
        static SQLiteDatabaseHelper _db;

        public static SQLiteDatabaseHelper Db
        {
            get
            {
                if (_db == null)
                {
                    string path = Path.Combine(
                        Environment.GetFolderPath(
                            Environment.SpecialFolder.LocalApplicationData),
                        "banco_sqlite_compras.db3");

                    _db = new SQLiteDatabaseHelper(path);
                }

                return _db;
            }
        }

        public App()
        {
            try
            {
                InitializeComponent();

                //MainPage = new AppShell();
                MainPage = new NavigationPage(new Views.ListaProduto());
            }
            catch (Exception ex)
            {
                // Prevent startup crash and surface the error in the UI so debugging can continue
                System.Diagnostics.Debug.WriteLine($"App initialization failed: {ex}");
                // Show a simple error page so the app stays alive and the debugger can attach/inspect
                MainPage = new ContentPage
                {
                    Content = new Label { Text = "Erro ao inicializar o aplicativo: " + ex.Message }
                };
            }
        }
    }
}