namespace Touch.Persistence
{
    /// <summary>
    /// In-memory context provider.
    /// </summary>
    sealed public class InMemoryContextProvider : IContextProvider
    {
        #region .ctor
        public InMemoryContextProvider()
        {
            _context = new InMemoryContext();
        }
        #endregion

        #region Data
        /// <summary>
        /// In-memory context.
        /// </summary>
        private readonly InMemoryContext _context;
        #endregion

        #region IContextProvider implementation
        public IContext GetContext()
        {
            return _context;
        }
        #endregion
    }
}
