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
        public async static Task GenerateThumbnailForImageAsync(string imagePath, string? outputPath = null)
        {
            if (outputPath == null)
            {
                var fileName = Path.GetFileName(imagePath);
                var fileParentFolder = GetParentFolder(imagePath);

                Directory.CreateDirectory(@$"{fileParentFolder}\thumbnails");
                outputPath = @$"{fileParentFolder}\thumbnails\{fileName}.jpg";
            }


            try
            {
                using (var originalImage = SLImage.Load(imagePath))
                {
                    await GenAndSaveImpl(originalImage, outputPath);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not open image file: {e}");
            }
        }

        /// <summary>
        /// Generate thumbnail for given image
        /// </summary>
        /// <param name="imageStream">FileStream to the image the thumbnail should be generated for</param>
        /// <param name="outputPath">Optional output path for the thumbnail. If not specified, the thumbnail will be saved with the same name in the "thumbnail" subdirectory.</param>
        /// <returns></returns>
        public async static Task GenerateThumbnailForImageAsync(FileStream imageStream, string? outputPath = null)
        {
            if (outputPath == null)
            {
                var fileName = Path.GetFileName(imageStream.Name);
                var fileParentFolder = GetParentFolder(imageStream.Name);

                Directory.CreateDirectory(@$"{fileParentFolder}\thumbnails");
                outputPath = @$"{fileParentFolder}\thumbnails\{fileName}.jpg";
            }


            try
            {
                using (var originalImage = SLImage.Load(imageStream))
                {
                    await GenAndSaveImpl(originalImage, outputPath);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not open image file: {e}");
            }
        }

        private async static Task GenAndSaveImpl(SLImage img, string outputPath)
        {
            var (w, h) = img.Size;
            var tgtSize = w > h ? new Size(64, 0) : new Size(0, 64);
            img.Mutate(x => x.Resize(tgtSize));
            try
            {
                await img.SaveAsJpegAsync(outputPath);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not save thumbnail image at {outputPath}: {e}");
            }
        }
    }
}