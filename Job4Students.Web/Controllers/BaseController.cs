using Microsoft.AspNetCore.Mvc;

namespace Job4Students.Web.Controllers;

public abstract class BaseController : Controller
{
    protected string? SaveFile(IFormFile? file, string folder, string[] allowedExtensions, long maxBytes)
    {
        if (file == null || file.Length == 0)
        {
            return null;
        }

        string ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(ext) || file.Length > maxBytes)
        {
            return null;
        }

        string webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        string targetDir = Path.Combine(webRoot, "uploads", folder);
        Directory.CreateDirectory(targetDir);

        string fileName = folder + "_" + Guid.NewGuid().ToString("N") + ext;
        string path = Path.Combine(targetDir, fileName);

        using (var stream = System.IO.File.Create(path))
        {
            file.CopyTo(stream);
        }

        return "/uploads/" + folder + "/" + fileName;
    }
}
