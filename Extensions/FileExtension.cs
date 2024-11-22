namespace UniqloMvc.Extensions;

public static class FileExtension
{
    public static bool IsValidType(this IFormFile file, string contentType) => file.ContentType.StartsWith(contentType);

    public static bool IsValidSize(this IFormFile file, int kb) => file.Length <= kb * 1024;

    public static async Task<string> Upload(this IFormFile file, string path)
    {
        string newFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        using (Stream stream = System.IO.File.Create(Path.Combine(path, newFileName)))
        {
            await file.CopyToAsync(stream);
        }

        return newFileName;
    }

    public static async Task<string> Upload(this IFormFile file, string path, string oldFileNameWithExtension)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        File.Delete(Path.Combine(path, oldFileNameWithExtension));
        using (Stream stream = System.IO.File.Create(Path.Combine(path, oldFileNameWithExtension)))
        {
            await file.CopyToAsync(stream);
        }

        return oldFileNameWithExtension;
    }
}
