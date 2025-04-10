

using System.Text.Json;
using UrbanDesign.Ui;

namespace UrbanDesign.Models
{
    public record WebViewEventModel(string Id, CommandAction Command, JsonElement? Payload);

    public record ParcelSize(double ParcelDepth, double ParcelWidth);

    public record RoadWidth(double Width);

    public record ParcelArea(double Area);

    public record MinMax(double Min, double Max);

    public record HidePacellation(bool Hide);

    public record AIMessage(string message);
}
