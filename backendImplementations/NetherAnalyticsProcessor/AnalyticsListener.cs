using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Rest;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Nether.Analytics;
using Nether.Analytics.DataLake;
using Nether.Analytics.EventHubs;
using Nether.Analytics.Parsers;

namespace NetherAnalyticsProcessor
{
    public class AnalyticsListener : ICommunicationListener
    {
        private readonly EventHubsListenerConfiguration configuration;
        private readonly string dataLakeName;
        private MessageProcessor<EventHubListenerMessage> messageProcessor;
        private readonly ServiceClientCredentials serviceClientCredentials;
        private readonly string subscription;

        public AnalyticsListener(EventHubsListenerConfiguration configuration,
            ServiceClientCredentials servicePrinciple, string subscription, string dataLakeName)
        {
            this.configuration = configuration;
            this.serviceClientCredentials = servicePrinciple;
            this.dataLakeName = dataLakeName;
            this.subscription = subscription;
        }

        public async Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            var listener = new EventHubsListener(this.configuration);

            var parser = new EventHubListenerMessageJsonParser();

            var builder = new MessageRouterBuilder();

            var clusteringSerializer = new CsvOutputFormatter("id", "type", "version", "enqueueTimeUtc",
                "gameSessionId", "lat", "lon", "geoHash", "geoHashPrecision", "geoHashCenterLat", "geoHashCenterLon",
                "rnd");

            var clusteringDlsOutputManager = new DataLakeStoreOutputManager(
                clusteringSerializer,
                new PipelineDateFilePathAlgorithm(newFileOption: NewFileNameOptions.Every5Minutes),
                this.serviceClientCredentials,
                this.subscription,
                this.dataLakeName);

            var clusteringConsoleOutputManager = new ConsoleOutputManager(clusteringSerializer);

            builder.Pipeline("clustering")
                .HandlesMessageType("geo-location", "1.0.0")
                .HandlesMessageType("geo-location", "1.0.1")
                .AddHandler(new RandomIntMessageHandler())
                .OutputTo(clusteringConsoleOutputManager, clusteringDlsOutputManager);


            // Build all pipelines
            var router = builder.Build();

            // Attach the differeing parts of the message processor together
            this.messageProcessor = new MessageProcessor<EventHubListenerMessage>(listener, parser, router);

            // The following method will never exit
            //await messageProcessor.ProcessAndBlockAsync();

            return this.configuration.EventHubPath;
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            try
            {
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                // Trace Error
                ServiceEventSource.Current.Message(ex.Message);
                throw;
            }
        }

        public void Abort()
        {
            try
            {
            }
            catch (Exception ex)
            {
                // Trace Error
                ServiceEventSource.Current.Message(ex.Message);
                throw;
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await this.messageProcessor.ProcessAndBlockAsync(cancellationToken);
        }
    }
}