using ADO.NET.Constants;
using ADO.NET.Extensions;
using System.Data.SqlClient;

namespace ADO.NET.Services
{
    public static class CountryService
    {
        public static void GetAllCountries()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionStrings.Default))
            {
                connection.Open();

                var command = new SqlCommand("SELECT * FROM Countries WHERE IsDeleted=0", connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["Id"]);
                        string name = Convert.ToString(reader["Name"]);
                        decimal area = Convert.ToDecimal(reader["Area"]);

                        Messages.PrintMessage("Id", id.ToString());
                        Messages.PrintMessage("Name", name);
                        Messages.PrintMessage("Area", area.ToString());
                    }
                }
            }
        }
        public static void AddCountry()
        {
            Messages.InputMessage("country name");
            string name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name))
            {
                using (SqlConnection connection = new SqlConnection(ConnectionStrings.Default))
                {
                    connection.Open();

                    var selectCommand = new SqlCommand("SELECT * FROM Countries WHERE Name=@name", connection);
                    selectCommand.Parameters.AddWithValue("name", name);
                    try
                    {
                        int id = Convert.ToInt32(selectCommand.ExecuteScalar());
                        if (id > 0)
                            Messages.AlreadyExistMessage("Country", name);
                        else
                        {
                            Messages.InputMessage("Country area");
                            string areaInput = Console.ReadLine();
                            decimal area;
                            bool IsSucceeded = decimal.TryParse(areaInput, out area);
                            if (IsSucceeded)
                            {
                                var command = new SqlCommand("INSERT INTO Countries VALUES(@name, @area)", connection);
                                command.Parameters.AddWithValue("@name", name);
                                command.Parameters.AddWithValue("@area", area);

                                var affectedRows = command.ExecuteNonQuery();
                                if (affectedRows > 0)
                                    Messages.SuccessAddMessage("country", name);
                                else
                                    Messages.ErrorOccuredMessage();
                            }
                        }
                    }
                    catch (Exception)
                    {
                        Messages.ErrorOccuredMessage();
                    }
                }
            }
            else
                Messages.InvalidInputMessage("Country name");
        }
        public static void UpdateCountry()
        {
            GetAllCountries();

            Messages.InputMessage("Country name");
            string name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name))
            {
                using (SqlConnection connection = new SqlConnection(ConnectionStrings.Default))
                {
                    connection.Open();

                    var command = new SqlCommand("SELECT * FROM Countries WHERE Name=@name", connection);
                    command.Parameters.AddWithValue("@name", name);
                    try
                    {
                        int id = Convert.ToInt32(command.ExecuteScalar());
                        if (id > 0)
                        {

                        NameWantToChangeSection: Messages.PrintWantToChangeMessage("name");
                            var choiceForName = Console.ReadLine();
                            char choice = choiceForName[0]; ;
                            if (choice.IsValidChoice())
                            {
                                string newName = string.Empty;

                                if (choice.Equals('y'))
                                {
                                InputNewNameSection: Messages.InputMessage("new name");
                                    newName = Console.ReadLine();
                                    if (!string.IsNullOrWhiteSpace(newName))
                                    {
                                        var alreadyExistsCommand = new SqlCommand("SELECT * FROM Countries WHERE Name=@name AND Id !=@id", connection);
                                        alreadyExistsCommand.Parameters.AddWithValue("@name", newName);
                                        alreadyExistsCommand.Parameters.AddWithValue("@id", id);

                                        int existId = Convert.ToInt32(alreadyExistsCommand.ExecuteScalar());
                                        if (existId > 0)
                                        {
                                            Messages.AlreadyExistMessage("Country", newName);
                                            goto NameWantToChangeSection;
                                        }
                                    }
                                    else
                                    {
                                        Messages.InvalidInputMessage("New name");
                                        goto InputNewNameSection;
                                    }
                                }

                            AreaWantToChangeSection: Messages.PrintWantToChangeMessage("area");
                                var choiceForArea = Console.ReadLine();
                                bool isSucceeded = char.TryParse(choiceForArea, out choice);
                                decimal newArea = default;
                                if (isSucceeded && choice.IsValidChoice())
                                {
                                    if (choice.Equals('y'))
                                    {
                                    InputNewArea: Messages.InputMessage("new area");
                                        var newAreaInput = Console.ReadLine();
                                        isSucceeded = decimal.TryParse(newAreaInput, out newArea);
                                        if (!isSucceeded)
                                        {
                                            Messages.InvalidInputMessage("New area");
                                            goto InputNewArea;
                                        }
                                    }
                                }
                                else
                                {
                                    Messages.InvalidInputMessage("New area");
                                    goto AreaWantToChangeSection;
                                };

                                var updateCommand = new SqlCommand("UPDATE Countries SET ", connection);

                                if (newName != string.Empty || newArea != default)
                                {
                                    bool isRequiredComa = false;
                                    if (newName != string.Empty)
                                    {
                                        isRequiredComa = true;
                                        updateCommand.CommandText = updateCommand.CommandText + "Name=@name";
                                        updateCommand.Parameters.AddWithValue("@name", newName);

                                    }

                                    if (newArea != default)
                                    {
                                        string commaText = isRequiredComa ? "," : "";
                                        updateCommand.CommandText = updateCommand.CommandText + $"{commaText}Area=@area";
                                        updateCommand.Parameters.AddWithValue("@area", newArea);
                                    }

                                    updateCommand.CommandText = updateCommand.CommandText + " WHERE id=@id";
                                    updateCommand.Parameters.AddWithValue("id", id);
                                    int affectedRows = Convert.ToInt32(updateCommand.ExecuteNonQuery());
                                    if (affectedRows > 0)
                                    {
                                        Messages.SuccessUpdateMessage("Country", newName);
                                    }
                                    else
                                        Messages.ErrorOccuredMessage();
                                }
                            }
                            else
                                Messages.InvalidInputMessage("Choice");
                        }
                        else
                            Messages.NotFoundMessage("Country", name);
                    }
                    catch (Exception)
                    {

                        Messages.ErrorOccuredMessage();
                    }

                }
            }
            else
                Messages.InvalidInputMessage("Country name");
        }
        public static void DeleteCountry()
        {
            GetAllCountries();

            string name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name))
            {
                using (SqlConnection connection = new SqlConnection(ConnectionStrings.Default))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand("SELECT * FROM Countries WHERE Name=@name", connection);
                    command.Parameters.AddWithValue("@name", name);

                    try
                    {
                        int id = Convert.ToInt32(command.ExecuteScalar());
                        if (id > 0)
                        {
                            SqlCommand deleteCommand = new SqlCommand("UPDATE Countries SET IsDeleted=1 WHERE Id=@id", connection);
                            deleteCommand.Parameters.AddWithValue("id", id);

                            int affectRows = deleteCommand.ExecuteNonQuery();
                            if (affectRows > 0)
                                Messages.SuccessDeleteMessage("Country", name);
                            else 
                                Messages.ErrorOccuredMessage();
                        }
                        else
                            Messages.NotFoundMessage("Country", name);
                    }
                    catch (Exception)
                    {
                        Messages.ErrorOccuredMessage();
                    }
                }
            }
            else
                Messages.InvalidInputMessage("Country name");
        }
        public static void GetDetailsCountry()
        {
            GetAllCountries();

            string name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name))
            {
                using (SqlConnection connection = new SqlConnection(ConnectionStrings.Default))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand("SELECT * FROM Countries WHERE Name=@name", connection);
                    command.Parameters.AddWithValue ("@name", name);

                    try
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();

                                Messages.PrintMessage("Id", Convert.ToString(reader["Id"]));
                                Messages.PrintMessage("Name", Convert.ToString(reader["Name"]));
                                Messages.PrintMessage("Area", Convert.ToString(reader["Area"]));
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Messages.ErrorOccuredMessage();
                    }
                }
            }
            else
                Messages.InvalidInputMessage("Name");
        }
    }
}
