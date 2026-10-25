using Microsoft.Extensions.Logging;
using ScaffTrack1.Services;
using ScaffTrack1.ViewModels.Auth;
using ScaffTrack1.ViewModels.Alertes;
using ScaffTrack1.ViewModels.Chantiers;
using ScaffTrack1.ViewModels.Clients;
using ScaffTrack1.ViewModels.Dashboard;
using ScaffTrack1.ViewModels.Historique;
using ScaffTrack1.ViewModels.Locations;
using ScaffTrack1.ViewModels.Materiel;
using ScaffTrack1.ViewModels.Mouvements;
using ScaffTrack1.Views.Auth;
using ScaffTrack1.Views.Alertes;
using ScaffTrack1.Views.Chantiers;
using ScaffTrack1.Views.Clients;
using ScaffTrack1.Views.Dashboard;
using ScaffTrack1.Views.Historique;
using ScaffTrack1.Views.Locations;
using ScaffTrack1.Views.Materiel;
using ScaffTrack1.Views.Mouvements;
using ScaffTrack1.Views.Plus;
using ScaffTrack1.Views.Profile;

namespace ScaffTrack1
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // 1. Services
            builder.Services.AddSingleton<HttpClient>();
            builder.Services.AddSingleton<IApiService, ApiService>();
            builder.Services.AddSingleton<AuthStateService>(); // Session utilisateur
            builder.Services.AddSingleton<AppShell>();          // Shell injecté dans App

            // 2. ViewModels & Vues (Pattern MVVM)
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<LoginPage>();

            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<ForgotPasswordPage>();

            builder.Services.AddTransient<DashboardViewModel>();
            builder.Services.AddTransient<DashboardPage>();

            builder.Services.AddTransient<ChantierListViewModel>();
            builder.Services.AddTransient<ChantierListPage>();

            builder.Services.AddTransient<CreateChantierViewModel>();
            builder.Services.AddTransient<CreateChantierPage>();

            builder.Services.AddTransient<MaterielListViewModel>();
            builder.Services.AddTransient<MaterielListPage>();

            builder.Services.AddTransient<SortieStockViewModel>();
            builder.Services.AddTransient<SortieStockPage>();

            builder.Services.AddTransient<ClientsViewModel>();
            builder.Services.AddTransient<ClientsPage>();
            builder.Services.AddTransient<ClientFormPage>();
            builder.Services.AddTransient<ClientDetailPage>();

            builder.Services.AddTransient<CreateMaterielViewModel>();
            builder.Services.AddTransient<CreateMaterielPage>();
            builder.Services.AddTransient<MaterielDetailPage>();
            builder.Services.AddTransient<AjustementStockViewModel>();
            builder.Services.AddTransient<AjustementStockPage>();

            builder.Services.AddTransient<ChantierDetailPage>();
            builder.Services.AddTransient<LocationsViewModel>();
            builder.Services.AddTransient<LocationsPage>();
            builder.Services.AddTransient<MouvementDetailPage>();
            builder.Services.AddTransient<RestitutionsViewModel>();
            builder.Services.AddTransient<RestitutionsPage>();
            builder.Services.AddTransient<AlertesViewModel>();
            builder.Services.AddTransient<AlertesPage>();
            builder.Services.AddTransient<HistoriqueViewModel>();
            builder.Services.AddTransient<HistoriquePage>();
            builder.Services.AddTransient<PlusPage>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<OfflineStatusPage>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
