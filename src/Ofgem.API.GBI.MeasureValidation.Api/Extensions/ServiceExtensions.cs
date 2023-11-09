using DotNetRuleEngine.Interface;
using Microsoft.ApplicationInsights.Extensibility;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Infrastructure;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.ApiClients;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Repositories;
using Ofgem.API.GBI.MeasureValidation.Service.Stage1Rules;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;
using Ofgem.API.GBI.MeasureValidation.Service.Services;
using Ofgem.API.GBI.MeasureValidation.Application.Common;
using Microsoft.EntityFrameworkCore;
using Ofgem.Database.GBI.Measures.Domain.Persistence;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.UserManagement;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.ServiceBus;
using MassTransit;
using Azure.Identity;

using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Api.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            var userManagementUri = new Uri(configuration["UserManagementUrl"]!);
            var addressVerificationUrl = new Uri(configuration["AddressVerificationApiUrl"]!);

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddTransient<IPreProcessingService, PreProcessingService>();
            services.AddTransient<IStage1ProcessingService, Stage1ProcessingService>();
            services.AddTransient<IStage2ProcessingService, Stage2ProcessingService>();
            services.AddTransient<IStage2PassedMeasuresProcessor, Stage2PassedMeasuresProcessor>();
            services.AddTransient<INotificationDateService, NotificationDateService>();
            services.AddTransient<IErrorsReportService, ErrorsReportService>();
            services.AddTransient<TimeProvider, TimeProviderImplementation>();
            services.Configure<SchemeDetailsOptions>(configuration.GetSection(SchemeDetailsOptions.Name));

            services.AddHttpClient<DocumentApiClient>();

            services.AddHttpClient<ISupplierApiClient, SupplierApiClient>(x => x.BaseAddress = userManagementUri);
            services.AddTransient<IDocumentApiClient, DocumentApiClient>();
            services.AddHttpClient<IAddressApiClient, AddressApiClient>(x => x.BaseAddress = addressVerificationUrl);
            services.AddTransient<IAddressProcessingService, AddressProcessingService>();
            services.AddTransient<IFilesWithErrorsMetadataService, FilesWithErrorsMetadataService>();
            services.AddTransient<IInFillMeasureService, InFillMeasureService>();
            services.AddDbContext<MeasuresDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("MeasureConnection"),
                                     provider =>
                                     {
                                         provider.EnableRetryOnFailure();
                                     });
            });

            services.AddTransient<IMeasureRepository, MeasureRepository>();
            services.AddTransient<IErrorReportRepository, ErrorReportRepository>();
            services.AddSingleton<IMeasureTypeToCategoryService, MeasureTypeToCategoryService>();
            services.AddSingleton<IMeasureTypeToInnovationMeasureService, MeasureTypeToInnovationMeasureService>();

            RegisterMeasureStage1Services(services);
            RegisterMeasureStage2Services(services);

            services.AddMassTransit(x =>
            {
                x.UsingAzureServiceBus((context, cfg) =>
                {
                    var serviceBusName = configuration.GetValue<string>("ServiceBus:ServiceBusConnection");
                    cfg.Host(new Uri($"sb://{serviceBusName}"), e =>
                    {
                        e.TokenCredential = new DefaultAzureCredential();
                        e.RetryLimit = 5;
                    });

                    cfg.UseMessageRetry(r =>
                    {
                        r.Interval(5, TimeSpan.FromSeconds(5));
                    });
                    cfg.ConfigureEndpoints(context);
                });
            });

            services.AddTransient<ISendMessageService, SendMessageService>();

            return services;
        }

        public static IServiceCollection AddLogsConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ITelemetryInitializer, CustomTelemetryInitialiser>();
            services.AddApplicationInsightsTelemetry(configuration.GetSection("APPINSIGHTS_CONNECTIONSTRING"));

            return services;
        }

        private static void RegisterMeasureStage1Services(IServiceCollection services)
        {
            var stage1RuleTypes = new[]
            {
                typeof(Gbis0100),
                typeof(Gbis0101),
                typeof(Gbis0102),
                typeof(Gbis0103),
                typeof(Gbis0104),
                typeof(Gbis0105),
                typeof(Gbis0106),
                typeof(Gbis0107),
                typeof(Gbis0108),
                typeof(Gbis0109),
                typeof(Gbis0110),
                typeof(Gbis0111),
                typeof(Gbis0112),
                typeof(Gbis0113),
                typeof(Gbis0114),
                typeof(Gbis0115),
                typeof(Gbis0116),
                typeof(Gbis0117),
                typeof(Gbis0118),
                typeof(Gbis0119),
            };

            var ruleCreator = new Func<IServiceProvider, Type, IGeneralRule<MeasureModel>>(
                (serviceProvider, type) => serviceProvider.GetService(type) as IGeneralRule<MeasureModel> ?? UnableToCreateTypeAction(type));

            RegisterMeasureStageServices<IMeasureStage1RulesService, MeasureStage1RulesService, IGeneralRule<MeasureModel>>(services, stage1RuleTypes, ruleCreator);
        }

        private static void RegisterMeasureStage2Services(IServiceCollection services)
        {
            var stage2RuleLogicTypes = new[]
            {
                typeof(Gbis0200RuleLogic),
                typeof(Gbis0201RuleLogic),
                typeof(Gbis0202RuleLogic),
                typeof(Gbis0203RuleLogic),
                typeof(Gbis0204RuleLogic),
                typeof(Gbis0205RuleLogic),
                typeof(Gbis0206RuleLogic),
                typeof(Gbis0207RuleLogic),
                typeof(Gbis0208RuleLogic),
                typeof(Gbis0209RuleLogic),
                typeof(Gbis0210RuleLogic),
                typeof(Gbis0300RuleLogic),
                typeof(Gbis0301RuleLogic),
                typeof(Gbis0302RuleLogic),
                typeof(Gbis0303RuleLogic),
                typeof(Gbis0304RuleLogic),
                typeof(Gbis0305RuleLogic),
                typeof(Gbis0306RuleLogic),
                typeof(Gbis0400RuleLogic),
                typeof(Gbis0401RuleLogic),
                typeof(Gbis0402RuleLogic),
                typeof(Gbis0403RuleLogic),
                typeof(Gbis0500RuleLogic),
                typeof(Gbis0501RuleLogic),
                typeof(Gbis0502RuleLogic),
                typeof(Gbis0503RuleLogic),
                typeof(Gbis0600RuleLogic),
                typeof(Gbis0601RuleLogic),
                typeof(Gbis0602RuleLogic),
                typeof(Gbis0603RuleLogic),
                typeof(Gbis0604RuleLogic),
                typeof(Gbis0605RuleLogic),
                typeof(Gbis0606RuleLogic),
                typeof(Gbis0607RuleLogic),
                typeof(Gbis0608RuleLogic),
                typeof(Gbis0609RuleLogic),
                typeof(Gbis0610RuleLogic),
                typeof(Gbis0611RuleLogic),
                typeof(Gbis0612RuleLogic),
                typeof(Gbis0613RuleLogic),
                typeof(Gbis0614RuleLogic),
                typeof(Gbis0615RuleLogic),
                typeof(Gbis0616RuleLogic),
                typeof(Gbis0617RuleLogic),
                typeof(Gbis0618RuleLogic),
                typeof(Gbis0619RuleLogic),
                typeof(Gbis0620RuleLogic),
                typeof(Gbis0621RuleLogic),
                typeof(Gbis0622RuleLogic),
                typeof(Gbis0623RuleLogic),
                typeof(Gbis0624RuleLogic),
                typeof(Gbis0625RuleLogic),
                typeof(Gbis0626RuleLogic),
                typeof(Gbis0627RuleLogic),
                typeof(Gbis0628RuleLogic),
                typeof(Gbis0629RuleLogic),
                typeof(Gbis0630RuleLogic),
                typeof(Gbis0631RuleLogic),
                typeof(Gbis0632RuleLogic),
                typeof(Gbis0633RuleLogic),
                typeof(Gbis0634RuleLogic),
                typeof(Gbis0700RuleLogic),
                typeof(Gbis0701RuleLogic),
                typeof(Gbis0702RuleLogic),
                typeof(Gbis0703RuleLogic),
                typeof(Gbis0704RuleLogic),
                typeof(Gbis0705RuleLogic),
                typeof(Gbis0706RuleLogic),
                typeof(Gbis0707RuleLogic),
                typeof(Gbis0708RuleLogic),
                typeof(Gbis0709RuleLogic),
                typeof(Gbis0710RuleLogic),
                typeof(Gbis0711RuleLogic),
                typeof(Gbis0712RuleLogic),
                typeof(Gbis0714RuleLogic),
                typeof(Gbis0715RuleLogic),
                typeof(Gbis0716RuleLogic),
                typeof(Gbis0717RuleLogic),
                typeof(Gbis0718RuleLogic),
                typeof(Gbis0719RuleLogic),
                typeof(Gbis0720RuleLogic),
                typeof(Gbis0721RuleLogic),
                typeof(Gbis0722RuleLogic),
                typeof(Gbis0800RuleLogic),
                typeof(Gbis0801RuleLogic),
                typeof(Gbis0802RuleLogic),
                typeof(Gbis0803RuleLogic),
                typeof(Gbis0804RuleLogic),
                typeof(Gbis0805RuleLogic),
                typeof(Gbis0806RuleLogic),
                typeof(Gbis0807RuleLogic),
                typeof(Gbis0808RuleLogic),
                typeof(Gbis0809RuleLogic),
                typeof(Gbis0811RuleLogic),
                typeof(Gbis0812RuleLogic),
                typeof(Gbis0813RuleLogic),
                typeof(Gbis0814RuleLogic),
				typeof(Gbis0900RuleLogic),
				typeof(Gbis0901RuleLogic),
				typeof(Gbis0902RuleLogic),
				typeof(Gbis0903RuleLogic),
                typeof(Gbis0904RuleLogic),
                typeof(Gbis0905RuleLogic),
                typeof(Gbis0906RuleLogic),
                typeof(Gbis0909RuleLogic),
                typeof(Gbis0910RuleLogic),
                typeof(Gbis0911RuleLogic),
                typeof(Gbis0912RuleLogic),

                typeof(Gbis1001RuleLogic),
                typeof(Gbis1002RuleLogic),
                typeof(Gbis1004RuleLogic),
                typeof(Gbis1005RuleLogic),
                typeof(Gbis1002RuleLogic),
                typeof(Tgbis0100RuleLogic),
                typeof(Tgbis0101RuleLogic),
                typeof(Tgbis0102RuleLogic),
                typeof(Tgbis0103RuleLogic),
                typeof(Tgbis0104RuleLogic)
            };

            var ruleCreator = new Func<IServiceProvider, Type, GbisRule>(
                (serviceProvider, type) =>
                    ActivatorUtilities.CreateInstance<GbisRule>(serviceProvider, serviceProvider.GetService(type) ?? UnableToCreateTypeAction(type)));

            RegisterMeasureStageServices<IMeasureStage2RulesService, MeasureStage2RulesService, IRuleLogic>(services, stage2RuleLogicTypes, ruleCreator);
        }

        public static void RegisterMeasureStageServices<TMeasureStageRulesServiceInterface,
            TMeasureStageRulesServiceImplementation, TInterfaceType>(IServiceCollection services, IEnumerable<Type> ruleTypes, Func<IServiceProvider, Type, IGeneralRule<MeasureModel>> typeCreator)
            where TMeasureStageRulesServiceInterface : class
            where TMeasureStageRulesServiceImplementation : class, TMeasureStageRulesServiceInterface
        {
            if (ruleTypes == null) throw new ArgumentNullException(nameof(ruleTypes));

            var ruleTypesArray = ruleTypes as Type[] ?? ruleTypes.ToArray();
            if (!Array.TrueForAll(ruleTypesArray, TypeImplementsRuleInterface))
            {
                throw new NotSupportedException($"All rule types must implement {nameof(TInterfaceType)}.");
            }

            foreach (var ruleType in ruleTypesArray)
            {
                services.AddTransient(ruleType);
            }

            services.AddTransient<TMeasureStageRulesServiceInterface, TMeasureStageRulesServiceImplementation>(
                serviceProvider =>
                {
                    var stageRules = ruleTypesArray.Select(x => typeCreator(serviceProvider, x));
                    var measureStageRulesService =
                        ActivatorUtilities.CreateInstance<TMeasureStageRulesServiceImplementation>(serviceProvider,
                            stageRules);

                    return measureStageRulesService;
                });

            bool TypeImplementsRuleInterface(Type type) => Array.Exists(type.GetInterfaces(),
                i => i.IsAssignableFrom(typeof(TInterfaceType)));
        }

        private static IGeneralRule<MeasureModel> UnableToCreateTypeAction(Type type)
        {
            throw new InvalidOperationException($"Unable to create type '{type.FullName}'");
        }
    }
}
