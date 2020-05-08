using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zwyssigly.ImageServer
{
    public class IdTest
    {
        [Test]
        public void ToStringWorks()
        {
            var sut = new GuidGenerator().Generate().Result;

            Assert.AreEqual(22, sut.ToString().Length);
        }

        [Test]
        public void FromStringWorks()
        {
            var sut = new GuidGenerator().Generate().Result;

            Assert.AreEqual(sut, Id.FromString(sut.ToString()).UnwrapOrThrow());
        }

        [Test]
        public void EqualsWorks()
        {
            var a1 = new GuidGenerator().Generate().Result;
            var a2 = Id.FromString(a1.ToString()).UnwrapOrThrow();
            var b = new GuidGenerator().Generate().Result;

            Assert.AreEqual(a1, a2);
            Assert.AreEqual(a1.GetHashCode(), a2.GetHashCode());

            Assert.AreNotEqual(a1, b);
            Assert.AreNotEqual(a1.GetHashCode(), b.GetHashCode());
        }
    }
}
