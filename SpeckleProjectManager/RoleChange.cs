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
    public class RoleChange : GH_Component
    {
        public RoleChange()
          : base(
            "Change Role",
            "cRole",
            "changes role",
            "Speckle 2",
            "Project"
          )
        { }

        public override Guid ComponentGuid => new Guid("66c83825-ec80-439f-9d7f-c14d2f121645");
        protected override Bitmap Icon => Resources.addContributor2;
        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Stream", "S", "Unique ID of the stream to be updated.", GH_ParamAccess.item);
            pManager.AddGenericParameter(
             "Collaborator",
             "Cs",
             "Users that have collaborated in this stream",
             GH_ParamAccess.item
           );
            pManager.AddTextParameter("Role", "R", "Role of the user", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter(
              "Collaborators",
              "Cs",
              "Users that have collaborated in this stream",
              GH_ParamAccess.list
            );
            pManager.AddTextParameter("Message", "M", "Speckle message", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {

            string ghSpeckleStream = null;
            var role = "";
            Speckle.Core.Credentials.Account newAccount = new Account();
            Speckle.Core.Api.Stream stream = new Speckle.Core.Api.Stream();


            if (!DA.GetData(0, ref ghSpeckleStream))
            {
                return;
            }
            DA.GetData(1, ref newAccount);
            DA.GetData(2, ref role);


            var streamWrapper = new StreamWrapper(ghSpeckleStream);



            Task.Run(async () =>
            {
                try
                {
                    var account = await streamWrapper.GetAccount();
                    var client = new Client(account);
                    var res = await client.StreamUpdatePermission(
                      new StreamPermissionInput
                      {
                          role = role,
                          streamId = ghSpeckleStream,
                          userId = newAccount.userInfo.id
                      }
                    );


                }
                catch (Exception exception)
                {
                    DA.SetData(1, $"{nameof(Exception)}: {exception.Message}");
                }
            }).Wait();


            Task.Run(async () =>
            {
                var account = await streamWrapper.GetAccount();
                var client = new Client(account);
                stream = await client.StreamGet(ghSpeckleStream);


            }).Wait();

            DA.SetDataList(0, stream.collaborators);
        }



    }
}