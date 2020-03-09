// Copyright (c) Nate McMaster.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.Conventions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("McMaster.Extensions.Hosting.CommandLine.Tests,PublicKey=" + "00240000048000009400000006020000002400005253413100040000010001001df0eba4297c8ffdf114a13714ad787744619dfb18e29191703f6f782d6a09e4a4cac35b8c768cbbd9ade8197bc0f66ec66fabc9071a206c8060af8b7a332236968d3ee44b90bd2f30d0edcb6150555c6f8d988e48234debaf2d427a08d7c06ba1343411142dc8ac996f7f7dbe0e93d13f17a7624db5400510e6144b0fd683b9")]
namespace McMaster.Extensions.Hosting.CommandLine.Internal
{
    /// <inheritdoc />
    internal class CommandLineService<T> : IDisposable, ICommandLineService where T : class
    {
        private readonly CommandLineApplication _application;
        private readonly ILogger _logger;
        private readonly CommandLineState _state;

        /// <summary>
        ///     Creates a new instance.
        /// </summary>
        /// <param name="logger">A logger</param>
        /// <param name="state">The command line state</param>
        /// <param name="serviceProvider">The DI service provider</param>
        public CommandLineService(ILogger<CommandLineService<T>> logger, CommandLineState state,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _state = state;

            logger.LogDebug("Constructing CommandLineApplication<{type}> with args [{args}]",
                typeof(T).FullName, string.Join(",", state.Arguments));
            _application = new CommandLineApplication<T>(state.Console, state.WorkingDirectory);
            _application.Conventions
                .UseDefaultConventions()
                .UseConstructorInjection(serviceProvider);

            foreach (var convention in serviceProvider.GetServices<IConvention>())
            {
                _application.Conventions.AddConvention(convention);
            }
        }

        /// <inheritdoc />
        public async Task<int> RunAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Running");
            _state.ExitCode = await _application.ExecuteAsync(_state.Arguments, cancellationToken);
            return _state.ExitCode;
        }

        public void Dispose()
        {
            _application.Dispose();
        }
    }

    internal class CommandLineService : IDisposable, ICommandLineService
    {
        internal const string UnconfiguredErrorMessage = "Command line application is not configured. Please configure it by calling UseCommandLineApplication() when building the host.";

        private readonly CommandLineApplication _application;
        private readonly ILogger _logger;
        private readonly CommandLineState _state;

        /// <summary>
        ///     Creates a new instance.
        /// </summary>
        /// <param name="logger">A logger</param>
        /// <param name="state">The command line state</param>
        /// <param name="serviceProvider">The DI service provider</param>
        /// <param name="configure">An action which configures an instance of <see cref="CommandLineApplication" /></param>
        public CommandLineService(ILogger<CommandLineService> logger, CommandLineState state,
            IServiceProvider serviceProvider, Action<CommandLineApplication, IServiceProvider> configure = null)
        {
            _logger = logger;
            _state = state;

            logger.LogDebug("Constructing CommandLineApplication with args [{args}]",
                string.Join(",", state.Arguments));
            _application = new CommandLineApplication(state.Console, state.WorkingDirectory);
            _application.Conventions
                .UseDefaultConventions()
                .UseConstructorInjection(serviceProvider);

            foreach (var convention in serviceProvider.GetServices<IConvention>())
            {
                _application.Conventions.AddConvention(convention);
            }

            if (configure == null)
            {
                throw new InvalidOperationException(UnconfiguredErrorMessage);
            }
            else
            {
                // If an action was specified with .UseCommandLineApplication<T>(), then invoke it to configure the application
                configure.Invoke((CommandLineApplication)_application, serviceProvider);
            }
        }

        /// <inheritdoc />
        public async Task<int> RunAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Running");
            _state.ExitCode = await _application.ExecuteAsync(_state.Arguments, cancellationToken);
            return _state.ExitCode;
        }

        public void Dispose()
        {
            _application.Dispose();
        }
    }
}
