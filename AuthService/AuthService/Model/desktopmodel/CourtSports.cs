namespace AuthService.Model.desktopmodel
{
    public class CourtSports
    {
        public int Id { get; set; }
        public string SportsName { get; set; }
        public int CourtFields { get; set; }
    }


    public class CourtSportsRequest
    {
        public string SportsName { get; set; }
        public int CourtFields { get; set; }
    }



}
