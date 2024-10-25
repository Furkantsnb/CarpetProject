﻿using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace CarpetProject.Pages;

public class Index_Tests : CarpetProjectWebTestBase
{
    [Fact]
    public async Task Welcome_Page()
    {
        var response = await GetResponseAsStringAsync("/");
        response.ShouldNotBeNull();
    }
}
