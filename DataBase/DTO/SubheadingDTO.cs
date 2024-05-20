namespace Bible_Blazer_PWA.DataBase.DTO
{
    public record SubheadingDTO
    {
        public int BookNumber { get; set; }
        public int Chapter { get; set; }
        public int Verse { get; set; }
        public string Subheading { get; set; }
    }
}
