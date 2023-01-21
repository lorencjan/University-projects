using Hotel.Command.Persistence.Cassandra;
using Hotel.Query.Application.Model;
using Hotel.Query.Application.RowSetExtensions;
using System.Collections.Generic;
using System.Linq;

namespace Hotel.Query.Application.Repository
{
    public static class ClientRepository
    {
        private static string GetAllClientsQuery = "SELECT * FROM hotel.clients;";
        private static string GetClientByEmailQuery = "SELECT * FROM hotel.clients WHERE email = ? ALLOW FILTERING;";
        private static string GetClientByIdQuery = "SELECT * FROM hotel.clients WHERE id = ?;";
        private static string GetCurrentClientsIdsQuery = "SELECT clientId FROM hotel.reservationsByClients WHERE fromDate <= toDate(now()) AND toDate >= toDate(now()) ALLOW FILTERING";
        private static string GetCurrentClientsQuery = "SELECT * FROM hotel.clients WHERE id IN ?;";


        public static List<ClientDTO> GetAllClients() {
            using var db = new HotelContextCassandra();
            return RowSetMapper.MapToClients(db.Execute(GetAllClientsQuery));
        }
         
        public static ClientDTO GetClientById(int id)
        {
            using var db = new HotelContextCassandra();
            var result = RowSetMapper.MapToClients(db.ExecutePrepare(GetClientByIdQuery, new object[] { id }));
            return result.Count == 1 ? result.First() : null;
        }


        public static List<ClientDTO> GetClientByEmail(string email)
        {
            using var db = new HotelContextCassandra();
            return RowSetMapper.MapToClients(db.ExecutePrepare(GetClientByEmailQuery, new [] { email}));
        }

        public static List<ClientDTO> GetAllCurrentClients()
        {
            var clients = new List<ClientDTO>();
            using var db = new HotelContextCassandra();
            var clientIds = RowSetMapper.MapIds(db.Execute(GetCurrentClientsIdsQuery), "clientid");
            if (clientIds.Any())
                clients = RowSetMapper.MapToClients(db.ExecutePrepare(GetCurrentClientsQuery, new object[] { clientIds }));

            return clients;
        }

    }
}
