using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsApiVW.Models
{
    public class SQLTransaction
    {
        internal bool SQLGuardaTabla(string _querry)
        {
            DVADB.DB2 dbCnx = new DVADB.DB2();
            try
            {
                dbCnx.AbrirConexion();
                dbCnx.BeginTransaccion();
                dbCnx.SetQuery(_querry);
                dbCnx.CommitTransaccion();
                dbCnx.CerrarConexion();
                return true;
            }
            catch (Exception)
            {

                dbCnx.RollbackTransaccion();
                dbCnx.CerrarConexion();
                return false; 
            }
        }
    }
}