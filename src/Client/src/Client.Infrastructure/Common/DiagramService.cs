using Blazor.Diagrams;
using Blazor.Diagrams.Core.Anchors;
using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Models.Base;
using Blazor.Diagrams.Core.PathGenerators;
using Blazor.Diagrams.Core.Routers;
using Blazor.Diagrams.Options;
using Showmatics.Blazor.Client.Infrastructure.ApiClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Showmatics.Blazor.Client.Infrastructure.Common;
public class DiagramService
{
    private BlazorDiagram Diagram { get; set; } = null!;
    public DiagramService()
    {
        Diagram = new BlazorDiagram();
        Init();
    }

    public void Init()
    {
        var options = new BlazorDiagramOptions
        {
            AllowMultiSelection = false,
            Zoom =
            {
                Enabled = true,
            },
            Links =
            {
                DefaultRouter = new OrthogonalRouter(),
                DefaultPathGenerator = new SmoothPathGenerator(),
                //DefaultColor = "#000000",
            },
            Constraints =
            {
                ShouldDeleteNode = async (NodeModel node) => await ValueTask.FromResult(false),
                ShouldDeleteLink = async (BaseLinkModel model) => await ValueTask.FromResult(false),
            },
        };

        Diagram = new BlazorDiagram(options);
    }

    public BlazorDiagram GetProcessDiagram(UpdateProcessRequest process)
    {
        var createdNodes = new List<NodeResult>();
        var lastPosition = new Point(50, 50);
        foreach (var stage in process.ProcessStages.OrderBy(x => x.IndexNo))
        {
            var currentNode = Diagram.Nodes.FirstOrDefault(n => n.Title == stage.Name);
            if (currentNode == null)
            {
                currentNode = new NodeModel(position: lastPosition)
                {
                    Title = stage.Name,
                };
                Diagram.Nodes.Add(currentNode);
            }

            if (stage.OnSuccessStageId.HasValue)
            {
                var successStage = process.ProcessStages.FirstOrDefault(s => s.Id == stage.OnSuccessStageId);
                if (successStage == null)
                {
                    continue;
                }

                var targetNode = Diagram.Nodes.FirstOrDefault(n => n.Title == successStage.Name);
                if (targetNode == null)
                {
                    targetNode = new NodeModel(position: lastPosition)
                    {
                        Title = successStage.Name,
                    };
                    Diagram.Nodes.Add(targetNode);
                }

                var sourceBottomPort = currentNode.AddPort(PortAlignment.Right);
                var targetBottomPort = targetNode.AddPort(PortAlignment.Left);
                sourceBottomPort.Size = new Size(10, 10);
                targetBottomPort.Size = new Size(10, 10);

                currentNode.Position = new Point(targetNode.Position.X + 200, targetNode.Position.Y);
                var sourceAnchor = new SinglePortAnchor(sourceBottomPort);
                var targetAnchor = new SinglePortAnchor(targetBottomPort);

                var link = Diagram.Links.Add(new LinkModel(sourceAnchor, targetAnchor));
                link.AddLabel(stage.OnSuccessActionText ?? "Success");
                link.Color = "var(--mud-palette-success)";
                link.SourceMarker = LinkMarker.Circle;
                link.TargetMarker = LinkMarker.Arrow;
            }

            if (stage.OnFailureStageId.HasValue)
            {
                var failureStage = process.ProcessStages.FirstOrDefault(s => s.Id == stage.OnFailureStageId);
                if (failureStage == null)
                {
                    continue;
                }

                var targetNode = Diagram.Nodes.FirstOrDefault(n => n.Title == failureStage.Name);
                if (targetNode == null)
                {
                    targetNode = new NodeModel(position: lastPosition)
                    {
                        Title = failureStage.Name
                    };
                    Diagram.Nodes.Add(targetNode);
                }

                var sourceBottomPort = currentNode.AddPort(PortAlignment.Bottom);
                var targetBottomPort = targetNode.AddPort(PortAlignment.Bottom);
                sourceBottomPort.Size = new Size(10, 10);
                targetBottomPort.Size = new Size(10, 10);

                currentNode.Position = new Point(targetNode.Position.X + 200, targetNode.Position.Y);
                var sourceAnchor = new SinglePortAnchor(sourceBottomPort);
                var targetAnchor = new SinglePortAnchor(targetBottomPort);

                var link = Diagram.Links.Add(new LinkModel(sourceAnchor, targetAnchor));
                link.AddLabel(stage.OnFailureActionText ?? "Fail");
                link.Color = "var(--mud-palette-error)";
                link.SourceMarker = LinkMarker.Circle;
                link.TargetMarker = LinkMarker.Arrow;
            }
        }

        return Diagram;
    }
}

public class NodeResult : NodeModel
{
    public NodeModel Node { get; set; } = default!;
    public PortModel Top { get; set; } = default!;
    public PortModel TopRight { get; set; } = default!;
    public PortModel Right { get; set; } = default!;
    public PortModel BottomRight { get; set; } = default!;
    public PortModel Bottom { get; set; } = default!;
    public PortModel BottomLeft { get; set; } = default!;
    public PortModel Left { get; set; } = default!;
    public PortModel TopLeft { get; set; } = default!;

}
