namespace BankServer.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(int id)
            : base($"Entity {id} was not found.")
        {
            Id = id;
        }

        public int Id { get; }
    }
}
