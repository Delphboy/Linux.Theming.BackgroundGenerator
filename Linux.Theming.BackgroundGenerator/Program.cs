using System;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Text;
using System.Drawing;

namespace Linux.Theming.BackgroundGenerator
{
    class Program
    {
        private static string[] _colours;
        private static int _width = 0;
        private static int _height = 0;

        public static async Task Main(string[] args)
        {
            const string errorMsg = "The program requires 3 parameters: width, height, and theme.json location";
            if(args.Length < 3) throw new ArgumentException(errorMsg);

            int.TryParse(args[0], out _width);
            int.TryParse(args[1], out _height);
            var themeLocation = args[2];

            if(_width == 0 || _height == 0 || themeLocation == string.Empty) throw new ArgumentException(errorMsg);

            _colours = await readThemeJsonFile(themeLocation);
            var image = generateBackgroundImage(_width, _height, _colours);
            image.Save("image.png");
        }

        public Program(int width, int height, string[] colours)
        {
            _width = width;
            _height = height;
            _colours = colours;
        }

        public static async Task<string[]> readThemeJsonFile(string fileLocation)
        {
            if(!File.Exists(fileLocation))
            {
                return new string[]{"#000000", "#FFFFFF"};
            }

            using(var fs = File.OpenRead(fileLocation))
            {
                return await JsonSerializer.DeserializeAsync<string[]>(fs);
            }
        }

        public static Bitmap generateBackgroundImage(int width, int height, string[] colours)
        {
            var image = new Bitmap(width, height);

            var colour = randomColour();

            // Set background
            for(var x = 0; x < image.Width; x++)
            {
                for(var y = 0; y < image.Height; y++)
                {
                    image.SetPixel(x, y, ColorTranslator.FromHtml(colour));
                }
            }

            //Fill Circles
            var circleCount = randomNumber(Math.Abs(_height - _width) / 16);

            for(int i = 0; i < circleCount; i++)
            {
                using(Graphics graphics = Graphics.FromImage(image))
                {
                    using(Brush brush = new SolidBrush(ColorTranslator.FromHtml(randomColour())))
                    {
                        var centre = randomPoint();
                        var radius = randomNumber(_width);
                        graphics.FillEllipse(brush, centre.X, centre.Y, radius, radius);
                    }
                }
            }

            return image;
        }

        private static string randomColour()
        {
            var rand = new Random();
            var num = rand.Next(0, _colours.Length);
            return _colours[num];
        }

        private static Point randomPoint()
        {
            var x = randomNumber(_width);
            var y = randomNumber(_height);
            return new Point(x,y);
        }

        private static int randomNumber(int max)
        {
            var rand = new Random();
            return rand.Next(max);
        }

    }
}
