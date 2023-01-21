using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassandra;
using Hotel.Command.Persistence.Sql.Entities;

namespace Hotel.Command.Persistence.Cassandra
{
    public class HotelContextCassandra : IDisposable
    {

        internal Cluster cluster;
        internal ISession hotelKeyspace;

        public HotelContextCassandra()
        {
            CassandraConnect();
        }

        public void CassandraConnect()
        {
            cluster = Cluster.Builder()
                .WithDefaultKeyspace("hotel")
                .AddContactPoints("127.0.0.1")
                .Build();
            hotelKeyspace = cluster.ConnectAndCreateDefaultKeyspaceIfNotExists();
        }

        public void CassandraPseudoMigrationTables()
        {
            string CreateRoomTable = "CREATE TABLE IF NOT EXISTS hotel.rooms " +
                "(id int, " +
                "numberOfBeds int, " +
                "numberOfSideBeds int, " +
                "roomSize float," +
                "floor int, " +
                "roomNumber int, " +
                "isCleaned int, " +
                "costPerNight int, " +
                "PRIMARY KEY(id, isCleaned));";

            string CreateClientsTable = "CREATE TABLE IF NOT EXISTS hotel.clients " +
                "(id int, " +
                "name text, " +
                "lastName text, " +
                "email text, " +
                "phone text, " +
                "identityCardNumber text, " +
                "PRIMARY KEY(id, email));";

            string CreateReservationByClientsTable = "CREATE TABLE IF NOT EXISTS hotel.reservationsByClients " +
                "(id int, " +
                "numberOfPersons int, " +
                "price int, " +
                "paid int, " +
                "reservationState int, " +
                "note text, " +
                "fromDate date, " +
                "toDate date, " +
                "roomId int, " +
                "clientId int, " +
                "PRIMARY KEY(id, clientId));";

            string CreateReservationByRoomsTable = "CREATE TABLE IF NOT EXISTS hotel.reservationsByRooms " +
                "(id int, " +
                "numberOfPersons int, " +
                "price int, " +
                "paid int, " +
                "reservationState int, " +
                "note text, " +
                "fromDate date, " +
                "toDate date, " +
                "roomId int, " +
                "clientId int, " +
                "PRIMARY KEY(id, roomId));";


            Execute(CreateRoomTable);
            Execute(CreateClientsTable);
            Execute(CreateReservationByClientsTable);
            Execute(CreateReservationByRoomsTable);
        }

        public void CassandraPseudoMigrationMaterializedViews()
        {
            string CreateUncleanedRoomsMaterializedView = "CREATE MATERIALIZED VIEW IF NOT EXISTS hotel.uncleaned AS " +
                "SELECT id, floor, roomNumber " +
                "FROM hotel.rooms WHERE isCleaned = 0 AND id IS NOT NULL AND floor IS NOT NULL AND roomNumber IS NOT NULL " +
                "PRIMARY KEY(isCleaned, id);";

            Execute(CreateUncleanedRoomsMaterializedView);
        }

        public void Dispose()
        {
            cluster.Dispose();
        }
        
        public RowSet Execute(string query)
        {
            return hotelKeyspace.Execute(query);
        }

        public PreparedStatement Prepare(string query)
        {
            return hotelKeyspace.Prepare(query);
        }

        public RowSet ExecutePrepare(string query, object[] parameters)
        {
            try
            {
                PreparedStatement stmt = hotelKeyspace.Prepare(query);
                var boundStatement = stmt.Bind(parameters);

                return hotelKeyspace.Execute(boundStatement);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public RowSet ExecutePrepare(PreparedStatement prepared, object[] parameters)
        {
            var boundStatement = prepared.Bind(parameters);
            return hotelKeyspace.Execute(boundStatement);
        }


    }
}