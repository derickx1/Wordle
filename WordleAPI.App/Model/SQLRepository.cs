using System.Data.SqlClient;

namespace WordleAPI.App
{
    public class SqlRepository : IRepository
    {
        private readonly string connectionString;

        public SqlRepository( string connectionString)
        {
            this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public void InsertUser(User user)
        {
            using SqlConnection connection = new SqlConnection(this.connectionString);
    
            connection.Open();

            // Check if the user already exists.
            SqlCommand check_ID = new SqlCommand("SELECT * FROM Wordle.Users WHERE Name = @Name", connection);
            check_ID.Parameters.AddWithValue("@Name", user.name);
            SqlDataReader reader = check_ID.ExecuteReader();

            // If they already exist, update their stats.
            if(reader.HasRows)
            {
                reader.Close();

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
                cmd.ExecuteNonQuery();
            }
            else // Otherwise put them into the database.
            {
                reader.Close();

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

                cmd.ExecuteNonQuery();
            }

            connection.Close();
        }

        // Importing user stats.
        public User GetUser(string name)
        {
            User tmpUser = new User(name);

            using SqlConnection connection = new SqlConnection(this.connectionString);
            connection.Open();

            string cmdText = @"SELECT * FROM Wordle.Users WHERE Name = @Name;";

            using SqlCommand cmd = new SqlCommand(cmdText, connection);
            cmd.Parameters.AddWithValue("@Name", name);

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
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

            return tmpUser;
        }
    }
}