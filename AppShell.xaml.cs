using Microsoft.Maui.Controls;

namespace plz_fix
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register routes
            Routing.RegisterRoute("start", typeof(pages.login.start));
            Routing.RegisterRoute("loginpage", typeof(pages.login.loginpage));
        }
    }
}