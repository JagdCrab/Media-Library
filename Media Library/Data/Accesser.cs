using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SQLite;
using System.Configuration;

namespace Media_Library.Data
{
    public sealed class Accesser
    {
        private static readonly Accesser instance = new Accesser();
        private static readonly object transactionLock = new object();

        public SQLiteConnection Connection { get; }
        public SQLiteTransaction Transaction { get; private set; }

        public SQLiteDataReader ExecuteReader(SQLiteCommand _command)
        {
            _command.Connection = Connection;

            if (Transaction != null)
                _command.Transaction = Transaction;
            
            lock (transactionLock)
            {
                return _command.ExecuteReader();
            }
        }

        public void ExecuteCommand(SQLiteCommand _command)
        {
            if (Transaction == null)
                Transaction = Connection.BeginTransaction();

            _command.Connection = Connection;
            _command.Transaction = Transaction;

            lock (transactionLock)
            {
                _command.ExecuteNonQuery();
            }
        }

        public void Commit()
        {
            lock(transactionLock)
            {
                Transaction.Commit();
                Transaction.Dispose();
                Transaction = null;
            }
        }

        public void Rollback()
        {
            lock(transactionLock)
            {
                Transaction.Rollback();
                Transaction.Dispose();
                Transaction = null;
            }
        }

        public void Close()
        {
            Connection.Dispose();

            if (Transaction != null)
                Transaction.Dispose();
        }

        static Accesser() { }
        private Accesser()
        {
            Connection = new SQLiteConnection(ConfigurationManager.ConnectionStrings["Primary"].ToString());
            Connection.Open();
            
        }
        public static Accesser Instance { get { return instance; } }
    }
}
