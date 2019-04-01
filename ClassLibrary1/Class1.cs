using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class Simcha
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }     
        public decimal Total { get; set; }
    }
    public class Contributer
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal Balance { get; set; }
        public DateTime Date { get; set; }
        public string Cell { get; set; }
        public bool AlwaysInclude { get; set; }
        public List<Deposit> Deposits { get; set; }
        public List<Donations> Donations { get; set; }
       public List<DepositDonation> DepositDonations { get; set; }
    }
    public class Deposit
    {
        public int Id { get; set; }
        public int ContributerId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
    public class Donations
    {
        public int SimchaId { get; set; }
        public int ContributerId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public bool Donated { get; set; }

    }
    public class DepositDonation
    {
        public string Action { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }
  
    public class SimchaFundManager
    {
        private string _connectionstring;

        public SimchaFundManager(string connectionstring)
        {
            _connectionstring = connectionstring;
        }
 
        public void AddSimcha(Simcha simcha)
        {
            SqlConnection conn = new SqlConnection(_connectionstring);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"insert into simcha
                                values (@name, @date)";
            cmd.Parameters.AddWithValue("@name", simcha.Name);
            cmd.Parameters.AddWithValue("@date", simcha.Date);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            conn.Dispose();
        }
        public IEnumerable<Simcha> GetSimchos()
        {
            SqlConnection conn = new SqlConnection(_connectionstring);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"select s.Date as 'SimchaDate',s.Id,s.Name,sc.Amount,sc.ContributerId,sc.date from Simcha s
                                left join SimchaContributer sc
                                on sc.SimchaId = s.Id
                                group by s.Date,s.Id,s.Name,sc.Amount,sc.ContributerId,sc.date";
            conn.Open();
            List<Simcha> simchas = new List<Simcha>();
            List<Donations> donations = new List<Donations>();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var amt = reader["amount"];
                var conid = reader["contributerid"];
                var date = reader["date"];
                decimal amount = 0;
                int contributerId = 0;
                DateTime dateTime = DateTime.Now;
                if (amt != DBNull.Value && conid != DBNull.Value && date != DBNull.Value)
                {
                    amount = (decimal)amt;
                    contributerId = (int)conid;
                    dateTime = (DateTime)date;
                }
                if (amt != null)
                {
                    donations.Add(new Donations
                    {
                        SimchaId = (int)reader["Id"],
                        ContributerId = contributerId,
                        Amount = amount,
                        Date = dateTime
                    });
                }
                simchas.Add(new Simcha
                {
                    Id = (int)reader["Id"],
                    Date = (DateTime)reader["SimchaDate"],
                    Name = (string)reader["name"],
                    
                });

            }
            foreach (Simcha s in simchas)
            {
                foreach (Donations d in donations)
                {
                    if (d.Amount != 0)
                    {
                        if (s.Id == d.SimchaId)
                        {
                            
                            s.Total += d.Amount;
                        }
                    }

                }
            }
            return simchas;

        }
        public int AddContributer(Contributer contributer)
        {
            SqlConnection conn = new SqlConnection(_connectionstring);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"insert into contributer
                                values (@firstname, @lastname,@date, @cell, @alwaysinclude)SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@firstname", contributer.FirstName);
            cmd.Parameters.AddWithValue("@lastname", contributer.LastName);
            cmd.Parameters.AddWithValue("@cell", contributer.Cell);
            cmd.Parameters.AddWithValue("@date", contributer.Date);
            cmd.Parameters.AddWithValue("@alwaysinclude", contributer.AlwaysInclude);
            conn.Open();
            int id = (int)(decimal)cmd.ExecuteScalar();
            conn.Close();
            conn.Dispose();
            return id;
        }
        public void AddDeposit(decimal amount, int contributerid, DateTime date)
        {
            SqlConnection conn = new SqlConnection(_connectionstring);
            SqlCommand command = conn.CreateCommand();
            command.CommandText = @"insert into deposit
                                values (@amount, @contributerid,@date)";
            command.Parameters.AddWithValue("@amount", amount);
            command.Parameters.AddWithValue("@contributerid", contributerid);
            command.Parameters.AddWithValue("@date", date);
            conn.Open();
            command.ExecuteNonQuery();
            conn.Close();
            conn.Dispose();

        }
        public void AddDonation(Donations donations, bool first)
        {
            SqlConnection conn = new SqlConnection(_connectionstring);
            SqlCommand command = conn.CreateCommand();
            if (first)
            {
                command.CommandText = @"delete simchacontributer
                                    insert into simchacontributer
                                values (@simchaid, @contributerid,@amount,@date)";
            }
            else
            {
                command.CommandText = @"insert into simchacontributer
                                values (@simchaid, @contributerid,@amount,@date)";
            }            
            command.Parameters.AddWithValue("@amount", donations.Amount);
            command.Parameters.AddWithValue("@contributerid", donations.ContributerId);
            command.Parameters.AddWithValue("@date", DateTime.Today);
            command.Parameters.AddWithValue("@simchaid", donations.SimchaId);
            conn.Open();
            command.ExecuteNonQuery();
            conn.Close();
            conn.Dispose();
        }
        public void EditContributer (Contributer contributer)
        {
            SqlConnection conn = new SqlConnection(_connectionstring);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"update contributer
                                set FirstName = @firstname,  
                                LastName = @lastname,
                                Date = @date,
                                Cell = @cell,
                                AlwaysInclude = @alwaysinclude
                                where id = @id";
            cmd.Parameters.AddWithValue("@id", contributer.Id);
            cmd.Parameters.AddWithValue("@firstname", contributer.FirstName);
            cmd.Parameters.AddWithValue("@lastname", contributer.LastName);
            cmd.Parameters.AddWithValue("@cell", contributer.Cell);
            cmd.Parameters.AddWithValue("@date", contributer.Date);
            cmd.Parameters.AddWithValue("@alwaysinclude", contributer.AlwaysInclude);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            conn.Dispose();

        }

        public IEnumerable<Contributer> GetContributers()
        {
            SqlConnection conn = new SqlConnection(_connectionstring);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"select * from contributer";
            conn.Open();
            List<Contributer> contributers = new List<Contributer>();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                contributers.Add(new Contributer
                {
                    AlwaysInclude = (bool)reader["alwaysinclude"],
                    FirstName = (string)reader["firstname"],
                    LastName = (string)reader["lastname"],
                    Date = (DateTime)reader["date"],
                    Id = (int)reader["id"],
                    Cell = (string)reader["cell"],
                    Deposits = new List<Deposit>(),
                    Donations = new List<Donations>(),
                    DepositDonations = new List<DepositDonation>(),

                });
            } 
            foreach(Contributer c in contributers)
            {
                AddDepositsContributer(c);
                AddDonationsToContributer(c);
            }
            return contributers;

        }
        public void AddDonationsToContributer(Contributer c)
        {
            SqlConnection conn = new SqlConnection(_connectionstring);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"select sc.Amount, sc.SimchaId, sc.ContributerId , sc.date from SimchaContributer sc 
                                where sc.contributerid = @id";
            cmd.Parameters.AddWithValue("@id", c.Id);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var amt = reader["amount"];
                var simid = reader["simchaid"];
                var date = reader["date"];
                decimal amount = 0;
                int simchaId = 0;
                DateTime dateTime = DateTime.Now;
                if (amt != DBNull.Value && simid != DBNull.Value && date != DBNull.Value)
                {
                    amount = (decimal)amt;
                    simchaId = (int)simid;
                    dateTime = (DateTime)date;
                }
                if (amount != 0)
                {
                    c.Balance -= amount;
                    c.Donations.Add(new Donations
                    {
                        SimchaId = simchaId,
                        ContributerId = c.Id,
                        Amount = amount,
                        Date = dateTime
                    });
                }
            }
            }

        public void AddDepositsContributer(Contributer c)
        {
            SqlConnection conn = new SqlConnection(_connectionstring);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"select d.Amount as'depositamount', d.ContributerId as'depositcontributerid', d.Date as'depositdate', d.Id as'depositid' from Deposit d
                                where @id = d.ContributerId ";
            cmd.Parameters.AddWithValue("@id", c.Id);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                c.Balance += (decimal)reader["depositamount"];
                    c.Deposits.Add(new Deposit
                    {
                        Amount = (decimal)reader["depositamount"],
                        ContributerId = (int)reader["depositcontributerid"],
                        Id = (int)reader["depositid"],
                        Date = (DateTime)reader["depositdate"],
                    });
                
            }
        }
        public int GetCountContributers()
    {
        SqlConnection conn = new SqlConnection(_connectionstring);
        SqlCommand cmd = conn.CreateCommand();
        cmd.CommandText = @"select COUNT(*) from Contributer";
        conn.Open();
        int count = (int)cmd.ExecuteScalar();
        conn.Close();
        conn.Dispose();
        return count;
    }
    public int GetAmountOfPeopleGiving(int id)
    {
        SqlConnection conn = new SqlConnection(_connectionstring);
        SqlCommand cmd = conn.CreateCommand();
        cmd.CommandText = @" select COUNT(*) from SimchaContributer sc
                                where sc.SimchaId = @id";
        cmd.Parameters.AddWithValue("@id", id);
        conn.Open();
        int count = (int)cmd.ExecuteScalar();
        conn.Close();
        conn.Dispose();
        return count;
    }
        public void CombineAndSortDepositsAndDonations(Contributer c)
        {
            foreach(Deposit deposit in c.Deposits)
            {
               c.DepositDonations.Add(new DepositDonation
                {
                    Action = "Deposit",
                    Date = deposit.Date,
                    Amount = deposit.Amount
                });
            }
            foreach(Donations donations in c.Donations)
            {
                string name = GetSimchaNameForId(donations.SimchaId);
               c.DepositDonations.Add(new DepositDonation
                {
                    Action = $"Contribution to the {name}",
                    Date = donations.Date,
                    Amount = -donations.Amount,
                });
            }
            c.DepositDonations.Sort((d1,d2) => DateTime.Compare(d1.Date,d2.Date));           
        }
        public string GetSimchaNameForId(int id)
        {
            SqlConnection conn = new SqlConnection(_connectionstring);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"select Name from Simcha 
                                where id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            string name = (string)cmd.ExecuteScalar();
            conn.Close();
            conn.Dispose();
            return name;
        }
        public decimal GetAmountDonated(int simchaId, int contributerId)
        {
            SqlConnection conn = new SqlConnection(_connectionstring);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @" 	select sc.amount from SimchaContributer sc
                                where sc.ContributerId = @contributerid and
								 sc.simchaid = @simchaid";
            cmd.Parameters.AddWithValue("@contributerid", contributerId);
            cmd.Parameters.AddWithValue("@simchaid", simchaId);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            decimal amount = 0;
            while (reader.Read())
            {
                var amt = reader["amount"];
                
                if (amt != DBNull.Value)
                {
                   amount = (decimal)amt;
                }
            
            }
               
           
            conn.Close();
            conn.Dispose();
            return amount;
        }
    }
}
 



