
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

        #region color coding
        public static Color GetBuildingColor(this BuildingType type)
        {
            switch (type)
            {
                case BuildingType.Residential:
                case BuildingType.Apartments:
                case BuildingType.House:
                case BuildingType.Detached:
                case BuildingType.Dormitory:
                case BuildingType.Terrace:
                    return ColorTranslator.FromHtml("#FFECB3");

                case BuildingType.Hotel:
                case BuildingType.Hostel:
                    return ColorTranslator.FromHtml("#FFCC99");

                case BuildingType.Commercial:
                case BuildingType.Retail:
                case BuildingType.Shop:
                case BuildingType.Mall:
                case BuildingType.Supermarket:
                case BuildingType.Kiosk:
                case BuildingType.MarketPlace:
                    return ColorTranslator.FromHtml("#FFDF00");

                case BuildingType.Office:
                    return ColorTranslator.FromHtml("#0099CC");

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
                    return ColorTranslator.FromHtml("#BF9050");

                case BuildingType.School:
                case BuildingType.University:
                case BuildingType.Kindergarten:
                    return ColorTranslator.FromHtml("#AADE77");

                case BuildingType.Hospital:
                case BuildingType.Clinic:
                    return ColorTranslator.FromHtml("#FF8080");

                case BuildingType.Church:
                case BuildingType.Mosque:
                case BuildingType.Synagogue:
                case BuildingType.Temple:
                case BuildingType.Shrine:
                case BuildingType.Monastery:
                case BuildingType.PlaceOfWorship:
                case BuildingType.Religious:
                case BuildingType.Chapel:
                    return ColorTranslator.FromHtml("#CA99FF");

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
                    return ColorTranslator.FromHtml("#99CCFF");

                case BuildingType.SportsFacility:
                case BuildingType.Stadium:
                case BuildingType.Gym:
                case BuildingType.SportsHall:
                    return ColorTranslator.FromHtml("#9966FF");

                case BuildingType.Construction:
                case BuildingType.UnderConstruction:
                case BuildingType.Proposed:
                case BuildingType.Abandoned:
                case BuildingType.Disused:
                    return ColorTranslator.FromHtml("#B4B4B4");

                case BuildingType.Historic:
                case BuildingType.Castle:
                case BuildingType.Fortress:
                case BuildingType.Fort:
                case BuildingType.Ruin:
                case BuildingType.Ruins:
                    return ColorTranslator.FromHtml("#A88664");

                case BuildingType.TrainStation:
                case BuildingType.BusStation:
                case BuildingType.FerryTerminal:
                case BuildingType.AirportTerminal:
                case BuildingType.TollBooth:
                case BuildingType.ControlTower:
                case BuildingType.Gatehouse:
                case BuildingType.Guardhouse:
                    return ColorTranslator.FromHtml("#72AEBE");

                case BuildingType.PowerStation:
                case BuildingType.Substation:
                case BuildingType.WaterTower:
                case BuildingType.Windmill:
                case BuildingType.Lighthouse:
                case BuildingType.Bridge:
                case BuildingType.Tower:
                    return ColorTranslator.FromHtml("#A0A0D2");

                case BuildingType.Farm:
                case BuildingType.Barn:
                case BuildingType.Stable:
                case BuildingType.Greenhouse:
                case BuildingType.Shed:
                case BuildingType.Cabin:
                    return ColorTranslator.FromHtml("#CBE9A1");

                case BuildingType.Shelter:
                case BuildingType.Toilet:
                case BuildingType.Container:
                case BuildingType.Tent:
                case BuildingType.Roof:
                case BuildingType.Bunker:
                case BuildingType.Pavilion:
                    return ColorTranslator.FromHtml("#C8C8A0");

                case BuildingType.Parking:
                    return ColorTranslator.FromHtml("#BEBEFF");

                case BuildingType.Theatre:
                case BuildingType.Museum:
                case BuildingType.Cinema:
                case BuildingType.Library:
                case BuildingType.Observatory:
                case BuildingType.Aquarium:
                case BuildingType.Planetarium:
                case BuildingType.Zoo:
                    return ColorTranslator.FromHtml("#FFCCE5");

                default:
                    return Color.FromArgb(220, 220, 220); // fallback gray
            }
        }

        public static Color GetHighwayColor(this HighwayType type)
        {
            switch (type)
            {
                case HighwayType.Motorway:
                    return Color.FromArgb(200, 0, 0);
                case HighwayType.Primary:
                    return Color.FromArgb(255, 128, 0);
                case HighwayType.Secondary:
                    return Color.FromArgb(255, 191, 0);
                case HighwayType.Tertiary:
                    return Color.FromArgb(255, 255, 0);
                case HighwayType.Residential:
                    return Color.FromArgb(204, 204, 204);
                case HighwayType.Service:
                case HighwayType.Track:
                case HighwayType.Unclassified:
                    return Color.FromArgb(190, 190, 190);
                case HighwayType.Footway:
                case HighwayType.Pedestrian:
                case HighwayType.LivingStreet:
                    return Color.FromArgb(180, 180, 255);
                case HighwayType.Path:
                case HighwayType.Cycleway:
                    return Color.FromArgb(153, 204, 255);
                case HighwayType.Construction:
                case HighwayType.Proposed:
                    return Color.FromArgb(160, 160, 160);
                default:
                    return Color.FromArgb(220, 220, 220);
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

        public static Color GetGreenSpaceColor(this GreenSpaceType type)
        {
            switch (type)
            {
                case GreenSpaceType.Park:
                case GreenSpaceType.Garden:
                case GreenSpaceType.Recreation:
                    return Color.FromArgb(144, 238, 144);

                case GreenSpaceType.Forest:
                case GreenSpaceType.Grass:
                case GreenSpaceType.Wood:
                case GreenSpaceType.Meadow:
                    return Color.FromArgb(170, 222, 119);

                case GreenSpaceType.Playground:
                    return Color.FromArgb(255, 235, 205);

                case GreenSpaceType.Cemetery:
                    return Color.FromArgb(204, 204, 255);

                case GreenSpaceType.NatureReserve:
                    return Color.FromArgb(100, 200, 100);

                default:
                    return Color.FromArgb(220, 220, 220);
            }
        }

        public static Color GetTransportationColor(this TransportationType type)
        {
            switch (type)
            {
                case TransportationType.Railway:
                case TransportationType.TrainStation:
                    return Color.FromArgb(114, 174, 190);

                case TransportationType.BusStop:
                case TransportationType.BusStation:
                case TransportationType.Busway:
                case TransportationType.BusGuideway:
                    return Color.FromArgb(180, 140, 255);

                case TransportationType.Subway:
                case TransportationType.SubwayStation:
                    return Color.FromArgb(255, 102, 102);

                case TransportationType.FerryTerminal:
                    return Color.FromArgb(100, 149, 237);

                case TransportationType.Airport:
                case TransportationType.AirportTerminal:
                case TransportationType.Heliport:
                    return Color.FromArgb(200, 200, 255);

                case TransportationType.CableCar:
                case TransportationType.ChairLift:
                case TransportationType.Gondola:
                case TransportationType.Funicular:
                case TransportationType.Aerialway:
                    return Color.FromArgb(153, 204, 255);

                default:
                    return Color.FromArgb(220, 220, 220);
            }
        }

        public static Color GetWaterBodyColor(this WaterBodyType type)
        {
            switch (type)
            {
                case WaterBodyType.Sea:
                case WaterBodyType.Bay:
                case WaterBodyType.Strait:
                    return Color.FromArgb(100, 149, 237); // Cornflower Blue

                case WaterBodyType.River:
                case WaterBodyType.Canal:
                case WaterBodyType.Stream:
                    return Color.FromArgb(0, 191, 255); // Deep Sky Blue

                case WaterBodyType.Lake:
                case WaterBodyType.Pond:
                    return Color.FromArgb(173, 216, 230); // Light Blue

                case WaterBodyType.Reservoir:
                    return Color.FromArgb(135, 206, 250); // Sky Blue

                default:
                    return Color.FromArgb(220, 220, 220);
            }
        }


        #endregion




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
