
using System.Drawing;


namespace UD.Helper
{
    public static class TypeHelper
    {
        public static BuildingType FromString(string? value)
        {
            return value?.ToLowerInvariant() switch
            {
                "sports_hall" => BuildingType.SportsHall,
                "library" => BuildingType.Library,
                "service" => BuildingType.Service,
                "theatre" => BuildingType.Theatre,
                "construction" => BuildingType.UnderConstruction,
                "bar" => BuildingType.Hotel,
                "place_of_worship" => BuildingType.Religious,
                "residential" => BuildingType.Residential,
                "apartments" => BuildingType.Apartments,
                "house" => BuildingType.House,
                "detached" => BuildingType.Detached,
                "dormitory" => BuildingType.Dormitory,
                "terrace" => BuildingType.Terrace,
                "commercial" => BuildingType.Commercial,
                "retail" => BuildingType.Retail,
                "office" => BuildingType.Office,
                "industrial" => BuildingType.Industrial,
                "warehouse" => BuildingType.Warehouse,
                "kiosk" => BuildingType.Kiosk,
                "garage" => BuildingType.Garage,
                "carport" => BuildingType.Carport,
                "parking" => BuildingType.Parking,
                "school" => BuildingType.School,
                "university" => BuildingType.University,
                "hospital" => BuildingType.Hospital,
                "church" => BuildingType.Church,
                "cathedral" => BuildingType.Cathedral,
                "temple" => BuildingType.Temple,
                "mosque" => BuildingType.Mosque,
                "synagogue" => BuildingType.Synagogue,
                "public" => BuildingType.Public,
                "hotel" => BuildingType.Hotel,
                "train_station" => BuildingType.TrainStation,
                "transportation" => BuildingType.Transportation,
                "civic" => BuildingType.Civic,
                "stadium" => BuildingType.SportsFacility,
                "fuel" => BuildingType.Transportation,
                "college" => BuildingType.School,
                "restaurant" => BuildingType.Hotel,
                "museum" => BuildingType.Museum,

                _ => BuildingType.Unknown
            };
        }


        public static BuildingType FindBuildingType(Dictionary<string, string>? props)
        {
            if (props == null)
                return BuildingType.Unknown;

            if (props.TryGetValue("building", out var buildingVal))
            {
                var type = FromString(buildingVal);

                if (type is not BuildingType.Unknown)
                    return type;

                if (buildingVal != "no")
                {
                    if (props.TryGetValue("amenity", out string amenityVal))
                    {
                        type = FromString(amenityVal);
                        return type;

                    }

                    if (props.TryGetValue("tourism", out string tourismVal))
                    {
                        type = FromString(tourismVal);
                        return type;
                    }

                    if (props.TryGetValue("landuse", out string landuseVal))
                    {
                        type = FromString(landuseVal);
                        return type;
                    }


                    if (props.TryGetValue("office", out string officeVal))
                    {
                        type = FromString(officeVal);
                        return type;
                    }


                    if (props.TryGetValue("shop", out string shopVal))
                    {
                        type = FromString(shopVal);
                        return type;
                    }
                }
         
            }

            return BuildingType.Unknown;
        }

        public static GreenSpaceType FindGreenSpaceType(Dictionary<string, string>? props)
        {
            if (props == null)
                return GreenSpaceType.Unknown;

            if (props.TryGetValue("landuse", out var landuseVal))
            {
                switch (landuseVal.ToLowerInvariant())
                {
                    case "forest":
                        return GreenSpaceType.Forest;
                    case "recreation_ground":
                        return GreenSpaceType.Recreation;
                    case "grass":
                        return GreenSpaceType.Grass;
                    case "cemetery":
                        return GreenSpaceType.Cemetery;
                    case "meadow":
                        return GreenSpaceType.Meadow;
                    case "nature_reserve":
                        return GreenSpaceType.NatureReserve;
                }
            }

            if (props.TryGetValue("leisure", out var leisureVal))
            {
                switch (leisureVal.ToLowerInvariant())
                {
                    case "park":
                        return GreenSpaceType.Park;
                    case "garden":
                        return GreenSpaceType.Garden;
                    case "playground":
                        return GreenSpaceType.Playground;
                    case "nature_reserve":
                        return GreenSpaceType.NatureReserve;
                }
            }

            if (props.TryGetValue("natural", out var naturalVal))
            {
                switch (naturalVal.ToLowerInvariant())
                {
                    case "wood":
                        return GreenSpaceType.Wood;
                }
            }

            return GreenSpaceType.Unknown;
        }

        public static HighwayType FindHighwayType(Dictionary<string, string>? props)
        {
            if (props == null || !props.TryGetValue("highway", out var highwayVal))
                return HighwayType.Unknown;

            return highwayVal.ToLowerInvariant() switch
            {
                "motorway" => HighwayType.Motorway,
                "trunk" => HighwayType.Trunk,
                "primary" => HighwayType.Primary,
                "secondary" => HighwayType.Secondary,
                "tertiary" => HighwayType.Tertiary,
                "unclassified" => HighwayType.Unclassified,
                "residential" => HighwayType.Residential,
                "service" => HighwayType.Service,
                "living_street" => HighwayType.LivingStreet,
                "pedestrian" => HighwayType.Pedestrian,
                "track" => HighwayType.Track,
                "busway" => HighwayType.Busway,
                "bus_guideway" => HighwayType.BusGuideway,
                "escape" => HighwayType.Escape,
                "raceway" => HighwayType.Raceway,
                "road" => HighwayType.Road,
                "footway" => HighwayType.Footway,
                "bridleway" => HighwayType.Bridleway,
                "steps" => HighwayType.Steps,
                "path" => HighwayType.Path,
                "cycleway" => HighwayType.Cycleway,
                "corridor" => HighwayType.Corridor,
                "construction" => HighwayType.Construction,
                "proposed" => HighwayType.Proposed,
                _ => HighwayType.Unknown
            };
        }

        public static WaterBodyType FindWaterBodyType(Dictionary<string, string>? props)
        {
            if (props == null)
                return WaterBodyType.Unknown;

            if (props.TryGetValue("natural", out var naturalVal))
            {
                switch (naturalVal.ToLowerInvariant())
                {
                    case "water":
                        // We'll refine using other tags below
                        break;
                    case "bay": return WaterBodyType.Bay;
                    case "strait": return WaterBodyType.Strait;
                }
            }

            if (props.TryGetValue("waterway", out var waterwayVal))
            {
                switch (waterwayVal.ToLowerInvariant())
                {
                    case "riverbank": return WaterBodyType.River;
                    case "canal": return WaterBodyType.Canal;
                    case "stream": return WaterBodyType.Stream;
                }
            }

            if (props.TryGetValue("landuse", out var landuseVal))
            {
                if (landuseVal.ToLowerInvariant() == "reservoir")
                    return WaterBodyType.Reservoir;
            }

            if (props.TryGetValue("water", out var waterVal))
            {
                switch (waterVal.ToLowerInvariant())
                {
                    case "lake": return WaterBodyType.Lake;
                    case "pond": return WaterBodyType.Pond;
                    case "reservoir": return WaterBodyType.Reservoir;
                }
            }

            // Fallback: generic water area
            if (props.TryGetValue("natural", out var natVal) && natVal.ToLowerInvariant() == "water")
            {
                return WaterBodyType.Sea;
            }

            return WaterBodyType.Unknown;
        }

        public static LanduseType GetLanduseTypeFromTags(Dictionary<string, string> tags)
        {
            if (tags == null || !tags.TryGetValue("landuse", out var landuseValue))
                return LanduseType.Unknown;

            return ParseLanduse(landuseValue.ToLowerInvariant());
        }



        public static Color GetBuildingTypeColor(this BuildingType type)
        {
            switch (type)
            {
                // 🏠 Residential
                case BuildingType.Residential:
                case BuildingType.Apartments:
                case BuildingType.House:
                case BuildingType.Detached:
                case BuildingType.Dormitory:
                case BuildingType.Terrace:
                    return Color.FromArgb(255, 236, 179); // Soft Cream

                // 🏨 Hospitality
                case BuildingType.Hotel:
                case BuildingType.Hostel:
                    return Color.FromArgb(255, 204, 153); // Peach

                // 🛍️ Commercial / Retail
                case BuildingType.Commercial:
                case BuildingType.Retail:
                case BuildingType.Shop:
                case BuildingType.Mall:
                case BuildingType.Supermarket:
                case BuildingType.Kiosk:
                case BuildingType.MarketPlace:
                    return Color.FromArgb(255, 223, 0); // Golden Yellow

                // 🏢 Office / Business
                case BuildingType.Office:
                    return Color.FromArgb(0, 153, 204); // Sky Blue

                // 🏭 Industrial
                case BuildingType.Industrial:
                case BuildingType.Warehouse:
                case BuildingType.Factory:
                case BuildingType.Garage:
                case BuildingType.Carport:
                case BuildingType.Hangar:
                case BuildingType.Silo:
                case BuildingType.StorageTank:
                case BuildingType.BoilerHouse:
                case BuildingType.Digester:
                case BuildingType.ReservoirCovered:
                    return Color.FromArgb(191, 144, 80); // Industrial Brown

                // 🧠 Education
                case BuildingType.School:
                case BuildingType.University:
                case BuildingType.Kindergarten:
                    return Color.FromArgb(170, 222, 119); // Fresh Green

                // 🏥 Health
                case BuildingType.Hospital:
                case BuildingType.Clinic:
                    return Color.FromArgb(255, 128, 128); // Soft Red

                // ⛪ Religious
                case BuildingType.Church:
                case BuildingType.Cathedral:
                case BuildingType.Mosque:
                case BuildingType.Synagogue:
                case BuildingType.Temple:
                case BuildingType.Shrine:
                case BuildingType.Monastery:
                case BuildingType.PlaceOfWorship:
                case BuildingType.Religious:
                case BuildingType.Chapel:
                    return Color.FromArgb(202, 153, 255); // Light Violet

                // 🏛️ Government / Public
                case BuildingType.Public:
                case BuildingType.Civic:
                case BuildingType.Government:
                case BuildingType.Courthouse:
                case BuildingType.Prison:
                case BuildingType.Police:
                case BuildingType.FireStation:
                case BuildingType.Embassy:
                case BuildingType.Customs:
                case BuildingType.CommunityCentre:
                    return Color.FromArgb(153, 204, 255); // Cool Blue

                // 🏟️ Sports
                case BuildingType.SportsFacility:
                case BuildingType.Stadium:
                case BuildingType.Gym:
                case BuildingType.SportsHall:
                    return Color.FromArgb(153, 102, 255); // Deep Purple

                // 🏗️ Construction / Proposals
                case BuildingType.Construction:
                case BuildingType.UnderConstruction:
                case BuildingType.Proposed:
                case BuildingType.Abandoned:
                case BuildingType.Disused:
                    return Color.FromArgb(180, 180, 180); // Neutral Gray

                // 🏰 Historic / Fortified
                case BuildingType.Historic:
                case BuildingType.Castle:
                case BuildingType.Fortress:
                case BuildingType.Fort:
                case BuildingType.Ruin:
                    return Color.FromArgb(168, 134, 100); // Sepia

                // 🚌 Transport / Infrastructure
                case BuildingType.TrainStation:
                case BuildingType.Transportation:
                case BuildingType.BusStation:
                case BuildingType.FerryTerminal:
                case BuildingType.AirportTerminal:
                case BuildingType.ControlTower:
                case BuildingType.TollBooth:
                case BuildingType.Gatehouse:
                case BuildingType.Guardhouse:
                    return Color.FromArgb(114, 174, 190); // Slate Blue

                // 🧱 Utility / Technical
                case BuildingType.PowerStation:
                case BuildingType.TransformerTower:
                case BuildingType.Substation:
                case BuildingType.WaterTower:
                case BuildingType.Windmill:
                case BuildingType.Lighthouse:
                case BuildingType.Bridge:
                case BuildingType.Tower:
                    return Color.FromArgb(160, 160, 210); // Periwinkle

                // 🚜 Agricultural / Rural
                case BuildingType.Farm:
                case BuildingType.Barn:
                case BuildingType.Stable:
                case BuildingType.Greenhouse:
                case BuildingType.Shed:
                case BuildingType.Cabin:
                case BuildingType.Hut:
                    return Color.FromArgb(203, 233, 161); // Pale Grass Green

                // 🛠️ Service / Minor Structures
                case BuildingType.Service:
                case BuildingType.Shelter:
                case BuildingType.Toilet:
                case BuildingType.Container:
                case BuildingType.Tent:
                case BuildingType.Roof:
                case BuildingType.Bunker:
                case BuildingType.Pavilion:
                    return Color.FromArgb(200, 200, 160); // Sand

                // 🪖 Military
                case BuildingType.Military:
                case BuildingType.Barracks:
                case BuildingType.TrainingArea:
                case BuildingType.AmmunitionDepot:
                case BuildingType.Nuclear:
                case BuildingType.RadarStation:
                    return Color.FromArgb(155, 100, 100); // Olive Red

                // 🎭 Culture / Leisure
                case BuildingType.Theatre:
                case BuildingType.Cinema:
                case BuildingType.Museum:
                case BuildingType.Library:
                case BuildingType.Zoo:
                case BuildingType.Aquarium:
                case BuildingType.Planetarium:
                case BuildingType.Observatory:
                    return Color.FromArgb(255, 204, 229); // Soft Pink

                // 🚗 Parking
                case BuildingType.Parking:
                    return Color.FromArgb(190, 190, 255); // Pale Blue

                default:
                    return Color.FromArgb(220, 220, 220); // Fallback Gray
            }
        }


        public static Color GetLanduseColor(this LanduseType type)
        {
            switch (type)
            {
                // 🏡 Residential
                case LanduseType.Residential:
                    return Color.FromArgb(255, 228, 196); // Light Beige

                // 🏪 Commercial / Retail
                case LanduseType.Commercial:
                case LanduseType.Retail:
                    return Color.FromArgb(255, 223, 128); // Soft Orange

                // 🏭 Industrial
                case LanduseType.Industrial:
                    return Color.FromArgb(191, 144, 80); // Industrial Brown

                // 🌲 Forest / Green
                case LanduseType.Forest:
                case LanduseType.Grass:
                case LanduseType.Meadow:
                case LanduseType.Orchard:
                case LanduseType.Vineyard:
                case LanduseType.VillageGreen:
                case LanduseType.CemeteryGreen:
                    return Color.FromArgb(170, 222, 119); // Fresh Green

                // 🚜 Agricultural
                case LanduseType.Farmland:
                case LanduseType.Farmyard:
                case LanduseType.Allotments:
                    return Color.FromArgb(203, 233, 161); // Pale Grass Green

                // ⚰️ Cemetery
                case LanduseType.Cemetery:
                    return Color.FromArgb(204, 204, 255); // Soft Lavender

                // 🪖 Military
                case LanduseType.Military:
                    return Color.FromArgb(155, 100, 100); // Olive Red

                // 🚄 Railway
                case LanduseType.Railway:
                    return Color.FromArgb(114, 174, 190); // Slate Blue

                // 🏞️ Recreation
                case LanduseType.RecreationGround:
                    return Color.FromArgb(144, 238, 144); // Light Green

                // 🪨 Quarry / Industrial Ground
                case LanduseType.Quarry:
                case LanduseType.Landfill:
                    return Color.FromArgb(189, 183, 107); // Khaki

                // 🏗️ Construction
                case LanduseType.Construction:
                case LanduseType.Greenfield:
                case LanduseType.Brownfield:
                    return Color.FromArgb(180, 180, 180); // Neutral Gray

                // 💧 Water Bodies
                case LanduseType.Basin:
                case LanduseType.Reservoir:
                    return Color.FromArgb(173, 216, 230); // Light Blue

                // ⛪ Religious landuse
                case LanduseType.Religious:
                    return Color.FromArgb(202, 153, 255); // Light Violet

                // ❓ Unknown / Default
                case LanduseType.Unknown:
                default:
                    return Color.FromArgb(220, 220, 220); // Fallback Gray
            }
        }


        public static LanduseType ParseLanduse(string? landuse)
        {
            return landuse?.ToLower() switch
            {
                "residential" => LanduseType.Residential,
                "commercial" => LanduseType.Commercial,
                "industrial" => LanduseType.Industrial,
                "retail" => LanduseType.Retail,
                "forest" => LanduseType.Forest,
                "farmland" => LanduseType.Farmland,
                "farmyard" => LanduseType.Farmyard,
                "grass" => LanduseType.Grass,
                "meadow" => LanduseType.Meadow,
                "orchard" => LanduseType.Orchard,
                "vineyard" => LanduseType.Vineyard,
                "cemetery" => LanduseType.Cemetery,
                "allotments" => LanduseType.Allotments,
                "military" => LanduseType.Military,
                "railway" => LanduseType.Railway,
                "recreation_ground" => LanduseType.RecreationGround,
                "quarry" => LanduseType.Quarry,
                "landfill" => LanduseType.Landfill,
                "construction" => LanduseType.Construction,
                "greenfield" => LanduseType.Greenfield,
                "brownfield" => LanduseType.Brownfield,
                "basin" => LanduseType.Basin,
                "reservoir" => LanduseType.Reservoir,
                "religious" => LanduseType.Religious,
                _ => LanduseType.Unknown,
            };
        }


        public static TransportationType FindTransportationType(Dictionary<string, string>? props)
        {
            if (props == null)
                return TransportationType.Unknown;

            if (props.TryGetValue("railway", out var railwayVal))
            {
                switch (railwayVal.ToLowerInvariant())
                {
                    case "rail":
                        return TransportationType.Railway;
                    case "subway":
                        return TransportationType.Subway;
                    case "light_rail":
                        return TransportationType.LightRail;
                    case "tram":
                        return TransportationType.Tram;
                    case "monorail":
                        return TransportationType.Monorail;
                }
            }

            if (props.TryGetValue("route", out var routeVal))
            {
                switch (routeVal.ToLowerInvariant())
                {
                    case "bus":
                        return TransportationType.Busway;
                    case "ferry":
                        return TransportationType.FerryTerminal;
                    case "train":
                        return TransportationType.TrainStation;
                    case "subway":
                        return TransportationType.SubwayStation;
                    case "tram":
                        return TransportationType.TramStop;
                }
            }

            if (props.TryGetValue("aerialway", out var aerialwayVal))
            {
                switch (aerialwayVal.ToLowerInvariant())
                {
                    case "cable_car":
                        return TransportationType.CableCar;
                    case "chair_lift":
                        return TransportationType.ChairLift;
                    case "gondola":
                        return TransportationType.Gondola;
                    case "funicular":
                        return TransportationType.Funicular;
                }
            }

            if (props.TryGetValue("public_transport", out var ptVal))
            {
                switch (ptVal.ToLowerInvariant())
                {
                    case "platform":
                        return TransportationType.Platform;
                    case "stop_position":
                        return TransportationType.StopPosition;
                    case "stop_area":
                        return TransportationType.StopArea;
                }
            }

            if (props.TryGetValue("amenity", out var amenityVal))
            {
                switch (amenityVal.ToLowerInvariant())
                {
                    case "bus_station":
                        return TransportationType.BusStation;
                    case "ferry_terminal":
                        return TransportationType.FerryTerminal;
                    case "airport":
                        return TransportationType.Airport;
                    case "heliport":
                        return TransportationType.Heliport;
                }
            }

            if (props.TryGetValue("highway", out var highwayVal))
            {
                switch (highwayVal.ToLowerInvariant())
                {
                    case "bus_stop":
                        return TransportationType.BusStop;
                    case "bus_guideway":
                        return TransportationType.BusGuideway;
                }
            }

            return TransportationType.Unknown;
        }


    }


}
