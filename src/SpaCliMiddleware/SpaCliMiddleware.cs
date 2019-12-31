using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SpaServices;

namespace SpaCliMiddleware
{
    internal static class SpaCliMiddleware
    {
        private const string LogCategoryName = "SpaCliMiddleware";
        internal const string DefaultRegex = "running at";

        private static TimeSpan RegexMatchTimeout = TimeSpan.FromMinutes(5); // This is a development-time only feature, so a very long timeout is fine

        public static void Attach(
            ISpaBuilder spaBuilder,
            string scriptName, int port = 8080, ScriptRunnerType runner = ScriptRunnerType.Npm, string regex = DefaultRegex, bool forceKill = false, bool useProxy = true)
        {
            var sourcePath = spaBuilder.Options.SourcePath;
            if (string.IsNullOrEmpty(sourcePath))
            {
                throw new ArgumentException("Cannot be null or empty", nameof(sourcePath));
            }

            if (string.IsNullOrEmpty(scriptName))
            {
                throw new ArgumentException("Cannot be null or empty", nameof(scriptName));
            }

            // Start vue-cli and attach to middleware pipeline
            var appBuilder = spaBuilder.ApplicationBuilder;
            var logger = LoggerFinder.GetOrCreateLogger(appBuilder, LogCategoryName);
            var portTask = StartSpaCliServerAsync(sourcePath, scriptName, logger, port, runner, regex, forceKill);
            if (!useProxy)
            {
                return;
            }

            // Everything we proxy is hardcoded to target http://localhost because:
            // - the requests are always from the local machine (we're not accepting remote
            //   requests that go directly to the vue-cli server)
            // - given that, there's no reason to use https, and we couldn't even if we
            //   wanted to, because in general the vue-cli server has no certificate
            var targetUriTask = portTask.ContinueWith(
                task => new UriBuilder("http", "localhost", task.Result).Uri);

            SpaProxyingExtensions.UseProxyToSpaDevelopmentServer(spaBuilder, () =>
            {
                // On each request, we create a separate startup task with its own timeout. That way, even if
                // the first request times out, subsequent requests could still work.
                var timeout = spaBuilder.Options.StartupTimeout;
                return targetUriTask.WithTimeout(timeout,
                    $"The svelte-cli server did not start listening for requests " +
                    $"within the timeout period of {timeout.Seconds} seconds. " +
                    $"Check the log output for error information.");
            });
        }

        private static async Task<int> StartSpaCliServerAsync(
            string sourcePath, string npmScriptName, ILogger logger, int portNumber, ScriptRunnerType runner, string regex, bool forceKill = false, bool useProxy = true)
        {            
            if (portNumber < 80)
            {
                portNumber = TcpPortFinder.FindAvailablePort();
            }
            else
            {
                // if the port we want to use is occupied, terminate the process utilizing that port.
                // this occurs when "stop" is used from the debugger and the middleware does not have the opportunity to kill the process
                PidUtils.KillPort((ushort)portNumber, forceKill);
            }
            logger.LogInformation($"Starting server on port {portNumber}...");

            var envVars = new Dictionary<string, string>
            {
                { "PORT", portNumber.ToString() },
                { "DEV_SERVER_PORT", portNumber.ToString() }, // vue cli 3 uses --port {number}, included below
                { "BROWSER", "none" }, // We don't want vue-cli to open its own extra browser window pointing to the internal dev server port
            };
            string nodeCmdArgs = useProxy ? "" : $"--port {portNumber:0}";
            var npmScriptRunner = new ScriptRunner(sourcePath, npmScriptName, nodeCmdArgs, envVars, runner: runner);

            AppDomain.CurrentDomain.DomainUnload += (s, e) => npmScriptRunner?.Kill();
            AppDomain.CurrentDomain.ProcessExit += (s, e) => npmScriptRunner?.Kill();
            AppDomain.CurrentDomain.UnhandledException += (s, e) => npmScriptRunner?.Kill();
            npmScriptRunner.AttachToLogger(logger);

            using (var stdErrReader = new EventedStreamStringReader(npmScriptRunner.StdErr))
            {
                try
                {
                    // Although the SPA dev server may eventually tell us the URL it's listening on,
                    // it doesn't do so until it's finished compiling, and even then only if there were
                    // no compiler warnings. So instead of waiting for that, consider it ready as soon
                    // as it starts listening for requests.
                    await npmScriptRunner.StdOut.WaitForMatch(new Regex(!string.IsNullOrWhiteSpace(regex) ? regex : DefaultRegex, RegexOptions.None, RegexMatchTimeout));
                }
                catch (EndOfStreamException ex)
                {
                    string message = stdErrReader.ReadAsString();
                    throw new InvalidOperationException(
                        $"The NPM script '{npmScriptName}' exited without indicating that the " +
                        $"server was listening for requests. The error output was: " +
                        $"{message}", ex);
                }
            }
            return portNumber;
        }
    }
}