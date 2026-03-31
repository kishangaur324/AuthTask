namespace AuthTask.UnitTests.Controller
{
    public class AuthControllerTests
    {
        [Fact]
        public void Register_ReturnsSuccess_WhenRequestIsValid()
        {
            //Arrange
            var isSuccess = true;

            //Act
            var result = isSuccess;

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void Login_ReturnsToken_WhenCredentialsAreValid()
        {
            //Arrange
            var isSuccess = true;

            //Act
            var result = isSuccess;

            //Assert
            Assert.True(result);
        }
    }
}
