using MAF.Core.Domain;

namespace MAF.Adapters.Input.WebServer.DTO;

public class TreeDtoReq
{
    public double Latitude { get; set; } = 0.0;
    public double Longitude { get; set; } = 0.0;
    public string Name { get; set; } = string.Empty;
    public string Fruit { get; set; } = string.Empty;
    public string SciName { get; set; } = string.Empty;
    public string FkEmail { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public static bool ValidarExtensao(IFormFile arquivo)
    {
        string[] extensoes_permitidas = new[] { ".jpg", ".jpeg", ".png" };

        var extensao = Path.GetExtension(arquivo.FileName)?.ToLowerInvariant();

        return extensoes_permitidas.Contains(extensao);
    }
    public static bool ValidarTipoMime(IFormFile arquivo)
    {
        string[] tipos_mime_permitidos = new[] { "image/jpeg", "image/png", "image/jpg" };

        var tipoMime = arquivo.ContentType;


        return tipos_mime_permitidos.Contains(tipoMime);
    }

}

public class TreeDtoRes
{
    public string Name { get; set; } = string.Empty;
    public string Fruit { get; set; } = string.Empty;
    public string SciName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string UrlBinPhoto { get; set; } = string.Empty;

    public static TreeDtoRes ToTreeDtoRes(Tree t, string url_bin)
    {
        return new TreeDtoRes
        {
            Name = t.GetName(),
            Fruit = t.GetFruit(),
            SciName = t.GetSciName(),
            Description = t.GetDescription(),
            UrlBinPhoto = url_bin
        };
    }
}

    public class UserDTOReq
    {
        public string EmailCurrent { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }