namespace Parcellation_API.Models;

public class ParcellationOutput
{
    public int Id { get; set; }

    public string OutputData { get; set; } = "";

    public int InputId { get; set; }
    public ParcellationInput? Input { get; set; }
}
