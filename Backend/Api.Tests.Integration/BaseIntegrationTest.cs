using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Tests.Integration;

public abstract class BaseIntegrationTest : IClassFixture<ApiFactory>, IAsyncLifetime
{
    protected readonly HttpClient Client;
    protected readonly AppDbContext DbContext;

    public BaseIntegrationTest(ApiFactory factory)
    {
        Client = factory.CreateClient();

        var scope = factory.Services.CreateScope();
        DbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await DbContext.Courses.ExecuteDeleteAsync();
        await DbContext.Users.ExecuteDeleteAsync();
    }
}
