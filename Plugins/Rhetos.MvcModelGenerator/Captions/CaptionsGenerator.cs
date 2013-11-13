using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using Rhetos.Compiler;
using Rhetos.Extensibility;
using Rhetos.Logging;

namespace Rhetos.MvcModelGenerator.Captions
{
    [Export(typeof(IGenerator))]
    public class CaptionsGenerator : IGenerator
    {
        private readonly IPluginsContainer<ICaptionsGeneratorPlugin> _plugins;
        private readonly ICodeGenerator _codeGenerator;
        private readonly ILogger _logger;
        private readonly ILogger _sourceLogger;
        private const string CaptionFileName = "Captions";

        public CaptionsGenerator(
            IPluginsContainer<ICaptionsGeneratorPlugin> plugins,
            ICodeGenerator codeGenerator,
            ILogProvider logProvider
        )
        {
            _plugins = plugins;
            _codeGenerator = codeGenerator;

            _logger = logProvider.GetLogger("CaptionsGenerator");
            _sourceLogger = logProvider.GetLogger("Mvc Captions source");
        }

        public void Generate()
        {
            IAssemblySource assemblySource = _codeGenerator.ExecutePlugins(_plugins, "/*", "*/", new CaptionsInitialCodeGenerator());
            _logger.Trace("References: " + string.Join(", ", assemblySource.RegisteredReferences));
            _sourceLogger.Trace(assemblySource.GeneratedCode);

            File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Generated", CaptionFileName + ".resx"), assemblySource.GeneratedCode.Trim());
        }

        public IEnumerable<string> Dependencies
        {
            get { return new []{""}; }
        }
    }
}
