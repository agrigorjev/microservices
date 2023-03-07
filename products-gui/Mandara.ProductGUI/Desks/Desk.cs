
namespace Mandara.ProductGUI.Desks
{
    public class Desk
    {
        private const string DefaultName = "";
        public const int DefaultId = 0;
        public static readonly Desk Default = new Desk(DefaultName, DefaultId);

        public Desk() : this(DefaultName, DefaultId)
        {
        }

        public Desk(string name, int id)
        {
            Name = name.Trim();
            Id = id;
        }

        private string _name;
        public string Name
        {
            get;
            set;
        }

        public int Id { get; set; }

        public bool IsDefault => DefaultId == Id && DefaultName == Name;
        public bool IsNew => DefaultId == Id && DefaultName != Name;
    }
}