using WordlessAPI;
using System.Reflection;

Assembly assembly = Assembly.GetExecutingAssembly();
Version? apiVersion = assembly.GetName().Version;

const string headerName = "X-wordless-api-version";
string headerValue = apiVersion?.ToString() ?? "unknown";

var builder = WebApplication.CreateBuilder(args);

// Access CORS settings from configuration
var configuration = builder.Configuration;
var corsPolicyName = configuration["Kestrel:Cors:PolicyName"];
var allowedOrigins = configuration.GetSection("Kestrel:Cors:AllowedOrigins").Get<string[]>();
var allowedMethods = configuration.GetSection("Kestrel:Cors:AllowedMethods").Get<string[]>();
var allowedHeaders = configuration.GetSection("Kestrel:Cors:AllowedHeaders").Get<string[]>();
var allowCredentials = configuration.GetValue<bool>("Kestrel:Cors:AllowCredentials");

builder.Services.AddCors(options =>
{
     options.AddPolicy(corsPolicyName,
          builder =>
          {
               builder.WithOrigins(allowedOrigins)
                    .WithMethods(allowedMethods)
                    .WithHeaders(allowedHeaders)
                    .DisallowCredentials();
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
          var R = Words.RandomWord( );
          return new RandomWordResponse( R.Item1, R.Item2 );
     }
);

app.MapGet("/checkword/{word}",  ( HttpContext context, string word ) => {
 
          context.Response.Headers.Add( headerName, headerValue );
          return new WordExistsResponse( Words.WordExists( word ) );
     }
);

app.MapGet("/getword/{daysago}",  ( HttpContext context, int daysago ) => { 

          context.Response.Headers.Add( headerName, headerValue );
          var R = Words.TodaysWord( daysago );
          return new RandomWordResponse( R.Item1, R.Item2 );
     }
);

app.MapPost("/querymatchcount",  ( HttpContext context, QueryMatchCountRequest request) => {
     
          context.Response.Headers.Add( headerName, headerValue );
          return new QueryMatchCountResponse( Words.FindMatches( Words.wordList, request.answer, new List<String>(request.guesses ) ) );
     }
);

app.UseSwaggerUI();
app.UseDefaultFiles();
app.UseStaticFiles();
app.Run();

public record HealthCheckResponse( bool alive );

public record RandomWordResponse( int index, string word );

public record WordExistsResponse( bool exists );

public record QueryMatchCountResponse( int count );

public record QueryMatchCountRequest( string answer, string[] guesses );

public class QueryMatchCountRequest1
{
     public string answer { get; set; } 
     public string [] guesses { get; set; } 

     public QueryMatchCountRequest1()
     {
          this.answer = "";
          this.guesses = new string[] {};
     }
}
