using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace aoc
{
    record TreeNode
    {
        public TreeNode Parent {get; set;}

        public string Name { get; set; }

        public long Size {get; set; }

        public List<TreeNode> Subdirs { get; set; } = new List<TreeNode>();
        
        public List<TreeNode> Files { get; set; } = new List<TreeNode>();

        
        public TreeNode AddDir(string name)
        {
            if (name == "..")
                return Parent;
            
            var dir = new TreeNode
            {
                Name = name,
                Parent = this
            };
            Subdirs.Add(dir);
            return dir;
        }

        public void AddFile(string name, long size)
        {
            var file = new TreeNode
            {
                Name = name,
                Size = size,
                Parent = this
            };

            Files.Add(file);
        }

    }
    class Program
    {
        private static readonly List<(string Name, long Size)> allDirs = new();
        static void Main(string[] args)
        {
            Report.Start();
            var history = LoadCommandBrowsingHistory("input.txt");
            //history = LoadCommandBrowsingHistory("sample.txt");
            var fs = ParseIntoFilesystem(history);

            CalculateDirectorySizes(fs);

            allDirs.Where(d => d.Size <= 100000L)
                .Select(d => d.Size)
                .Sum()
                .AsResult1();

            long neededSpace = CalculateNeededSpace();
            allDirs
                .Where(d => d.Size >= neededSpace)
                .OrderBy(d => d.Size)
                .First()
                .Size.AsResult2();

            Report.End();
        }

        private static TreeNode ParseIntoFilesystem(List<string> history)
        {
            var root = new TreeNode
            {
                Name = "/"
            };
            var cwd = root;

            for (int i = 1; i < history.Count; i++)
            {
                var commandLine = history[i];
                var cmdParts = commandLine.Splizz(" ").Skip(1).ToList();
                if (cmdParts.First() == "ls") {
                    var outputLength = ParseLsOutput(cwd, history, i + 1);
                    i+= outputLength;
                    continue;
                }
                if (cmdParts.First() == "cd"){
                    var subdirName = cmdParts.Last();
                    var subdir = cwd.Subdirs.FirstOrDefault(s => s.Name == subdirName);
                    cwd = subdir ?? cwd.AddDir(subdirName);
                    continue;
                }
                throw new Exception("Unknown cmd: "  + commandLine);
            }
            return root;
        }

        private static int ParseLsOutput(TreeNode cwd, List<string> history, int pointer)
        {
            for (int pos = pointer; pos < history.Count; pos++)
            {
                var line = history[pos];
                if (line.StartsWith("$"))
                    return pos - pointer;
                var lsParts = line.Splizz(" ");
                if (lsParts.First() == "dir")
                    cwd.AddDir(lsParts.Last());
                else
                    cwd.AddFile(lsParts.Last(), long.Parse(lsParts.First()));
            }
            return history.Count;
        }
        private static long CalculateNeededSpace()
        {
            var fulldiskSpace = 70000000L;
            var requiredSpace = 30000000L;
            var usedSpace = allDirs.Where(d => d.Name == "/").Single().Size;

            var freeSpace = fulldiskSpace - usedSpace;
            long neededSpace = requiredSpace - freeSpace;

            return neededSpace;
        }

        private static (string Name, long Size) CalculateDirectorySizes(TreeNode cwd)
        {
            var filesSizeSum = cwd.Files.Select(f => f.Size).Sum();
            
            var subdirSizeSum = cwd.Subdirs
                .Select(d => CalculateDirectorySizes(d))
                .Select(d => d.Size)
                .Sum();
           
            var res = (cwd.Name, filesSizeSum + subdirSizeSum);
            allDirs.Add(res);
            return res;
        }

        public static List<string> LoadCommandBrowsingHistory(string inputTxt)
        {
            return File
                .ReadAllLines(inputTxt)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .ToList();
        }
    }
}
