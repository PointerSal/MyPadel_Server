using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Model.desktopmodel
{
    public class CourtSports
    {
        public int Id { get; set; }
        public string SportsName { get; set; }
        public string FieldName { get; set; }  // Added Field Name
        public string FieldType { get; set; }  // Added Field Type
        public string TerrainType { get; set; }  // Added Terrain Type
        public int FieldCapacity { get; set; }  // Added Field Capacity

        // Slot durations and prices with precision and scale
        public int Slot1Duration { get; set; }  // Slot 1 Duration
        [Column(TypeName = "decimal(18,2)")]
        public decimal Slot1Price { get; set; }  // Slot 1 Price with Precision and Scale

        public int Slot2Duration { get; set; }  // Slot 2 Duration
        [Column(TypeName = "decimal(18,2)")]
        public decimal Slot2Price { get; set; }  // Slot 2 Price with Precision and Scale

        public int Slot3Duration { get; set; }  // Slot 3 Duration
        [Column(TypeName = "decimal(18,2)")]
        public decimal Slot3Price { get; set; }  // Slot 3 Price with Precision and Scale

       
        public bool CanBeBooked { get; set; }  // Can the court be booked?
        public string OpeningHours { get; set; }  // Added Opening Hours
    }

    public class CourtSportsRequest
    {
        public string SportsName { get; set; }
        public string FieldName { get; set; }
        public string FieldType { get; set; }
        public string TerrainType { get; set; }
        public int FieldCapacity { get; set; }
        public int Slot1Duration { get; set; }
        public decimal Slot1Price { get; set; }
        public int Slot2Duration { get; set; }
        public decimal Slot2Price { get; set; }
        public int Slot3Duration { get; set; }
        public decimal Slot3Price { get; set; }
        public bool CanBeBooked { get; set; }
        public string OpeningHours { get; set; }
    }
}
