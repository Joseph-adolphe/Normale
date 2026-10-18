using ScaffTrack1.Services;
using ScaffTrack1.Views.Auth;
using ScaffTrack1.Views.Chantiers;
using ScaffTrack1.Views.Mouvements;

namespace ScaffTrack1
{
    public partial class AppShell : Shell
    {
        private readonly AuthStateService _authState;

        // Routes accessibles SANS être connecté
        private static readonly HashSet<string> RoutesPubliques = new()
        {
            "//LoginPage",
            "LoginPage",
            "RegisterPage"
        };

        public AppShell(AuthStateService authState)
        {
            InitializeComponent();
            _authState = authState;

            // Enregistrement des routes de navigation
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(CreateChantierPage), typeof(CreateChantierPage));
            Routing.RegisterRoute(nameof(SortieStockPage), typeof(SortieStockPage));

            // Garde de navigation — protège toutes les routes privées
            this.Navigating += OnShellNavigating;
        }

        private async void OnShellNavigating(object? sender, ShellNavigatingEventArgs e)
        {
            // Si la route cible est publique, on laisse passer
            if (RoutesPubliques.Contains(e.Target.Location.OriginalString))
                return;

            // Si l'utilisateur n'est PAS connecté → redirection vers LoginPage
            if (!_authState.IsAuthenticated)
            {
                e.Cancel();
                await Current.GoToAsync("//LoginPage");
            }
        }
    }
}

