// ��������ǉ�
using Microsoft.AspNetCore.StaticFiles;
// �����܂Œǉ�

var builder = WebApplication.CreateBuilder(args);

// �R���e�i�ɃT�[�r�X��ǉ����܂��B
builder.Services.AddRazorPages();

var app = builder.Build();

// HTTP ���N�G�X�g�p�C�v���C����ݒ肵�܂��B
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	// �f�t�H���g��HSTS�l��30���ł��B�{�ԃV�i���I�ł͂����ύX���邱�Ƃ������߂��܂��Bhttps://aka.ms/aspnetcore-hsts ���Q�Ƃ��Ă��������B
	app.UseHsts();
}

app.UseHttpsRedirection();

// ��������ǉ�
var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".data"] = "application/octet-stream";
provider.Mappings[".wasm"] = "application/wasm";
provider.Mappings[".br"] = "application/octet-stream";   // .br �t�@�C���ɃA�N�Z�X�ł���悤�ɒǉ�
provider.Mappings[".js"] = "application/javascript";     // ��̕ϊ��ׂ̈ɒǉ�

app.UseStaticFiles(new StaticFileOptions()
{
	ContentTypeProvider = provider,
	OnPrepareResponse = context =>
	{
		var path = context.Context.Request.Path.Value;
		var extension = Path.GetExtension(path);

		// �u.gz�v�u.br�v�t�@�C���ɃA�N�Z�X�����ꍇ�� Content-Type �� Content-Encoding ��ݒ肷��
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
// �����܂Œǉ�

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
