
using UD.GeoJson;

namespace UD.Helper
{
    public class TransportationModel : Feature
    {
        // ✅ Computed property to extract enum
        [JsonIgnore]
        public TransportationType TransportationType => TypeHelper.FindTransportationType(Properties);
    }
}
