static class CorsExtensions 
{
     public static IServiceCollection AddConfiguredCors( this IServiceCollection services, WebApplicationBuilder builder, string configPath )
     {
          builder.Services.AddCors( options =>
          {
               CorsPolicyConfig corsPolicy;

               if( configPath != null )
               {
                     corsPolicy =  builder.Configuration
                                   .GetSection( configPath )
                                   .Get<CorsPolicyConfig>() 
                                   ?? CorsPolicyConfig.AllowAllPolicy();
               }
               else
               {
                    corsPolicy = CorsPolicyConfig.AllowAllPolicy();
               }

               options.AddDefaultPolicy( 
                    builder =>
                    {
                         builder.WithOrigins( corsPolicy.AllowedOrigins ?? CorsPolicyConfig.EMPTYARRAY)
                              .WithMethods( corsPolicy.AllowedMethods ?? CorsPolicyConfig.EMPTYARRAY )
                              .WithHeaders( corsPolicy.AllowedHeaders ?? CorsPolicyConfig.EMPTYARRAY );

                              if( corsPolicy.AllowCredentials )
                              {
                                   builder.AllowCredentials();
                              }
                              else
                              {
                                   builder.DisallowCredentials();
                              }
                    });
          });

          return services;
     } 
}

public class CorsPolicyConfig
{
     public static readonly string[] WILDCARD = new string[]{"*"};
     public static readonly string[] EMPTYARRAY = Array.Empty<string>();

     public string[]? AllowedOrigins { get; set; }
     public string[]? AllowedMethods { get; set; }
     public string[]? AllowedHeaders { get; set; }
     public bool AllowCredentials { get; set; } = false;

     public CorsPolicyConfig()
     {
     }

     public static CorsPolicyConfig AllowAllPolicy()
     {
          var cfg = new CorsPolicyConfig();
          cfg.AllowedOrigins = cfg.AllowedHeaders = cfg.AllowedMethods = WILDCARD;
          return cfg;
     }
}
