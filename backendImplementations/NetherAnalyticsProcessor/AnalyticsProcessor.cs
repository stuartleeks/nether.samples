using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Microsoft.Rest.Azure.Authentication;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Nether.Analytics.EventHubs;

namespace NetherAnalyticsProcessor
{
    /// <summary>
    ///     An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class AnalyticsProcessor : StatelessService
    {
        private const string ParameterCannotBeNullFormat =
            "The parameter [{0}] is not defined in the Setting.xml configuration file.";

        private AnalyticsListener _listener;

        public AnalyticsProcessor(StatelessServiceContext context)
            : base(context)
        {
        }

        private EventHubsListenerConfiguration getEventHubsListenerConfiguration(ConfigurationPackage config)
        {
            if (!config.Settings.Sections.Contains("EventHubConfig"))
                throw new ArgumentException(string.Format(ParameterCannotBeNullFormat, "EventHubConfig"),
                    "EventHubConfig");

            var eventHubConfig = config.Settings.Sections["EventHubConfig"];

            if (!eventHubConfig.Parameters.Contains("EventHubPath"))
                throw new ArgumentException(string.Format(ParameterCannotBeNullFormat, "EventHubPath"), "EventHubPath");


            if (!eventHubConfig.Parameters.Contains("ConsumerGroupName"))
                throw new ArgumentException(string.Format(ParameterCannotBeNullFormat, "ConsumerGroupName"),
                    "ConsumerGroupName");


            if (!eventHubConfig.Parameters.Contains("EventHubConnectionString"))
                throw new ArgumentException(string.Format(ParameterCannotBeNullFormat, "EventHubConnectionString"),
                    "EventHubConnectionString");


            if (!eventHubConfig.Parameters.Contains("StorageConnectionString"))
                throw new ArgumentException(
                    string.Format(ParameterCannotBeNullFormat, "EventHuStorageConnectionStringbPath"),
                    "StorageConnectionString");


            if (!eventHubConfig.Parameters.Contains("LeaseContainerName"))
                throw new ArgumentException(string.Format(ParameterCannotBeNullFormat, "LeaseContainerName"),
                    "LeaseContainerName");

            return new EventHubsListenerConfiguration
            {
                ConsumerGroupName = eventHubConfig.Parameters["ConsumerGroupName"].Value,
                EventHubConnectionString = eventHubConfig.Parameters["EventHubConnectionString"].Value,
                EventHubPath = eventHubConfig.Parameters["EventHubPath"].Value,
                LeaseContainerName = eventHubConfig.Parameters["LeaseContainerName"].Value,
                StorageConnectionString = eventHubConfig.Parameters["StorageConnectionString"].Value
            };
        }

        private async Task<ServiceClientCredentials> getServiceClientCredentials(ConfigurationPackage config)
        {
            if (!config.Settings.Sections.Contains("ServicePrinciple"))
                throw new ArgumentException(string.Format(ParameterCannotBeNullFormat, "ServicePrinciple"),
                    "ServicePrinciple");

            var servicePrinciple = config.Settings.Sections["ServicePrinciple"];

            if (!servicePrinciple.Parameters.Contains("Tenant"))
                throw new ArgumentException(
                    string.Format(ParameterCannotBeNullFormat, "Tenant"),
                    "Tenant");

            if (!servicePrinciple.Parameters.Contains("ObjectId"))
                throw new ArgumentException(
                    string.Format(ParameterCannotBeNullFormat, "ObjectId"),
                    "ObjectId");

            if (!servicePrinciple.Parameters.Contains("Secret"))
                throw new ArgumentException(string.Format(ParameterCannotBeNullFormat, "Secret"),
                    "Secret");

            return await ApplicationTokenProvider.LoginSilentAsync(servicePrinciple.Parameters["Tenant"].Value,
                new ClientCredential(servicePrinciple.Parameters["ObjectId"].Value,
                    servicePrinciple.Parameters["Secret"].Value));
        }

        private string getDataLakeName(ConfigurationPackage config)
        {
            if (!config.Settings.Sections.Contains("DataLake"))
                throw new ArgumentException(string.Format(ParameterCannotBeNullFormat, "DataLake"), "DataLake");

            var datalakeConfig = config.Settings.Sections["DataLake"];

            if (!datalakeConfig.Parameters.Contains("DataLakeStoreName"))
                throw new ArgumentException(string.Format(ParameterCannotBeNullFormat, "DataLakeStoreName"),
                    "DataLakeStoreName");

            return datalakeConfig.Parameters["DataLakeStoreName"].Value;
        }

        private string getSubscriptionId(ConfigurationPackage config)
        {
            if (!config.Settings.Sections.Contains("DataLake"))
                throw new ArgumentException(string.Format(ParameterCannotBeNullFormat, "DataLake"), "DataLake");

            var datalakeConfig = config.Settings.Sections["DataLake"];

            if (!datalakeConfig.Parameters.Contains("SubscriptionId"))
                throw new ArgumentException(string.Format(ParameterCannotBeNullFormat, "SubscriptionId"),
                    "SubscriptionId");

            return datalakeConfig.Parameters["SubscriptionId"].Value;
        }


        private ICommunicationListener createCommunicationListener()
        {
            var codePackageActivationContext = this.Context.CodePackageActivationContext;
            var config = codePackageActivationContext.GetConfigurationPackageObject("Config");

            this._listener = new AnalyticsListener(this.getEventHubsListenerConfiguration(config),
                this.getServiceClientCredentials(config).Result, this.getSubscriptionId(config),
                this.getDataLakeName(config));

            return this._listener;
        }

        /// <summary>
        ///     Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new[]
            {
                new ServiceInstanceListener(context => this.createCommunicationListener(), "AnalyticsListener")
            };
        }

        /// <summary>
        ///     This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            await this._listener.StartAsync(cancellationToken);
        }
    }
}