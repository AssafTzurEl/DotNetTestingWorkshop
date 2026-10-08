using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

// ReadFromJsonAsync returns T?. In a test, a null there is a failure anyway: the next line throws.
#nullable disable warnings

namespace BankServer.Tests
{
    /// <summary>
    /// Black-box tests through the HTTP API. The server runs in-process; against a deployed
    /// server, the same code works with a different BaseAddress.
    /// Every test creates its own account: no reset, no fixed IDs, no counting.
    /// </summary>
    public class AccountApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private const string AccountPath = "account";

        public AccountApiTests(WebApplicationFactory<Program> factory, ITestOutputHelper output)
        {
            _client = factory.CreateClient();
            _output = output;
        }

        [Fact]
        public async Task CreateAccount_ValidName_CanBeReadBack()
        {
            // Arrange
            const string AccountName = "Assaf";
            AccountResponse created = await CreateAccountAsync(AccountName);

            // Act
            HttpResponseMessage response = await _client.GetAsync(AccountPath + "/" + created.Id);
            _output.WriteLine("Test result: " + await response.Content.ReadAsStringAsync());

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            AccountResponse read = await response.Content.ReadFromJsonAsync<AccountResponse>();
            read.Name.Should().Be(AccountName);
        }

        [Fact]
        public async Task Credit_ExistingAccount_BalanceIsStored()
        {
            // Arrange
            const string AccountName = "Assaf";
            const decimal Amount = 100m;
            AccountResponse created = await CreateAccountAsync(AccountName);

            // Act
            HttpResponseMessage creditResponse = await CreditAsync(created.Id, Amount);

            // Assert: read the balance back from the server, not from the credit response
            creditResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            AccountResponse read = await GetAccountAsync(created.Id);
            read.Balance.Should().Be(Amount);
        }

        [Fact]
        public async Task Charge_BlockedAccount_FailsWithAccountBlocked()
        {
            // Arrange: an account that a first charge has blocked
            const string AccountName = "Assaf";
            const decimal AmountBeyondThreshold = 5_000.01m;
            const decimal Amount = 1m;
            AccountResponse created = await CreateAccountAsync(AccountName);
            HttpResponseMessage blockingResponse = await ChargeAsync(created.Id, AmountBeyondThreshold);
            blockingResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Act
            HttpResponseMessage response = await ChargeAsync(created.Id, Amount);

            // Assert: it failed for the right reason, and nothing was charged
            ErrorResponse error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            error.Error.Should().Be("ACCOUNT_BLOCKED");
            AccountResponse read = await GetAccountAsync(created.Id);
            read.Balance.Should().Be(-AmountBeyondThreshold);
        }

        [Fact]
        public async Task GetAll_AfterCreate_ContainsTheNewAccount()
        {
            // Arrange
            const string AccountName = "Assaf";
            AccountResponse created = await CreateAccountAsync(AccountName);

            // Act
            HttpResponseMessage response = await _client.GetAsync(AccountPath);
            _output.WriteLine("Test result: " + await response.Content.ReadAsStringAsync());

            // Assert: my account is in the list – not "there is exactly one account"
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            List<AccountResponse> accounts = await response.Content.ReadFromJsonAsync<List<AccountResponse>>();
            accounts.Should().Contain(account => account.Id == created.Id);
        }

        private async Task<AccountResponse> CreateAccountAsync(string name)
        {
            HttpResponseMessage response = await _client.PostAsJsonAsync(AccountPath, new { name = name });
            _output.WriteLine("Account creation result: " + await response.Content.ReadAsStringAsync());
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            return await response.Content.ReadFromJsonAsync<AccountResponse>();
        }

        private async Task<AccountResponse> GetAccountAsync(int accountId)
        {
            HttpResponseMessage response = await _client.GetAsync(AccountPath + "/" + accountId);
            _output.WriteLine("Test result: " + await response.Content.ReadAsStringAsync());
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            return await response.Content.ReadFromJsonAsync<AccountResponse>();
        }

        private Task<HttpResponseMessage> CreditAsync(int accountId, decimal amount)
        {
            return PatchAsync(accountId, "credit", amount);
        }

        private Task<HttpResponseMessage> ChargeAsync(int accountId, decimal amount)
        {
            return PatchAsync(accountId, "charge", amount);
        }

        private async Task<HttpResponseMessage> PatchAsync(int accountId, string operation, decimal amount)
        {
            string url = AccountPath + "/" + accountId + "/" + operation + "/" + amount.ToString(CultureInfo.InvariantCulture);
            HttpResponseMessage response = await _client.PatchAsync(url, null);
            _output.WriteLine("Test result: " + await response.Content.ReadAsStringAsync());

            return response;
        }

        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;
    }

    /// <summary>
    /// The account as the API returns it. The tests own this type instead of using the
    /// server's Account: a black-box test knows only the API.
    /// </summary>
    public class AccountResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public decimal Balance { get; set; }
        public bool IsBlocked { get; set; }
    }

    /// <summary>
    /// The API's error body: { "error": "ACCOUNT_BLOCKED", "message": "..." }.
    /// </summary>
    public class ErrorResponse
    {
        public string Error { get; set; } = "";
        public string Message { get; set; } = "";
    }
}
