using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Haru.IO;

namespace Haru.IO.Tests.Units
{
    [TestClass]
    public class ResxTest
    {
        private readonly Resx _resx;
        private const string _file = "test.txt";
        private const string _text = "Hello, world!";
        private readonly byte[] _data;

        public ResxTest()
        {
            _resx = new Resx(GetType().Assembly);
            _data = Encoding.UTF8.GetBytes(_text);
        }

        [TestMethod]
        public void TestGetStream()
        {
            byte[] data;

            using (var ms = new MemoryStream())
            {
                using (var rs = _resx.GetStream(_file))
                {
                    rs.CopyTo(ms);
                }

                data = ms.ToArray();
            }

            var result = data.SequenceEqual(_data);
            Assert.IsTrue(result);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void TestGetStreamException()
        {
            using (var rs = _resx.GetStream("not.existing.file.txt"))
            {
            }
        }

        [TestMethod]
        public void TestGetText()
        {
            var data = _resx.GetText(_file);
            var result = (data == _text);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestGetData()
        {
            var data = _resx.GetData(_file);
            var result = data.SequenceEqual(_data);
            Assert.IsTrue(result);
        }
    }
}