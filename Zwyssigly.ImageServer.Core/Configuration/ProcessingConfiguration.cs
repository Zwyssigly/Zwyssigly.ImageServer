using System.Collections.Generic;
using System.Linq;
using Zwyssigly.Functional;

namespace Zwyssigly.ImageServer.Configuration
{
    public class ProcessingConfiguration
    {
        public bool AvoidDuplicates { get; }
        public IReadOnlyCollection<SizeConfiguration> Sizes { get; }

        public static Result<ProcessingConfiguration, Error> New(bool avoidDuplicates, IReadOnlyCollection<SizeConfiguration> sizes)
        {
            if (sizes.Select(s => s.Tag).Distinct().Count() < sizes.Count)
                return Result.Failure(Error.ValidationError("Sizes must have unique tags!"));

            return Result.Success(new ProcessingConfiguration(avoidDuplicates, sizes));
        }

        private ProcessingConfiguration(bool avoidDuplicates, IReadOnlyCollection<SizeConfiguration> sizes)
        {
            AvoidDuplicates = avoidDuplicates;
            Sizes = sizes;
        }
    }
}
