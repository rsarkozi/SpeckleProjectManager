using Grasshopper.Kernel;
using System;
using Speckle.Core.Api;
using Speckle.Core.Credentials;
using Speckle.Core.Logging;
using System.Threading.Tasks;
using SpeckleProjectManager.Properties;

namespace SpeckleProjectManager
{
    public class BranchCreatorComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public BranchCreatorComponent()
          : base("BranchCreatorComponent", "bCreate",
            "Creates a branch on a speckle stream",
            "Speckle 2", "Project")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("StreamUrlOrId", "StreamUrlOrId", "[string]", GH_ParamAccess.item);
            pManager.AddTextParameter("BranchName", "BranchName", "[string]", GH_ParamAccess.item);
            pManager.AddTextParameter("Description", "Description", "[string]", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Run", "Run", "[boolean]", GH_ParamAccess.item);

            pManager[3].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Result", "Result", "The result of the branch creation", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var streamUrlOrId = "";
            var branchName = "";
            var description = "";
            var run = false;

            if (!DA.GetData(0, ref streamUrlOrId)) return;
            if (!DA.GetData(1, ref branchName)) return;
            DA.GetData(2, ref description);
            DA.GetData(3, ref run);

            if (!run) return;

            var streamWrapper = new StreamWrapper(streamUrlOrId);

            if (streamWrapper == null)
            {
                DA.SetData(0, "Stream evaluated to null before branch evaluation! Check your stream id / url input.");
                return;
            }

            Task.Run(async () =>
            {
                try
                {
                    var account = await streamWrapper.GetAccount();
                    var client = new Client(account);

                    try
                    {
                        var result = await client.BranchCreate(new BranchCreateInput
                        {
                            streamId = streamWrapper.StreamId,
                            name = branchName,
                            description = description
                        });

                        DA.SetData(0, $"Created new branch with streamId: {result}");
                    }
                    catch (SpeckleException speckleException)
                    {
                        DA.SetData(0, $"{nameof(SpeckleException)}: {speckleException.Message}. - says Speckle. Branch almost certainly exists.");
                    }
                }
                catch (Exception exception)
                {
                    DA.SetData(0, $"{nameof(Exception)}: {exception.Message}");
                }
            }).Wait();
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Resources.addBranch2;

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("c8cf82d3-4d5d-40d5-9708-9580a3ee4458");
    }
}