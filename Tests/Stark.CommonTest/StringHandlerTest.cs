using Xunit;
using Stark.Common;

namespace Stark.CommonTest
{
    public class StringHandlerTest
    {
        [Fact]
        public void StringHandler_GivenStringWithNoSpaces_ReturnsStringSeparatedBySpace()
        {
            // Arrange
            var source = "IngcoScrewdriver";
            var expected = "Ingco Screwdriver";
            //var handler = new StringHandler();

            // Act
            //var actual = StringHandler.InsertSpaces(source);
            var actual = source.InsertSpaces();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void StringHandler_GivenStringWithSpaces_ReturnsSameString()
        {
            // Arrange
            var source = "Ingco Screwdriver";
            var expected = "Ingco Screwdriver";
            //var handler = new StringHandler();

            // Act
            //var actual = StringHandler.InsertSpaces(source);
            var actual = source.InsertSpaces();

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
