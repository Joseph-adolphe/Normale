using ScaffTrack1.Services;
using ScaffTrack1.Views.Alertes;
using ScaffTrack1.Views.Auth;
using ScaffTrack1.Views.Clients;
using ScaffTrack1.Views.Chantiers;
using ScaffTrack1.Views.Historique;
using ScaffTrack1.Views.Materiel;
using ScaffTrack1.Views.Mouvements;
using ScaffTrack1.Views.Profile;
using ScaffTrack1.Views.Plus;

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
            "RegisterPage",
            nameof(ForgotPasswordPage)
        };

        public AppShell(AuthStateService authState)
        {
            InitializeComponent();
            _authState = authState;

            // Enregistrement des routes de navigation
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(ForgotPasswordPage), typeof(ForgotPasswordPage));
            Routing.RegisterRoute(nameof(CreateChantierPage), typeof(CreateChantierPage));
            Routing.RegisterRoute(nameof(ChantierDetailPage), typeof(ChantierDetailPage));
            Routing.RegisterRoute(nameof(ClientDetailPage), typeof(ClientDetailPage));
            Routing.RegisterRoute(nameof(ClientFormPage), typeof(ClientFormPage));
            Routing.RegisterRoute(nameof(CreateMaterielPage), typeof(CreateMaterielPage));
            Routing.RegisterRoute(nameof(MaterielDetailPage), typeof(MaterielDetailPage));
            Routing.RegisterRoute(nameof(AjustementStockPage), typeof(AjustementStockPage));
            Routing.RegisterRoute(nameof(SortieStockPage), typeof(SortieStockPage));
            Routing.RegisterRoute(nameof(MouvementDetailPage), typeof(MouvementDetailPage));
            Routing.RegisterRoute(nameof(RestitutionsPage), typeof(RestitutionsPage));
            Routing.RegisterRoute(nameof(AlertesPage), typeof(AlertesPage));
            Routing.RegisterRoute(nameof(HistoriquePage), typeof(HistoriquePage));
            Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
            Routing.RegisterRoute(nameof(OfflineStatusPage), typeof(OfflineStatusPage));
            Routing.RegisterRoute(nameof(PlusPage), typeof(PlusPage));

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
