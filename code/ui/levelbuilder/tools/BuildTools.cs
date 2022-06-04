using System;
using System.Collections.Generic;

using Sandbox.UI;

namespace TerryBros.UI.LevelBuilder
{
    [UseTemplate]
    public class BuildTools : Panel
    {
        public Panel Wrapper { get; set; }

        public BuildTools()
        {
            IEnumerable<Type> buildToolTypes = TypeLibrary.GetTypes<Tools.BuildTool>();

            List<Tools.BuildTool> buildTools = new();

            foreach (Type type in buildToolTypes)
            {
                if (type.IsAbstract || type.ContainsGenericParameters)
                {
                    continue;
                }

                buildTools.Add(TypeLibrary.Create<Tools.BuildTool>(type));
            }

            buildTools.Sort((x, y) => x.Priority.CompareTo(y.Priority));

            foreach (Tools.BuildTool buildTool in buildTools)
            {
                Wrapper.AddChild(buildTool);
            }
        }
    }
}
