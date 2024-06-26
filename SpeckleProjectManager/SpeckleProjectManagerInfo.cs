﻿using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace SpeckleProjectManager
{
    public class SpeckleProjectManagerInfo : GH_AssemblyInfo
    {
        public override string Name => "SpeckleProjectManager";

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon => null;

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "";

        public override Guid Id => new Guid("1a7eb485-4c73-4d8c-9d4b-8237d2d1b245");

        //Return a string identifying you or your company.
        public override string AuthorName => "";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "";
    }
}