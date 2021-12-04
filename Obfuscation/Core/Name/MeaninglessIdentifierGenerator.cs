namespace Obfuscation.Core.Name
{
    public class MeaninglessIdentifierGenerator: IIdentifierGenerator
    {
        public string DisplayName => "Meaningless";

        public string GenerateName()
        {
            throw new System.NotImplementedException();
        }
    }
}