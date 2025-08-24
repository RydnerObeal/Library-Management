namespace Library_Management_Domain.Entities
{
    public class BookCopy
    {
        public Guid Id { get; set; }           
        public Guid BookId { get; set; }       
        public DateTime? PulloutDate { get; set; }  
        public string? PulloutReason { get; set; }  

        public Book? Book { get; set; }
        public string? CoverImageUrl { get; internal set; }
        public string? Condition { get; internal set; }
        public string? Source { get; internal set; }
        public DateTime AddedDate { get; internal set; }
    }
}
