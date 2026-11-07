using System.Collections.Generic;
using System.Linq;

namespace AstroGateway.FirstMission.HUD
{
    // Display labels only. Sensor IDs and the stored evidence stay in the catalog format.
    internal static class ScienceReadout
    {
        public static string Label(string id) => id switch
        {
            "nav.position" => "Position",
            "nav.heading" => "Heading",
            "nav.speed" => "Speed",
            "nav.distance_travelled" => "Distance driven",
            "terrain.slope" => "Ground slope",
            "terrain.roughness" => "Surface roughness",
            "terrain.traversability" => "Terrain rating",
            "env.dust_condition" => "Dust conditions",
            "env.air_temperature" => "Air temperature",
            "env.air_pressure" => "Air pressure",
            "env.radiation_dose_rate" => "Radiation dose rate",
            "sci.station_id" => "Survey site",
            "sci.target_range" => "Distance to site",
            "sci.target_bearing" => "Bearing to site",
            "sci.sample_id" => "Sample ID",
            "sci.soil_composition" => "Soil composition",
            "sci.spectral_reading" => "Spectral reading",
            _ => id,
        };

        public static string Value(SensorRecord reading) => (reading.sensorId, reading.ValueLabel) switch
        {
            ("env.dust_condition", "clear") => "Clear",
            ("env.dust_condition", "light_dust") => "Light dust",
            ("env.dust_condition", "dust_storm") => "Dust storm",
            ("terrain.traversability", "nominal") => "Normal",
            ("terrain.traversability", "caution") => "Caution",
            ("terrain.traversability", "hazard") => "Hazard",
            _ => reading.ValueLabel,
        };

        public static string AnalysisTag(SensorRecord reading) => reading.analysisState switch
        {
            AnalysisState.Complete => "COMPLETE",
            AnalysisState.Processing => "PROCESSING",
            AnalysisState.Unavailable => "UNAVAILABLE",
            _ => "NOT CONNECTED",
        };

        public static string Assessment(IReadOnlyList<SensorRecord> readings)
        {
            string SimulatedValue(string id) => readings.FirstOrDefault(r =>
                r.sensorId == id && r.provenance == Provenance.Simulated)?.valueText;
            var terrain = SimulatedValue("terrain.traversability");
            if (terrain == "hazard") return "Hazardous ground. Check the map for a gentler route.";
            if (terrain == "caution") return "Uneven or steep ground. Slow down and use Precision drive.";
            if (terrain == "nominal") return "The scanned patch is within the rover’s normal terrain limits.";

            var dust = SimulatedValue("env.dust_condition");
            if (dust == "dust_storm") return "Dust storm detected in the simulation. Slow down and follow the route markers.";
            if (dust == "light_dust") return "Light dust. Keep the route markers in view.";
            if (dust == "clear") return "Dust conditions are clear at the time of this scan.";

            if (!string.IsNullOrEmpty(SimulatedValue("sci.sample_id")))
                return "Sample logged for this site. Use the sample ID to identify it in your records.";
            if (!string.IsNullOrEmpty(SimulatedValue("sci.station_id")))
                return "Site logged. Check the recorded range and bearing before moving closer.";

            return readings.Any(r => r.provenance == Provenance.Simulated)
                ? "Review the captured readings and compare them with another scan."
                : "No usable readings in this scan. Check the instrument or try another location.";
        }
    }
}
