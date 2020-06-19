using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using System.Threading.Tasks;

namespace An.Image
{
    public class Util
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="d"></param>
        public static async Task ToPngAsync(string img, string save)
        {
            using var image = SixLabors.ImageSharp.Image.Load(img);
            //image.Mutate(c => c.Resize(width, height).GaussianSharpen());
            await image.SaveAsync(save, new PngEncoder());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="img"></param>
        /// <param name="save"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static async Task ToPngAsync(string img, string save, int width, int height)
        {
            using var image = SixLabors.ImageSharp.Image.Load(img);
            image.Mutate(c => c.Resize(width, height).GaussianSharpen());
            await image.SaveAsync(save, new PngEncoder());
        }




    }
}
