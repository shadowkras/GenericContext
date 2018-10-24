namespace GenericContext.Interfaces
{
    public interface IGenericUnitOfWork
    {
        /// <summary>
        /// Start a new transaction.
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// Save changes to our database.
        /// </summary>
        bool Commit();
    }
}