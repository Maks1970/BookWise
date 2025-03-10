namespace DataLibrary
{
    public class Reader : Employee 
    {
        public string Name { get; set; }
        public string LastName {  get; set; }
        public int DocumentTypeId { get; set; }
        public DocumentType DocumenttType { get; set; }
        public string DocumentNumber { get; set; }
        public ICollection<BorrowedBook> BorrowedBooks { get; set; }

    }

}
