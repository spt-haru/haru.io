using System;
using System.IO;
using System.Reflection;
using Haru.Pools;

namespace Haru.IO
{
    public class Resx
    {
        private readonly Assembly _assembly;
        private readonly string[] _paths;

        public Resx(Assembly assembly)
        {
            _assembly = assembly;
            _paths = assembly.GetManifestResourceNames();
            // note: if something ends with 'name.<langcode>.json', it will
            //       generate a sattelite assembly. Rename to
            //       'name-<landcode>.json' to fix it.
        }

        public Stream GetStream(string filepath)
        {
            var name = _assembly.GetName().Name;
            var fullpath = $"{name}.embedded.{filepath}";

            if (Array.IndexOf(_paths, fullpath) == -1)
            {
                throw new FileNotFoundException($"Cannot find resource {filepath}");
            }

            return _assembly.GetManifestResourceStream(fullpath);
        }

        public ReadOnlySpan<byte> GetData(string path)
        {
            var ms = MemoryStreamPool.Rent();

            try
            {
                using (var rs = GetStream(path))
                {
                    rs.CopyTo(ms);   
                }

                return ms.ToArray();
            }
            finally
            {
                MemoryStreamPool.Return(ms);
            }
        }

        public string GetText(string path)
        {
            using (var rs = GetStream(path))
            {
                using (var sr = new StreamReader(rs))
                {
                    return sr.ReadToEnd();
                }
            }
        }
    }
}
