var builder = WebApplication.CreateBuilder(args);
var allServices = builder.Services;

builder.Services.AddSingleton<IUserRepository, UserRepository>();

var app = builder.Build();





app.MapWhen(context => context.Request.Path.StartsWithSegments("/"), appBuilder =>
{
    appBuilder.Run(async context =>
    {
        var users = context.RequestServices.GetService<IUserRepository, UserRepository>();
    });

});

app.Run();
