namespace Job4Students.Models;

public static class FileUploadOptions
{
    public static readonly string[] ImageExtensions = new string[] { ".jpg", ".jpeg", ".png", ".gif" };
    public static readonly string[] CvExtensions = new string[] { ".pdf", ".doc", ".docx" };
    public const long MaxCvBytes = 5 * 1024 * 1024;
    public const long MaxImageBytes = 3 * 1024 * 1024;
}
