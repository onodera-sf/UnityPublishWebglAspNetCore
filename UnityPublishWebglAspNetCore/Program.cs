// ここから追加
using Microsoft.AspNetCore.StaticFiles;
// ここまで追加

var builder = WebApplication.CreateBuilder(args);

// コンテナにサービスを追加します。
builder.Services.AddRazorPages();

var app = builder.Build();

// HTTP リクエストパイプラインを設定します。
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	// デフォルトのHSTS値は30日です。本番シナリオではこれを変更することをお勧めします。https://aka.ms/aspnetcore-hsts を参照してください。
	app.UseHsts();
}

app.UseHttpsRedirection();

// ここから追加
var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".data"] = "application/octet-stream";
provider.Mappings[".wasm"] = "application/wasm";
provider.Mappings[".br"] = "application/octet-stream";   // .br ファイルにアクセスできるように追加
provider.Mappings[".js"] = "application/javascript";     // 後の変換の為に追加

app.UseStaticFiles(new StaticFileOptions()
{
	ContentTypeProvider = provider,
	OnPrepareResponse = context =>
	{
		var path = context.Context.Request.Path.Value;
		var extension = Path.GetExtension(path);

		// 「.gz」「.br」ファイルにアクセスした場合は Content-Type と Content-Encoding を設定する
		if (extension == ".gz" || extension == ".br")
		{
			var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path) ?? "";
			if (provider.TryGetContentType(fileNameWithoutExtension, out string? contentType))
			{
				context.Context.Response.ContentType = contentType;
				context.Context.Response.Headers.Add("Content-Encoding", extension == ".gz" ? "gzip" : "br");
			}
		}
	},
});
// ここまで追加

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
