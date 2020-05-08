using System.Collections.Generic;

namespace Zwyssigly.ImageServer.Contracts
{
    public class ProcessingConfiguration
    {
        public bool AvoidDuplicates { get;  }
        public IReadOnlyCollection<SizeConfiguration> Sizes { get; }

        public ProcessingConfiguration(bool avoidDuplicates, IReadOnlyCollection<SizeConfiguration> sizes)
        {
            AvoidDuplicates = avoidDuplicates;
            Sizes = sizes;
        }
    }
}
