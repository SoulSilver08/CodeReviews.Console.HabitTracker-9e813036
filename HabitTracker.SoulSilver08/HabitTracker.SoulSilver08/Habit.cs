namespace HabitTracker.SoulSilver08
{
    internal class Habit
    {
        private string _id = "";
        private string _name = "";
        private string _amount = "";
        private string _dateTime = "";
        private string _note = "";

        public string ID
        {
            get => _id;
            set { _id = value; }
        }

        public string Name
        {
            get => _name;
            set { _name = value; }
        }

        public string Amount
        {
            get => _amount;
            set { _amount = value; }
        }

        public string DateTime
        {
            get => _dateTime;
            set { _dateTime = value + " 00:00:00"; }
        }

        public string Note
        {
            get => _note;
            set { _note = value; }
        }
    }
}
