using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Obfuscation.Core.Bloat.ReplaceLiteralWithProperty;
using Obfuscation.Core.Name;
using Obfuscation.Utils;

namespace Obfuscation.Controls.Obfuscation.Model
{
    public class ObfuscationOptions
    {
        public bool RenameClasses { get; set; }
        public bool RenameMethods { get; set; }
        public bool RenameVariables { get; set; }

        public bool BloatWithClasses { get; set; }

        public IList<MutableKeyValuePair<PropertyGenerator.IBuilder<PropertyGenerator>, bool>>
            PropertyGeneratorBuilders { get; } = PropertyGenerator.IBuilder<PropertyGenerator>.AllPropertyGeneratorBuilders()
            .Select(builder =>
                new MutableKeyValuePair<PropertyGenerator.IBuilder<PropertyGenerator>, bool>(builder, false))
            .ToList();

        public IImmutableList<PropertyGenerator.IBuilder<PropertyGenerator>> ChosenPropertyGeneratorBuilders =>
            PropertyGeneratorBuilders.Where(pair => pair.Value).Select(pair => pair.Key)
                .ToImmutableList();

        public IList<MutableKeyValuePair<IIdentifierGenerator, bool>> IdentifierGenerators { get; } =
            IIdentifierGenerator.AllIdentifierGenerators()
                .Select(generator => new MutableKeyValuePair<IIdentifierGenerator, bool>(generator, false)).ToList();

        public IImmutableList<IIdentifierGenerator> ChosenIdentifierGenerators =>
            IdentifierGenerators.Where(pair => pair.Value).Select(pair => pair.Key).ToImmutableList();
    }
}