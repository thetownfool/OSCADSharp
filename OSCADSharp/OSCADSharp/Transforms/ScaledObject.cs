﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSCADSharp.Transforms
{
    /// <summary>
    /// An object that's been rescaled
    /// </summary>
    internal class ScaledObject : OSCADObject
    {
        /// <summary>
        /// The scale factor to be applied
        /// </summary>
        internal Vector3 ScaleFactor { get; set; } = new Vector3(1, 1, 1);
        private OSCADObject obj;

        /// <summary>
        /// Creates a scaled object
        /// </summary>
        /// <param name="obj">Object(s) to be scaled</param>
        /// <param name="scale">Scale factor in x/y/z components</param>
        internal ScaledObject(OSCADObject obj, Vector3 scale)
        {
            this.obj = obj;
            this.ScaleFactor = scale;
        }

        public override string ToString()
        {
            string scaleCommand = String.Format("scale(v = [{0}, {1}, {2}])",
                this.ScaleFactor.X.ToString(), this.ScaleFactor.Y.ToString(), this.ScaleFactor.Z.ToString());
            var formatter = new BlockFormatter(scaleCommand, this.obj.ToString());
            return formatter.ToString();
        }
    }
}