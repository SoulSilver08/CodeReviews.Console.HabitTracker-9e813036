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

string? optionSelection;

do
{
    optionSelection = "";
    while (!Regex.IsMatch(optionSelection.ToLower(), "c|r|u|d"))
    {
        Console.Clear();
        Console.WriteLine("--- HABIT TRACKER ---");
        Console.WriteLine("---------------------\n");
        Console.WriteLine("What do you want to do today?");
        Console.WriteLine("C\t - Create a new entry");
        Console.WriteLine("R\t - Read last entries");
        Console.WriteLine("U\t - Update last entries");
        Console.WriteLine("D\t - Delete a last entry");
        Console.WriteLine("\nX\t - Exit");
        optionSelection = Console.ReadLine() ?? "";
        if (optionSelection.ToLower() == "x")
            break;

        if (!Regex.IsMatch(optionSelection.ToLower(), "c|r|u|d"))
        {
            Console.Write("\nInvalid option, please input a valid option.\nPress enter to continue....");
            Console.ReadLine();
        }
    }
    Console.Clear();

    switch (optionSelection.ToLower())
    {
        case "c":
            CreateFunction();
            break;
        case "r":
            ReadFunction();
            break;
        case "u":
            UPdateFunction();
            break;
        case "d":
            DeleteFunction();
            break;
    }
} while (optionSelection != "x");

Console.WriteLine("\nGood Bye!!!\n");

void CreateFunction()
{
    Habit habitToTrack = new();

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

    Console.WriteLine("Habit Tracked!!!");
}

void ReadFunction()
{
    Console.WriteLine("---- Read last entries ----");
    Console.WriteLine("---------------------------\n");

    DataTable table = GetTableFromDatabase();
    for (int i = 0; i < table.Rows.Count; i++)
    {
        Console.WriteLine($"Habit: {table.Rows[i][1]}\tAmount: {table.Rows[i][2]}\tDate: {table.Rows[i][3]}\nNote: \n{table.Rows[i][4]}");
        Console.WriteLine("\n-----------------------------\n");
    }

    Console.WriteLine("Press enter to go back to the menu.");
    Console.ReadLine();
}

void UPdateFunction()
{
    DataTable table = GetTableFromDatabase();
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
    DataTable table = GetTableFromDatabase();
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