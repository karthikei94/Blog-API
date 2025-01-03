using Asp.Versioning;
using BlogApp.Services;
using Firebase.Auth;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Storage.V1;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using BlogApp.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

// Add services to the container.
var credPath = builder.Configuration.GetValue<string>("FirebaseCredentialsPath");
Console.WriteLine($"GOOGLE cred file Setting: {credPath}");
Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credPath);

builder.Services.AddSingleton(FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile(credPath),
})
);

// builder.Services.AddSingleton<IFirebaseAuthClient,FirebaseAuthClient>();
builder.Services.AddSingleton<BlogApp.Services.IAuthenticationService, BlogApp.Services.AuthenticationService>();
builder.Services.Configure<BlogPostsDatabaseSettings>(
        builder.Configuration.GetSection("BlogPostsDatabase"));
builder.Services.AddScoped(typeof(IMongoClient), _ =>
{
    return new MongoClient(builder.Configuration.GetValue<string>("BlogPostsDatabase:ConnectionString"));
});
builder.Services.AddScoped<IBlogPostService, BlogPostService>();
builder.Services.AddScoped<IPostCommentService, PostCommentService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddSingleton<IFileUploadService>(provider => new FileUploadService(StorageClient.Create(), provider.GetRequiredService<IConfiguration>()));
// builder.Services.AddSingleton(_ => new FirestoreProvider(
//     new FirestoreDbBuilder 
//     { 
//         ProjectId = firebaseSettings.ProjectId, 
//         JsonCredentials = firebaseJson // <-- service account json file
//     }.Build()
// ));

// Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credPath);
// builder.Services.AddSingleton(FirebaseApp.Create());
// var firebaseProjectName = builder.Configuration.GetValue<string>("Firebase:ProjectId");
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//         .AddJwtBearer(options =>
//         {
//             options.Authority = $"https://securetoken.google.com/{firebaseProjectName}/";
//             options.TokenValidationParameters = new TokenValidationParameters
//             {
//                 ValidateIssuer = true,
//                 ValidIssuer = $"https://securetoken.google.com/{firebaseProjectName}/",
//                 ValidateAudience = true,
//                 ValidAudience = firebaseProjectName,
//                 ValidateLifetime = true,
//                 // ValidateIssuerSigningKey = true,
//                 // IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_secret_key"))
//             };
//         });

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddScheme<AuthenticationSchemeOptions, FirebaseAuthenticationHandler>(JwtBearerDefaults.AuthenticationScheme, (action) =>
        {
        });

// var key = System.Text.Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("Firebase:Key")!);
//     builder.Services.AddAuthentication(
//         auth => {
//         auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//         auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;})
//     .AddJwtBearer(options => {
//         options.RequireHttpsMetadata = false;
//         options.IncludeErrorDetails = true;
//         options.TokenValidationParameters = new TokenValidationParameters {
//             ValidateIssuerSigningKey = true,
//             IssuerSigningKey = new SymmetricSecurityKey(key),
//             ValidateIssuer = false,
//             ValidateAudience = false,
//             ValidateLifetime = true
//         };
//    });

builder.Services.AddControllers();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Api-Version")
    );
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Blog-API-V1", Contact = new OpenApiContact() { Name = "Karthikeya", Email = "mangalpally.10705417@ltimindtree.com" }, Version = "v1.0" });
    c.SwaggerDoc("v2", new OpenApiInfo { Title = "Blog-API-V2", Contact = new OpenApiContact() { Name = "Karthikeya", Email = "mangalpally.10705417@ltimindtree.com" }, Version = "v2.0" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});
builder.Services.AddCors(options => options.AddDefaultPolicy(t => t.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Blog API - V1");
    c.SwaggerEndpoint("/swagger/v2/swagger.json", "Blog API - V2");
});

// app.UseHttpsRedirection();

app.UseAuthentication();
// app.UseFirebaseAuthentication("https://securetoken.google.com/MYPROJECTNAME", "MYPROJECTNAME");
// app.UseJwtAuthentication()
app.UseAuthorization();
app.UseCors();

app.MapControllers();

app.Run();
