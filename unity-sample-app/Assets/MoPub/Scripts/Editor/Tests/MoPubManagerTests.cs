using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{

    public class MoPubManagerTests : MoPubTest
    {
        [Test]
        public void DecodeArgsWithNullShouldErrorAndYieldEmptyList()
        {
            var res = MoPubManager.DecodeArgs(null, 0);

            LogAssert.Expect(LogType.Error, "Invalid JSON data: ");
            Assert.That(res, Is.Not.Null);
            Assert.That(res.Length, Is.Zero);
        }

        [Test]
        public void DecodeArgsWithInvalidShouldErrorAndYieldEmptyList()
        {
            var res = MoPubManager.DecodeArgs("{\"a\"]", 0);

            LogAssert.Expect(LogType.Error, "Invalid JSON data: {\"a\"]");
            Assert.That(res, Is.Not.Null);
            Assert.That(res.Length, Is.Zero);
        }

        [Test]
        public void DecodeArgsWithValueShouldYieldListWithValue()
        {
            var res = MoPubManager.DecodeArgs("[\"a\"]", 0);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Length, Is.EqualTo(1));
            Assert.That(res[0], Is.EqualTo("a"));
        }

        [Test]
        public void DecodeArgsWithoutMinimumValuesShouldErrorAndYieldListWithDesiredLength()
        {
            var res = MoPubManager.DecodeArgs("[\"a\", \"b\"]", 3);

            LogAssert.Expect(LogType.Error, "Missing one or more values: [\"a\", \"b\"] (expected 3)");
            Assert.That(res, Is.Not.Null);
            Assert.That(res.Length, Is.EqualTo(3));
            Assert.That(res[0], Is.EqualTo("a"));
            Assert.That(res[1], Is.EqualTo("b"));
            Assert.That(res[2], Is.EqualTo(""));
        }

        [Test]
        public void DecodeArgsWithExpectedValuesShouldYieldListWithDesiredValues()
        {
            var res = MoPubManager.DecodeArgs("[\"a\", \"b\", \"c\"]", 3);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Length, Is.EqualTo(3));
            Assert.That(res[0], Is.EqualTo("a"));
            Assert.That(res[1], Is.EqualTo("b"));
            Assert.That(res[2], Is.EqualTo("c"));
        }
    }
}
