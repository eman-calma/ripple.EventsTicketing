using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ripple.EventsTicketing.Infrastructure.Persistence;
using Ripple.EventsTicketing.Infrastructure.Repositories;

namespace Ripple.EventsTicketing.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<EventsTicketingDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddScoped<IReportsRepository, ReportsRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
