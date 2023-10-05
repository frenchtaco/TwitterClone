
namespace UnitTest1
{
    public class CommandHandlerTests
    {
        [Fact]
        public async Task Fondle_Should_Call_GetAsync_When_ShouldReadAll_IsTrue()
        {
            // Arrange
            var commandHandler = new CommandHandler();

            // Act
            await commandHandler.GetAllRequest(commandHandler._client);

            // Assert
            HttpResponseMessage response = await commandHandler._client.GetAsync("/cheeps");


        }

    }

    public class KestrelPathTest
    {
        [Fact]
        public async Task KestrelPathTest_Should_Do_Something()
        {
            // Arrange
            var commandHandler = new CommandHandler();

            // Act
            var response = await commandHandler._client.GetAsync("/");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();

            Assert.Contains("Kestrel is running on", content);
        }
    }

    /*  public class PostTest
      {
          [Fact]
          public async Task PostRequest_ReturnsSuccessStatusCode()
          {
              // Arrange
              var client = new HttpClient();
              client.BaseAddress = new Uri("http://localhost:5000"); // Replace with your actual server address
              var message = "Test message";

              // Act
              await PostRequest(message, client);

              // Assert
              // You could add assertions here based on what you expect to happen in the PostRequest method.
          }

      }
      */


    public class TimeStampConversionTest
    {

        [Fact]
        public void ConvertUnixTimestampToFormattedString()
        {
            // Arrange
            var commandHandler = new CommandHandler();
            long input = 1696322254;

            // Act
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(input);
            string output = dateTimeOffset.ToString("dd-MM-yy HH:mm:ss");

            // Assert
            Assert.Equal("03-10-23 08:37:34", output);
        }
    }
}


