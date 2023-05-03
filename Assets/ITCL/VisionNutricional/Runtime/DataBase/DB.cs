using System;
using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;
using WhateverDevs.Core.Runtime.Common;
// ReSharper disable MemberCanBePrivate.Global

namespace ITCL.VisionNutricional.Runtime.DataBase
{
    public class DB : Loggable<DB>
    {
        private const string DBName = "Database";
        private static string _dbUri;
        private static IDbConnection _dbConnection;
        
        public struct User
        {
            public string email;
            public string userName;
            public string password;
        }
        
        public struct Food
        {
            public string foodName;
            public float foodCalories;
        }
        
        public struct HistoricEntry
        {
            public string userEmail;
            public string foodName;
            public DateTime date;
        }
        
        /// <summary>
        /// Creates the database connection and the tables.
        /// </summary>
        public static void CreateDatabaseTables()
        {
            // Open a connection to the database.
            string filepath = Application.persistentDataPath + "/" + DBName;
            _dbUri = "URI=file:" + filepath + ".sqlite";
            _dbConnection = new SqliteConnection(_dbUri);

            // Create tables for the users, foods and historic in the database if they do not exist yet.
            const string queryCreateUsers =
                "CREATE TABLE IF NOT EXIST Users (email VARCHAR(100) PRIMARY KEY, "
                                                + "userName VARCHAR(100) NOT NULL, "
                                                + "password VARCHAR(100) NOT NULL)";
            Command(queryCreateUsers);
            
            const string queryCreateNutritionalValues =
                "CREATE TABLE IF NOT EXIST NutritionalValues (foodName VARCHAR(100) PRIMARY KEY, "
                                                            + "foodCalories FLOAT )";
            Command(queryCreateNutritionalValues);
            
            const string queryCreateHistoric =
                "CREATE TABLE IF NOT EXIST Historic (FOREIGN KEY(userEmail) REFERENCES Users(email), "
                                                + "FOREIGN KEY(foodName) REFERENCES NutritionalValues(foodName), "
                                                + "_date DATE, "
                                                + "PRIMARY KEY(userName, foodId, _date) )";
            Command(queryCreateHistoric);
        }
        
        public static IDbConnection GetDB() => _dbConnection;

        /// <summary>
        /// Executes an sql command on the database.
        /// </summary>
        /// <param name="command">The sql command.</param>
        public static void Command(string command)
        {
            _dbConnection.Open(); // Open connection to the database.
            IDbCommand dbCommand = _dbConnection.CreateCommand();
            dbCommand.CommandText = command;
            dbCommand.ExecuteReader();
            _dbConnection.Close();
        }

        #region Users

        /// <summary>
        /// Inserts a new user on the database.
        /// </summary>
        /// <param name="email">Email of the user, primary key.</param>
        /// <param name="userName">User name in the app.</param>
        /// <param name="password">User password.</param>
        /// <returns>Inserted user.</returns>
        public User InsertUser(string email, string userName, string password)
        {
            User user = new User();
            using (_dbConnection = new SqliteConnection(_dbUri))
            {
                _dbConnection.Open();
                IDbCommand dbCommand = _dbConnection.CreateCommand();

                dbCommand.CommandText = "INSERT INTO Users (email,userName,password) VALUES ("
                                        + email
                                        + "','"
                                        + userName
                                        + "','"
                                        + password
                                        + ") WHERE NOT EXISTS(SELECT 1 FROM Users WHERE email='"
                                        + email
                                        + "')";

                IDataReader reader = dbCommand.ExecuteReader();
                
                while (reader.Read())
                {
                    user.email = reader.GetString(0);
                    user.userName = reader.GetString(1);
                    user.password = reader.GetString(2);
                }

                reader.Close();
                dbCommand.Dispose();
                _dbConnection.Close();
            }

            return user;
        }

        
        /// <summary>
        /// Deletes an user from the database with its email.
        /// </summary>
        /// <param name="email">Email of the user, primary key.</param>
        /// <param name="password">User password.</param>
        /// <returns>Deleted user.</returns>
        public User DeleteUser(string email, string password)
        {
            User user = new User();
            using (_dbConnection = new SqliteConnection(_dbUri))
            {
                _dbConnection.Open();
                IDbCommand dbCommand = _dbConnection.CreateCommand();

                dbCommand.CommandText = "DELETE FROM Users WHERE email='"
                                        + email
                                        + "' AND password='"
                                        + password
                                        +"'";
                
                IDataReader reader = dbCommand.ExecuteReader();
                
                while (reader.Read())
                {
                    user.email = reader.GetString(0);
                    user.userName = reader.GetString(1);
                    user.password = reader.GetString(2);
                }

                reader.Close();
                dbCommand.Dispose();
                _dbConnection.Close();
            }

            return user;
        }

        /// <summary>
        /// Return all users in database.
        /// </summary>
        /// <returns>List of users.</returns>
        public List<User> SelectAllUsers()
        {
            List<User> usersInDB = new List<User>();

            using (_dbConnection = new SqliteConnection(_dbUri))
            {
                _dbConnection.Open();
                IDbCommand dbCommand = _dbConnection.CreateCommand();
                dbCommand.CommandText = "SELECT email, userName, password FROM Users ";
                IDataReader reader = dbCommand.ExecuteReader();

                while (reader.Read())
                {
                    User userAux = new User
                    {
                        email = reader.GetString(0),
                        userName = reader.GetString(1),
                        password = reader.GetString(2)
                    };
                    usersInDB.Add(userAux);
                }

                reader.Close();
                dbCommand.Dispose();
                _dbConnection.Close();
            }

            return usersInDB;
        }
        
        /// <summary>
        /// Gets a user from the database with its email.
        /// </summary>
        /// <param name="email">email from the user in the database.</param>
        /// <returns>User with that email.</returns>
        public User SelectUserByEmail(string email)
        {
            User user = new User();
            using (_dbConnection = new SqliteConnection(_dbUri))
            {
                _dbConnection.Open();
                IDbCommand dbCommand = _dbConnection.CreateCommand();
                dbCommand.CommandText = "SELECT email, userName, password FROM Users WHERE email ='" + email + "'";
                IDataReader dataReader = dbCommand.ExecuteReader();

                while (dataReader.Read())
                {
                    user.email = dataReader.GetString(0);
                    user.userName = dataReader.GetString(1);
                    user.password = dataReader.GetString(2);
                }

                dataReader.Close();
                dbCommand.Dispose();
                _dbConnection.Close();
            }

            return user;
        }
        
        /// <summary>
        /// Gets a user from the database with its userName.
        /// </summary>
        /// <param name="user">userName from the user in the app.</param>
        /// <returns>List of the users in the database with that userName.</returns>
        public List<User> SelectUserByName(string user)
        {
            List<User> usersInDB = new List<User>();

            using (_dbConnection = new SqliteConnection(_dbUri))
            {
                _dbConnection.Open();
                IDbCommand dbCommand = _dbConnection.CreateCommand();
                dbCommand.CommandText = "SELECT email, userName, password FROM Users WHERE userName ='" + user + "'";
                IDataReader dataReader = dbCommand.ExecuteReader();

                while (dataReader.Read())
                {
                    User userAux = new User
                    {
                        email = dataReader.GetString(0),
                        userName = dataReader.GetString(1),
                        password = dataReader.GetString(2)
                    };
                    usersInDB.Add(userAux);
                }

                dataReader.Close();
                dbCommand.Dispose();
                _dbConnection.Close();
            }

            return usersInDB;
        }
        
        /// <summary>
        /// Updates one users data in the database.
        /// </summary>
        /// <param name="email">email from the user in the database.</param>
        /// <param name="password">Old password from the user.</param>
        /// <param name="newUserName">New userName for the user in the database.</param>
        /// <param name="newPassword">New password for the user in the database.</param>
        /// <returns>Updated user..</returns>
        public User UpdateOneUser(string email, string password, string newUserName, string newPassword)
        {
            User user = new User();
            using (_dbConnection = new SqliteConnection(_dbUri))
            {
                _dbConnection.Open();
                IDbCommand dbCommand = _dbConnection.CreateCommand();

                dbCommand.CommandText = "UPDATE Users SET userName='"
                                        + newUserName
                                        + "', password='"
                                        + newPassword
                                        + "' WHERE email ='"
                                        + email
                                        + "' AND password='"
                                        + password
                                        + "'";

                IDataReader reader = dbCommand.ExecuteReader();
                
                while (reader.Read())
                {
                    user.email = reader.GetString(0);
                    user.userName = reader.GetString(1);
                    user.password = reader.GetString(2);
                }

                reader.Close();
                dbCommand.Dispose();
                _dbConnection.Close();
            }

            return user;
        }

        #endregion

        #region Food

        /// <summary>
        /// Inserts or modifies a food on the NutritionalValues table.
        /// </summary>
        /// <param name="fName">Food name.</param>
        /// <param name="fCalories">Food calories.</param>
        /// <returns>Inserted food.</returns>
        public static Food InsertFood(string fName, float fCalories)
        {
            Food food = new Food();
            using (_dbConnection = new SqliteConnection(_dbUri))
            {
                _dbConnection.Open();
                IDbCommand dbCommand = _dbConnection.CreateCommand();
                dbCommand.CommandText = "INSERT OR REPLACE INTO NutritionalValues (foodName, foodCalories) VALUES (" + fName + ", " + fCalories + ")";
                IDataReader dataReader = dbCommand.ExecuteReader();
                
                while (dataReader.Read())
                {
                    food.foodName = dataReader.GetString(1);
                    food.foodCalories = dataReader.GetFloat(2);
                }

                dataReader.Close();
                dbCommand.Dispose();
                _dbConnection.Close();
            }

            return food;
        }
        
        /// <summary>
        /// Deletes a food from the database with its name.
        /// </summary>
        /// <param name="foodName">Name from the food, primary key.</param>
        /// <returns>Deleted food.</returns>
        public Food DeleteFood(string foodName)
        {
            Food food = new Food();
            using (_dbConnection = new SqliteConnection(_dbUri))
            {
                _dbConnection.Open();
                IDbCommand dbCommand = _dbConnection.CreateCommand();
                dbCommand.CommandText = "DELETE FROM NutritionalValues WHERE foodName='" + foodName +"'";
                IDataReader reader = dbCommand.ExecuteReader();
                
                while (reader.Read())
                {
                    food.foodName = reader.GetString(1);
                    food.foodCalories = reader.GetFloat(2);
                }

                reader.Close();
                dbCommand.Dispose();
                _dbConnection.Close();
            }

            return food;
        }
        
        /// <summary>
        /// Return all foods in database.
        /// </summary>
        /// <returns>List of foods.</returns>
        public List<Food> SelectAllFoods()
        {
            List<Food> foodsInDB = new List<Food>();

            using (_dbConnection = new SqliteConnection(_dbUri))
            {
                _dbConnection.Open();
                IDbCommand dbCommand = _dbConnection.CreateCommand();
                dbCommand.CommandText = "SELECT foodName, foodCalories FROM NutritionalValues ";
                IDataReader reader = dbCommand.ExecuteReader();

                while (reader.Read())
                {
                    Food foodAux = new Food
                    {
                        foodName = reader.GetString(1),
                        foodCalories = reader.GetFloat(2)
                    };
                    foodsInDB.Add(foodAux);
                }

                reader.Close();
                dbCommand.Dispose();
                _dbConnection.Close();
            }

            return foodsInDB;
        }

        /// <summary>
        /// Gets a food from the database with its name.
        /// </summary>
        /// <param name="foodName">Name from the food in the database.</param>
        /// <returns>The food with that name.</returns>
        public Food SelectFoodByName(string foodName)
        {
            Food food = new Food();
            using (_dbConnection = new SqliteConnection(_dbUri))
            {
                _dbConnection.Open();
                IDbCommand dbCommand = _dbConnection.CreateCommand();
                dbCommand.CommandText = "SELECT foodName, foodCalories FROM NutritionalValues WHERE foodName ='" + foodName + "'";
                IDataReader dataReader = dbCommand.ExecuteReader();

                while (dataReader.Read())
                {
                    food.foodName = dataReader.GetString(1);
                    food.foodCalories = dataReader.GetFloat(2);
                }

                dataReader.Close();
                dbCommand.Dispose();
                _dbConnection.Close();
            }

            return food;
        }
        
        /// <summary>
        /// Updates one foods data in the database.
        /// </summary>
        /// <param name="foodName">name identifier for the food.</param>
        /// <param name="newFoodCalories">New calories value for the food in the database.</param>
        /// <returns>Updated food.</returns>
        public Food UpdateOneFood(string foodName, string newFoodCalories)
        {
            Food food = new Food();
            using (_dbConnection = new SqliteConnection(_dbUri))
            {
                _dbConnection.Open();
                IDbCommand dbCommand = _dbConnection.CreateCommand();

                dbCommand.CommandText = "UPDATE NutritionalValues SET foodcalories='"
                                        + newFoodCalories
                                        + "' WHERE foodName ='"
                                        + foodName
                                        + "'";

                IDataReader reader = dbCommand.ExecuteReader();
                
                while (reader.Read())
                {
                    food.foodName = reader.GetString(1);
                    food.foodCalories = reader.GetFloat(2);
                }

                reader.Close();
                dbCommand.Dispose();
                _dbConnection.Close();
            }

            return food;
        }

        #endregion
        
        #region Historic

        /// <summary>
        /// Inserts a new entry in the historic.
        /// </summary>
        /// <param name="userEmail">email identifier from the user.</param>
        /// <param name="foodName">Name identifier from the food.</param>
        /// <param name="date">Actual date.</param>
        /// <returns>HistoricEntry struct.</returns>
        public static HistoricEntry InsertIntoHistoric(string userEmail, string foodName, DateTime date)
        {
            HistoricEntry entry = new HistoricEntry();
            using (_dbConnection = new SqliteConnection(_dbUri))
            {
                _dbConnection.Open();
                IDbCommand dbCommand = _dbConnection.CreateCommand();
                dbCommand.CommandText = "INSERT INTO Historic (userEmail, foodName, _date) VALUES (" + userEmail + ", " + foodName + ", " + date + ")";
                IDataReader dataReader = dbCommand.ExecuteReader();
                
                while (dataReader.Read())
                {
                    entry.userEmail = dataReader.GetString(0);
                    entry.foodName = dataReader.GetString(1);
                    entry.date = dataReader .GetDateTime(2);
                }

                dataReader.Close();
                dbCommand.Dispose();
                _dbConnection.Close();
            }

            return entry;
        }
        
        /// <summary>
        /// Deletes an entry from the historic.
        /// </summary>
        /// <param name="userEmail">email identifier from the user.</param>
        /// <param name="foodName">Name identifier from the food to delete.</param>
        /// <param name="date">Date from the entry to delete.</param>
        /// <returns>List of historic entries.</returns>
        public HistoricEntry DeleteEntry(string userEmail, string foodName, DateTime date)
        {
            HistoricEntry entry = new HistoricEntry();
            using (_dbConnection = new SqliteConnection(_dbUri))
            {
                _dbConnection.Open();
                IDbCommand dbCommand = _dbConnection.CreateCommand();
                dbCommand.CommandText = "DELETE FROM Historic WHERE userEmail='" + userEmail +"' AND foodName ='" + foodName + "' AND _date='" + date +"'";
                IDataReader reader = dbCommand.ExecuteReader();
                
                while (reader.Read())
                {
                    entry.userEmail = reader.GetString(0);
                    entry.foodName = reader.GetString(1);
                    entry.date = reader.GetDateTime(2);
                }

                reader.Close();
                dbCommand.Dispose();
                _dbConnection.Close();
            }

            return entry;
        }
        
        /// <summary>
        /// Return all entries in the historic.
        /// </summary>
        /// <returns>List of entries.</returns>
        public List<HistoricEntry> SelectAllEntries()
        {
            List<HistoricEntry> entriesInDB = new List<HistoricEntry>();

            using (_dbConnection = new SqliteConnection(_dbUri))
            {
                _dbConnection.Open();
                IDbCommand dbCommand = _dbConnection.CreateCommand();
                dbCommand.CommandText = "SELECT userEmail, foodName, _date FROM Historic ";
                IDataReader reader = dbCommand.ExecuteReader();

                while (reader.Read())
                {
                    HistoricEntry entryAux = new HistoricEntry
                    {
                        userEmail = reader.GetString(0),
                        foodName = reader.GetString(1),
                        date = reader.GetDateTime(2)
                    };
                    entriesInDB.Add(entryAux);
                }

                reader.Close();
                dbCommand.Dispose();
                _dbConnection.Close();
            }

            return entriesInDB;
        }
        
        /// <summary>
        /// Return all entries in the historic from a specific user.
        /// </summary>
        /// <param name="userEmail">email identifier from the user.</param>
        /// <returns>List of historic entries.</returns>
        public List<HistoricEntry> SelectAllEntriesFromUser(string userEmail)
        {
            List<HistoricEntry> entriesInDB = new List<HistoricEntry>();

            using (_dbConnection = new SqliteConnection(_dbUri))
            {
                _dbConnection.Open();
                IDbCommand dbCommand = _dbConnection.CreateCommand();
                dbCommand.CommandText = "SELECT userEmail, foodName, _date FROM Historic WHERE userEmail='" + userEmail +"'";
                IDataReader reader = dbCommand.ExecuteReader();

                while (reader.Read())
                {
                    HistoricEntry entryAux = new HistoricEntry
                    {
                        userEmail = reader.GetString(0),
                        foodName = reader.GetString(1),
                        date = reader.GetDateTime(2)
                    };
                    entriesInDB.Add(entryAux);
                }

                reader.Close();
                dbCommand.Dispose();
                _dbConnection.Close();
            }

            return entriesInDB;
        }
        
        /// <summary>
        /// Return all entries in the historic with a specific food.
        /// </summary>
        /// <param name="foodName">name identifier from the food.</param>
        /// <returns>List of historic entries.</returns>
        public List<HistoricEntry> SelectAllEntriesWithFood(string foodName)
        {
            List<HistoricEntry> entriesInDB = new List<HistoricEntry>();

            using (_dbConnection = new SqliteConnection(_dbUri))
            {
                _dbConnection.Open();
                IDbCommand dbCommand = _dbConnection.CreateCommand();
                dbCommand.CommandText = "SELECT userEmail, foodName, _date FROM Historic WHERE foodName='" + foodName +"'";
                IDataReader reader = dbCommand.ExecuteReader();

                while (reader.Read())
                {
                    HistoricEntry entryAux = new HistoricEntry
                    {
                        userEmail = reader.GetString(0),
                        foodName = reader.GetString(1),
                        date = reader.GetDateTime(2)
                    };
                    entriesInDB.Add(entryAux);
                }

                reader.Close();
                dbCommand.Dispose();
                _dbConnection.Close();
            }

            return entriesInDB;
        }

        #endregion
    }
}