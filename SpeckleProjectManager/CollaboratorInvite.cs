using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Speckle.Core.Api;
using Speckle.Core.Credentials;
using Speckle.Core.Logging;
using SpeckleProjectManager.Properties;

namespace SpeckleProjectManager
{
    public class CollaboratorInvite : GH_Component
    {
        public CollaboratorInvite()
          : base(
            "CollaboratorInvite",
            "cAdd",
            "Adds collaborator to a stream",
            "Speckle 2",
            "Project"
          )
        { }

        public override Guid ComponentGuid => new Guid("66c83825-ec80-439f-9d7f-c14d2f121453");
        protected override Bitmap Icon => Resources.addContributor2;
        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Stream", "S", "Unique ID of the stream to be updated.", GH_ParamAccess.item);
            pManager.AddTextParameter("CollaboratorEmail", "Cs", "User to add as collaborator in this stream", GH_ParamAccess.item);

        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {

            pManager.AddTextParameter("Message", "M", "Speckle message", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {

            string ghSpeckleStream = null;
            var collaborator = "";
            Speckle.Core.Api.Stream stream = new Speckle.Core.Api.Stream();



            if (!DA.GetData(0, ref ghSpeckleStream))
            {
                return;
            }
            DA.GetData(1, ref collaborator);



            var streamWrapper = new StreamWrapper(ghSpeckleStream);


            Task.Run(async () =>
            {
                try
                {
                    var account = await streamWrapper.GetAccount();
                    var client = new Client(account);

                    var res = await client.StreamInviteCreate(
                      new StreamInviteCreateInput
                      {
                          streamId = ghSpeckleStream,
                          email = collaborator,
                          message = "Whasssup!"
                      }
                    );
                }



                catch (Exception exception)
                {
                    DA.SetData(1, $"{nameof(Exception)}: {exception.Message}");
                }
            }).Wait();
        }



    }
}