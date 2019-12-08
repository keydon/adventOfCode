using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;

namespace day08
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("==== Part 1 ====");
            var stopwatch = Stopwatch.StartNew();
            var encodedImage = File
                .ReadAllText("input.txt");

            var width = 25;
            var height = 6;
            var pixels = width * height;

            Console.WriteLine("Total pixels: {0}", encodedImage.Length);
            Console.WriteLine("Total Layers: {0}", encodedImage.Length / pixels);
            var layers = DivideIntoLayers(encodedImage, pixels);

            Layer layerWithFewestZeros = layers
                .OrderBy(l => l.ImageData.Count(d => d == 0))
                .First();

            int ones = layerWithFewestZeros.ImageData.Count(d => d == 1);
            int twos = layerWithFewestZeros.ImageData.Count(d => d == 2);
            
            stopwatch.Stop();
            Console.WriteLine("Layer with fewest zeros: {0}", layerWithFewestZeros.LayerNumber);
            Console.WriteLine("Ones[{0}] x Twos[{1}]: {2}", ones, twos, ones*twos);
            Console.WriteLine("Calculation took: {0}", stopwatch.Elapsed);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            
            Console.WriteLine("==== Part 2 ====");
            stopwatch.Start();
            var canvas = new int[width, height];
            layers
                .OrderByDescending(l => l.LayerNumber)
                .SelectMany(l => l.ImageData.Select((pixel, index) => new {
                        X = index % width, 
                        Y = index / width, 
                        Pixel = pixel 
                    }))
                .Where(p => p.Pixel != 2)
                .ToList()
                .ForEach(p => canvas[p.X, p.Y] = p.Pixel);

            canvas.ToConsole(x => x == 0 ? " ": "█");

            stopwatch.Stop();
            Console.WriteLine("Calculation took: {0}", stopwatch.Elapsed);
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        private static IEnumerable<Layer> DivideIntoLayers(string imgdata, int pixels)
        {
            return imgdata
                .ToList()
                .Chunk(pixels)
                .Select((pixelsOfALayer, index) => new Layer(){
                    LayerNumber = index+1,
                    ImageData = pixelsOfALayer.Select(c => c.ToInt()).ToArray()
            });
        }
    }

    public class Layer{
        public int LayerNumber {get; set;}
        public int[] ImageData {get; set;}
    }
}
public static class Extensions
{
    public static void ToConsole<T>(this T[,] canvas, Func<T, string> tranform = null){
        var innerTransform = tranform ?? new Func<T, string>( (x) => x.ToString());
        
        Console.WriteLine($"\r\n[{canvas.GetUpperBound(0)+1},{canvas.GetUpperBound(1)+1}]");
        for (int y = 0; y <= canvas.GetUpperBound(1); y++)
        {
            for (int x = 0; x <= canvas.GetUpperBound(0); x++)
            {
                Console.Write(innerTransform(canvas[x,y]));
            }
            Console.Write("\r\n");
        }
        Console.Write("\r\n");
    }

    public static IEnumerable<string> Splizz(this string str, params string[] seps){
        if(seps == null || seps.Length == 0)
            seps = new[]{";", ","};
        return str.Split(seps,StringSplitOptions.RemoveEmptyEntries).Select(e => e.Trim());
    }
    public static IEnumerable<IEnumerable<string>> Splizz(this IEnumerable<string> enumerable, params string[] seps){
        foreach (var item in enumerable)
        {
            yield return item.Splizz(seps);
        }
    }

    public static int ToInt(this char character){
        return int.Parse(character.ToString());
    }
}
public static class EnumerableExtensions
{

  public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunkSize)
  {
     if (chunkSize == 0)
        throw new ArgumentNullException();

     var enumer = source.GetEnumerator();
     while (enumer.MoveNext())
     {
        yield return Take(enumer.Current, enumer, chunkSize);
     }
  }

  private static IEnumerable<T> Take<T>(T head, IEnumerator<T> tail, int chunkSize)
  {
     while (true)
     {
        yield return head;
        if (--chunkSize == 0)
           break;
        if (tail.MoveNext())
           head = tail.Current;
        else
           break;
     }
  }
}


