// Copyright (c) Nate McMaster.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace McMaster.Extensions.CommandLineUtils
{
    /// <summary>
    /// Configures the argument parser.
    /// </summary>
    public class ParserConfig
    {
        private char[] _optionNameValueSeparators = { ' ', ':', '=' };
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
        internal bool ClusterOptionsWasSetExplicitly => _clusterOptions.HasValue;

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
    }
}
