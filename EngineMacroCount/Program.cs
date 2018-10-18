using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace EngineMacroCount
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await FindUsages("C:\\Users\\Malte\\Desktop\\UnrealEngine");
            
            await FindUsages("C:\\Users\\Malte\\Desktop\\Godot");
            
            await FindUsages("C:\\Users\\Malte\\Desktop\\Lumberyard");
            
            Console.Read();
        }
        
        static async Task FindUsages(string engineDir)
        {
            var codeExtensions = new List<string> {".h", ".hpp", ".c", ".cpp"};

            var started = DateTime.Now;
            
            var allFiles = Directory.GetFiles(engineDir, "*.*", SearchOption.AllDirectories);
            var codeFiles = allFiles.Where(file => codeExtensions.Contains(Path.GetExtension(file))).ToList();
            
            var lines = 0;
            var defines = 0;
            var ifdefs = 0;
            var ifs = 0;

            var defineRegex = new Regex("#define", RegexOptions.Compiled);
            var ifdefRegex = new Regex("#ifdef", RegexOptions.Compiled);
            var ifRegex = new Regex("#if", RegexOptions.Compiled);


            foreach (var file in codeFiles)
            {
                var content = await File.ReadAllTextAsync(file);

                Interlocked.Add(ref lines, content.Split(new[]{'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries).Length);
                Interlocked.Add(ref defines, defineRegex.Matches(content).Count);
                Interlocked.Add(ref ifdefs, ifdefRegex.Matches(content).Count);
                Interlocked.Add(ref ifs, ifRegex.Matches(content).Count);
            }
            
            Console.WriteLine($"Total lines: {lines}");
            Console.WriteLine($"Total defines: {defines}");
            Console.WriteLine($"Total ifdefs: {ifdefs}");
            Console.WriteLine($"Total ifs: {ifs}");
            Console.WriteLine($"Time elapsed: {DateTime.Now.Subtract(started).TotalSeconds} seconds");
        }
    }
}