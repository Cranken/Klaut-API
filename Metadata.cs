using System.Collections.Immutable;
using MimeDetective;

namespace Metadata
{
    public class FileInspector
    {
        private readonly ContentInspector _cInspector;

        public FileInspector()
        {
            _cInspector = new ContentInspectorBuilder()
            {
                Definitions = new MimeDetective.Definitions.ExhaustiveBuilder()
                {
                    UsageType = MimeDetective.Definitions.Licensing.UsageType.PersonalNonCommercial
                }.Build()
            }.Build();
        }
        ImmutableArray<MimeDetective.Engine.DefinitionMatch> InspectFile(string filename)
        {
            var Content = ContentReader.Default.ReadFromFile(filename);
            return InspectFile(Content);
        }

        ImmutableArray<MimeDetective.Engine.DefinitionMatch> InspectFile(byte[] content)
        {
            return _cInspector.Inspect(content);
        }
    }
}