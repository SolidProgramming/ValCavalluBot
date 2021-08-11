using HtmlAgilityPack;
using Shares.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HowrseBotClient.Class
{
    public static class HorsePersonalization
    {
        public static string GetSpriteBase64(HowrseBotModel bot)
        {
            var html = bot.HTMLActions.CurrentHtml;

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var normalSpriteNode = htmlDoc.DocumentNode.SelectSingleNode("//figure[@class='cheval-icone  js-horse-image horsesprite horsesprite--300']");
            var godSpriteNode = htmlDoc.DocumentNode.SelectSingleNode("//img[contains(@id, 'cheval-robe-')]|//img[@id='cheval-image']");
            ////table[contains(@class, 'training-table-summary')]

            if (normalSpriteNode != null)
            {
                //is normal horse sprite
                var spriteUrls = Regex.Matches(normalSpriteNode.OuterHtml, "url\\((.*?)\\)")
                .Cast<Match>()
                .Select(m => "https://" +bot.Settings.Server + "/" + m.Groups[1])
                .ToArray();

                string base64Sprite = BitmapToBase64(CombineSprites(DownloadSprites(spriteUrls)));
                return base64Sprite;
            }
            else if (godSpriteNode != null && godSpriteNode.HasAttributes)
            {
                //is god sprite
                string[] linkToSprite = new[] { godSpriteNode.Attributes["src"].Value };

                for (int i = 0; i < linkToSprite.Length; i++)
                {
                    if (!linkToSprite[i].StartsWith($"https://{bot.Settings.Server}"))
                    {
                        linkToSprite[i] = $"https://{bot.Settings.Server}{linkToSprite[i]}";
                    }
                }

                string base64Sprite = BitmapToBase64(CombineSprites(DownloadSprites(linkToSprite)));
                return base64Sprite;
            }

            return null;
        }
        private static List<Image> DownloadSprites(string[] backgroundUrls)
        {
            var sprites = new List<Image>();

            Array.Reverse(backgroundUrls);

            using (System.Net.WebClient webClient = new())
            {
                foreach (string url in backgroundUrls)
                {
                    try
                    {
                        sprites.Add(Image.FromStream(webClient.OpenRead(url)));
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }

            }

            return sprites;
        }
        private static Bitmap CombineSprites(List<Image> sprites)
        {            
            Bitmap finalImage = null;

            try
            {
                int width = 300;
                int height = 300;

                //create a bitmap to hold the combined image
                finalImage = new Bitmap(width, height);

                //get a graphics object from the image so we can draw on it
                using (Graphics g = Graphics.FromImage(finalImage))
                {
                    //set background color
                    g.Clear(Color.White);

                    //go through each image and draw it on the final image                    
                    foreach (Bitmap image in sprites)
                    {
                        g.DrawImage(image,
                          new Rectangle(0, 0, image.Width, image.Height));

                    }
                }

                return finalImage;
            }
            catch (Exception)
            {
                if (finalImage != null)
                    finalImage.Dispose();

                return null;
            }
            finally
            {
                //clean up memory
                foreach (Bitmap image in sprites)
                {
                    image.Dispose();
                }
            }
        }
        private static string BitmapToBase64(Image image)
        {
            MemoryStream memory = new();
            image.Save(memory, ImageFormat.Bmp);
            string base64 = Convert.ToBase64String(memory.ToArray());
            memory.Close();
            memory.Dispose();
            return base64;
        }
    }
}
