using System.Text.Json.Serialization;

namespace DataLibrary
{
    //        тип видавничого коду(посилання на таблицю)
    //Рік
    //Країна видавництва
    //Місто видавництва
    public class BorrowedBook
    {
        //  public string Name { get; set; } = null!;
        public int Id { get; set; }
        public int BookId { get; set; }
        public  Book Book { get; set; } 
        public int ReaderId { get; set; }
        [JsonIgnore]
        public Reader Reader { get; set; }
        public DateTime DateBorrowed { get; set; }
        public DateTime DateForBorrowed { get; set; } = DateTime.Now.AddDays(30);
        public DateTime? DateReturned { get; set; }
    }

}
