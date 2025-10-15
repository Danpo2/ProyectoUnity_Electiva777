using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;

public class SQLiteExample : MonoBehaviour
{
    void Start()
    {
        string conn = "URI=file:" + Application.persistentDataPath + "/My_DB.db";
        IDbConnection dbconn = new SqliteConnection(conn);
        dbconn.Open();

        IDbCommand dbcmd = dbconn.CreateCommand();
        string q_createTable = "CREATE TABLE IF NOT EXISTS player (id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT, score INTEGER)";
        dbcmd.CommandText = q_createTable;
        dbcmd.ExecuteNonQuery();

        string q_insert = "INSERT INTO player (name, score) VALUES ('Sven', 100)";
        dbcmd.CommandText = q_insert;
        dbcmd.ExecuteNonQuery();

        string q_select = "SELECT * FROM player";
        dbcmd.CommandText = q_select;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            int id = reader.GetInt32(0);
            string name = reader.GetString(1);
            int score = reader.GetInt32(2);
            Debug.Log("ID: " + id + " Nombre: " + name + " Puntaje: " + score);
        }

        reader.Close();
        dbcmd.Dispose();
        dbconn.Close();
    }
}
