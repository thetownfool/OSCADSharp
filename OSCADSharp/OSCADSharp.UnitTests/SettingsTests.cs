﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSCADSharp.Files;
using OSCADSharp.Scripting;
using OSCADSharp.Solids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSCADSharp.UnitTests
{
    [TestClass]
    public class SettingsTests
    {
        [TestMethod]
        public void Settings_NullVariablesDoNothing()
        {
            var cube = new Cube();
            Variables.Global["thing"] = null;
            string[] output = null;

            var mock = new Mock<IFileWriter>();
            mock.Setup(_wrtr => _wrtr.WriteAllLines(It.IsAny<string>(), It.IsAny<string[]>()))
                .Callback<string, string[]>((path, contents) => { output = contents; });
            Settings.FileWriter = mock.Object;

            cube.ToFile("myFile");

            Assert.AreEqual("", output[1]);
        }
    }
}