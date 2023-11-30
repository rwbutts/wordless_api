using WordlessAPI;
using System.Reflection;

const string HTTP_VER_HEADER = "X-wordless-api-version";
const string CORS_CONFIG_PATH = "Kestrel:Cors";

// get version string for http header
Version? apiVersion = Assembly.GetExecutingAssembly().GetName().Version;
string verHeaderValue = apiVersion?.ToString() ?? "unknown";

var builder = WebApplication.CreateBuilder( args );

builder.Services.AddConfiguredCors( builder, CORS_CONFIG_PATH );

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors();

if ( app.Environment.IsDevelopment() )
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();

app.MapGet( "/healthcheck", (HttpContext context) => {

          context.Response.Headers.Add( HTTP_VER_HEADER, verHeaderValue );
          return new HealthCheckResponse( true );
     }
);

app.MapGet( "/randomword",  ( HttpContext context ) => { 

          context.Response.Headers.Add( HTTP_VER_HEADER, verHeaderValue );
          return Words.RandomWord();
     }
);

app.MapGet( "/checkword/{word}",  ( HttpContext context, string word ) => {
 
          context.Response.Headers.Add( HTTP_VER_HEADER, verHeaderValue );
          return Words.WordExists( word );
     }
);

app.MapGet( "/getword/{daysago}",  ( HttpContext context, int daysago ) => { 

          context.Response.Headers.Add( HTTP_VER_HEADER, verHeaderValue );
          return Words.TodaysWord( daysago );
     }
);

app.MapPost( "/querymatchcount",  ( HttpContext context, QueryMatchCountRequest request) => {
     
          context.Response.Headers.Add( HTTP_VER_HEADER, verHeaderValue );
          return Words.CountMatches( Words.wordList, request.answer, request.guesses );
     }
);

app.UseSwaggerUI();
app.UseDefaultFiles();
app.UseStaticFiles();
app.Run();

public record HealthCheckResponse( bool alive );

public record QueryMatchCountRequest( string answer, string[] guesses );
