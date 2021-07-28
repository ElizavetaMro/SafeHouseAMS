﻿using FluentAssertions;
using SafeHouseAMS.BizLayer.LifeSituations.Vulnerabilities;
using Xunit;
using Xunit.Categories;

namespace SafeHouseAMS.Test.BizLayer.LifeSituations.Vulnerabilities
{
    public class AddictionTest
    {
        [Fact, UnitTest]
        public void ToString_WhenKindIsEmpty_ReturnsNoBrackets() =>
            new Addiction(string.Empty).ToString()
                .Should().NotContain("(").And.NotContain(")");

        [Theory, UnitTest]
        [InlineData("123")]
        [InlineData(" 123 ")]
        [InlineData(" ")]
        [InlineData("\t")]
        public void ToString_WhenKindIsNotEmpty_ContainsInput(string input) =>
            new Addiction(input).ToString().Should().Contain($"({input})");
    }

}