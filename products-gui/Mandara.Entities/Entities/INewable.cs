namespace Mandara.Entities
{
    /// <summary>
    /// For objects that need to be able to report whether or not they're new in a consistent fashion.  For instance
    /// entities used in Entity Framework.
    /// This interface is actually a hack for use in response to a case of a longstanding misuse of Entity Framework
    /// that has resulted in it being necessary to know whether an entity is new directly from the entity because
    /// Entity Framework is no longer able to tell.
    /// </summary>
    public interface INewable
    {
        /// <summary>
        /// Return whether or not the implementer is new.
        /// </summary>
        /// <returns></returns>
        bool IsNew();
    }
}
