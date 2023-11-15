using System.Diagnostics;
using WordlessAPI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();

app.MapGet("/healthcheck", () => "Alive.");

app.MapGet("/api/randomword",  () =>{ 

          var R = Words.randomWord( );
          return new RandomWordResponse( R.Item1, R.Item2 );
     }
);

app.MapGet("/api/checkword/{word}",  ( string word ) =>
     new WordExistsResponse( Words.wordExists( word ) )
);

app.MapGet("/api/getword/{daysago}",  ( int daysago ) => { 

          var R = Words.todaysWord( daysago );
          return new RandomWordResponse( R.Item1, R.Item2 );
     }
);

app.MapPost("/api/querymatchcount",  (QueryMatchCountRequest request) =>
{
     int count = Filter.findMatches( Words.wordList, request.answer, new List<String>(request.guesses));
 
    return Results.Ok( new QueryMatchCountResponse(count));
});

app.UseSwaggerUI();
app.UseDefaultFiles();
app.UseStaticFiles();
app.Run();

public record RandomWordResponse( int index, string word );

public record WordExistsResponse( bool exists );

public class QueryMatchCountRequest
{
     public string answer { get; set; } 
     public string [] guesses { get; set; } 

     public QueryMatchCountRequest()
     {
          this.answer = "";
          this.guesses = new string[] {};
     }
}

 public class QueryMatchCountResponse
{
     public int count { get; set; }

     public QueryMatchCountResponse( int count )
     {
          this.count = count;
     }
}
