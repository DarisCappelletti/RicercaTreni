using RicercaTreni.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// Client tipizzato verso la BFF non ufficiale di lefrecce (le API che usa il sito di Trenitalia)
builder.Services.AddHttpClient<LefrecceClient>(client =>
{
    client.BaseAddress = new Uri("https://www.lefrecce.it/");
    client.DefaultRequestHeaders.Add("accept-language", "it-IT");
    client.Timeout = TimeSpan.FromSeconds(30);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();