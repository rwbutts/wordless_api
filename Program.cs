using WordlessAPI;
using System.Reflection;
using Microsoft.AspNetCore.Cors.Infrastructure;

Assembly assembly = Assembly.GetExecutingAssembly();
Version? apiVersion = assembly.GetName().Version;

const string headerName = "X-wordless-api-version";
string headerValue = apiVersion?.ToString() ?? "unknown";

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Access CORS settings from configuration
var configuration = builder.Configuration;
var corsSection = configuration.GetSection("Kestrel:Cors");

var corsPolicyName = corsSection["PolicyName"]?? "DefaultPolicy";
var allowedOrigins = corsSection.GetSection("AllowedOrigins").Get<string[]>() ?? new string[]{"*"};
var allowedMethods = corsSection.GetSection("AllowedMethods").Get<string[]>() ?? new string[]{"*"};
var allowedHeaders = corsSection.GetSection("AllowedHeaders").Get<string[]>() ?? new string[]{"*"};
var allowCredentials = corsSection.GetValue<bool>("AllowCredentials");

builder.Services.AddCors(options =>
{
     options.AddPolicy(corsPolicyName,
          builder =>
          {
               builder.WithOrigins(allowedOrigins)
                    .WithMethods(allowedMethods)
                    .WithHeaders(allowedHeaders);
                    if( allowCredentials )
                         builder.AllowCredentials();
                    else
                         builder.DisallowCredentials();
          });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors(corsPolicyName);

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();

app.MapGet("/healthcheck", (HttpContext context) => {

          context.Response.Headers.Add( headerName, headerValue );
          return new HealthCheckResponse( true );
     }
);

app.MapGet("/randomword",  ( HttpContext context ) => { 

          context.Response.Headers.Add( headerName, headerValue );
          return Words.RandomWord();
     }
);

app.MapGet("/checkword/{word}",  ( HttpContext context, string word ) => {
 
          context.Response.Headers.Add( headerName, headerValue );
          return Words.WordExists( word );
     }
);

app.MapGet("/getword/{daysago}",  ( HttpContext context, int daysago ) => { 

          context.Response.Headers.Add( headerName, headerValue );
          return Words.TodaysWord( daysago );
     }
);

app.MapPost("/querymatchcount",  ( HttpContext context, QueryMatchCountRequest request) => {
     
          context.Response.Headers.Add( headerName, headerValue );
          return Words.CountMatches( Words.wordList, request.answer, new List<String>(request.guesses ) );
     }
);

app.UseSwaggerUI();
app.UseDefaultFiles();
app.UseStaticFiles();
app.Run();

public record HealthCheckResponse( bool alive );


public record QueryMatchCountRequest( string answer, string[] guesses );
