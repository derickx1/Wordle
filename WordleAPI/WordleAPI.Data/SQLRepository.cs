using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using WordleAPI.Model;

namespace WordleAPI.Data
{
    public class SQLRepository : IRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<SQLRepository> _logger;

        public SQLRepository(string connectionString, ILogger<SQLRepository> logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            List<User> result = new();

            using SqlConnection connection = new(_connectionString);
            await connection.OpenAsync();

            string cmdText = "SELECT * FROM Wordle.Users;";

            using SqlCommand cmd = new(cmdText, connection);

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                string name = reader.GetString(0);
                int streak = reader.GetInt32(1);
                int wins = reader.GetInt32(2);
                int losses = reader.GetInt32(3);
                int[] guessNums = {reader.GetInt32(4),
                                   reader.GetInt32(5),
                                   reader.GetInt32(6),
                                   reader.GetInt32(7),
                                   reader.GetInt32(8),
                                   reader.GetInt32(9)};

                User tmpUser = new User(name, streak, wins, losses, guessNums);

                result.Add(tmpUser);
            }

            await connection.CloseAsync();

            _logger.LogInformation("Executed GetAllUsersAsync, returned {0} results", result.Count);

            return result;
        }

        public async Task InsertUserAsync(User user)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // Check if the user already exists.
            using SqlCommand check_ID = new SqlCommand("SELECT * FROM Wordle.Users WHERE Name = @Name", connection);

            check_ID.Parameters.AddWithValue("@Name", user.name);

            SqlDataReader reader = await check_ID.ExecuteReaderAsync();

            // If they already exist, update their stats.
            if (reader.HasRows)
            {
                await reader.CloseAsync();

                string cmdText = "UPDATE Wordle.Users SET Streak = @Streak, Wins = @Wins, Losses = @Losses, Guess1 = @Guess1, Guess2 = @Guess2, Guess3 = @Guess3, Guess4 = @Guess4, Guess5 = @Guess5, Guess6 = @Guess6 WHERE Name=@Name";

                using SqlCommand cmd = new SqlCommand(cmdText, connection);

                cmd.Parameters.AddWithValue("@Streak", user.streak);
                cmd.Parameters.AddWithValue("@Wins", user.wins);
                cmd.Parameters.AddWithValue("@Losses", user.losses);
                cmd.Parameters.AddWithValue("@Guess1", user.guessNums[0]);
                cmd.Parameters.AddWithValue("@Guess2", user.guessNums[1]);
                cmd.Parameters.AddWithValue("@Guess3", user.guessNums[2]);
                cmd.Parameters.AddWithValue("@Guess4", user.guessNums[3]);
                cmd.Parameters.AddWithValue("@Guess5", user.guessNums[4]);
                cmd.Parameters.AddWithValue("@Guess6", user.guessNums[5]);
                cmd.Parameters.AddWithValue("@Name", user.name);
                await cmd.ExecuteNonQueryAsync();
            }
            else // Otherwise put them into the database.
            {
                await reader.CloseAsync();

                string cmdText =
                @"INSERT INTO Wordle.Users (Name, Streak, Wins, Losses, Guess1, Guess2, Guess3, Guess4, Guess5, Guess6)
                VALUES
                (@Name, @Streak, @Wins, @Losses, @Guess1, @Guess2, @Guess3, @Guess4, @Guess5, @Guess6)";

                using SqlCommand cmd = new SqlCommand(cmdText, connection);

                cmd.Parameters.AddWithValue("@Name", user.name);
                cmd.Parameters.AddWithValue("@Streak", user.streak);
                cmd.Parameters.AddWithValue("@Wins", user.wins);
                cmd.Parameters.AddWithValue("@Losses", user.losses);
                cmd.Parameters.AddWithValue("@Guess1", user.guessNums[0]);
                cmd.Parameters.AddWithValue("@Guess2", user.guessNums[1]);
                cmd.Parameters.AddWithValue("@Guess3", user.guessNums[2]);
                cmd.Parameters.AddWithValue("@Guess4", user.guessNums[3]);
                cmd.Parameters.AddWithValue("@Guess5", user.guessNums[4]);
                cmd.Parameters.AddWithValue("@Guess6", user.guessNums[5]);

                await cmd.ExecuteNonQueryAsync();
            }

            await connection.CloseAsync();
        }

        // Importing user stats.
        public async Task<User> GetUserAsync(string name)
        {
            User tmpUser = new User(name);

            using SqlConnection connection = new SqlConnection(_connectionString);

            await connection.OpenAsync();

            string cmdText = @"SELECT * FROM Wordle.Users WHERE Name = @Name;";

            using SqlCommand cmd = new SqlCommand(cmdText, connection);
            cmd.Parameters.AddWithValue("@Name", name);

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                int streak = reader.GetInt32(1);
                int wins = reader.GetInt32(2);
                int losses = reader.GetInt32(3);
                int[] guessNums = {reader.GetInt32(4),
                                   reader.GetInt32(5),
                                   reader.GetInt32(6),
                                   reader.GetInt32(7),
                                   reader.GetInt32(8),
                                   reader.GetInt32(9)};

                tmpUser = new User(name, streak, wins, losses, guessNums);
            }

            _logger.LogInformation("Executed GetUserAsync");

            return tmpUser;
        }

        public async Task DeleteUserAsync(string name)
        {
            using SqlConnection connection = new(_connectionString);
            await connection.OpenAsync();

            string cmdText = "DELETE FROM Wordle.Users WHERE Name = @Name;";
            
            using SqlCommand cmd = new(cmdText, connection);
            cmd.Parameters.AddWithValue("@Name", name);

            await cmd.ExecuteNonQueryAsync();

            await connection.CloseAsync();

            _logger.LogInformation("Executed DeleteUsersAsync");
        }
    }
}