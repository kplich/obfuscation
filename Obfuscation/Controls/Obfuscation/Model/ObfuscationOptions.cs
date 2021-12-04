using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Obfuscation.Core.Name;
using Obfuscation.Utils;

namespace Obfuscation.Controls.Obfuscation.Model
{
    public class ObfuscationOptions
    {
        public bool RenameClasses { get; set; }
        public bool RenameMethods { get; set; }
        public bool RenameVariables { get; set; }

        public IList<MutableKeyValuePair<IIdentifierGenerator, bool>> IdentifierGenerationStrategies { get; } =
            IIdentifierGenerator.AllGenerators()
                .Select(generator => new MutableKeyValuePair<IIdentifierGenerator, bool>(generator, false)).ToList();

        public IImmutableList<IIdentifierGenerator> ChosenIdentifierGenerationStrategies =>
            IdentifierGenerationStrategies.Where(pair => pair.Value).Select(pair => pair.Key).ToImmutableList();
    }
}