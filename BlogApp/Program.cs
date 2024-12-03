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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var credPath = builder.Configuration.GetValue<string>("FirebaseCredentialsPath");
Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credPath);

builder.Services.AddSingleton(FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile(credPath),
})
);

// builder.Services.AddSingleton<IFirebaseAuthClient,FirebaseAuthClient>();
builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
builder.Services.Configure<BlogPostsDatabaseSettings>(
        builder.Configuration.GetSection("BlogPostsDatabase"));
builder.Services.AddScoped(typeof(IMongoClient), _ =>
{
    return new MongoClient(builder.Configuration.GetValue<string>("BlogPostsDatabase:ConnectionString"));
});
builder.Services.AddScoped<IBlogPostService, BlogPostService>();
builder.Services.AddScoped<IPostCommentService, PostCommentService>();

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
});
builder.Services.AddCors(options => options.AddDefaultPolicy(t => t.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Blog API - V1");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "Blog API - V2");
    });
}

// app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors();

app.MapControllers();

app.Run();
