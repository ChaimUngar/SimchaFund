using SimchaFund.Data;
using SimchaFund.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SimchaFund.Data
{
    public class SimchaFundDb
    {
        private string _connectionString;


        //simcha functions
        public SimchaFundDb(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Simcha> GetAllSimchas()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Simchas
                                ORDER BY Date DESC";
            connection.Open();
            using var reader = cmd.ExecuteReader();
            List<Simcha> simchas = new();
            while (reader.Read())
            {
                simchas.Add(new()
                {
                    Id = (int)reader["Id"],
                    Date = (DateTime)reader["Date"],
                    SimchaName = (string)reader["Simcha"]
                });
            }
            return simchas;
        }

        public int GetTotalContributorCount()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Contributors";
            connection.Open();
            return (int)cmd.ExecuteScalar();
        }

        public int GetAlwaysContributeCount()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT COUNT(*) FROM Contributors
                                WHERE AlwaysContribute = 1";
            connection.Open();
            return (int)cmd.ExecuteScalar();
        }

        public int GetContributorAmntBySimcha(int simchaId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT COUNT(*) FROM Contributions cs
                                JOIN Simchas s
                                on s.Id = cs.SimchaId
                                WHERE s.id = @id";

            cmd.Parameters.AddWithValue("@id", simchaId);

            connection.Open();
            return (int)cmd.ExecuteScalar();
        }

        public decimal GetTotalFundsForSimcha(int simchaId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT SUM(c.ContributionAmnt) AS 'TotalContributions'
                                FROM Contributions c
                                JOIN Simchas s
                                ON s.Id = c.SimchaId
                                WHERE s.id = @id
                                GROUP BY s.Id";

            cmd.Parameters.AddWithValue("@id", simchaId);

            connection.Open();
            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return 0;
            }

            return (decimal)reader["TotalContributions"];
        }

        public void InsertSimcha(Simcha simcha)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO Simchas(Simcha, Date)
                                VALUES(@name, @date)";

            cmd.Parameters.AddWithValue("@name", simcha.SimchaName);
            cmd.Parameters.AddWithValue("@date", simcha.Date);

            connection.Open();
            cmd.ExecuteNonQuery();
        }

        


        //contributor functions
        public List<Contributor> GetAllContributors()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Contributors
                                ORDER BY LastName ASC";
            connection.Open();
            using var reader = cmd.ExecuteReader();
            List<Contributor> contributors = new();
            while (reader.Read())
            {
                contributors.Add(new()
                {
                    Id = (int)reader["Id"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    CellNumber = (string)reader["CellNumber"],
                    AlwaysInclude = (bool)reader["AlwaysInclude"],
                    DateCreated = (DateTime)reader["DateCreated"]
                });
            }
            return contributors;
        }

        public decimal GetDepositsForContributor(int contributorId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT SUM(d.Amount) 
                                FROM Deposits d
                                JOIN Contributors c
                                ON c.Id = d.ContributorId
                                WHERE c.id = @id
                                GROUP BY c.Id";

            cmd.Parameters.AddWithValue("@id", contributorId);

            connection.Open();

            return (decimal)cmd.ExecuteScalar();
        }

        public decimal GetContributionsForContributor(int contributorId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT SUM(cs.ContributionAmnt) AS 'Amnt'
                                FROM Contributions cs
                                JOIN Contributors c
                                ON c.Id = cs.ContributorId
                                WHERE c.id = @id
                                GROUP BY c.Id";

            cmd.Parameters.AddWithValue("@id", contributorId);

            connection.Open();
            using var reader = cmd.ExecuteReader();
            if(!reader.Read())
            {
                return 0;
            }

            return (decimal)reader["Amnt"];
        }

        public decimal GetBalance()
        {
            return GetDeposits() + (GetContributions() * -1);
        }

        private decimal GetDeposits()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT SUM(Amount) FROM Deposits";

            connection.Open();

            return (decimal)cmd.ExecuteScalar();
        }

        private decimal GetContributions()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT SUM(ContributionAmnt) FROM Contributions";

            connection.Open();

            return (decimal)cmd.ExecuteScalar();
        }

        public void AddContributor(Contributor c)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO Contributors(FirstName, LastName, CellNumber, DateCreated, AlwaysInclude)
                                VALUES(@first, @last, @cell, @date, @always)
                                SELECT SCOPE_IDENTITY()";

            cmd.Parameters.AddWithValue("@first", c.FirstName);
            cmd.Parameters.AddWithValue("@last", c.LastName);
            cmd.Parameters.AddWithValue("@cell", c.CellNumber);
            cmd.Parameters.AddWithValue("@date", c.DateCreated);
            cmd.Parameters.AddWithValue("@always", c.AlwaysInclude);


            connection.Open();
            c.Id = (int)(decimal)cmd.ExecuteScalar();
        }

        public void CreateBalance(Contributor c, decimal amount)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO Deposits(Amount, ContributorId, DateOfAction)
                                VALUES(@amnt, @cid, @date)";

            cmd.Parameters.AddWithValue("@cid", c.Id);
            cmd.Parameters.AddWithValue("@date", c.DateCreated);
            cmd.Parameters.AddWithValue("@amnt", amount);

            connection.Open();
            cmd.ExecuteNonQuery();
        }

        public List<Contributor> Search(string text)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Contributors
                                WHERE FirstName LIKE '%@text%' OR LastName LIKE '%@text%'
                                ORDER BY LastName ASC";

            cmd.Parameters.AddWithValue("@text", text);

            connection.Open();
            using var reader = cmd.ExecuteReader();
            List<Contributor> contributors = new();
            while (reader.Read())
            {
                contributors.Add(new()
                {
                    Id = (int)reader["Id"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    CellNumber = (string)reader["CellNumber"],
                    AlwaysInclude = (bool)reader["AlwaysInclude"],
                    DateCreated = (DateTime)reader["DateCreated"]
                });
            }

            return contributors;
        }

        public void AddDeposit(Deposit d)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO Deposits(Amount, ContributorId, DateOfAction)
                                VALUES(@amnt, @cid, @date)";

            cmd.Parameters.AddWithValue("@amnt", d.Amount);
            cmd.Parameters.AddWithValue("@cid", d.ContributorId);
            cmd.Parameters.AddWithValue("@date", d.Date);

            connection.Open();
            cmd.ExecuteNonQuery();
        }


        public List<History> GetSimchaHistory(int contribId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT s.Simcha, s.Date, cs.ContributionAmnt FROM Contributors c
                                JOIN Contributions cs ON cs.ContributorId = c.Id
                                JOIN Simchas s ON s.Id = cs.SimchaId
                                WHERE c.Id = @id
                                ORDER BY cs.DateOfAction DESC";

            cmd.Parameters.AddWithValue("@id", contribId);

            connection.Open();
            using var reader = cmd.ExecuteReader();
            List<History> simchas = new();
            while (reader.Read())
            {
                simchas.Add(new()
                {
                    Action = $"Contribution for the {(string)reader["Simcha"]} simcha",
                    Amount = (decimal)reader["ContributionAmnt"] * -1,
                    Date = (DateTime)reader["Date"]
                });
            }

            return simchas;
        }

        public List<History> GetDepositHistory(int contribId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT d.DateOfAction, d.Amount FROM Contributors c
                                JOIN Deposits d ON d.ContributorId = c.Id
                                WHERE c.Id = @id
                                ORDER BY d.DateOfAction DESC";

            cmd.Parameters.AddWithValue("@id", contribId);

            connection.Open();
            using var reader = cmd.ExecuteReader();
            List<History> deposits = new();
            while (reader.Read())
            {
                deposits.Add(new()
                {
                    Action = "Deposit",
                    Amount = (decimal)reader["Amount"],
                    Date = (DateTime)reader["DateOfAction"]
                });
            }

            return deposits;
        }

        public string GetContributorName(int contribId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT FirstName, LastName FROM Contributors
                                WHERE id = @id";

            cmd.Parameters.AddWithValue("@id", contribId);

            connection.Open();
            using var reader = cmd.ExecuteReader();

            if(!reader.Read())
            {
                return null;
            }

            return $"{(string)reader["FirstName"]} {(string)reader["LastName"]}";
        }

        //contributions functions
        public List<Contributor> GetContributorsBySimcha()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Contributors";
                                

            //cmd.Parameters.AddWithValue("@id", simchaId);

            connection.Open();
            using var reader = cmd.ExecuteReader();
            List<Contributor> contributors = new();
            while (reader.Read())
            {
                contributors.Add(new()
                {
                    Id = (int)reader["Id"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    AlwaysInclude = (bool)reader["AlwaysInclude"],
                });
            }

            return contributors;
        }

        public string GetSimchaName(int simchaId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT Simcha FROM Simchas
                                WHERE id = @id";

            cmd.Parameters.AddWithValue("@id", simchaId);

            connection.Open();
            using var reader = cmd.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            return (string)reader["Simcha"];
        }

        public void Update(int simchaId, List<Update> contributors)
        {
            Delete(simchaId);
            InsertUpdate(simchaId, contributors);
        }

        private void Delete(int simchaId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"DELETE FROM Contributions WHERE SimchaId = @id";
            connection.Open();

            cmd.Parameters.AddWithValue("@id", simchaId);
            cmd.ExecuteNonQuery();
        }

        private void InsertUpdate(int simchaId, List<Update> contributors)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO Contributions (SimchaId, ContributorId, ContributionAmnt)
                                VALUES (@sid, @cid, @amnt)";

            connection.Open();

            contributors = contributors.Where(c => c.Include).ToList();

            foreach (var c in contributors)
            {

                cmd.Parameters.AddWithValue("@cid", c.ContributorId);
                cmd.Parameters.AddWithValue("@amnt", c.Amount);
                cmd.Parameters.AddWithValue("@sid", simchaId);

                cmd.ExecuteNonQuery();
            }
        }

        

        public List<int> GetIdsBySimcha(int simchaId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT SimchaId, ContributorId FROM Contributions
                                WHERE SimchaId = @id";

            cmd.Parameters.AddWithValue("@id", simchaId);

            connection.Open();
            using var reader = cmd.ExecuteReader();
            List<int> ids = new();
            while (reader.Read())
            {
                ids.Add((int)reader["ContributorId"]);
            }
            return ids;
        }


    }
}




