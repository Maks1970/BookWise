using System.Text.Json.Serialization;

namespace DataLibrary
{
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string SecondName { get; set; }
        public DateTime DateOfBirth { get; set; }
         [JsonIgnore]
        public ICollection<Book> Books { get; set; }

    }

}
