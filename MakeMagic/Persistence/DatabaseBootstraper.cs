using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MakeMagic.Persistence
{
    public class DatabaseBootstraper
    {

        public static void Init(IDbConnection connection)
        {
            try
            {
                connection.Open();

                connection.Execute("CREATE DATABASE IF NOT EXISTS MakeMagic;");
                connection.ChangeDatabase("MakeMagic");
                connection.Execute(@"
                    CREATE TABLE IF NOT EXISTS Characters(
                    id INT NOT NULL AUTO_INCREMENT,
                    name VARCHAR(128) NOT NULL,
                    role VARCHAR(16) NOT NULL,
                    school VARCHAR(128) NOT NULL,
                    house VARCHAR(32) NOT NULL,
                    patronus VARCHAR(16) NOT NULL,
                    PRIMARY KEY(id)); ");
            }
            finally
            {
                if(connection.State == ConnectionState.Open)
                    connection.Close();
            }
            
        }
    }
}
