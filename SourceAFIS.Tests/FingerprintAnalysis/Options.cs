﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using NUnit.Framework;
using White.Core;
using White.Core.Factory;
using White.Core.UIItems;
using White.Core.UIItems.WindowItems;
using White.Core.UIItems.ListBoxItems;
using White.Core.UIItems.Finders;

namespace SourceAFIS.Tests.FingerprintAnalysis
{
    [TestFixture]
    public class Options : Common
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            StartApp();
        }

        [Test]
        public void ClickThrough()
        {
            SelectFiles();
            ResetOptions();
            LayerChoice.SelectSlowly("MinutiaMask");
            SkeletonChoice.SelectSlowly("Valleys");
            MaskChoice.SelectSlowly("Segmentation");
            Contrast.Checked = true;
            Orientation.Checked = true;
            Minutiae.Checked = true;
            Paired.Checked = true;
        }

        [TearDown]
        public void TearDown()
        {
            Thread.Sleep(300);
            Assert.AreEqual(0, Win.ModalWindows().Count);
            Assert.IsFalse(App.HasExited);
        }
    }
}
