using CarpetProject.Samples;
using Xunit;

namespace CarpetProject.EntityFrameworkCore.Domains;

[Collection(CarpetProjectTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<CarpetProjectEntityFrameworkCoreTestModule>
{

}
