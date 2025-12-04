using MAF.Core.Domain.Exceptions;

namespace MAF.Core.Domain;

public class User
{
    private string Email { get; } = string.Empty;
    private string Password { get; } = string.Empty;
    private string Name { get; } = string.Empty;
    private DateTime CreatedAt { get; }
    public const int MAX_NAME = 10;
    public const int MIN_NAME = 4;
    public const int MIN_PW = 8;

    public User(string name, string email, string password)
    {
        try
        {
            if (ValidEmail(email) && ValidPassword(password) && ValidName(name))
            {
                Email = email;
                Password = password;
                Name = name;
                CreatedAt = DateTime.Now;
            }

        }
        catch{throw;}
    }
    public static bool ValidEmail(string email)
    {
        if (!(!string.IsNullOrEmpty(email) && email.Contains('@')))
        {
            throw new InvalidEmailException();
        }

        return true;
    }
    public static bool ValidPassword(string password)
    {
        if (!(!string.IsNullOrEmpty(password) && (password.Length >= MIN_PW)))
        {
            
            throw new PasswordOutOfRangeException();
        }

        return true;
    }
    public static bool ValidName(string name)
    {
        if (!(!string.IsNullOrEmpty(name) && name.Length <= MAX_NAME && name.Length >= MIN_NAME))
        {
            
            throw new NameOutOfRangeException();
        }

        return true;
    }
    public string GetName() { return Name; }
    public string GetEmail() { return Email; }
    public string GetPassword() { return Password; }
    public DateTime GetCreateAt() { return CreatedAt; }

}

public class Location
{
    private double Latitude { get; }
    private double Longitude { get; }

    public Location(double lat, double lon)
    {
        Latitude = lat;
        Longitude = lon;
    }

    public double GetLatitude() { return Latitude; }
    public double GetLongitude() { return Longitude; }
}

public class Photo : IDisposable
{
    private Stream StreamBytes { get; set; }
    private string NameFile { get; set; } = string.Empty;
    private string ContentType { get; }
    private string ExtensionFile { get; }
    public Photo(Stream stream, string content_type, string ext_file)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream), "O Stream de conteúdo não pode ser nulo.");
        }
        else if (content_type == null)
        {
            throw new ArgumentNullException(nameof(content_type), "O ContentType do arquivo não pode ser nulo.");
        }
        else if (ext_file == null)
        {
            throw new ArgumentNullException(nameof(ext_file), "A extensão do arquivo não pode ser nulo.");
        }

        StreamBytes = stream;
        ContentType = content_type;
        ExtensionFile = ext_file;

        if (StreamBytes.CanSeek)
        {
            StreamBytes.Position = 0;
        }
    }

    public void SetNameFile(string name_photo, DateTime data_time)
    {
        NameFile = $"{name_photo}_{data_time:dd-MM-yyyy_HHmmss}{ExtensionFile}";
    }
    public void SetStreamBytes(Stream s) { StreamBytes = s; }


    public string GetNameFile() { return NameFile; }
    public string GetContentType() { return ContentType; }
    public Stream GetStreamBytes() { return StreamBytes; }
    
    public void Dispose()
    {
        StreamBytes?.Dispose();
    }

}


/*Exceptions Tree:
    NameOutOfRangeException
    FruitNameOutOfRangeException
    SciNameOutOfRangeException
    DescrOutOfRangeException
    ArgumentNullException

    Obs.: Lembre-se de usar a diretiva using pois manipulo dados com o 'Stream'
*/
public class Tree : IDisposable
{
    private string Name { get; } = string.Empty;
    private string Fruit { get; } = string.Empty;
    private string SciName { get; } = string.Empty;
    private string FkEmail { get; } = string.Empty;
    private string Description { get; } = string.Empty;
    private Location Location { get; }
    private Photo Photo { get; }
    private DateTime CreatedAt { get; }
    public bool IsValid;
    public const int MAX_NAME_TREE = 20;
    public const int MAX_DESCR_TREE = 50;

    public Tree(string name, string fruit, string email_user, string sci_name, Location location, string description, Photo photo)
    {
        try
        {
            if (location == null)
            {
                throw new ArgumentNullException(nameof(location), "location não pode ser nulo.");
            }
            else if (photo == null)
            {
                throw new ArgumentNullException(nameof(photo), "photo não pode ser nulo.");
            }
            Location = location;
            Photo = photo;

            if (ValidSizeStrTree(name, 0) && ValidSizeStrTree(fruit, 1) && ValidSizeStrTree(sci_name, 2) && ValidSizeStrTree(description, 3))
            {
                Name = name;
                Fruit = fruit;
                SciName = sci_name;
                FkEmail = email_user;
                Description = description;
                CreatedAt = DateTime.Now;
                Photo.SetNameFile(name, CreatedAt);
                IsValid = false;
            }
        }
        catch { throw; }
    }

    public static bool ValidSizeStrTree(string str, int id_attrb)
    {
        switch (id_attrb)
        {
            case 0:
                if (!(!string.IsNullOrEmpty(str) && (str.Length <= MAX_NAME_TREE)))
                {
                    throw new NameOutOfRangeException();
                }
                break;

            case 1:
                if (!(!string.IsNullOrEmpty(str) && (str.Length <= MAX_NAME_TREE)))
                {
                    throw new FruitNameOutOfRangeException();
                }
                break;

            case 2:
                if (!(str.Length <= MAX_NAME_TREE))
                {
                    throw new SciNameOutOfRangeException();
                }
                break;

            case 3:
                if (!(str.Length <= MAX_DESCR_TREE))
                {
                    throw new DescrOutOfRangeException();
                }
                break;

            default:
                Console.WriteLine("id não existe");
                break;
        }

        return true;
    }
    public string GetName() { return Name; }
    public string GetFruit() { return Fruit; }
    public string GetSciName() { return SciName; }
    public string GetFkEmail() { return FkEmail; }
    public string GetDescription() { return Description; }
    public Photo GetPhoto() { return Photo; }
    public Location GetLocation() { return Location; }
    public DateTime GetCreateAt() { return CreatedAt; }

    public void Dispose()
    {
        Photo.Dispose();
    }
}

//Aventura está lá fora, e não nos seus livros e mapas :) estou ficando louco