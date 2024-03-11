using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SLImage = SixLabors.ImageSharp.Image;

namespace FileAPI.Image
{
    public abstract class Thumbnail
    {
        public static string GetParentFolder(string filePath)
        {
            return Path.GetFullPath(Path.GetDirectoryName(filePath)!);
        }

        /// <summary>
        /// Generate thumbnail for given image
        /// </summary>
        /// <param name="imagePath">Path to the image the thumbnail should be generated for</param>
        /// <param name="outputPath">Optional output path for the thumbnail. If not specified, the thumbnail will be saved with the same name in the "thumbnail" subdirectory.</param>
        /// <returns></returns>
        public async static Task<bool> GenerateThumbnailForImageAsync(string imagePath, string? outputPath = null)
        {
            try
            {
                using (var originalImage = SLImage.Load(imagePath))
                {
                    return await GenAndSaveImpl(originalImage, outputPath ?? GetOutputPath(imagePath));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not open image file at path {imagePath}: {e}");
                return false;
            }
        }

        /// <summary>
        /// Generate thumbnail for given image
        /// </summary>
        /// <param name="imageStream">FileStream to the image the thumbnail should be generated for</param>
        /// <param name="outputPath">Optional output path for the thumbnail. If not specified, the thumbnail will be saved with the same name in the "thumbnail" subdirectory.</param>
        /// <returns></returns>
        public async static Task<bool> GenerateThumbnailForImageAsync(FileStream imageStream, string? outputPath = null)
        {
            try
            {
                using (var originalImage = SLImage.Load(imageStream))
                {
                    return await GenAndSaveImpl(originalImage, outputPath ?? GetOutputPath(imageStream.Name));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not open image file stream: {e}");
                return false;
            }
        }

        private static string GetOutputPath(string originalFilePath)
        {
            var fileName = Path.GetFileName(originalFilePath);
            var fileParentFolder = GetParentFolder(originalFilePath);

            Directory.CreateDirectory(@$"{fileParentFolder}\thumbnails");
            return @$"{fileParentFolder}\thumbnails\{fileName}.jpg";
        }

        private async static Task<bool> GenAndSaveImpl(SLImage img, string outputPath)
        {
            var (w, h) = img.Size;
            var tgtSize = w > h ? new Size(64, 0) : new Size(0, 64);
            img.Mutate(x => x.Resize(tgtSize));
            try
            {
                await img.SaveAsJpegAsync(outputPath);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not save thumbnail image at {outputPath}: {e}");
                return false;
            }
        }
    }
}