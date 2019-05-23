using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class MoPubBaseTests
    {
        [Test]
        public void CompareVersionsWithFirstSmaller()
        {
            Assert.That(MoPubBase.CompareVersions("0", "1"), Is.EqualTo(-1));
            Assert.That(MoPubBase.CompareVersions("0.9", "1.0"), Is.EqualTo(-1));
            Assert.That(MoPubBase.CompareVersions("0.9.99", "1.0.0"), Is.EqualTo(-1));
            Assert.That(MoPubBase.CompareVersions("0.9.99", "0.10.0"), Is.EqualTo(-1));
            Assert.That(MoPubBase.CompareVersions("0.9.99", "0.9.100"), Is.EqualTo(-1));
        }

        [Test]
        public void CompareVersionsWithFirstGreater()
        {
            Assert.That(MoPubBase.CompareVersions("1", "0"), Is.EqualTo(1));
            Assert.That(MoPubBase.CompareVersions("1.0", "0.9"), Is.EqualTo(1));
            Assert.That(MoPubBase.CompareVersions("1.0.0", "0.9.99"), Is.EqualTo(1));
            Assert.That(MoPubBase.CompareVersions("0.10.0", "0.9.99"), Is.EqualTo(1));
            Assert.That(MoPubBase.CompareVersions("0.9.100", "0.9.99"), Is.EqualTo(1));
        }

        [Test]
        public void CompareVersionsWithEqual()
        {
            Assert.That(MoPubBase.CompareVersions("1", "1"), Is.EqualTo(0));
            Assert.That(MoPubBase.CompareVersions("1.0", "1.0"), Is.EqualTo(0));
            Assert.That(MoPubBase.CompareVersions("1.0.0", "1.0.0"), Is.EqualTo(0));
        }
    }
}
