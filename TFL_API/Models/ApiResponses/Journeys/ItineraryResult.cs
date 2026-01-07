using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public sealed class ItineraryResult
{
    // TfL adds this for .NET type metadata; you usually ignore it.
    [JsonPropertyName("$type")]
    public string? Type { get; set; }

    [JsonPropertyName("journeys")]
    public List<Journey>? Journeys { get; set; }

    [JsonPropertyName("lines")]
    public List<Line>? Lines { get; set; }

    [JsonPropertyName("stopMessages")]
    public List<string>? StopMessages { get; set; }

    [JsonPropertyName("recommendedMaxAgeMinutes")]
    public int? RecommendedMaxAgeMinutes { get; set; }

    [JsonPropertyName("searchCriteria")]
    public SearchCriteria? SearchCriteria { get; set; }

    [JsonPropertyName("journeyVector")]
    public JourneyVector? JourneyVector { get; set; }
}

public sealed class Journey
{
    [JsonPropertyName("$type")]
    public string? Type { get; set; }

    [JsonPropertyName("startDateTime")]
    public DateTime? StartDateTime { get; set; }

    [JsonPropertyName("duration")]
    public int? Duration { get; set; }

    [JsonPropertyName("arrivalDateTime")]
    public DateTime? ArrivalDateTime { get; set; }

    [JsonPropertyName("alternativeRoute")]
    public bool? AlternativeRoute { get; set; }

    [JsonPropertyName("legs")]
    public List<Leg>? Legs { get; set; }

    [JsonPropertyName("fare")]
    public JourneyFare? Fare { get; set; }
}

public sealed class Leg
{
    [JsonPropertyName("$type")]
    public string? Type { get; set; }

    [JsonPropertyName("duration")]
    public int? Duration { get; set; }

    [JsonPropertyName("instruction")]
    public Instruction? Instruction { get; set; }

    [JsonPropertyName("obstacles")]
    public List<object>? Obstacles { get; set; } // empty in your sample

    [JsonPropertyName("departureTime")]
    public DateTime? DepartureTime { get; set; }

    [JsonPropertyName("arrivalTime")]
    public DateTime? ArrivalTime { get; set; }

    [JsonPropertyName("departurePoint")]
    public StopPointLite? DeparturePoint { get; set; }

    [JsonPropertyName("arrivalPoint")]
    public StopPointLite? ArrivalPoint { get; set; }

    [JsonPropertyName("path")]
    public Path1? Path { get; set; }

    [JsonPropertyName("routeOptions")]
    public List<RouteOption>? RouteOptions { get; set; }

    [JsonPropertyName("mode")]
    public Identifier? Mode { get; set; }

    [JsonPropertyName("disruptions")]
    public List<object>? Disruptions { get; set; } // empty in your sample

    [JsonPropertyName("plannedWorks")]
    public List<object>? PlannedWorks { get; set; } // empty in your sample

    [JsonPropertyName("distance")]
    public double? Distance { get; set; } // present on walking leg

    [JsonPropertyName("isDisrupted")]
    public bool? IsDisrupted { get; set; }

    [JsonPropertyName("hasFixedLocations")]
    public bool? HasFixedLocations { get; set; }

    [JsonPropertyName("scheduledDepartureTime")]
    public DateTime? ScheduledDepartureTime { get; set; }

    [JsonPropertyName("scheduledArrivalTime")]
    public DateTime? ScheduledArrivalTime { get; set; }
}

public sealed class Instruction
{
    [JsonPropertyName("$type")]
    public string? Type { get; set; }

    [JsonPropertyName("summary")]
    public string? Summary { get; set; }

    [JsonPropertyName("detailed")]
    public string? Detailed { get; set; }

    [JsonPropertyName("steps")]
    public List<InstructionStep>? Steps { get; set; }
}

public sealed class InstructionStep
{
    [JsonPropertyName("$type")]
    public string? Type { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("turnDirection")]
    public string? TurnDirection { get; set; }

    [JsonPropertyName("streetName")]
    public string? StreetName { get; set; }

    [JsonPropertyName("distance")]
    public int? Distance { get; set; }

    [JsonPropertyName("cumulativeDistance")]
    public int? CumulativeDistance { get; set; }

    [JsonPropertyName("skyDirection")]
    public int? SkyDirection { get; set; }

    [JsonPropertyName("skyDirectionDescription")]
    public string? SkyDirectionDescription { get; set; }

    [JsonPropertyName("cumulativeTravelTime")]
    public int? CumulativeTravelTime { get; set; }

    [JsonPropertyName("latitude")]
    public double? Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double? Longitude { get; set; }

    [JsonPropertyName("descriptionHeading")]
    public string? DescriptionHeading { get; set; }

    [JsonPropertyName("trackType")]
    public string? TrackType { get; set; }

    [JsonPropertyName("travelTime")]
    public int? TravelTime { get; set; }
}

public sealed class StopPointLite
{
    [JsonPropertyName("$type")]
    public string? Type { get; set; }

    [JsonPropertyName("naptanId")]
    public string? NaptanId { get; set; }

    [JsonPropertyName("icsCode")]
    public string? IcsCode { get; set; }

    [JsonPropertyName("individualStopId")]
    public string? IndividualStopId { get; set; }

    [JsonPropertyName("commonName")]
    public string? CommonName { get; set; }

    [JsonPropertyName("placeType")]
    public string? PlaceType { get; set; }

    [JsonPropertyName("lat")]
    public double? Lat { get; set; }

    [JsonPropertyName("lon")]
    public double? Lon { get; set; }
}

public sealed class Path1
{
    [JsonPropertyName("$type")]
    public string? Type { get; set; }

    [JsonPropertyName("lineString")]
    public string? LineString { get; set; }

    [JsonPropertyName("stopPoints")]
    public List<Identifier>? StopPoints { get; set; }

    [JsonPropertyName("elevation")]
    public List<object>? Elevation { get; set; } // empty in your sample
}

public sealed class RouteOption
{
    [JsonPropertyName("$type")]
    public string? Type { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("directions")]
    public List<string>? Directions { get; set; }

    [JsonPropertyName("lineIdentifier")]
    public Identifier? LineIdentifier { get; set; }

    [JsonPropertyName("direction")]
    public string? Direction { get; set; }
}

/// <summary>
/// TfL reuses this "Identifier" shape all over: id/name/uri/type/etc.
/// </summary>
public sealed class Identifier
{
    [JsonPropertyName("$type")]
    public string? TypeMetadata { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("uri")]
    public string? Uri { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("routeType")]
    public string? RouteType { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("motType")]
    public string? MotType { get; set; }

    [JsonPropertyName("network")]
    public string? Network { get; set; }

    [JsonPropertyName("crowding")]
    public object? Crowding { get; set; } // usually empty object
}

public sealed class JourneyFare
{
    [JsonPropertyName("$type")]
    public string? Type { get; set; }

    [JsonPropertyName("totalCost")]
    public int? TotalCost { get; set; }

    [JsonPropertyName("fares")]
    public List<Fare>? Fares { get; set; }

    [JsonPropertyName("caveats")]
    public List<FareCaveat>? Caveats { get; set; }
}

public sealed class Fare
{
    [JsonPropertyName("$type")]
    public string? Type { get; set; }

    [JsonPropertyName("lowZone")]
    public int? LowZone { get; set; }

    [JsonPropertyName("highZone")]
    public int? HighZone { get; set; }

    [JsonPropertyName("cost")]
    public int? Cost { get; set; }

    [JsonPropertyName("chargeProfileName")]
    public string? ChargeProfileName { get; set; }

    [JsonPropertyName("isHopperFare")]
    public bool? IsHopperFare { get; set; }

    [JsonPropertyName("chargeLevel")]
    public string? ChargeLevel { get; set; }

    [JsonPropertyName("peak")]
    public int? Peak { get; set; }

    [JsonPropertyName("offPeak")]
    public int? OffPeak { get; set; }

    [JsonPropertyName("taps")]
    public List<FareTap>? Taps { get; set; }
}

public sealed class FareTap
{
    [JsonPropertyName("$type")]
    public string? Type { get; set; }

    [JsonPropertyName("atcoCode")]
    public string? AtcoCode { get; set; }

    [JsonPropertyName("tapDetails")]
    public FareTapDetails? TapDetails { get; set; }
}

public sealed class FareTapDetails
{
    [JsonPropertyName("$type")]
    public string? Type { get; set; }

    [JsonPropertyName("modeType")]
    public string? ModeType { get; set; }

    [JsonPropertyName("validationType")]
    public string? ValidationType { get; set; }

    [JsonPropertyName("hostDeviceType")]
    public string? HostDeviceType { get; set; }

    [JsonPropertyName("nationalLocationCode")]
    public int? NationalLocationCode { get; set; }

    [JsonPropertyName("tapTimestamp")]
    public DateTimeOffset? TapTimestamp { get; set; }
}

public sealed class FareCaveat
{
    [JsonPropertyName("$type")]
    public string? Type { get; set; }

    [JsonPropertyName("text")]
    public string? Text { get; set; }

    [JsonPropertyName("type")]
    public string? CaveatType { get; set; }
}

public sealed class Line
{
    [JsonPropertyName("$type")]
    public string? Type { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("modeName")]
    public string? ModeName { get; set; }

    [JsonPropertyName("disruptions")]
    public List<object>? Disruptions { get; set; }

    [JsonPropertyName("created")]
    public DateTime? Created { get; set; }

    [JsonPropertyName("modified")]
    public DateTime? Modified { get; set; }

    [JsonPropertyName("lineStatuses")]
    public List<LineStatus>? LineStatuses { get; set; }

    [JsonPropertyName("serviceTypes")]
    public List<LineServiceTypeInfo>? ServiceTypes { get; set; }

    [JsonPropertyName("crowding")]
    public object? Crowding { get; set; }
}

public sealed class LineStatus
{
    [JsonPropertyName("$type")]
    public string? Type { get; set; }

    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("statusSeverity")]
    public int? StatusSeverity { get; set; }

    [JsonPropertyName("statusSeverityDescription")]
    public string? StatusSeverityDescription { get; set; }

    [JsonPropertyName("created")]
    public DateTime? Created { get; set; }

    [JsonPropertyName("validityPeriods")]
    public List<object>? ValidityPeriods { get; set; }
}

public sealed class LineServiceTypeInfo
{
    [JsonPropertyName("$type")]
    public string? Type { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("uri")]
    public string? Uri { get; set; }
}

public sealed class SearchCriteria
{
    [JsonPropertyName("$type")]
    public string? Type { get; set; }

    [JsonPropertyName("dateTime")]
    public DateTime? DateTime { get; set; }

    [JsonPropertyName("dateTimeType")]
    public string? DateTimeType { get; set; }

    [JsonPropertyName("timeAdjustments")]
    public TimeAdjustments? TimeAdjustments { get; set; }
}

public sealed class TimeAdjustments
{
    [JsonPropertyName("$type")]
    public string? Type { get; set; }

    [JsonPropertyName("earliest")]
    public TimeAdjustment? Earliest { get; set; }

    [JsonPropertyName("earlier")]
    public TimeAdjustment? Earlier { get; set; }

    [JsonPropertyName("later")]
    public TimeAdjustment? Later { get; set; }

    [JsonPropertyName("latest")]
    public TimeAdjustment? Latest { get; set; }
}

public sealed class TimeAdjustment
{
    [JsonPropertyName("$type")]
    public string? Type { get; set; }

    [JsonPropertyName("date")]
    public string? Date { get; set; }

    [JsonPropertyName("time")]
    public string? Time { get; set; }

    [JsonPropertyName("timeIs")]
    public string? TimeIs { get; set; }

    [JsonPropertyName("uri")]
    public string? Uri { get; set; }
}

public sealed class JourneyVector
{
    [JsonPropertyName("$type")]
    public string? Type { get; set; }

    [JsonPropertyName("from")]
    public string? From { get; set; }

    [JsonPropertyName("to")]
    public string? To { get; set; }

    [JsonPropertyName("via")]
    public string? Via { get; set; }

    [JsonPropertyName("uri")]
    public string? Uri { get; set; }
}