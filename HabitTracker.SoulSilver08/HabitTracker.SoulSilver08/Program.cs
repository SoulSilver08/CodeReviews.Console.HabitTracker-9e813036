using HabitTracker.SoulSilver08;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Text.RegularExpressions;

string connString = @"Data Source=habit-Tracker.db";

using (SqliteConnection connection = new SqliteConnection(connString)) 
{
    try
    {
        connection.Open();
        var tableCmd = connection.CreateCommand();
        if (connection.Database == null)
            Console.WriteLine("Database created!!!");
        tableCmd.CommandText = @"CREATE TABLE IF NOT EXISTS habits_table (Id INTEGER PRIMARY KEY AUTOINCREMENT, Habit_Name TEXT, Amount TEXT, Date_Time DATETIME, Note TEXT)";
        tableCmd.ExecuteNonQuery();
        connection.Close();
    }
    catch (Exception ex) 
    {
        Console.WriteLine($"ERROR: {ex}");
    }
}

string? optionSelection = "";
DataTable table = GetTableFromDatabase();

while (optionSelection != "x")
{
    Console.Clear();
    Console.WriteLine($"--- HABIT TRACKER ---\tActual entries:{table.Rows.Count}");
    Console.WriteLine("---------------------\n");
    Console.WriteLine("What do you want to do today?");
    Console.WriteLine("C\t - Create a new entry");
    if (table.Rows.Count != 0) Console.WriteLine(@"R	 - Read last entries
U	 - Update last entries
D	 - Delete a last entry");
    Console.WriteLine("\nX\t - Exit");
    optionSelection = Console.ReadLine() ?? "";

    if (table.Rows.Count > 0)

    switch (optionSelection.ToLower())
    {
        case "c":
            CreateFunction();
            break;
        case "r":
            if (table.Rows.Count == 0) 
            {
                Console.Write("\nInvalid option, please input a valid option.\nPress enter to continue....");
                Console.ReadLine();
                break;
            }
            ReadFunction();
            break;
        case "u":
            if (table.Rows.Count == 0)
            {
                Console.Write("\nInvalid option, please input a valid option.\nPress enter to continue....");
                Console.ReadLine();
                break;
            }
            UpdateFunction();
            break;
        case "d":
            if (table.Rows.Count == 0)
            {
                Console.Write("\nInvalid option, please input a valid option.\nPress enter to continue....");
                Console.ReadLine();
                break;
            }
            DeleteFunction();
            break;
        case "x":
            Console.WriteLine("\nGood Bye!!!\n");
            break;
        case "#test[f]":
            TestFill();
            break;
        default:
            Console.Write("\nInvalid option, please input a valid option.\nPress enter to continue....");
            Console.ReadLine();
            break;
    }
} 

void CreateFunction()
{
    Habit habitToTrack = new();

    Console.Clear();
    Console.WriteLine("---- Create a new entry ----");
    Console.WriteLine("----------------------------");

    Console.WriteLine("\nEnter the name of the habit:");
    habitToTrack.Name = Console.ReadLine() ?? "Null";

    Console.WriteLine("\nEnter the value to register:");
    habitToTrack.Amount = Console.ReadLine() ?? "Null";

    Console.WriteLine("\nEnter the date of the habit [YYYY-MM-DD]:");
    habitToTrack.DateTime = Console.ReadLine() ?? "2000-01-01";

    Console.WriteLine("\nDo you want to add a note?");
    habitToTrack.Note = Console.ReadLine() ?? "";

    ConnectionToDatabase($"INSERT INTO habits_table (Habit_Name, Amount, Date_Time, Note) VALUES ('{habitToTrack.Name}', '{habitToTrack.Amount}', '{habitToTrack.DateTime}', '{habitToTrack.Note}')");
    table = GetTableFromDatabase();

    Console.WriteLine("Habit Tracked!!!");
    Console.ReadLine();
}

void ReadFunction()
{
    Console.Clear();
    Console.WriteLine("---- Read last entries ----");
    Console.WriteLine("---------------------------\n");

    for (int i = 0; i < table.Rows.Count; i++)
    {
        Console.WriteLine($"Habit: {table.Rows[i][1]}\tAmount: {table.Rows[i][2]}\tDate: {table.Rows[i][3]}\nNote: \n{table.Rows[i][4]}");
        Console.WriteLine("\n-----------------------------\n");
    }

    Console.WriteLine("Press enter to go back to the menu.");
    Console.ReadLine();
}

void UpdateFunction()
{
    while (true)
    {
        Console.Clear();
        Console.WriteLine("---- Update last entries ----");
        Console.WriteLine("-----------------------------\n");

        for (int i = 0; i < table.Rows.Count; i++)
        {
            Console.WriteLine($"ID: {table.Rows[i][0]} |\tHabit: {table.Rows[i][1]}\tAmount: {table.Rows[i][2]}\tDate: {table.Rows[i][3]}\nNote: \n{table.Rows[i][4]}");
            Console.WriteLine("\n-----------------------------\n");
        }
        
        Console.Write("Input the ID of the Entry you want to update or input x to cancel:");
        string idSelected = Console.ReadLine() ?? "";
        if (idSelected.ToLower() == "x")
            return;
        for (int i = 0; i < table.Rows.Count; i++)
        {
            if (int.TryParse(idSelected, out int idToInt) && idToInt == int.Parse(table.Rows[i][0].ToString()))
            {
                Console.Clear();
                Console.WriteLine("---- Update last entries ----");
                Console.WriteLine("-----------------------------\nNOTE: If you don´t want to update a value just leave the input in blank.");

                Habit updatedHabit = new();
                string? input;
                Console.WriteLine("\nInput the new name");
                input = Console.ReadLine();
                updatedHabit.Name = (input != "") ? input : table.Rows[i][1].ToString();
                Console.WriteLine("\nInput the new amount");
                input = Console.ReadLine();
                updatedHabit.Amount = (input != "") ? input : table.Rows[i][2].ToString();
                Console.WriteLine("\nInput the new date");
                input = Console.ReadLine();
                updatedHabit.DateTime = (input != "") ? input : table.Rows[i][3].ToString();
                Console.WriteLine("\nInput the new note");
                input = Console.ReadLine();
                updatedHabit.Note = (input != "") ? input : table.Rows[i][4].ToString();

                ConnectionToDatabase($"UPDATE habits_table SET habit_Name = '{updatedHabit.Name}', Amount = '{updatedHabit.Amount}', Date_Time = '{updatedHabit.DateTime}', Note = '{updatedHabit.Note}' WHERE Id = '{idSelected}'");
                table = GetTableFromDatabase();

                Console.WriteLine("Entry Updated!!!");
                Console.ReadLine();

                return;
            }
        }

        Console.WriteLine("Invalid Input");
        Console.ReadLine();
    }
}

void DeleteFunction()
{
    while (true)
    {
        Console.Clear();
        Console.WriteLine("---- Delete a last entry ----");
        Console.WriteLine("-----------------------------\n");

        for (int i = 0; i < table.Rows.Count; i++)
        {
            Console.WriteLine($"ID: {table.Rows[i][0]} |\tHabit: {table.Rows[i][1]}\tAmount: {table.Rows[i][2]}\tDate: {table.Rows[i][3]}\nNote: \n{table.Rows[i][4]}");
            Console.WriteLine("\n-----------------------------\n");
        }

        Console.WriteLine("Input the ID of the entry you want to delete.\nOr input x if you want to cancel");
        string? idSelected = Console.ReadLine();
        if (idSelected?.ToLower() == "x")
            return;

        for (int i = 0; i < table.Rows.Count; i++) 
        {
            if (int.TryParse(idSelected, out int idToInt) && idToInt == int.Parse(table.Rows[i][0].ToString())) 
            {
                Console.Clear();

                Console.WriteLine($"ID: {table.Rows[i][0]} |\tHabit: {table.Rows[i][1]}\tAmount: {table.Rows[i][2]}\tDate: {table.Rows[i][3]}\nNote: \n{table.Rows[i][4]}");
                Console.WriteLine("\n-----------------------------\n");
                Console.WriteLine("Are you sure that you want to delete this entry?");
                if (Console.ReadLine()?.ToLower() == "y")
                {
                    ConnectionToDatabase($"DELETE FROM habits_table WHERE Id = '{idSelected}'");
                    table = GetTableFromDatabase();

                    Console.WriteLine("\nENTRY DELETED\n");
                    Console.ReadLine();
                }
                return;
            }
        }

        Console.WriteLine("\nInvalid Input");
        Console.ReadLine();
    }
}

void ConnectionToDatabase(string querry) 
{
    SqliteConnection connection = new SqliteConnection(connString);
    try
    {
        connection.Open();
        var indexCmd = connection.CreateCommand();
        indexCmd.CommandText = querry;
        indexCmd.ExecuteNonQuery();
        connection.Close();
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}

DataTable GetTableFromDatabase() 
{
    DataTable table = new DataTable();

    SqliteConnection connection = new(connString);
    SqliteCommand cmd = new("SELECT * FROM habits_table", connection);
    try
    {
        connection.Open();
        SqliteDataReader reader = cmd.ExecuteReader();

        if (reader.HasRows)
            for (int i = 0; i < reader.FieldCount; i++)
                table.Columns.Add(new DataColumn(reader.GetName(i)));

        int j = 0;
        while (reader.Read())
        {
            DataRow row = table.NewRow();
            table.Rows.Add(row);

            for (int i = 0; i < reader.FieldCount; i++)
                table.Rows[j][i] = (reader.GetValue(i));

            j++;
        }
        connection.Close();
    }
    catch (Exception ex) 
    {
        Console.WriteLine(ex.Message);
    }

    return table;
}

void TestFill() 
{
    using (SqliteConnection connection = new SqliteConnection(connString))
    {
        try
        {
            connection.Open();
            var tableCmd = connection.CreateCommand();
            tableCmd.CommandText = $"INSERT INTO habits_table (Habit_Name, Amount, Date_Time, Note) VALUES ('Test', '2 1/2h', '2026-01-01', 'This is an entry test')";
            for (int i = 0; i < 50; i++)
                tableCmd.ExecuteNonQuery();
            connection.Close();

            table = GetTableFromDatabase();
            Console.WriteLine("TEST ENTRIES CREATED!!!");
            Console.ReadLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex}");
        }
    }
}