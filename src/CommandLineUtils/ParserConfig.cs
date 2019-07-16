// Copyright (c) Nate McMaster.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using McMaster.Extensions.CommandLineUtils.Abstractions;

namespace McMaster.Extensions.CommandLineUtils
{
    /// <summary>
    /// Configures the argument parser.
    /// </summary>
    public class ParserConfig
    {
        /// <summary>
        /// Determines if '--' can be used to stop the parser from processing any more arguments.
        /// </summary>
        public bool AllowArgumentSeparator { get; set; }

        private bool? _clusterOptions;

        /// <summary>
        /// <para>
        /// One or more options of <see cref="CommandOptionType.NoValue"/>, followed by at most one option that takes values, should be accepted when grouped behind one '-' delimiter.
        /// </para>
        /// <para>
        /// When true, the following are equivalent.
        ///
        /// <code>
        /// -abcXyellow
        /// -abcX=yellow
        /// -abcX:yellow
        /// -abc -X=yellow
        /// -ab -cX=yellow
        /// -a -b -c -Xyellow
        /// -a -b -c -X yellow
        /// -a -b -c -X=yellow
        /// -a -b -c -X:yellow
        /// </code>
        /// </para>
        /// <para>
        /// This defaults to true unless an option with a short name of two or more characters is added.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <seealso href="https://www.gnu.org/software/libc/manual/html_node/Argument-Syntax.html"/>
        /// </remarks>
        public bool ClusterOptions
        {
            // unless explicitly set, default to true
            get => _clusterOptions ?? true;
            set => _clusterOptions = value;
        }

        internal bool ClusterOptionsWasSetExplicitly => _clusterOptions.HasValue;

        /// <summary>
        /// The way arguments and options are matched.
        /// </summary>
        public StringComparison OptionsComparison { get; set; } = StringComparison.Ordinal;

        private char[] _optionNameValueSeparators = { ' ', ':', '=' };

        /// <summary>
        /// Characters used to separate the option name from the value.
        /// <para>
        /// By default, allowed separators are ' ' (space), :, and =
        /// </para>
        /// </summary>
        /// <remarks>
        /// Space actually implies multiple spaces due to the way most operating system shells parse command
        /// line arguments before starting a new process.
        /// </remarks>
        /// <example>
        /// Given --name=value, = is the separator.
        /// </example>
        public char[] OptionNameValueSeparators
        {
            get => _optionNameValueSeparators;
            set
            {
                _optionNameValueSeparators = value ?? throw new ArgumentNullException(nameof(value));
                if (value.Length == 0)
                {
                    throw new ArgumentException(Strings.IsNullOrEmpty, nameof(value));
                }
            }
        }

        internal bool OptionNameAndValueCanBeSpaceSeparated => Array.IndexOf(OptionNameValueSeparators, ' ') >= 0;

        /// <summary>
        /// When an invalid argument is given, make suggestions in the error message
        /// about similar, valid commands or options.
        /// <para>
        /// $ git pshu
        /// Specify --help for a list of available options and commands
        /// Unrecognized command or argument 'pshu'
        ///
        /// Did you mean this?
        ///     push
        /// </para>
        /// </summary>
        public bool MakeSuggestionsInErrorMessage { get; set; } = true;

        /// <summary>
        /// <para>
        /// When enabled, the parser will treat any arguments beginning with '@' as a file path to a response file.
        /// A response file contains additional arguments that will be treated as if they were passed in on the command line.
        /// </para>
        /// <para>
        /// Defaults to <see cref="ResponseFileHandling.Disabled" />.
        /// </para>
        /// <para>
        /// Nested response false are not supported.
        /// </para>
        /// </summary>
        public ResponseFileHandling ResponseFileHandling { get; set; }

        /// <summary>
        /// Gets the default value parser provider.
        /// <para>
        /// The value parsers control how argument values are converted from strings to other types. Additional value
        /// parsers can be added so that domain specific types can converted. In-built value parsers can also be replaced
        /// for precise control of all type conversion.
        /// </para>
        /// <remarks>
        /// Value parsers are currently only used by the Attribute API.
        /// </remarks>
        /// </summary>
        public ValueParserProvider ValueParsers { get; } = new ValueParserProvider();

        /// <summary>
        /// Set the behavior for how to handle unrecognized arguments.
        /// </summary>
        public UnrecognizedArgumentHandling UnrecognizedArgumentHandling { get; set; } =
            UnrecognizedArgumentHandling.Throw;
    }
}
