

using System.Text.Json;

namespace UrbanDesign.Ui.Models
{
    public record ParcellationEventModel(string Id, CommandAction Command, JsonElement? Payload);

    public record ParcelSize(double ParcelDepth,double ParcelWidth);

    public record RoadWidth(double Width);

    public record ParcelArea(double Area);

    public record MinMax(double Min,double Max);

    public record HidePacellation(bool Hide);

    public record AIMessage(string message);
}
