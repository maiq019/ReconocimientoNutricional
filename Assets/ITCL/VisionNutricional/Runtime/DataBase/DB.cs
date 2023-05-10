using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Mono.Data.Sqlite;
using UnityEngine;
using WhateverDevs.Core.Runtime.Common;
// ReSharper disable MemberCanBePrivate.Global

namespace ITCL.VisionNutricional.Runtime.DataBase
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DB : Loggable<DB>
    {
        private const string DBName = "Database";
        private static string _dbUri;
        private static IDbConnection _dbConnection;
        
        [Serializable]
        public struct User
        {
            public string email;
            public string userName;
            public string password;
        }
        
        [Serializable]
        public struct Food
        {
            public string foodName;
            public float calories;
            public float fat; 
            public float saturatedFat;
            public float carbHyd;
            public float sugar;
            public float protein;
            public float salt;
        }
        
        [Serializable]
        public struct HistoricEntry
        {
            public string userEmail;
            public string foodName;
            public string date;
        }

        /// <summary>
        /// Creates the database connection.
        /// </summary>
        /// <returns></returns>
        public static IDbConnection CreateDataBase()
        {
            // Open a connection to the database.
            string filepath = Application.persistentDataPath + "/" + DBName;
            _dbUri = "URI=file:" + filepath + ".sqlite";
            _dbConnection = new SqliteConnection(_dbUri);

            return _dbConnection;
        }
        
        public static IDbConnection GetDB() => _dbConnection;
        
        /// <summary>
        /// Create tables for the users, foods and historic in the database if they do not exist yet.
        /// </summary>
        public static void CreateDatabaseTables()
        {
            const string usersQuery = "CREATE TABLE IF NOT EXISTS Users (email TEXT PRIMARY KEY, "
                                                                    + "userName TEXT NOT NULL, "
                                                                    + "password TEXT NOT NULL) STRICT";
            Command(usersQuery);
            
            const string foodsQuery =
                "CREATE TABLE IF NOT EXISTS Foods (foodName TEXT PRIMARY KEY, "
                                            + "calories REAL, "
                                            + "fat REAL, "
                                            + "saturatedFat REAL, "
                                            + "carbhyd REAL, "
                                            + "sugar REAL, "
                                            + "protein REAL, "
                                            + "salt REAL) STRICT";
            Command(foodsQuery);

            const string historicQuery =
                "CREATE TABLE IF NOT EXISTS Historic (userEmail TEXT NOT NULL, "
                                                + "foodName TEXT NOT NULL, "
                                                + "_date TEXT NOT NULL, "
                                                + "FOREIGN KEY(userEmail) REFERENCES Users(email), "
                                                + "FOREIGN KEY(foodName) REFERENCES Foods(foodName), "
                                                + "PRIMARY KEY(userEmail, foodName, _date))";
                /*"CREATE TABLE IF NOT EXISTS Historic (userEmail TEXT NOT NULL, "
                                                + "foodName TEXT NOT NULL, "
                                                + "_date DATE NOT NULL, "
                                                + "PRIMARY KEY(userEmail, foodName, _date))";*/
            Command(historicQuery);
        }

        /// <summary>
        /// Executes an sql command on the database.
        /// </summary>
        /// <param name="command">The sql command.</param>
        public static void Command(string command)
        {
            _dbConnection.Open(); // Open connection to the database.
            IDbCommand cmd = _dbConnection.CreateCommand();
            cmd.CommandText = "PRAGMA foreign_keys = ON";
            cmd.ExecuteNonQuery();
            
            IDbCommand dbCommand = _dbConnection.CreateCommand();
            dbCommand.CommandText = command;
            
            try
            {
                dbCommand.ExecuteScalar();
            }
            catch (Exception e)
            {
                StaticLogger.Error(e);
            }
            
            _dbConnection.Close();
        }
        
        public static void DeleteDatabase()
        {
            using (_dbConnection = new SqliteConnection(_dbUri))
            {
                _dbConnection.Open();
                IDbCommand dbCommand = _dbConnection.CreateCommand();
                
                dbCommand.CommandText = "DROP TABLE Users";
                dbCommand.ExecuteScalar();
                
                dbCommand.CommandText = "DROP TABLE Foods";
                dbCommand.ExecuteScalar();
                
                dbCommand.CommandText = "DROP TABLE Historic";
                dbCommand.ExecuteScalar();
                
                _dbConnection.Close();
            }
        }

        #region Users

        /// <summary>
        /// Inserts a new user on the database.
        /// </summary>
        /// <param name="email">Email of the user, primary key.</param>
        /// <param name="userName">User name in the app.</param>
        /// <param name="password">User password.</param>
        /// <returns>Inserted user.</returns>
        public static User InsertUser(string email, string userName, string password)
        {
            User user = new User();
            using (_dbConnection = new SqliteConnection(_dbUri))
            {
                _dbConnection.Open();
                IDbCommand dbCommand = _dbConnection.CreateCommand();

                dbCommand.CommandText = "INSERT OR IGNORE INTO Users (email,userName,password) VALUES ('"
                                        + email
                                        + "', '"
                                        + userName
                                        + "', '"
                                        + password
                                        + "')";
                try
                {
                    IDataReader dataReader = dbCommand.ExecuteReader();

                    while (dataReader.Read())
                    {
                        user.email = dataReader.GetString(0);
                        user.userName = dataReader.GetString(1);
                        user.password = dataReader.GetString(2);
                    }
                    
                    dataReader.Close();
                }
                catch (Exception e)
                {
                    StaticLogger.Debug(e);
                }

                dbCommand.Dispose();
                _dbConnection.Close();
            }

            return user;
        }
        
        /// <summary>
        /// Inserts a new user on the database.
        /// </summary>
        /// <param name="user">User to insert.</param>
        /// <returns></returns>
        public static User InsertUser(User user)
        {
            using (_dbConnection = new SqliteConnection(_dbUri))
            {
                _dbConnection.Open();
                IDbCommand dbCommand = _dbConnection.CreateCommand();

                string email = user.email;
                string userName = user.userName;
                string password = user.password;
                dbCommand.CommandText = "INSERT OR IGNORE INTO Users (email,userName,password) VALUES ('"
                                        + email
                                        + "', '"
                                        + userName
                                        + "', '"
                                        + password
                                        + "')";
                try
                {
                    IDataReader dataReader = dbCommand.ExecuteReader();

                    while (dataReader.Read())
                    {
                        user.email = dataReader.GetString(0);
                        user.userName = dataReader.GetString(1);
                        user.password = dataReader.GetString(2);
                    }
                    
                    dataReader.Close();
                }
                catch (Exception e)
                {
                    StaticLogger.Debug(e);
                }

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
        public static User DeleteUser(string email, string password)
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

                try
                {
                    IDataReader dataReader = dbCommand.ExecuteReader();

                    while (dataReader.Read())
                    {
                        user.email = dataReader.GetString(0);
                        user.userName = dataReader.GetString(1);
                        user.password = dataReader.GetString(2);
                    }
                    
                    dataReader.Close();
                }
                catch (Exception e)
                {
                    StaticLogger.Debug(e);
                }
                
                dbCommand.Dispose();
                _dbConnection.Close();
            }

            return user;
        }
        
        /// <summary>
        /// Deletes an user from the database.
        /// </summary>
        /// <param name="user">User to delete.</param>
        /// <returns></returns>
        public static User DeleteUser(User user)
        {
            using (_dbConnection = new SqliteConnection(_dbUri))
            {
                _dbConnection.Open();
                IDbCommand dbCommand = _dbConnection.CreateCommand();

                string email = user.email;
                string password = user.password;
                dbCommand.CommandText = "DELETE FROM Users WHERE email='"
                                        + email
                                        + "' AND password='"
                                        + password
                                        +"'";

                try
                {
                    IDataReader dataReader = dbCommand.ExecuteReader();

                    while (dataReader.Read())
                    {
                        user.email = dataReader.GetString(0);
                        user.userName = dataReader.GetString(1);
                        user.password = dataReader.GetString(2);
                    }
                    
                    dataReader.Close();
                }
                catch (Exception e)
                {
                    StaticLogger.Debug(e);
                }
                
                dbCommand.Dispose();
                _dbConnection.Close();
            }

            return user;
        }

        /// <summary>
        /// Return all users in database.
        /// </summary>
        /// <returns>List of users.</returns>
        public static List<User> SelectAllUsers()
        {
            List<User> usersInDB = new List<User>();

            using (_dbConnection = new SqliteConnection(_dbUri))
            {
                _dbConnection.Open();
                IDbCommand dbCommand = _dbConnection.CreateCommand();
                dbCommand.CommandText = "SELECT * FROM Users";
                
                try
                {
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
                }
                catch (Exception e)
                {
                    StaticLogger.Error(e);
                }
                
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
        public static User SelectUserByEmail(string email)
        {
            User user = new User();
            using (_dbConnection = new SqliteConnection(_dbUri))
            {
                _dbConnection.Open();
                IDbCommand dbCommand = _dbConnection.CreateCommand();
                dbCommand.CommandText = "SELECT * FROM Users WHERE email ='" + email + "'";
                
                try
                {
                    IDataReader dataReader = dbCommand.ExecuteReader();

                    while (dataReader.Read())
                    {
                        user.email = dataReader.GetString(0);
                        user.userName = dataReader.GetString(1);
                        user.password = dataReader.GetString(2);
                    }

                    dataReader.Close();
                }
                catch (Exception e)
                {
                    StaticLogger.Error(e);
                }
                
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
        public static List<User> SelectUserByName(string user)
        {
            List<User> usersInDB = new List<User>();

            using (_dbConnection = new SqliteConnection(_dbUri))
            {
                _dbConnection.Open();
                IDbCommand dbCommand = _dbConnection.CreateCommand();
                dbCommand.CommandText = "SELECT * FROM Users WHERE userName ='" + user + "'";
                
                try
                {
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
                }
                catch (Exception e)
                {
                    StaticLogger.Error(e);
                }
                
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
        /// <returns>Updated user.</returns>
        public static User UpdateOneUser(string email, string password, string newUserName, string newPassword)
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
                                        + "' WHERE email='"
                                        + email
                                        + "' AND password='"
                                        + password
                                        + "'";

                try
                {
                    IDataReader dataReader = dbCommand.ExecuteReader();

                    while (dataReader.Read())
                    {
                        user.email = dataReader.GetString(0);
                        user.userName = dataReader.GetString(1);
                        user.password = dataReader.GetString(2);
                    }

                    dataReader.Close();
                }
                catch (Exception e)
                {
                    StaticLogger.Error(e);
                }
                
                dbCommand.Dispose();
                _dbConnection.Close();
            }

            return user;
        }

        #endregion

        #region Food

        /// <summary>
        /// Inserts a food on the Foods table.
        /// </summary>
        /// <param name="fName">Food name.</param>
        /// <param name="calories">Food calories.</param>
        /// <param name="fat">Food fat.</param>
        /// <param name="saturatedFat">Food saturatedFat.</param>
        /// <param name="carbHyd">Food carbHyd.</param>
        /// <param name="sugar">Food sugar.</param>
        /// <param name="protein">Food protein.</param>
        /// <param name="salt">Food salt.</param>
        /// <returns>Inserted food.</returns>
        public static Food InsertFood(string fName, float calories, float fat, float saturatedFat, float carbHyd, float sugar, float protein, float salt)
        {
            Food food = new Food();
            using (_dbConnection = new SqliteConnection(_dbUri))
            {
                _dbConnection.Open();
                IDbCommand dbCommand = _dbConnection.CreateCommand();
                dbCommand.CommandText = "INSERT OR IGNORE INTO Foods (foodName, calories, fat, saturatedFat, carbhyd, sugar, protein, salt) VALUES ('" 
                                        + fName+"', "+calories+", "+fat+", "+saturatedFat+", "+carbHyd+", "+sugar+", "+protein+", "+salt+")";
                
                try
                {
                    IDataReader dataReader = dbCommand.ExecuteReader();
                    
                    while (dataReader.Read())
                    {
                        food.foodName = dataReader.GetString(0);
                        food.calories = dataReader.GetFloat(1);
                        food.fat = dataReader.GetFloat(2);
                        food.saturatedFat = dataReader.GetFloat(3);
                        food.carbHyd = dataReader.GetFloat(4);
                        food.sugar = dataReader.GetFloat(5);
                        food.protein = dataReader.GetFloat(6);
                        food.salt = dataReader.GetFloat(7);
                    }

                    dataReader.Close();
                }
                catch (Exception e)
                {
                    StaticLogger.Error(e);
                }
                
                dbCommand.Dispose();
                _dbConnection.Close();
            }

            return food;
        }
        
        /// <summary>
        /// Inserts a food on the Foods table.
        /// </summary>
        /// <param name="food">Food to insert.</param>
        /// <returns></returns>
        public static Food InsertFood(Food food)
        {
            using (_dbConnection = new SqliteConnection(_dbUri))
            {
                _dbConnection.Open();
                IDbCommand dbCommand = _dbConnection.CreateCommand();

                string foodName = food.foodName;
                dbCommand.CommandText = "INSERT OR IGNORE INTO Foods (foodName, calories, fat, saturatedFat, carbhyd, sugar, protein, salt) VALUES ('" 
                                        +foodName+"', "+food.calories+", "+food.fat+", "+food.saturatedFat+", "
                                        +food.carbHyd+", "+food.sugar+", "+food.protein+", "+food.salt+")";
                
                try
                {
                    IDataReader dataReader = dbCommand.ExecuteReader();
                    
                    while (dataReader.Read())
                    {
                        food.foodName = dataReader.GetString(0);
                        food.calories = dataReader.GetFloat(1);
                        food.fat = dataReader.GetFloat(2);
                        food.saturatedFat = dataReader.GetFloat(3);
                        food.carbHyd = dataReader.GetFloat(4);
                        food.sugar = dataReader.GetFloat(5);
                        food.protein = dataReader.GetFloat(6);
                        food.salt = dataReader.GetFloat(7);
                    }

                    dataReader.Close();
                }
                catch (Exception e)
                {
                    StaticLogger.Error(e);
                }
                
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
        public static Food DeleteFood(string foodName)
        {
            Food food = new Food();
            using (_dbConnection = new SqliteConnection(_dbUri))
            {
                _dbConnection.Open();
                IDbCommand dbCommand = _dbConnection.CreateCommand();
                dbCommand.CommandText = "DELETE FROM Foods WHERE foodName='" + foodName + "'";
                
                try
                {
                    IDataReader dataReader = dbCommand.ExecuteReader();
                
                    while (dataReader.Read())
                    {
                        food.foodName = dataReader.GetString(0);
                        food.calories = dataReader.GetFloat(1);
                        food.fat = dataReader.GetFloat(2);
                        food.saturatedFat = dataReader.GetFloat(3);
                        food.carbHyd = dataReader.GetFloat(4);
                        food.sugar = dataReader.GetFloat(5);
                        food.protein = dataReader.GetFloat(6);
                        food.salt = dataReader.GetFloat(7);
                    }

                    dataReader.Close();
                }
                catch (Exception e)
                {
                    StaticLogger.Debug(e);
                }
                
                dbCommand.Dispose();
                _dbConnection.Close();
            }

            return food;
        }
        
        /// <summary>
        /// Deletes a food from the database.
        /// </summary>
        /// <param name="food">Food to delete.</param>
        /// <returns></returns>
        public static Food DeleteFood(Food food)
        {
            using (_dbConnection = new SqliteConnection(_dbUri))
            {
                _dbConnection.Open();
                IDbCommand dbCommand = _dbConnection.CreateCommand();
                string foodName = food.foodName;
                dbCommand.CommandText = "DELETE FROM Foods WHERE foodName='" + foodName + "'";
                
                try
                {
                    IDataReader dataReader = dbCommand.ExecuteReader();
                
                    while (dataReader.Read())
                    {
                        food.foodName = dataReader.GetString(0);
                        food.calories = dataReader.GetFloat(1);
                        food.fat = dataReader.GetFloat(2);
                        food.saturatedFat = dataReader.GetFloat(3);
                        food.carbHyd = dataReader.GetFloat(4);
                        food.sugar = dataReader.GetFloat(5);
                        food.protein = dataReader.GetFloat(6);
                        food.salt = dataReader.GetFloat(7);
                    }

                    dataReader.Close();
                }
                catch (Exception e)
                {
                    StaticLogger.Debug(e);
                }
                
                dbCommand.Dispose();
                _dbConnection.Close();
            }

            return food;
        }
        
        /// <summary>
        /// Return all foods in database.
        /// </summary>
        /// <returns>List of foods.</returns>
        public static List<Food> SelectAllFoods()
        {
            List<Food> foodsInDB = new List<Food>();

            using (_dbConnection = new SqliteConnection(_dbUri))
            {
                _dbConnection.Open();
                IDbCommand dbCommand = _dbConnection.CreateCommand();
                dbCommand.CommandText = "SELECT * FROM Foods";
                
                try
                {
                    IDataReader dataReader = dbCommand.ExecuteReader();

                    while (dataReader.Read())
                    {
                        Food foodAux = new Food
                        {
                            foodName = dataReader.GetString(0),
                            calories = dataReader.GetFloat(1),
                            fat = dataReader.GetFloat(2),
                            saturatedFat = dataReader.GetFloat(3),
                            carbHyd = dataReader.GetFloat(4),
                            sugar = dataReader.GetFloat(5),
                            protein = dataReader.GetFloat(6),
                            salt = dataReader.GetFloat(7)
                        };
                        foodsInDB.Add(foodAux);
                    }

                    dataReader.Close();
                }
                catch (Exception e)
                {
                    StaticLogger.Error(e);
                }
                
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
        public static Food SelectFoodByName(string foodName)
        {
            Food food = new Food();
            using (_dbConnection = new SqliteConnection(_dbUri))
            {
                _dbConnection.Open();
                IDbCommand dbCommand = _dbConnection.CreateCommand();
                dbCommand.CommandText = "SELECT * FROM Foods WHERE foodName ='" + foodName + "'";
                
                try
                {
                    IDataReader dataReader = dbCommand.ExecuteReader();

                    while (dataReader.Read())
                    {
                        food.foodName = dataReader.GetString(0);
                        food.calories = dataReader.GetFloat(1);
                        food.fat = dataReader.GetFloat(2);
                        food.saturatedFat = dataReader.GetFloat(3);
                        food.carbHyd = dataReader.GetFloat(4);
                        food.sugar = dataReader.GetFloat(5);
                        food.protein = dataReader.GetFloat(6);
                        food.salt = dataReader.GetFloat(7);
                    }

                    dataReader.Close();
                }
                catch (Exception e)
                {
                    StaticLogger.Error(e);
                }
                
                dbCommand.Dispose();
                _dbConnection.Close();
            }

            return food;
        }

        /// <summary>
        /// Updates one foods data in the database.
        /// </summary>
        /// <param name="foodName">name identifier for the food.</param>
        /// <param name="newCalories">New calories value for the food.</param>
        /// <param name="newFat">New fat value for the food.</param>
        /// <param name="newSatFat">New saturated fat value for the food.</param>
        /// <param name="newCarbhyd">New carbohydrates value for the food.</param>
        /// <param name="newSugar">New sugar value for the food.</param>
        /// <param name="newProtein">New protein value for the food.</param>
        /// <param name="newSalt">New salt value for the food.</param>
        /// <returns>Updated food.</returns>
        public static Food UpdateOneFood(string foodName, float newCalories, float newFat, float newSatFat, float newCarbhyd, float newSugar, float newProtein, float newSalt)
        {
            Food food = new Food();
            using (_dbConnection = new SqliteConnection(_dbUri))
            {
                _dbConnection.Open();
                IDbCommand dbCommand = _dbConnection.CreateCommand();

                dbCommand.CommandText = "UPDATE Foods SET calories="
                                        + newCalories
                                        + ", fat="
                                        + newFat
                                        + ", saturatedFat="
                                        + newSatFat
                                        + ", carbhyd="
                                        + newCarbhyd
                                        + ", sugar="
                                        + newSugar
                                        + ", protein="
                                        + newProtein
                                        + ", salt="
                                        + newSalt
                                        + " WHERE foodName ='"
                                        + foodName
                                        + "'";
                
                try
                {
                    IDataReader dataReader = dbCommand.ExecuteReader();
                
                    while (dataReader.Read())
                    {
                        food.foodName = dataReader.GetString(0);
                        food.calories = dataReader.GetFloat(1);
                        food.fat = dataReader.GetFloat(2);
                        food.saturatedFat = dataReader.GetFloat(3);
                        food.carbHyd = dataReader.GetFloat(4);
                        food.sugar = dataReader.GetFloat(5);
                        food.protein = dataReader.GetFloat(6);
                        food.salt = dataReader.GetFloat(7);
                    }

                    dataReader.Close();
                }
                catch (Exception e)
                {
                    StaticLogger.Error(e);
                }
                
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
        /// <returns>HistoricEntry struct.</returns>
        public static HistoricEntry InsertIntoHistoric(string userEmail, string foodName)
        {
            HistoricEntry entry = new HistoricEntry();
            using (_dbConnection = new SqliteConnection(_dbUri))
            {
                _dbConnection.Open();
                
                IDbCommand cmd = _dbConnection.CreateCommand();
                cmd.CommandText = "PRAGMA foreign_keys = ON";
                cmd.ExecuteNonQuery();
                
                CultureInfo spanishCultureInfo = CultureInfo.CreateSpecificCulture("es-ES");
                string date = DateTime.Now.ToString(spanishCultureInfo);
                
                IDbCommand dbCommand = _dbConnection.CreateCommand();
                dbCommand.CommandText = "INSERT OR IGNORE INTO Historic (userEmail, foodName, _date) VALUES ('" + userEmail + "', '" + foodName + "', '" + date + "')";
                
                try
                {
                    IDataReader dataReader = dbCommand.ExecuteReader();
                
                    while (dataReader.Read())
                    {
                        entry.userEmail = dataReader.GetString(0);
                        entry.foodName = dataReader.GetString(1);
                        entry.date = dataReader .GetString(2);
                    }

                    dataReader.Close();
                }
                catch (Exception e)
                {
                    StaticLogger.Debug(e);
                }
                
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
        public static HistoricEntry DeleteEntry(string userEmail, string foodName, string date)
        {
            HistoricEntry entry = new HistoricEntry();
            using (_dbConnection = new SqliteConnection(_dbUri))
            {
                _dbConnection.Open();
                
                IDbCommand cmd = _dbConnection.CreateCommand();
                cmd.CommandText = "PRAGMA foreign_keys = ON";
                cmd.ExecuteNonQuery();
                
                IDbCommand dbCommand = _dbConnection.CreateCommand();
                dbCommand.CommandText = "DELETE FROM Historic WHERE userEmail='" + userEmail + "' AND foodName ='" + foodName + "' AND _date='" + date + "'";
                
                try
                {
                    IDataReader reader = dbCommand.ExecuteReader();
                
                    while (reader.Read())
                    {
                        entry.userEmail = reader.GetString(0);
                        entry.foodName = reader.GetString(1);
                        entry.date = reader.GetString(2);
                    }

                    reader.Close();
                }
                catch (Exception e)
                {
                    StaticLogger.Error(e);
                }
                
                dbCommand.Dispose();
                _dbConnection.Close();
            }

            return entry;
        }

        /// <summary>
        /// Deletes an entry from the historic.
        /// </summary>
        /// <param name="entry">Entry to delete.</param>
        /// <returns></returns>
        public static HistoricEntry DeleteEntry(HistoricEntry entry)
        {
            using (_dbConnection = new SqliteConnection(_dbUri))
            {
                _dbConnection.Open();
                
                IDbCommand cmd = _dbConnection.CreateCommand();
                cmd.CommandText = "PRAGMA foreign_keys = ON";
                cmd.ExecuteNonQuery();
                
                IDbCommand dbCommand = _dbConnection.CreateCommand();
                string userEmail = entry.userEmail;
                string foodName = entry.foodName;
                string date = entry.date;
                dbCommand.CommandText = "DELETE FROM Historic WHERE userEmail='" + userEmail + "' AND foodName ='" + foodName + "' AND _date='" + date + "'";
                
                try
                {
                    IDataReader reader = dbCommand.ExecuteReader();
                
                    while (reader.Read())
                    {
                        entry.userEmail = reader.GetString(0);
                        entry.foodName = reader.GetString(1);
                        entry.date = reader.GetString(2);
                    }

                    reader.Close();
                }
                catch (Exception e)
                {
                    StaticLogger.Error(e);
                }
                
                dbCommand.Dispose();
                _dbConnection.Close();
            }

            return entry;
        }
        
        /// <summary>
        /// Return all entries in the historic.
        /// </summary>
        /// <returns>List of entries.</returns>
        public static List<HistoricEntry> SelectAllEntries()
        {
            List<HistoricEntry> entriesInDB = new List<HistoricEntry>();

            using (_dbConnection = new SqliteConnection(_dbUri))
            {
                _dbConnection.Open();
                
                IDbCommand cmd = _dbConnection.CreateCommand();
                cmd.CommandText = "PRAGMA foreign_keys = ON";
                cmd.ExecuteNonQuery();
                
                IDbCommand dbCommand = _dbConnection.CreateCommand();
                dbCommand.CommandText = "SELECT * FROM Historic";
                
                try
                {
                    IDataReader dataReader = dbCommand.ExecuteReader();

                    while (dataReader.Read())
                    {
                        HistoricEntry entryAux = new HistoricEntry
                        {
                            userEmail = dataReader.GetString(0),
                            foodName = dataReader.GetString(1),
                            date = dataReader.GetString(2)
                        };
                        entriesInDB.Add(entryAux);
                    }

                    dataReader.Close();
                }
                catch (Exception e)
                {
                    StaticLogger.Error(e);
                }
                
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
        public static List<HistoricEntry> SelectAllEntriesFromUser(string userEmail)
        {
            List<HistoricEntry> entriesInDB = new List<HistoricEntry>();

            using (_dbConnection = new SqliteConnection(_dbUri))
            {
                _dbConnection.Open();
                
                IDbCommand cmd = _dbConnection.CreateCommand();
                cmd.CommandText = "PRAGMA foreign_keys = ON";
                cmd.ExecuteNonQuery();
                
                IDbCommand dbCommand = _dbConnection.CreateCommand();
                dbCommand.CommandText = "SELECT * FROM Historic WHERE userEmail='" + userEmail + "'";
                
                try
                {
                    IDataReader dataReader = dbCommand.ExecuteReader();

                    while (dataReader.Read())
                    {
                        HistoricEntry entryAux = new HistoricEntry
                        {
                            userEmail = dataReader.GetString(0),
                            foodName = dataReader.GetString(1),
                            date = dataReader.GetString(2)
                        };
                        entriesInDB.Add(entryAux);
                    }

                    dataReader.Close();
                }
                catch (Exception e)
                {
                    StaticLogger.Error(e);
                }
                
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
        public static List<HistoricEntry> SelectAllEntriesWithFood(string foodName)
        {
            List<HistoricEntry> entriesInDB = new List<HistoricEntry>();

            using (_dbConnection = new SqliteConnection(_dbUri))
            {
                _dbConnection.Open();
                
                IDbCommand cmd = _dbConnection.CreateCommand();
                cmd.CommandText = "PRAGMA foreign_keys = ON";
                cmd.ExecuteNonQuery();
                
                IDbCommand dbCommand = _dbConnection.CreateCommand();
                dbCommand.CommandText = "SELECT * FROM Historic WHERE foodName='" + foodName + "'";
                
                try
                {
                    IDataReader dataReader = dbCommand.ExecuteReader();

                    while (dataReader.Read())
                    {
                        HistoricEntry entryAux = new HistoricEntry
                        {
                            userEmail = dataReader.GetString(0),
                            foodName = dataReader.GetString(1),
                            date = dataReader.GetString(2)
                        };
                        entriesInDB.Add(entryAux);
                    }

                    dataReader.Close();
                }
                catch (Exception e)
                {
                    StaticLogger.Error(e);
                }
                
                dbCommand.Dispose();
                _dbConnection.Close();
            }

            return entriesInDB;
        }
        
        /// <summary>
        /// Return all entries in the historic from a specific user with a specific food.
        /// </summary>
        /// <param name="userEmail">email identifier from the user.</param>
        /// <param name="foodName">name identifier from the food.</param>
        /// <returns></returns>
        public static List<HistoricEntry> SelectAllEntriesFromUserAndFood(string userEmail, string foodName)
        {
            List<HistoricEntry> entriesInDB = new List<HistoricEntry>();

            using (_dbConnection = new SqliteConnection(_dbUri))
            {
                _dbConnection.Open();
                
                IDbCommand cmd = _dbConnection.CreateCommand();
                cmd.CommandText = "PRAGMA foreign_keys = ON";
                cmd.ExecuteNonQuery();
                
                IDbCommand dbCommand = _dbConnection.CreateCommand();
                dbCommand.CommandText = "SELECT * FROM Historic WHERE userEmail='" + userEmail + "' AND foodName='" + foodName + "'";
                
                try
                {
                    IDataReader dataReader = dbCommand.ExecuteReader();

                    while (dataReader.Read())
                    {
                        HistoricEntry entryAux = new HistoricEntry
                        {
                            userEmail = dataReader.GetString(0),
                            foodName = dataReader.GetString(1),
                            date = dataReader.GetString(2)
                        };
                        entriesInDB.Add(entryAux);
                    }

                    dataReader.Close();
                }
                catch (Exception e)
                {
                    StaticLogger.Error(e);
                }
                
                dbCommand.Dispose();
                _dbConnection.Close();
            }

            return entriesInDB;
        }

        #endregion
    }
}