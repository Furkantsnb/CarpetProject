using CarpetProject.Samples;
using Xunit;

namespace CarpetProject.EntityFrameworkCore.Applications;

[Collection(CarpetProjectTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<CarpetProjectEntityFrameworkCoreTestModule>
{

}
