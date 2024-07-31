using ADO.NET.Constants;
using ADO.NET.Extensions;
using System.Data.SqlClient;

namespace ADO.NET.Services
{
    public static class CityService
    {
        public static void GetAllCities()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionStrings.Default))
            {
                connection.Open();

                var command = new SqlCommand("SELECT * FROM Cities WHERE IsDeleted=0", connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["Id"]);
                        string name = Convert.ToString(reader["Name"]);
                        int population = Convert.ToInt32(reader["Population"]);

                        Messages.PrintMessage("Id", id.ToString());
                        Messages.PrintMessage("Name", name);
                        Messages.PrintMessage("Population", population.ToString());
                    }
                }
            }
        }

        public static void AddCity()
        {
            Messages.InputMessage("City name");
            string name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name))
            {
                using (SqlConnection connection = new SqlConnection(ConnectionStrings.Default))
                {
                    connection.Open();

                    var selectCommand = new SqlCommand("SELECT * FROM Cities WHERE Name=@name", connection);
                    selectCommand.Parameters.AddWithValue("@name", name);
                    try
                    {
                        int id = Convert.ToInt32(selectCommand.ExecuteScalar());
                        if (id > 0)
                            Messages.AlreadyExistMessage("City", name);
                        else
                        {
                            Messages.InputMessage("City population");
                            string populationInput = Console.ReadLine();
                            int population;
                            bool isSucceeded = int.TryParse(populationInput, out population);
                            if (isSucceeded)
                            {
                                var command = new SqlCommand("INSERT INTO Cities (Name, Population) VALUES(@name, @population)", connection);
                                command.Parameters.AddWithValue("@name", name);
                                command.Parameters.AddWithValue("@population", population);

                                var affectedRows = command.ExecuteNonQuery();
                                if (affectedRows > 0)
                                    Messages.SuccessAddMessage("City", name);
                                else
                                    Messages.ErrorOccuredMessage();
                            }
                            else
                                Messages.InvalidInputMessage("City population");
                        }
                    }
                    catch (Exception)
                    {
                        Messages.ErrorOccuredMessage();
                    }
                }
            }
            else
                Messages.InvalidInputMessage("City name");
        }

        public static void UpdateCity()
        {
            GetAllCities();

            Messages.InputMessage("City name");
            string name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name))
            {
                using (SqlConnection connection = new SqlConnection(ConnectionStrings.Default))
                {
                    connection.Open();

                    var command = new SqlCommand("SELECT * FROM Cities WHERE Name=@name", connection);
                    command.Parameters.AddWithValue("@name", name);
                    try
                    {
                        int id = Convert.ToInt32(command.ExecuteScalar());
                        if (id > 0)
                        {
                        NameWantToChangeSection: Messages.PrintWantToChangeMessage("name");
                            var choiceForName = Console.ReadLine();
                            char choice = choiceForName[0];
                            if (choice.IsValidChoice())
                            {
                                string newName = string.Empty;

                                if (choice.Equals('y'))
                                {
                                InputNewNameSection: Messages.InputMessage("new name");
                                    newName = Console.ReadLine();
                                    if (!string.IsNullOrWhiteSpace(newName))
                                    {
                                        var alreadyExistsCommand = new SqlCommand("SELECT * FROM Cities WHERE Name=@name AND Id !=@id", connection);
                                        alreadyExistsCommand.Parameters.AddWithValue("@name", newName);
                                        alreadyExistsCommand.Parameters.AddWithValue("@id", id);

                                        int existId = Convert.ToInt32(alreadyExistsCommand.ExecuteScalar());
                                        if (existId > 0)
                                        {
                                            Messages.AlreadyExistMessage("City", newName);
                                            goto NameWantToChangeSection;
                                        }
                                    }
                                    else
                                    {
                                        Messages.InvalidInputMessage("New name");
                                        goto InputNewNameSection;
                                    }
                                }

                            PopulationWantToChangeSection: Messages.PrintWantToChangeMessage("population");
                                var choiceForPopulation = Console.ReadLine();
                                bool isSucceeded = char.TryParse(choiceForPopulation, out choice);
                                int newPopulation = default;
                                if (isSucceeded && choice.IsValidChoice())
                                {
                                    if (choice.Equals('y'))
                                    {
                                    InputNewPopulation: Messages.InputMessage("new population");
                                        var newPopulationInput = Console.ReadLine();
                                        isSucceeded = int.TryParse(newPopulationInput, out newPopulation);
                                        if (!isSucceeded)
                                        {
                                            Messages.InvalidInputMessage("New population");
                                            goto InputNewPopulation;
                                        }
                                    }
                                }
                                else
                                {
                                    Messages.InvalidInputMessage("New population");
                                    goto PopulationWantToChangeSection;
                                };

                                var updateCommand = new SqlCommand("UPDATE Cities SET ", connection);

                                if (newName != string.Empty || newPopulation != default)
                                {
                                    bool isRequiredComa = false;
                                    if (newName != string.Empty)
                                    {
                                        isRequiredComa = true;
                                        updateCommand.CommandText = updateCommand.CommandText + "Name=@name";
                                        updateCommand.Parameters.AddWithValue("@name", newName);
                                    }

                                    if (newPopulation != default)
                                    {
                                        string commaText = isRequiredComa ? "," : "";
                                        updateCommand.CommandText = updateCommand.CommandText + $"{commaText}Population=@population";
                                        updateCommand.Parameters.AddWithValue("@population", newPopulation);
                                    }

                                    updateCommand.CommandText = updateCommand.CommandText + " WHERE Id=@id";
                                    updateCommand.Parameters.AddWithValue("id", id);
                                    int affectedRows = Convert.ToInt32(updateCommand.ExecuteNonQuery());
                                    if (affectedRows > 0)
                                    {
                                        Messages.SuccessUpdateMessage("City", newName);
                                    }
                                    else
                                        Messages.ErrorOccuredMessage();
                                }
                            }
                            else
                                Messages.InvalidInputMessage("Choice");
                        }
                        else
                            Messages.NotFoundMessage("City", name);
                    }
                    catch (Exception)
                    {
                        Messages.ErrorOccuredMessage();
                    }
                }
            }
            else
                Messages.InvalidInputMessage("City name");
        }

        public static void DeleteCity()
        {
            GetAllCities();

            Messages.InputMessage("City name");
            string name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name))
            {
                using (SqlConnection connection = new SqlConnection(ConnectionStrings.Default))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand("SELECT * FROM Cities WHERE Name=@name", connection);
                    command.Parameters.AddWithValue("@name", name);

                    try
                    {
                        int id = Convert.ToInt32(command.ExecuteScalar());
                        if (id > 0)
                        {
                            SqlCommand deleteCommand = new SqlCommand("UPDATE Cities SET IsDeleted=1 WHERE Id=@id", connection);
                            deleteCommand.Parameters.AddWithValue("id", id);

                            int affectRows = deleteCommand.ExecuteNonQuery();
                            if (affectRows > 0)
                                Messages.SuccessDeleteMessage("City", name);
                            else
                                Messages.ErrorOccuredMessage();
                        }
                        else
                            Messages.NotFoundMessage("City", name);
                    }
                    catch (Exception)
                    {
                        Messages.ErrorOccuredMessage();
                    }
                }
            }
            else
                Messages.InvalidInputMessage("City name");
        }

        public static void GetDetailsCity()
        {
            GetAllCities();

            Messages.InputMessage("City name");
            string name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name))
            {
                using (SqlConnection connection = new SqlConnection(ConnectionStrings.Default))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand("SELECT * FROM Cities WHERE Name=@name", connection);
                    command.Parameters.AddWithValue("@name", name);

                    try
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();

                                Messages.PrintMessage("Id", Convert.ToString(reader["Id"]));
                                Messages.PrintMessage("Name", Convert.ToString(reader["Name"]));
                                Messages.PrintMessage("Population", Convert.ToString(reader["Population"]));
                            }
                            else
                                Messages.NotFoundMessage("City", name);
                        }
                    }
                    catch (Exception)
                    {
                        Messages.ErrorOccuredMessage();
                    }
                }
            }
            else
                Messages.InvalidInputMessage("City name");
        }
    }
}
