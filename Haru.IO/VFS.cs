using System;
using System.Collections.Generic;
using System.IO;
using Haru.Pools;

namespace Haru.IO
{
    public class VFS
    {
        public static FileStream GetFileReadStream(string filepath)
        {
            return new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public static FileStream GetFileWriteStream(string filepath, FileMode mode = FileMode.Create)
        {
            return new FileStream(filepath, mode, FileAccess.ReadWrite, FileShare.Read);
        }

        public static string CombinePath(string a, string b)
        {
            return Path.Combine(a, b);
        }

        public static bool Exists(string filepath)
        {
            return File.Exists(filepath) || Directory.Exists(filepath);
        }

        public static string[] GetDirectories(string filepath)
        {
            return Directory.GetDirectories(filepath);
        }

        public static void CreateDirectory(string filepath)
        {
            var path = Path.GetDirectoryName(filepath);
            Directory.CreateDirectory(path);
        }

        public static string[] GetFiles(string filepath)
        {
            return Directory.GetFiles(filepath);
        }

        public static string GetFileName(string filepath)
        {
            return Path.GetFileNameWithoutExtension(filepath);
        }

        public static string GetFileExtension(string filepath)
        {
            return Path.GetExtension(filepath);
        }

        public static long GetFileSize(string filepath)
        {
            using (var fs = GetFileReadStream(filepath))
            {
                return fs.Length;
            }
        }

        public static List<string> GetFilesRecursive(string filepath, List<string> files = null)
        {
            if (files == null)
            {
                files = new List<string>();
            }

            foreach (var file in GetFiles(filepath))
            {
                files.Add(file);
            }

            foreach (var directory in GetDirectories(filepath))
            {
                files = GetFilesRecursive(directory, files);
            }

            return files;
        }

        public static void WriteText(string filepath, string text, bool append = false)
        {
            if (!Exists(filepath))
            {
                CreateDirectory(filepath);
            }

            var mode = append ? FileMode.Append : FileMode.Create;

            using (var fs = GetFileWriteStream(filepath, mode))
            {
                using (var sr = new StreamWriter(fs))
                {
                    sr.WriteLine(text);
                }
            }
        }

        public static void WriteBytes(string filepath, ReadOnlySpan<byte> data)
        {
            if (!Exists(filepath))
            {
                CreateDirectory(filepath);
            }

            using (var fs = GetFileWriteStream(filepath))
            {
                fs.Write(data);
            }
        }

        public static string ReadText(string filepath)
        {
            if (!Exists(filepath))
            {
                throw new FileNotFoundException(filepath);
            }

            using (var fs = GetFileReadStream(filepath))
            {
                using (var sr = new StreamReader(fs))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        public static ReadOnlySpan<byte> ReadBytes(string filepath)
        {
            if (!Exists(filepath))
            {
                throw new FileNotFoundException(filepath);
            }

            using (var fs = GetFileReadStream(filepath))
            {
                var ms = MemoryStreamPool.Rent();

                try
                {
                    fs.CopyTo(ms);
                    return ms.ToArray();
                }
                finally
                {
                    MemoryStreamPool.Return(ms);
                }
            }
        }
    }
}