// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
        private readonly EventHubsListenerConfiguration _configuration;
        private readonly string _dataLakeName;
        private MessageProcessor<EventHubListenerMessage> _messageProcessor;
        private readonly ServiceClientCredentials _serviceClientCredentials;
        private readonly string _subscription;

        public AnalyticsListener(EventHubsListenerConfiguration configuration,
            ServiceClientCredentials servicePrinciple, string subscription, string dataLakeName)
        {
            _configuration = configuration;
            _serviceClientCredentials = servicePrinciple;
            _dataLakeName = dataLakeName;
            _subscription = subscription;
        }

        public async Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            var listener = new EventHubsListener(_configuration);

            var parser = new EventHubListenerMessageJsonParser();

            var builder = new MessageRouterBuilder();

            var clusteringSerializer = new CsvOutputFormatter("id", "type", "version", "enqueueTimeUtc",
                "gameSessionId", "lat", "lon", "geoHash", "geoHashPrecision", "geoHashCenterLat", "geoHashCenterLon",
                "rnd");

            var clusteringDlsOutputManager = new DataLakeStoreOutputManager(
                clusteringSerializer,
                new PipelineDateFilePathAlgorithm(newFileOption: NewFileNameOptions.Every5Minutes),
                _serviceClientCredentials,
                _subscription,
                _dataLakeName);

            var clusteringConsoleOutputManager = new ConsoleOutputManager(clusteringSerializer);

            builder.Pipeline("clustering")
                .HandlesMessageType("geo-location", "1.0.0")
                .HandlesMessageType("geo-location", "1.0.1")
                .AddHandler(new RandomIntMessageHandler())
                .OutputTo(clusteringConsoleOutputManager, clusteringDlsOutputManager);


            // Build all pipelines
            var router = builder.Build();

            // Attach the differeing parts of the message processor together
            _messageProcessor = new MessageProcessor<EventHubListenerMessage>(listener, parser, router);

            // The following method will never exit
            //await messageProcessor.ProcessAndBlockAsync();

            return _configuration.EventHubPath;
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
            await _messageProcessor.ProcessAndBlockAsync(cancellationToken);
        }
    }
}