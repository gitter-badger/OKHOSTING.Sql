using OKHOSTING.Sql.Schema;

namespace OKHOSTING.Sql.Operations
{
    /// <summary>
    /// Base class for all database operations
    /// </summary>
    public abstract class Operation
    {
        /// <summary>
        /// Table affected by the operation
        /// </summary>
        public Table Table { get; set; }
    }
}